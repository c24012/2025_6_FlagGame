using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerBullet : MonoBehaviour
    {
        public PlayerManager pManager;
        [SerializeField] Rigidbody2D rb;

        [Header("弾の速度"), SerializeField] float bulletSpeed;
        [Header("弾の滞在時間"), SerializeField] float bulletTime;
        [Header("攻撃の種類"), SerializeField] AttackType attackType;

        const int speedPow = 10000;//速度桁調整用
        bool isLeft = false;

        enum AttackType
        {
            Attack,
            JumpAttack
        }

        private void Start()
        {
            DestroyBullet(bulletTime);
            if (pManager.controller.isLeft)
            {
                isLeft = true;
            }
        }

        private void FixedUpdate()
        {
            if (isLeft)
            {
                rb.velocity = new Vector2(-bulletSpeed * speedPow * Time.deltaTime, 0);
            }
            else
            {
                rb.velocity = new Vector2(bulletSpeed * speedPow * Time.deltaTime, 0);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            //攻撃判定にプレイヤーが当たったら
            if (collider.CompareTag("Player"))
            {
                //そのプレイヤーのIDamageを攻撃力をのせて起動
                if (collider.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
                {
                    if (attackType == AttackType.Attack)
                    {
                        idamage.IDamage(
                            (int)(pManager.status.attackPow * pManager.status.attackFactor),      //攻撃力
                            pManager.status.attackKnockPow, //ノックバック
                            isLeft                          //弾丸の向き
                        );
                    }
                    else if (attackType == AttackType.JumpAttack) 
                    {
                        idamage.IDamage(
                            (int)(pManager.status.jumpAttackPow * pManager.status.attackFactor),      //攻撃力
                            pManager.status.jumpAttackKnockPow, //ノックバック
                            isLeft                              //弾丸の向き
                        );
                    }
                    DestroyBullet(0);
                }
            }

            //攻撃判定に旗オブジェクトが当たったら
            if (collider.CompareTag("Flag"))
            {
                //旗オブジェクトが自分旗オブジェクトと一致したら抜ける
                if (collider.gameObject == pManager.haveFlag.myFlagObj) return;

                //他プレイヤーの旗の場合,その旗のIDamageを攻撃力をのせて起動
                if (collider.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
                {
                    idamage.IDamage(pManager.status.attackPow);
                    DestroyBullet(0);
                }
            }

            //攻撃判定に地面や壁に当たったら
            if (collider.CompareTag("Ground"))
            {
                DestroyBullet(0);
            }
        }

        void DestroyBullet(float destroyTime)
        {
            Destroy(gameObject, destroyTime);
        }
    }
}

