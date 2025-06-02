using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerController : MonoBehaviour
    {
        #region #�֐��錾
        [NonSerialized] public PlayerManager pManager;

        [SerializeField, Header("�R���|�[�l���g")] Rigidbody2D rb;
        [SerializeField] Animator anim;
        [SerializeField] SpriteRenderer sr;
        [SerializeField] BoxCollider2D slipThroughCol;

        [SerializeField, Header("�֐�"), Tooltip("�ړ����x")] float moveSpeed = 10;
        [SerializeField, Tooltip("�������Ă��鎞�̈ړ����x")] float[] hasFlagMoveSpeed;
        [SerializeField, Tooltip("�W�����v��")] float jumpSpeed = 2;
        [Tooltip("�󒆃W�����v��")] float midairJumpCouter = 0;
        [SerializeField, Tooltip("�����蔲������(s)")] float slipThroughTime = 0.2f;
        [SerializeField, Tooltip("���������グ�鎞��(s)")] float raiseFlagTime = 1f;
        [SerializeField, Tooltip("�����~�낷����(s)")] float lowerFlagTime = 1f;
        [SerializeField, Tooltip("�A�C�e�������܂ł̎���(s)")] float pickItemTime = 1f;

        [Tooltip("�������^�C�}�[")] float hasFlagTime_counter = 0;
        [Tooltip("��������^�C�}�[")] float pickItemTime_counter = 0;

        [SerializeField, Header("Bool�m�F")] bool isJump = false;//�W�����v���͂��ꂽ
        [SerializeField] bool isAttack = false;//�U������
        [SerializeField] bool isDown = false;//�������蔲������
        [SerializeField] bool tryHasFlag = false;//���̎����~�낵����
        public bool hasFlag = false;//���������Ă��邩
        [SerializeField] bool useItem = false;//�A�C�e���g�p����
        [SerializeField] bool tryPickItem = false;//�A�C�e���擾����
        public bool hasItem = false;//�A�C�e���������Ă��邩
        [SerializeField] bool isSpecial = false;//�K�E�Z�g�p����
        [SerializeField] bool isDamage = false;//�U�����󂯂Ă���Œ���
        public bool isInvincible = false;//���G����

        [SerializeField] bool isGround = false;//���n���Ă��邩
        public bool isRise = false;//�W�����v�㏸����
        [SerializeField] bool isFall = false;//��������
        [SerializeField] bool onSlipThroughGround = false;//���蔲�����ɂ̂��Ă��邩
        [SerializeField] bool alreadyActionFlag = false;//���s�����I����Ă��邩
        [SerializeField] bool alreadyActionBox = false;//�A�C�e���{�b�N�X�s�����I����Ă��邩
        public bool isLeft = false;//���������Ă��邩
        public bool isDeid = false;//���S����
        public bool isCheck = false;//GUI�m�F����
        public bool inGoal = false;//�S�[���n�_�ɂ��邩

        const int const_JumpPow = 10000;//�W�����v�͒����萔
        const int const_movePow = 1000;//�ړ����x�����萔

        public float move_x = 0;//�ړ�Input
        int playerNum = 0;

        #endregion

        #region #�s����ނ̒�`
        public enum Action
        {
            Jump,       //�W�����v
            Attack,     //�U��
            SlipDown,   //���蔲��
            HasFlag,    //���̎����~�낵
            PickItem,   //�A�C�e���̎擾
            UseItem,    //�A�C�e���̎g�p
            Special,    //�K�E�Z
            Moving,     //�ړ�
        }
        #endregion

        public void LateStart()
        {
            playerNum = pManager.playerNum;
        }

        private void Update()
        {
            //�������ԈȊO�͕ԋp
            if (!pManager.gameManager.gameStart) return;

            //�W�����v����
            if (Input.GetButtonDown(playerNum + "P_Jump"))
            {
                //�s�����ł��邩���m�F
                if (ActionListChack(Action.Jump))
                {
                    //�n�ʔ��肪�I���ꍇ�����W�����v�t���O���I��
                    if (isGround || midairJumpCouter < pManager.status.midairJump)
                    {
                        isJump = true;
                    }
                }
            }

            //���蔲�������~���
            if (!isDown && Input.GetAxisRaw(playerNum + "P_Vertical") > 0.5f)
            {
                //�s�����ł��邩���m�F
                if (ActionListChack(Action.SlipDown))
                {
                    //���蔲�����̏�ɂ���ꍇ�A���蔲���������J�n
                    if (onSlipThroughGround)
                    {
                        //���蔲�����̏�ɂ���ꍇ�A���蔲���������J�n
                        StartCoroutine(DownSlipThroughGround());
                    }
                }
            }

            //��������,�~�낷����
            if (Input.GetButton(playerNum + "P_HaveFlag"))
            {
                //�s���ł��邩�m�F
                if (!ActionListChack(Action.HasFlag)) return;

                //�����S�[���ɒu����Ă�����ԋp
                if (pManager.haveFlag.myFlagState.isPutGoal) return;

                //���łɍs�����I���Ă�����ԋp
                if (alreadyActionFlag) return;

                //���͈͓̔��ɂ͂����Ă��Ȃ�������ԋp
                if (!(pManager.haveFlag.inFlag || hasFlag)) return;

                // ���������ԑ���
                hasFlagTime_counter += Time.deltaTime;

                //���s���t���OON
                if (!tryHasFlag) tryHasFlag = true;

                //���������Ă���Œ���
                if (hasFlag)
                {
                    //�A�N�V����UI�̕\��
                    pManager.guiManager.ViewActionRing(true);
                    pManager.guiManager.SetActionRingFillAmount(hasFlagTime_counter / lowerFlagTime);

                    //�K�莞�Ԃ𒴂�����~�낷
                    if (hasFlagTime_counter > lowerFlagTime)
                    {
                        //�S�[���G���A�����S�[����N���g���Ă��Ȃ��ꍇ
                        if(pManager.goal.inGoal && pManager.goal.goalPoint.pManager == null)
                        {
                            pManager.goal.PutGoalFlag();
                        }

                        pManager.haveFlag.RaiseAndLowerTheFlag(false);
                        hasFlagTime_counter = 0;
                        alreadyActionFlag = true;
                        if (tryHasFlag) tryHasFlag = false;
                        anim.SetBool("HasFlag", false);

                        //UI�̃��Z�b�g
                        pManager.guiManager.ViewActionRing(false);
                        pManager.guiManager.SetActionRingFillAmount(0);
                    }
                }
                //���������Ă��Ȃ��ꍇ
                else if (pManager.haveFlag.inFlag)
                {
                    //�A�N�V����UI�̕\��
                    pManager.guiManager.ViewActionRing(true);
                    pManager.guiManager.SetActionRingFillAmount(hasFlagTime_counter / raiseFlagTime);

                    //�K�莞�Ԃ𒴂����玝��
                    if (hasFlagTime_counter > raiseFlagTime)
                    {
                        hasFlagTime_counter = 0;
                        alreadyActionFlag = true;
                        pManager.haveFlag.RaiseAndLowerTheFlag(true);
                        if (tryHasFlag) tryHasFlag = false;
                        anim.SetBool("HasFlag", true);

                        //UI�̃��Z�b�g
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

                    //UI�̃��Z�b�g
                    pManager.guiManager.ViewActionRing(false);
                    pManager.guiManager.SetActionRingFillAmount(0);
                }
                alreadyActionFlag = false;
                hasFlagTime_counter = 0;
            }

            //�U������
            if (Input.GetButtonDown(playerNum + "P_Attack"))
            {
                //�s���ł��邩�m�F
                if (!ActionListChack(Action.Attack)) return;

                //�W�����v�����ƃW�����v�U��
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

            //�X�e�[�^�X�\��
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

            //�A�C�e���g�p���A�C�e���{�b�N�X��������
            if (Input.GetButton(playerNum + "P_UseItem"))
            {
                //�A�C�e���g�p
                if (hasItem)
                {
                    //�s���ł��邩�m�F
                    if (!ActionListChack(Action.UseItem)) return;

                    //���łɍs�����I���Ă�����ԋp
                    if (alreadyActionBox) return;

                    //�A�C�e���g�p����
                    pManager.itemBox.InstantiateHaveItem();
                    hasItem = false;
                    alreadyActionBox = true;
                }
                //�A�C�e���擾
                else
                {
                    //�s���ł��邩�m�F
                    if (!ActionListChack(Action.PickItem)) return;

                    //���łɍs�����I���Ă�����ԋp
                    if (alreadyActionBox) return;

                    //�A�C�e���{�b�N�X�͈͓̔��ɂ͂����Ă��Ȃ�������Q�[�W���Z�b�g��A�ԋp
                    if (!pManager.itemBox.inItemBox)
                    {
                        if (!alreadyActionBox)
                        {
                            if (tryPickItem) tryPickItem = false;

                            //UI�̃��Z�b�g
                            pManager.guiManager.ViewActionRing(false);
                            pManager.guiManager.SetActionRingFillAmount(0);
                        }
                        alreadyActionBox = false;
                        pickItemTime_counter = 0;
                        return;
                    }

                    // ���������ԑ���
                    pickItemTime_counter += Time.deltaTime;

                    //�A�C�e���{�b�N�X�̃X�v���C�g��Open�ɕύX
                    if (!tryPickItem) pManager.itemBox.TryGetItemFromItemBox();

                    //�s���t���OON
                    if (!tryPickItem) tryPickItem = true;

                    //�A�N�V����UI�̕\��
                    pManager.guiManager.ViewActionRing(true);
                    pManager.guiManager.SetActionRingFillAmount(pickItemTime_counter / pickItemTime);

                    //�K�莞�Ԃ𒴂�����A�C�e�����l��
                    if (pickItemTime_counter > pickItemTime)
                    {
                        //�A�C�e���擾����
                        pManager.itemBox.GetItemForItemBox();
                        hasItem = true;

                        pickItemTime_counter = 0;
                        alreadyActionBox = true;
                        if (tryPickItem) tryPickItem = false;

                        //UI�̃��Z�b�g
                        pManager.guiManager.ViewActionRing(false);
                        pManager.guiManager.SetActionRingFillAmount(0);
                    }
                }
                
            }
            else if (Input.GetButtonUp(playerNum + "P_UseItem"))
            {
                //�A�C�e���{�b�N�X�̊J�������߂��ꍇ
                if (pManager.itemBox.inItemBox && !alreadyActionBox)
                {
                    //�A�C�e���{�b�N�X�̃X�v���C�g��Close�ɕύX
                    pManager.itemBox.CancelGetItemFromItemBox();
                }

                if (!alreadyActionBox)
                {
                    if (tryPickItem) tryPickItem = false;

                    //UI�̃��Z�b�g
                    pManager.guiManager.ViewActionRing(false);
                    pManager.guiManager.SetActionRingFillAmount(0);
                }
                alreadyActionBox = false;
                pickItemTime_counter = 0;
            }
        }

        private void FixedUpdate()
        {
            //�������ԈȊO�͕ԋp
            if (!pManager.gameManager.gameStart)
            {
                rb.bodyType = RigidbodyType2D.Static;
                return;
            }
            else if(rb.bodyType != RigidbodyType2D.Dynamic)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            //�L��������ړ�
            if (ActionListChack(Action.Moving))
            {
                move_x = Input.GetAxisRaw(playerNum + "P_Horizontal");

                if (hasFlag)
                {
                    //�|�W�V�����ړ����ړ��A�j���[�V����
                    rb.velocity = new Vector2(
                        move_x * hasFlagMoveSpeed[pManager.haveFlag.myFlagState.flagLevel] * Time.deltaTime * const_movePow * pManager.status.speedFactor,
                        rb.velocity.y);
                }
                else
                {
                    //�|�W�V�����ړ����ړ��A�j���[�V����
                    rb.velocity = new Vector2(
                        move_x * moveSpeed * Time.deltaTime * pManager.status.speed * const_movePow * pManager.status.speedFactor,
                        rb.velocity.y);
                }
                anim.SetFloat("x", move_x);
            }
            else
            {
                //�_���[�W���[�V�������łȂ����
                if (!isDamage)
                {
                    rb.velocity = new Vector2(math.lerp(rb.velocity.x, 0, 0.15f), rb.velocity.y);
                    anim.SetFloat("x", 0);
                }
            }

            //�e�N�X�`���ƍU���R���C�_�[�̍��E�̌������w��
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
                    //�A�C�e���X�v���C�g�̌�����ύX
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
                    //�A�C�e���X�v���C�g�̌�����ύX
                    pManager.itemBox.SetSpriteDirection(isLeft);
                }
            }

            //�����ɃI�u�W�F�N�g�����݂��Ȃ��ꍇ
            if (!isGround)
            {
                //�󒆂ł̗������A�j���[�V����
                if (rb.velocity.y < 0)
                {
                    if (isRise) isRise = false;
                    if (!isFall) isFall = true;
                    anim.SetBool("IsFall", true);
                }
            }
            //�����ɏ���Ă���ꍇ
            else
            {
                //�󒆊֘A�̊֐������Z�b�g
                if (isFall) isFall = false;
                if (isRise) isRise = false;
                midairJumpCouter = 0;

                //�����ɋ󒆃A�j���[�V�����t���O���I�t
                anim.SetBool("IsJump", false);
                anim.SetBool("IsFall", false);
            }

            //�W�����v�t���O���I���̎��ɃW�����v����
            if (isJump)
            {
                //�s�����ł��邩���m�F
                if (!ActionListChack(Action.Jump))
                {
                    isJump = false;
                    return;
                }

                //�㏸�t���OON
                isRise = true;
                //�n�ʂ���̃W�����v
                if (isGround)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(const_JumpPow * jumpSpeed * pManager.status.jumpPow * pManager.status.jumpPowFactor * transform.up);
                }
                //�󒆂ł̃W�����v
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

            //�_���[�W�m�b�N�o�b�N���̏��Ƃ̖��C������
            if (isDamage)
            {
                if (isGround)
                {
                    rb.velocity = new Vector2(math.lerp(rb.velocity.x, 0, 0.1f), rb.velocity.y);
                }
            }
        }

        /// <summary>
        /// �v���C���[��ҋ@��Ԃɂ���
        /// </summary>
        public void SetIdle()
        {
            //�R���C�_�̌������C��
            if (isLeft)
            {
                this.isLeft = false;
                sr.flipX = false;
                Vector2 attackColOffset = pManager.attack.attackCol.offset;
                pManager.attack.attackCol.offset = new Vector2(attackColOffset.x * -1, attackColOffset.y);
                Vector2 jumpAttackColOffset = pManager.jumpAttack.attackCol.offset;
                pManager.jumpAttack.attackCol.offset = new Vector2(jumpAttackColOffset.x * -1, jumpAttackColOffset.y);
            }
            //�A�C�e���X�v���C�g�̌������C��
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
        /// �����̃I�u�W�F�N�g���m�F
        /// </summary>
        public void SetIsGround(Collider2D collision, bool isIn)
        {
            //�n�ʂɗ����Ă���ꍇ�͒n�ʒ��n�t���O���I��
            if (collision.gameObject.layer == LayerMask.NameToLayer("WorldObject"))
            {
                isGround = isIn;
            }

            //���蔲�����ɂ̂��Ă���ꍇ�ɂ��蔲�����t���O���I��
            if (collision.CompareTag("SlipThroughGround"))
            {
                onSlipThroughGround = isIn;
            }
        }

        /// <summary>
        /// �U���A�j���[�V�����I��(�㌄����)
        /// </summary>
        public void FinishAttackAnimation()
        {
            isAttack = false;
        }

        /// <summary>
        /// �����蔲������
        /// </summary>
        IEnumerator DownSlipThroughGround()
        {
            //���蔲���t���O�I��
            isDown = true;
            //SlipThroughCollider�̔���𖳌���
            slipThroughCol.enabled = false;
            //����ʉ߂���܂ő҂�
            yield return new WaitForSeconds(slipThroughTime);
            //SlipThroughCollider�̔����L����
            slipThroughCol.enabled = true;
            //���蔲���t���O�I�t
            isDown = false;
        }

        /// <summary>
        /// ��_���[�W���̃m�b�N�o�b�N
        /// </summary>
        public void DamageMotion(float force, bool isLeft)
        {
            //�e�s���t���O���L�����Z���������x���[����
            SetIdle();
            //��_���[�W���t���O��ON
            isDamage = true;
            //���G�t�^
            isInvincible = true;
            //�m�b�N�o�b�N�̌����Ɣ��΂Ɍ�������
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
            //��_���[�W�̃A�j���[�V�����t���O��ON
            anim.SetBool("IsDamage", true);
            //�m�b�N�o�b�N�̃x�N�g�����`
            Vector2 knockBackVec = (transform.right * (isLeft ? -1 : 1)) + transform.up * 0.4f;
            //AddForce�Ńm�b�N�o�b�N
            rb.AddForce(knockBackVec * force * 10000);

            //�w�莞�Ԍ�Ƀ_���[�W���[�V��������
            Invoke(nameof(ReleaseDamageMotion), force / 2);
            //�_�ŃA�j���[�V�����Đ�
            StartCoroutine(BlinkingAnimation());
        } 

        /// <summary>
        /// �_���[�W���[�V��������
        /// </summary>
        void ReleaseDamageMotion()
        {
            //��_���[�W���t���O��ON
            isDamage = false;
            //���S���łȂ���Ζ��G����
            if (!isDeid) isInvincible = false;
            //��_���[�W�̃A�j���[�V�����t���O��ON
            anim.SetBool("IsDamage", false);
        }

        /// <summary>
        /// �_���[�W�_�ŃA�j���[�V����
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
            //���S���̏ꍇSprite���\��
            if (isDeid)
            {
                sr.enabled = false;
            }
        }

        /// <summary>
        /// ���S�������̏���
        /// </summary>
        public void DiedOrAlive(bool isDeid)
        {
            //���S��
            if (isDeid)
            {
                //���S�t���O��ON
                this.isDeid = true;
                //RigidBody��OFF
                rb.simulated = false;
                //SpriteRenderer��OFF
                sr.enabled = false;
                //Canvas���\��
                pManager.guiManager.SetCanvasEnable(false);
                //���������Ă���ꍇ��������
                pManager.haveFlag.RaiseAndLowerTheFlag(false);
                //�A�j���[�^�[��HasFlag��OFF
                anim.SetBool("HasFlag", false);
                //���G�t�^
                isInvincible = true;
            }
            //������
            else
            {
                //��Ԃ����Z�b�g
                SetIdle();
                //���S�t���O��OFF
                this.isDeid = false;
                //RigidBody��ON
                rb.simulated = true;
                //SpriteRenderer��ON
                sr.enabled = true;
                //Canvas���\��
                pManager.guiManager.SetCanvasEnable(true);
                //���G������
                isInvincible = false;
            }
        }

        /// <summary>
        /// �s���\���m�F����
        /// </summary>
        bool ActionListChack(Action action)
        {
            //���S�t���O��ON�̎��͋����ԋp(�s���s�\)
            if (isDeid) return false;
            //��_���[�W���͋����ԋp(�s���s�\)
            if (isDamage) return false;

            //�s���ɂ���čs������\��
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
            //�Y���Ȃ��̎��͍s��������
            return true;
        }
    }
}

