using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerStatus : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        [Header("�ő�̗�")] public int maxHp = 100;
        [Header("�X�s�[�h")] public float speed = 1;
        [Header("�W�����v��")] public float jumpPow = 1;
        [Header("�󒆃W�����v��")] public float airJumpPow = 1;
        [Header("�U����")] public int attackPow = 1;
        [Header("�U���m�b�N�o�b�N")] public float attackKnockPow = 1;
        [Header("�W�����v�U����")] public int jumpAttackPow = 1;
        [Header("�W�����v�U���m�b�N�o�b�N")] public float jumpAttackKnockPow = 1;
        [Header("�󒆃W�����v��")] public int midairJump = 1;
        [Header("���G���A�񕜕p�x(s)")] public float healSpeed = 1;
        [Header("���G���A�񕜗�(hp)")] public int healParOnce = 1;
        [Header("���݂̗̑�"),SerializeField] int hp = 0;

        float healTimeCounter = 0;

        [Header("�X�s�[�h�{��")] public float speedFactor = 1f;
        [Header("�U���͔{��")] public float attackFactor = 1f;
        [Header("�_���[�W�{��")] public float damageFactor = 1f;
        [Header("�W�����v�͔{��")] public float jumpPowFactor = 1f;

        private void Awake()
        {
            //������
            SetMaxHp();
        }

        private void Update()
        {
            //�����̊��������Ă����班������
            if (pManager.controller.hasFlag)
            {
                healTimeCounter += Time.deltaTime;
                //�G���A�؍ݎ��Ԃ��񕜕p�x�̑��x�ɒB�������
                if(healTimeCounter > healSpeed)
                {
                    healTimeCounter = 0;
                    //�񕜎��ő�HP�𒴂���ꍇ�A�ő�HP����
                    if ((hp + healParOnce) > maxHp)
                    {
                        hp = maxHp;
                    }
                    //�����Ȃ��ꍇ�͂��̂܂܉��Z
                    else
                    {
                        hp += healParOnce;
                    }

                    //�\��UI���X�V
                    pManager.guiManager.SetHpRingFillAmount((float)hp / (float)maxHp);
                }
            }
        }

        /// <summary>
        /// HP���ő�܂ŉ�
        /// </summary>
        public void SetMaxHp()
        {
            hp = maxHp;
        }

        /// <summary>
        /// ��_���[�W���HP�̌���
        /// </summary>
        public void ReduceHpForDamage(int damage)
        {
            //HP��0�ȉ��ɂȂ�ꍇ��HP��0�Ɏw���,���S�֐����N��
            if((hp - damage) <= 0)
            {
                hp = 0;
                pManager.controller.DiedOrAlive(isDeid: true);
            }
            //����ȊO��HP��damage�����炷
            else
            {
                hp -= damage;
            }

            //HP��GUI���X�V
            pManager.guiManager.SetHpRingFillAmount((float)hp / (float)maxHp);
        }

    }
}
