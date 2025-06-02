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

        [Header("�e�̑��x"), SerializeField] float bulletSpeed;
        [Header("�e�̑؍ݎ���"), SerializeField] float bulletTime;
        [Header("�U���̎��"), SerializeField] AttackType attackType;

        const int speedPow = 10000;//���x�������p
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
            //�U������Ƀv���C���[������������
            if (collider.CompareTag("Player"))
            {
                //���̃v���C���[��IDamage���U���͂��̂��ċN��
                if (collider.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
                {
                    if (attackType == AttackType.Attack)
                    {
                        idamage.IDamage(
                            (int)(pManager.status.attackPow * pManager.status.attackFactor),      //�U����
                            pManager.status.attackKnockPow, //�m�b�N�o�b�N
                            isLeft                          //�e�ۂ̌���
                        );
                    }
                    else if (attackType == AttackType.JumpAttack) 
                    {
                        idamage.IDamage(
                            (int)(pManager.status.jumpAttackPow * pManager.status.attackFactor),      //�U����
                            pManager.status.jumpAttackKnockPow, //�m�b�N�o�b�N
                            isLeft                              //�e�ۂ̌���
                        );
                    }
                    DestroyBullet(0);
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
                    DestroyBullet(0);
                }
            }

            //�U������ɒn�ʂ�ǂɓ���������
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

