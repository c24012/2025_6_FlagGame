using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerController : MonoBehaviour
    {
        #region #関数宣言
        [NonSerialized] public PlayerManager pManager;

        [SerializeField, Header("コンポーネント")] Rigidbody2D rb;
        [SerializeField] Animator anim;
        [SerializeField] SpriteRenderer sr;
        [SerializeField] BoxCollider2D slipThroughCol;

        [SerializeField, Header("関数"), Tooltip("移動速度")] float moveSpeed = 10;
        [SerializeField, Tooltip("旗持っている時の移動速度")] float[] hasFlagMoveSpeed;
        [SerializeField, Tooltip("ジャンプ力")] float jumpSpeed = 2;
        [Tooltip("空中ジャンプ回数")] float midairJumpCouter = 0;
        [SerializeField, Tooltip("床すり抜け時間(s)")] float slipThroughTime = 0.2f;
        [SerializeField, Tooltip("旗を持ち上げる時間(s)")] float raiseFlagTime = 1f;
        [SerializeField, Tooltip("旗を降ろす時間(s)")] float lowerFlagTime = 1f;
        [SerializeField, Tooltip("アイテムを取るまでの時間(s)")] float pickItemTime = 1f;

        [Tooltip("旗持ちタイマー")] float hasFlagTime_counter = 0;
        [Tooltip("箱あさりタイマー")] float pickItemTime_counter = 0;

        [SerializeField, Header("Bool確認")] bool isJump = false;//ジャンプ入力された
        [SerializeField] bool isAttack = false;//攻撃中か
        [SerializeField] bool isDown = false;//床をすり抜け中か
        [SerializeField] bool tryHasFlag = false;//旗の持ち降ろし中か
        public bool hasFlag = false;//旗を持っているか
        [SerializeField] bool useItem = false;//アイテム使用中か
        [SerializeField] bool tryPickItem = false;//アイテム取得中か
        public bool hasItem = false;//アイテムを持っているか
        [SerializeField] bool isSpecial = false;//必殺技使用中か
        [SerializeField] bool isDamage = false;//攻撃を受けている最中か
        public bool isInvincible = false;//無敵中か

        [SerializeField] bool isGround = false;//着地しているか
        public bool isRise = false;//ジャンプ上昇中か
        [SerializeField] bool isFall = false;//落下中か
        [SerializeField] bool onSlipThroughGround = false;//すり抜け床にのっているか
        [SerializeField] bool alreadyActionFlag = false;//旗行動が終わっているか
        [SerializeField] bool alreadyActionBox = false;//アイテムボックス行動が終わっているか
        public bool isLeft = false;//左を向いているか
        public bool isDeid = false;//死亡中か
        public bool isCheck = false;//GUI確認中か
        public bool inGoal = false;//ゴール地点にいるか

        const int const_JumpPow = 10000;//ジャンプ力調整定数
        const int const_movePow = 1000;//移動速度調整定数

        public float move_x = 0;//移動Input
        int playerNum = 0;

        #endregion

        #region #行動種類の定義
        public enum Action
        {
            Jump,       //ジャンプ
            Attack,     //攻撃
            SlipDown,   //すり抜け
            HasFlag,    //旗の持ち降ろし
            PickItem,   //アイテムの取得
            UseItem,    //アイテムの使用
            Special,    //必殺技
            Moving,     //移動
        }
        #endregion

        public void LateStart()
        {
            playerNum = pManager.playerNum;
        }

        private void Update()
        {
            //試合時間以外は返却
            if (!pManager.gameManager.gameStart) return;

            //ジャンプ処理
            if (Input.GetButtonDown(playerNum + "P_Jump"))
            {
                //行動ができるかを確認
                if (ActionListChack(Action.Jump))
                {
                    //地面判定がオン場合だけジャンプフラグをオン
                    if (isGround || midairJumpCouter < pManager.status.midairJump)
                    {
                        isJump = true;
                    }
                }
            }

            //すり抜け床を降りる
            if (!isDown && Input.GetAxisRaw(playerNum + "P_Vertical") > 0.5f)
            {
                //行動ができるかを確認
                if (ActionListChack(Action.SlipDown))
                {
                    //すり抜け床の上にいる場合、すり抜け処理を開始
                    if (onSlipThroughGround)
                    {
                        //すり抜け床の上にいる場合、すり抜け処理を開始
                        StartCoroutine(DownSlipThroughGround());
                    }
                }
            }

            //旗を持つ,降ろす処理
            if (Input.GetButton(playerNum + "P_HaveFlag"))
            {
                //行動できるか確認
                if (!ActionListChack(Action.HasFlag)) return;

                //旗がゴールに置かれていたら返却
                if (pManager.haveFlag.myFlagState.isPutGoal) return;

                //すでに行動し終えていたら返却
                if (alreadyActionFlag) return;

                //旗の範囲内にはいっていなかったら返却
                if (!(pManager.haveFlag.inFlag || hasFlag)) return;

                // 長押し時間測定
                hasFlagTime_counter += Time.deltaTime;

                //旗行動フラグON
                if (!tryHasFlag) tryHasFlag = true;

                //旗を持っている最中か
                if (hasFlag)
                {
                    //アクションUIの表示
                    pManager.guiManager.ViewActionRing(true);
                    pManager.guiManager.SetActionRingFillAmount(hasFlagTime_counter / lowerFlagTime);

                    //規定時間を超えたら降ろす
                    if (hasFlagTime_counter > lowerFlagTime)
                    {
                        //ゴールエリア内かつゴールを誰も使っていない場合
                        if(pManager.goal.inGoal && pManager.goal.goalPoint.pManager == null)
                        {
                            pManager.goal.PutGoalFlag();
                        }

                        pManager.haveFlag.RaiseAndLowerTheFlag(false);
                        hasFlagTime_counter = 0;
                        alreadyActionFlag = true;
                        if (tryHasFlag) tryHasFlag = false;
                        anim.SetBool("HasFlag", false);

                        //UIのリセット
                        pManager.guiManager.ViewActionRing(false);
                        pManager.guiManager.SetActionRingFillAmount(0);
                    }
                }
                //旗を持っていない場合
                else if (pManager.haveFlag.inFlag)
                {
                    //アクションUIの表示
                    pManager.guiManager.ViewActionRing(true);
                    pManager.guiManager.SetActionRingFillAmount(hasFlagTime_counter / raiseFlagTime);

                    //規定時間を超えたら持つ
                    if (hasFlagTime_counter > raiseFlagTime)
                    {
                        hasFlagTime_counter = 0;
                        alreadyActionFlag = true;
                        pManager.haveFlag.RaiseAndLowerTheFlag(true);
                        if (tryHasFlag) tryHasFlag = false;
                        anim.SetBool("HasFlag", true);

                        //UIのリセット
                        pManager.guiManager.ViewActionRing(false);
                        pManager.guiManager.SetActionRingFillAmount(0);
                    }
                }
            }
            else if (Input.GetButtonUp(playerNum + "P_HaveFlag"))
            {
                if (!alreadyActionFlag)
                {
                    if (tryHasFlag) tryHasFlag = false;

                    //UIのリセット
                    pManager.guiManager.ViewActionRing(false);
                    pManager.guiManager.SetActionRingFillAmount(0);
                }
                alreadyActionFlag = false;
                hasFlagTime_counter = 0;
            }

            //攻撃処理
            if (Input.GetButtonDown(playerNum + "P_Attack"))
            {
                //行動できるか確認
                if (!ActionListChack(Action.Attack)) return;

                //ジャンプ中だとジャンプ攻撃
                if(isRise || isFall)
                {
                    anim.SetTrigger("JumpAttackTrigger");
                }
                else
                {
                    anim.SetTrigger("AttackTrigger");
                }
                isAttack = true;
            }

            //ステータス表示
            if(Input.GetAxisRaw(playerNum + "P_Vertical") < -0.5f)
            {
                if (!isCheck)
                {
                    isCheck = true;
                    pManager.guiManager.ViewPlayerData(true);
                }
            }
            else
            {
                if (isCheck)
                {
                    isCheck = false;
                    if (!pManager.guiManager.isView)
                    {
                        pManager.guiManager.ViewPlayerData(false);
                    }

                }
            }

            //アイテム使用＆アイテムボックスをあさる
            if (Input.GetButton(playerNum + "P_UseItem"))
            {
                //アイテム使用
                if (hasItem)
                {
                    //行動できるか確認
                    if (!ActionListChack(Action.UseItem)) return;

                    //すでに行動し終えていたら返却
                    if (alreadyActionBox) return;

                    //アイテム使用処理
                    pManager.itemBox.InstantiateHaveItem();
                    hasItem = false;
                    alreadyActionBox = true;
                }
                //アイテム取得
                else
                {
                    //行動できるか確認
                    if (!ActionListChack(Action.PickItem)) return;

                    //すでに行動し終えていたら返却
                    if (alreadyActionBox) return;

                    //アイテムボックスの範囲内にはいっていなかったらゲージリセット後、返却
                    if (!pManager.itemBox.inItemBox)
                    {
                        if (!alreadyActionBox)
                        {
                            if (tryPickItem) tryPickItem = false;

                            //UIのリセット
                            pManager.guiManager.ViewActionRing(false);
                            pManager.guiManager.SetActionRingFillAmount(0);
                        }
                        alreadyActionBox = false;
                        pickItemTime_counter = 0;
                        return;
                    }

                    // 長押し時間測定
                    pickItemTime_counter += Time.deltaTime;

                    //アイテムボックスのスプライトをOpenに変更
                    if (!tryPickItem) pManager.itemBox.TryGetItemFromItemBox();

                    //行動フラグON
                    if (!tryPickItem) tryPickItem = true;

                    //アクションUIの表示
                    pManager.guiManager.ViewActionRing(true);
                    pManager.guiManager.SetActionRingFillAmount(pickItemTime_counter / pickItemTime);

                    //規定時間を超えたらアイテムを獲得
                    if (pickItemTime_counter > pickItemTime)
                    {
                        //アイテム取得処理
                        pManager.itemBox.GetItemForItemBox();
                        hasItem = true;

                        pickItemTime_counter = 0;
                        alreadyActionBox = true;
                        if (tryPickItem) tryPickItem = false;

                        //UIのリセット
                        pManager.guiManager.ViewActionRing(false);
                        pManager.guiManager.SetActionRingFillAmount(0);
                    }
                }
                
            }
            else if (Input.GetButtonUp(playerNum + "P_UseItem"))
            {
                //アイテムボックスの開封を辞めた場合
                if (pManager.itemBox.inItemBox && !alreadyActionBox)
                {
                    //アイテムボックスのスプライトをCloseに変更
                    pManager.itemBox.CancelGetItemFromItemBox();
                }

                if (!alreadyActionBox)
                {
                    if (tryPickItem) tryPickItem = false;

                    //UIのリセット
                    pManager.guiManager.ViewActionRing(false);
                    pManager.guiManager.SetActionRingFillAmount(0);
                }
                alreadyActionBox = false;
                pickItemTime_counter = 0;
            }
        }

        private void FixedUpdate()
        {
            //試合時間以外は返却
            if (!pManager.gameManager.gameStart)
            {
                rb.bodyType = RigidbodyType2D.Static;
                return;
            }
            else if(rb.bodyType != RigidbodyType2D.Dynamic)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            //キャラ操作移動
            if (ActionListChack(Action.Moving))
            {
                move_x = Input.GetAxisRaw(playerNum + "P_Horizontal");

                if (hasFlag)
                {
                    //ポジション移動＆移動アニメーション
                    rb.velocity = new Vector2(
                        move_x * hasFlagMoveSpeed[pManager.haveFlag.myFlagState.flagLevel] * Time.deltaTime * const_movePow * pManager.status.speedFactor,
                        rb.velocity.y);
                }
                else
                {
                    //ポジション移動＆移動アニメーション
                    rb.velocity = new Vector2(
                        move_x * moveSpeed * Time.deltaTime * pManager.status.speed * const_movePow * pManager.status.speedFactor,
                        rb.velocity.y);
                }
                anim.SetFloat("x", move_x);
            }
            else
            {
                //ダメージモーション中でなければ
                if (!isDamage)
                {
                    rb.velocity = new Vector2(math.lerp(rb.velocity.x, 0, 0.15f), rb.velocity.y);
                    anim.SetFloat("x", 0);
                }
            }

            //テクスチャと攻撃コライダーの左右の向きを指定
            if (ActionListChack(Action.Moving))
            {
                if (move_x > 0 && isLeft)
                {
                    isLeft = false;
                    sr.flipX = false;
                    Vector2 attackColOffset = pManager.attack.attackCol.offset;
                    pManager.attack.attackCol.offset = new Vector2(attackColOffset.x * -1, attackColOffset.y);
                    Vector2 jumpAttackColOffset = pManager.jumpAttack.attackCol.offset;
                    pManager.jumpAttack.attackCol.offset = new Vector2(jumpAttackColOffset.x * -1, jumpAttackColOffset.y);
                    //アイテムスプライトの向きを変更
                    pManager.itemBox.SetSpriteDirection(isLeft);
                }
                else if (move_x < 0 && !isLeft)
                {
                    isLeft = true;
                    sr.flipX = true;
                    Vector2 attackColOffset = pManager.attack.attackCol.offset;
                    pManager.attack.attackCol.offset = new Vector2(attackColOffset.x * -1, attackColOffset.y);
                    Vector2 jumpAttackColOffset = pManager.jumpAttack.attackCol.offset;
                    pManager.jumpAttack.attackCol.offset = new Vector2(jumpAttackColOffset.x * -1, jumpAttackColOffset.y);
                    //アイテムスプライトの向きを変更
                    pManager.itemBox.SetSpriteDirection(isLeft);
                }
            }

            //足元にオブジェクトが存在しない場合
            if (!isGround)
            {
                //空中での落下時アニメーション
                if (rb.velocity.y < 0)
                {
                    if (isRise) isRise = false;
                    if (!isFall) isFall = true;
                    anim.SetBool("IsFall", true);
                }
            }
            //何かに乗っている場合
            else
            {
                //空中関連の関数をリセット
                if (isFall) isFall = false;
                if (isRise) isRise = false;
                midairJumpCouter = 0;

                //同時に空中アニメーションフラグをオフ
                anim.SetBool("IsJump", false);
                anim.SetBool("IsFall", false);
            }

            //ジャンプフラグがオンの時にジャンプする
            if (isJump)
            {
                //行動ができるかを確認
                if (!ActionListChack(Action.Jump))
                {
                    isJump = false;
                    return;
                }

                //上昇フラグON
                isRise = true;
                //地面からのジャンプ
                if (isGround)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(const_JumpPow * jumpSpeed * pManager.status.jumpPow * pManager.status.jumpPowFactor * transform.up);
                }
                //空中でのジャンプ
                else
                {
                    midairJumpCouter++;
                    if (isFall) isFall = false;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(const_JumpPow * jumpSpeed * pManager.status.airJumpPow * pManager.status.jumpPowFactor * transform.up);
                    anim.SetBool("IsFall", false);
                }

                anim.SetBool("IsJump", true);
                isJump = false;
            }

            //ダメージノックバック時の床との摩擦風処理
            if (isDamage)
            {
                if (isGround)
                {
                    rb.velocity = new Vector2(math.lerp(rb.velocity.x, 0, 0.1f), rb.velocity.y);
                }
            }
        }

        /// <summary>
        /// プレイヤーを待機状態にする
        /// </summary>
        public void SetIdle()
        {
            //コライダの向きを修正
            if (isLeft)
            {
                this.isLeft = false;
                sr.flipX = false;
                Vector2 attackColOffset = pManager.attack.attackCol.offset;
                pManager.attack.attackCol.offset = new Vector2(attackColOffset.x * -1, attackColOffset.y);
                Vector2 jumpAttackColOffset = pManager.jumpAttack.attackCol.offset;
                pManager.jumpAttack.attackCol.offset = new Vector2(jumpAttackColOffset.x * -1, jumpAttackColOffset.y);
            }
            //アイテムスプライトの向きを修正
            pManager.itemBox.SetSpriteDirection(false);

            isRise = false;
            isFall = false;
            isLeft = false;
            isAttack = false;
            isDamage = false;
            isJump = false;
            isSpecial = false;
            sr.flipX = false;
            rb.velocity = Vector2.zero;

            anim.SetFloat("x", 0);
            anim.SetBool("IsFall",false);
            anim.SetBool("IsJump",false);
            anim.SetBool("HasFlag",false);
            anim.SetBool("IsDamage",false);
        }

        /// <summary>
        /// 足元のオブジェクトを確認
        /// </summary>
        public void SetIsGround(Collider2D collision, bool isIn)
        {
            //地面に立っている場合は地面着地フラグをオン
            if (collision.gameObject.layer == LayerMask.NameToLayer("WorldObject"))
            {
                isGround = isIn;
            }

            //すり抜け床にのっている場合にすり抜け床フラグをオン
            if (collision.CompareTag("SlipThroughGround"))
            {
                onSlipThroughGround = isIn;
            }
        }

        /// <summary>
        /// 攻撃アニメーション終了(後隙解除)
        /// </summary>
        public void FinishAttackAnimation()
        {
            isAttack = false;
        }

        /// <summary>
        /// 床すり抜け処理
        /// </summary>
        IEnumerator DownSlipThroughGround()
        {
            //すり抜けフラグオン
            isDown = true;
            //SlipThroughColliderの判定を無効化
            slipThroughCol.enabled = false;
            //床を通過するまで待つ
            yield return new WaitForSeconds(slipThroughTime);
            //SlipThroughColliderの判定を有効化
            slipThroughCol.enabled = true;
            //すり抜けフラグオフ
            isDown = false;
        }

        /// <summary>
        /// 被ダメージ時のノックバック
        /// </summary>
        public void DamageMotion(float force, bool isLeft)
        {
            //各行動フラグをキャンセル＆加速度をゼロに
            SetIdle();
            //被ダメージ中フラグをON
            isDamage = true;
            //無敵付与
            isInvincible = true;
            //ノックバックの向きと反対に向かせる
            if (isLeft)
            {
                this.isLeft = false;
                sr.flipX = false;
                Vector2 attackColOffset = pManager.attack.attackCol.offset;
                pManager.attack.attackCol.offset = new Vector2(attackColOffset.x * -1, attackColOffset.y);
                Vector2 jumpAttackColOffset = pManager.jumpAttack.attackCol.offset;
                pManager.jumpAttack.attackCol.offset = new Vector2(jumpAttackColOffset.x * -1, jumpAttackColOffset.y);
            }
            else if (!isLeft)
            {
                this.isLeft = true;
                sr.flipX = true;
                Vector2 attackColOffset = pManager.attack.attackCol.offset;
                pManager.attack.attackCol.offset = new Vector2(attackColOffset.x * -1, attackColOffset.y);
                Vector2 jumpAttackColOffset = pManager.jumpAttack.attackCol.offset;
                pManager.jumpAttack.attackCol.offset = new Vector2(jumpAttackColOffset.x * -1, jumpAttackColOffset.y);
            }
            //被ダメージのアニメーションフラグをON
            anim.SetBool("IsDamage", true);
            //ノックバックのベクトルを定義
            Vector2 knockBackVec = (transform.right * (isLeft ? -1 : 1)) + transform.up * 0.4f;
            //AddForceでノックバック
            rb.AddForce(knockBackVec * force * 10000);

            //指定時間後にダメージモーション解除
            Invoke(nameof(ReleaseDamageMotion), force / 2);
            //点滅アニメーション再生
            StartCoroutine(BlinkingAnimation());
        } 

        /// <summary>
        /// ダメージモーション解除
        /// </summary>
        void ReleaseDamageMotion()
        {
            //被ダメージ中フラグをON
            isDamage = false;
            //死亡中でなければ無敵解除
            if (!isDeid) isInvincible = false;
            //被ダメージのアニメーションフラグをON
            anim.SetBool("IsDamage", false);
        }

        /// <summary>
        /// ダメージ点滅アニメーション
        /// </summary>
        IEnumerator BlinkingAnimation()
        {
            while (isDamage && !isDeid)
            {
                sr.enabled = false;
                yield return new WaitForSeconds(0.06f);
                sr.enabled = true;
                yield return new WaitForSeconds(0.06f);
            }
            //死亡中の場合Spriteを非表示
            if (isDeid)
            {
                sr.enabled = false;
            }
        }

        /// <summary>
        /// 死亡か復活の処理
        /// </summary>
        public void DiedOrAlive(bool isDeid)
        {
            //死亡時
            if (isDeid)
            {
                //死亡フラグをON
                this.isDeid = true;
                //RigidBodyをOFF
                rb.simulated = false;
                //SpriteRendererをOFF
                sr.enabled = false;
                //Canvasを非表示
                pManager.guiManager.SetCanvasEnable(false);
                //旗を持っている場合放棄する
                pManager.haveFlag.RaiseAndLowerTheFlag(false);
                //アニメーターのHasFlagをOFF
                anim.SetBool("HasFlag", false);
                //無敵付与
                isInvincible = true;
            }
            //復活時
            else
            {
                //状態をリセット
                SetIdle();
                //死亡フラグをOFF
                this.isDeid = false;
                //RigidBodyをON
                rb.simulated = true;
                //SpriteRendererをON
                sr.enabled = true;
                //Canvasを非表示
                pManager.guiManager.SetCanvasEnable(true);
                //無敵を解除
                isInvincible = false;
            }
        }

        /// <summary>
        /// 行動可能か確認処理
        /// </summary>
        bool ActionListChack(Action action)
        {
            //死亡フラグがONの時は強制返却(行動不能)
            if (isDeid) return false;
            //被ダメージ中は強制返却(行動不能)
            if (isDamage) return false;

            //行動によって行動許可を申請
            switch (action)
            {
                case Action.Jump:
                    if (isAttack) return false;
                    if (tryHasFlag) return false;
                    if (tryPickItem) return false;
                    if (useItem) return false;
                    if (isSpecial) return false;
                    break;

                case Action.Attack:
                    if (hasFlag) return false;
                    if (isAttack) return false;
                    if (tryHasFlag) return false;
                    if (tryPickItem) return false;
                    if (useItem) return false;
                    if (isSpecial) return false;
                    break;

                case Action.SlipDown:
                    if (isAttack) return false;
                    if (isRise || isFall) return false;
                    if (tryHasFlag) return false;
                    if (tryPickItem) return false;
                    if (useItem) return false;
                    break;

                case Action.HasFlag:
                    if (isRise || isFall) return false;
                    if (isAttack) return false;
                    if (tryPickItem) return false;
                    if (useItem) return false;
                    if (isSpecial) return false;
                    break;

                case Action.PickItem:
                    if (isRise || isFall) return false;
                    if (isAttack) return false;
                    if (tryHasFlag) return false;
                    if (hasFlag) return false;
                    if (useItem) return false;
                    if (isSpecial) return false;
                    break;

                case Action.UseItem:
                    if (hasFlag) return false;
                    if (isAttack) return false;
                    if (tryHasFlag) return false;
                    if (tryPickItem) return false;
                    if (isSpecial) return false;
                    break;

                case Action.Special:
                    if (hasFlag) return false;
                    if (isAttack) return false;
                    if (tryHasFlag) return false;
                    if (tryPickItem) return false;
                    if (useItem) return false;
                    if (isSpecial) return false;
                    break;

                case Action.Moving:
                    if (isAttack) return false;
                    if (tryHasFlag) return false;
                    if (tryPickItem) return false;
                    if (useItem) return false;
                    if (isSpecial) return false;
                    break;
            }
            //該当なしの時は行動を許可
            return true;
        }
    }
}

