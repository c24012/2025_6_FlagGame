using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerScript
{
    public class PlayerGUIsManager : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        [SerializeField] Canvas canvas;
        [SerializeField] Image hpRing;
        [SerializeField] Image spRing;
        [SerializeField] Image actionRing;
        [SerializeField] Text playerNum;

        [SerializeField] Sprite[] hpRingSprits = new Sprite[3];

        [SerializeField,Header("�ϐ�"),Tooltip("����������")]
        float viewPlayerDataTime = 1f;
        float viewPlayerDataTime_counter = 1f;
        public bool isView = false;

        private void Awake()
        {
            //������
            Init();
            ViewPlayerData(false);
        }

        public void LateStart()
        {
            playerNum.text = pManager.playerNum + "P";
        }

        void Update()
        {
            //�X�V���̈�莞�ԏ��\��
            if (isView)
            {
                if(!hpRing.enabled) ViewPlayerData(true);
                viewPlayerDataTime_counter += Time.deltaTime;
                if(viewPlayerDataTime_counter > viewPlayerDataTime)
                {
                    if (!pManager.controller.isCheck)
                    {
                        ViewPlayerData(false);
                    }
                    viewPlayerDataTime_counter = 0;
                    isView = false;
                }
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        void Init()
        {
            hpRing.fillAmount = 1;
            spRing.fillAmount = 0;
            actionRing.fillAmount = 0;
        }

        /// <summary>
        /// ���GUI��\��,��\��
        /// </summary>
        public void ViewPlayerData(bool isView)
        {
            hpRing.enabled = isView;
            spRing.enabled = isView;
        }

        /// <summary>
        /// Canvas��\��
        /// </summary>
        public void SetCanvasEnable(bool isEnable)
        {
            canvas.enabled = isEnable;
        }

        #region #�e�X�̕\���w��p�֐�
        public void ViewHpRing(bool isView)
        {
            hpRing.enabled = isView;
        }
        public void ViewSpRing(bool isView)
        {
            spRing.enabled = isView;
        }
        public void ViewActionRing(bool isView)
        {
            actionRing.enabled = isView;
        }
        public void ViewPlayerNum(bool isView)
        {
            playerNum.enabled = isView;
        }
        #endregion

        #region #FillAmount����p�֐�
        public void SetHpRingFillAmount(float value)
        {
            hpRing.fillAmount = value;
            //HP�̌�����UI��ς���
            if (hpRing.fillAmount > 0.5f)
            {
                hpRing.sprite = hpRingSprits[0];
            }
            else if (hpRing.fillAmount > 0.25f)
            {
                hpRing.sprite = hpRingSprits[1];
            }
            else
            {
                hpRing.sprite = hpRingSprits[2];
            }

            //�ύX����������GUI����莞�ԕ\��
            if(value != 1)
            {
                isView = true;
                viewPlayerDataTime_counter = 0;
            }
        }

        public void SetSpRingFillAmount(float value)
        {
            spRing.fillAmount = value;
        }

        public void SetActionRingFillAmount(float value)
        {
            actionRing.fillAmount = value;
        }
        #endregion

    }
}

