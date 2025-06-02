using PlayerScript;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    public PlayerManager pManager = null;

    bool isStartCountDown = false;
    float goalTime = 0;
    float goalTime_counter = 0;

    public void StartCountDown()
    {
        //カウントダウンフラグをON
        isStartCountDown = true;
        //旗のレベルによるゴールに必要な時間を取得
        goalTime = pManager.haveFlag.myFlagState.goalTimes[pManager.haveFlag.myFlagState.flagLevel];
    }

    private void Update()
    {
        if (isStartCountDown)
        {
            goalTime_counter += Time.deltaTime;
            //一定時間守り切った
            if(goalTime < goalTime_counter)
            {
                //旗のゴールに置かれているかフラグをOFF
                pManager.haveFlag.myFlagState.isPutGoal = false;

                pManager.goal.Goal();
                goalTime_counter = 0;
                goalTime = 0;
                isStartCountDown = false;
                pManager = null;
            }
            //途中で破壊された
            else if (pManager.haveFlag.myFlagState.isBreak)
            {
                //旗のゴールに置かれているかフラグをOFF
                pManager.haveFlag.myFlagState.isPutGoal = false;

                goalTime_counter = 0;
                goalTime = 0;
                isStartCountDown = false;
                pManager = null;
            }
        }
    }
}
