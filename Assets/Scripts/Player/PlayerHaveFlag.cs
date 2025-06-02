using UnityEngine;

namespace PlayerScript
{
    public class PlayerHaveFlag : MonoBehaviour
    {
        public PlayerManager pManager;

        public GameObject myFlagObj = null;
        public Rigidbody2D myFlagRb = null;
        public FlagState myFlagState = null;

        public bool inFlagArea = false;//�񕜃G���A���ɂ��邩
        public bool inFlag = false;//�������Ă�͈͓��ɂ��邩

        public void LateStart()
        {
            //�����̊����擾
            GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");
            foreach(GameObject flagObj in flags)
            {
                //Flag�^�O�̃I�u�W�F�N�g����FlagState���擾
                if(flagObj.TryGetComponent<FlagState>(out FlagState flagState))
                {
                    //���������̔ԍ��Ɗ��̔ԍ�����v������K�v�ȃR���|�[�l���g���擾
                    if(flagState.flagNum == pManager.playerNum)
                    {
                        myFlagObj = flagObj;
                        myFlagRb = flagObj.GetComponent<Rigidbody2D>();
                        myFlagState = flagState;
                        break;
                    }
                }
                else
                {
                    Debug.LogError("FlagState�̂Ȃ�Flag�^�O�̃I�u�W�F�N�g��������܂���");
                }
            }
        }

        public void RaiseAndLowerTheFlag(bool hasFlag)
        {
            pManager.controller.hasFlag = hasFlag;
            if (!hasFlag)
            {
                //�����Ă������̃L�l�}�e�B�b�N��OFF
                if (myFlagRb.isKinematic)
                {
                    myFlagRb.isKinematic = false;
                }
            }
            myFlagState.SetFlagAnimation(hasFlag);
        }

        private void FixedUpdate()
        {
            //���������Ă����Ԃ̏���
            if (pManager.controller.hasFlag)
            {
                //�����Ă���Ԋ��̃L�l�}�e�B�b�N��ON
                if (!myFlagRb.isKinematic)
                {
                    myFlagRb.isKinematic = true;
                }
                myFlagObj.transform.position = (Vector2)transform.position + new Vector2(0, 100);

                //�����j�󂳂�Ă���ꍇ
                if (myFlagState.isBreak)
                {
                    RaiseAndLowerTheFlag(false);
                }
            }
        }

        private void Update()
        {
            //���������Ă����Ԃ̏���
            if (pManager.controller.hasFlag)
            {
                //�����j�󂳂�Ă���ꍇ
                if (myFlagState.isBreak)
                {
                    RaiseAndLowerTheFlag(false);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Flag"))
            {
                //�����̊��Ɠ���������������t���OON
                if (collision.gameObject == myFlagObj)
                {
                    inFlag = true;
                }
            }
            if (collision.CompareTag("FlagArea"))
            {
                //���̃I�u�W�F�N�g�̐e�������̊��Ɠ�������������G���A���t���OON
                if (collision.gameObject.transform.parent.gameObject == myFlagObj)
                {
                    inFlagArea = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Flag"))
            {
                //�����̊��Ɠ���������������t���OOFF
                if (collision.gameObject == myFlagObj)
                {
                    inFlag = false;
                }
            }
            if (collision.CompareTag("FlagArea"))
            {
                //���̃I�u�W�F�N�g�̐e�������̊��Ɠ�������������G���A���t���OOFF
                if (collision.gameObject.transform.parent.gameObject == myFlagObj)
                {
                    inFlagArea = false;
                }
            }
        }
    }
}

