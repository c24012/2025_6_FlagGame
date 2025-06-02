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

        [SerializeField,Header("変数"),Tooltip("情報可視化時間")]
        float viewPlayerDataTime = 1f;
        float viewPlayerDataTime_counter = 1f;
        public bool isView = false;

        private void Awake()
        {
            //初期化
            Init();
            ViewPlayerData(false);
        }

        public void LateStart()
        {
            playerNum.text = pManager.playerNum + "P";
        }

        void Update()
        {
            //更新時の一定時間情報表示
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
        /// 初期化
        /// </summary>
        void Init()
        {
            hpRing.fillAmount = 1;
            spRing.fillAmount = 0;
            actionRing.fillAmount = 0;
        }

        /// <summary>
        /// 情報GUIを表示,非表示
        /// </summary>
        public void ViewPlayerData(bool isView)
        {
            hpRing.enabled = isView;
            spRing.enabled = isView;
        }

        /// <summary>
        /// Canvas非表示
        /// </summary>
        public void SetCanvasEnable(bool isEnable)
        {
            canvas.enabled = isEnable;
        }

        #region #各々の表示指定用関数
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

        #region #FillAmount代入用関数
        public void SetHpRingFillAmount(float value)
        {
            hpRing.fillAmount = value;
            //HPの減り具合でUIを変える
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

            //変更があったらGUIを一定時間表示
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

