using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerJumpAttack : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        public CapsuleCollider2D attackCol;

        private void Awake()
        {
            //初期化
            attackCol.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            //攻撃判定にプレイヤーが当たったら
            if (collider.CompareTag("Player"))
            {
                //そのプレイヤーのIDamageを攻撃力をのせて起動
                if (collider.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
                {
                    idamage.IDamage(
                        (int)(pManager.status.jumpAttackPow * pManager.status.attackFactor),      //攻撃力
                        pManager.status.jumpAttackKnockPow, //ノックバック
                        pManager.controller.isLeft          //プレイヤーの向き
                        );
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
                    idamage.IDamage(pManager.status.jumpAttackPow);
                }
            }
        }
    }
}
