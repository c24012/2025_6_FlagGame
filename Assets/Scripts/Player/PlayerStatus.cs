using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerStatus : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        [Header("最大体力")] public int maxHp = 100;
        [Header("スピード")] public float speed = 1;
        [Header("ジャンプ力")] public float jumpPow = 1;
        [Header("空中ジャンプ力")] public float airJumpPow = 1;
        [Header("攻撃力")] public int attackPow = 1;
        [Header("攻撃ノックバック")] public float attackKnockPow = 1;
        [Header("ジャンプ攻撃力")] public int jumpAttackPow = 1;
        [Header("ジャンプ攻撃ノックバック")] public float jumpAttackKnockPow = 1;
        [Header("空中ジャンプ回数")] public int midairJump = 1;
        [Header("旗エリア回復頻度(s)")] public float healSpeed = 1;
        [Header("旗エリア回復量(hp)")] public int healParOnce = 1;
        [Header("現在の体力"),SerializeField] int hp = 0;

        float healTimeCounter = 0;

        [Header("スピード倍率")] public float speedFactor = 1f;
        [Header("攻撃力倍率")] public float attackFactor = 1f;
        [Header("ダメージ倍率")] public float damageFactor = 1f;
        [Header("ジャンプ力倍率")] public float jumpPowFactor = 1f;

        private void Awake()
        {
            //初期化
            SetMaxHp();
        }

        private void Update()
        {
            //自分の旗を持っていたら少しずつ回復
            if (pManager.controller.hasFlag)
            {
                healTimeCounter += Time.deltaTime;
                //エリア滞在時間が回復頻度の速度に達したら回復
                if(healTimeCounter > healSpeed)
                {
                    healTimeCounter = 0;
                    //回復時最大HPを超える場合、最大HPを代入
                    if ((hp + healParOnce) > maxHp)
                    {
                        hp = maxHp;
                    }
                    //超えない場合はそのまま加算
                    else
                    {
                        hp += healParOnce;
                    }

                    //表示UIを更新
                    pManager.guiManager.SetHpRingFillAmount((float)hp / (float)maxHp);
                }
            }
        }

        /// <summary>
        /// HPを最大まで回復
        /// </summary>
        public void SetMaxHp()
        {
            hp = maxHp;
        }

        /// <summary>
        /// 被ダメージよるHPの減少
        /// </summary>
        public void ReduceHpForDamage(int damage)
        {
            //HPが0以下になる場合はHPを0に指定後,死亡関数を起動
            if((hp - damage) <= 0)
            {
                hp = 0;
                pManager.controller.DiedOrAlive(isDeid: true);
            }
            //それ以外はHPをdamage分減らす
            else
            {
                hp -= damage;
            }

            //HPのGUIを更新
            pManager.guiManager.SetHpRingFillAmount((float)hp / (float)maxHp);
        }

    }
}
