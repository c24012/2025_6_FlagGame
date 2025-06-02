using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerGoalManager : MonoBehaviour
    {
        public PlayerManager pManager;

        GameObject goalObj;
        public GoalPoint goalPoint;
        public bool inGoal = false;//ゴール範囲内にいるか

        public void LateStart()
        {
            //ゴールオブジェクトとゴールスクリプトを取得
            goalObj = GameObject.FindGameObjectWithTag("GoalPoint");
            goalPoint = goalObj.GetComponent<GoalPoint>();
        }

        public void PutGoalFlag()
        {
            //誰もゴールポイントを使っていない時だけPlayerManagerを登録
            if(goalPoint.pManager == null)
            {
                goalPoint.pManager = this.pManager;
            }
            else return;

            //持っていた旗をゴールオブジェクトの上に設置
            pManager.haveFlag.myFlagObj.transform.position =
                (Vector2)goalObj.transform.position + new Vector2(0, 70);

            //旗のゴールに置かれているかフラグをON
            pManager.haveFlag.myFlagState.isPutGoal = true;

            //ゴールカウントダウン開始
            goalPoint.StartCountDown();
        }

        public void Goal()
        {
            //旗のレベルを１上げる
            pManager.haveFlag.myFlagState.flagLevel++;

            //ゲームマネージャーの旗レベル配列にも代入
            pManager.gameManager.flagLevels[pManager.playerNum - 1] = pManager.haveFlag.myFlagState.flagLevel;

            //ゲーム終了か確認
            if (pManager.haveFlag.myFlagState.flagLevel == 3)
            {
                //プレイヤーを待機状態にする
                pManager.controller.SetIdle();

                //ゲーム終了処理
                pManager.gameObject.transform.position = goalObj.transform.GetChild(0).transform.position;
                pManager.gameManager.AwardTheWinner(pManager.playerNum);
                return;
            }

            //旗オブジェクトの破壊
            pManager.haveFlag.myFlagState.BreakAndRespawn(isBreak: true);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("GoalPoint"))
            {
                inGoal = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("GoalPoint"))
            {
                inGoal = false;
            }
        }
    }
}
