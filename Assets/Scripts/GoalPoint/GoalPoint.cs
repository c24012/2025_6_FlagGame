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
        //�J�E���g�_�E���t���O��ON
        isStartCountDown = true;
        //���̃��x���ɂ��S�[���ɕK�v�Ȏ��Ԃ��擾
        goalTime = pManager.haveFlag.myFlagState.goalTimes[pManager.haveFlag.myFlagState.flagLevel];
    }

    private void Update()
    {
        if (isStartCountDown)
        {
            goalTime_counter += Time.deltaTime;
            //��莞�Ԏ��؂���
            if(goalTime < goalTime_counter)
            {
                //���̃S�[���ɒu����Ă��邩�t���O��OFF
                pManager.haveFlag.myFlagState.isPutGoal = false;

                pManager.goal.Goal();
                goalTime_counter = 0;
                goalTime = 0;
                isStartCountDown = false;
                pManager = null;
            }
            //�r���Ŕj�󂳂ꂽ
            else if (pManager.haveFlag.myFlagState.isBreak)
            {
                //���̃S�[���ɒu����Ă��邩�t���O��OFF
                pManager.haveFlag.myFlagState.isPutGoal = false;

                goalTime_counter = 0;
                goalTime = 0;
                isStartCountDown = false;
                pManager = null;
            }
        }
    }
}
