using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerAttack : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        public CapsuleCollider2D attackCol;

        private void Awake()
        {
            //������
            attackCol.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            //�U������Ƀv���C���[������������
            if (collider.CompareTag("Player"))
            {
                //���̃v���C���[��IDamage���U���͂��̂��ċN��
                if(collider.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
                {
                    idamage.IDamage(
                        (int)(pManager.status.attackPow * pManager.status.attackFactor),      //�U����
                        pManager.status.attackKnockPow, //�m�b�N�o�b�N
                        pManager.controller.isLeft      //�v���C���[�̌���
                        );
                }
            }

            //�U������Ɋ��I�u�W�F�N�g������������
            if (collider.CompareTag("Flag"))
            {
                //���I�u�W�F�N�g���������I�u�W�F�N�g�ƈ�v�����甲����
                if (collider.gameObject == pManager.haveFlag.myFlagObj) return;

                //���v���C���[�̊��̏ꍇ,���̊���IDamage���U���͂��̂��ċN��
                if (collider.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
                {
                    idamage.IDamage(pManager.status.attackPow);
                }
            }
        }
    }
}
