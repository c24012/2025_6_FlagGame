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
        public bool inGoal = false;//�S�[���͈͓��ɂ��邩

        public void LateStart()
        {
            //�S�[���I�u�W�F�N�g�ƃS�[���X�N���v�g���擾
            goalObj = GameObject.FindGameObjectWithTag("GoalPoint");
            goalPoint = goalObj.GetComponent<GoalPoint>();
        }

        public void PutGoalFlag()
        {
            //�N���S�[���|�C���g���g���Ă��Ȃ�������PlayerManager��o�^
            if(goalPoint.pManager == null)
            {
                goalPoint.pManager = this.pManager;
            }
            else return;

            //�����Ă��������S�[���I�u�W�F�N�g�̏�ɐݒu
            pManager.haveFlag.myFlagObj.transform.position =
                (Vector2)goalObj.transform.position + new Vector2(0, 70);

            //���̃S�[���ɒu����Ă��邩�t���O��ON
            pManager.haveFlag.myFlagState.isPutGoal = true;

            //�S�[���J�E���g�_�E���J�n
            goalPoint.StartCountDown();
        }

        public void Goal()
        {
            //���̃��x�����P�グ��
            pManager.haveFlag.myFlagState.flagLevel++;

            //�Q�[���}�l�[�W���[�̊����x���z��ɂ����
            pManager.gameManager.flagLevels[pManager.playerNum - 1] = pManager.haveFlag.myFlagState.flagLevel;

            //�Q�[���I�����m�F
            if (pManager.haveFlag.myFlagState.flagLevel == 3)
            {
                //�v���C���[��ҋ@��Ԃɂ���
                pManager.controller.SetIdle();

                //�Q�[���I������
                pManager.gameObject.transform.position = goalObj.transform.GetChild(0).transform.position;
                pManager.gameManager.AwardTheWinner(pManager.playerNum);
                return;
            }

            //���I�u�W�F�N�g�̔j��
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
