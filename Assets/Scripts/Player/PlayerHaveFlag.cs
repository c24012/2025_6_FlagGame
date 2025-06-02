using UnityEngine;

namespace PlayerScript
{
    public class PlayerHaveFlag : MonoBehaviour
    {
        public PlayerManager pManager;

        public GameObject myFlagObj = null;
        public Rigidbody2D myFlagRb = null;
        public FlagState myFlagState = null;

        public bool inFlagArea = false;//回復エリア内にいるか
        public bool inFlag = false;//旗を持てる範囲内にいるか

        public void LateStart()
        {
            //自分の旗を取得
            GameObject[] flags = GameObject.FindGameObjectsWithTag("Flag");
            foreach(GameObject flagObj in flags)
            {
                //FlagタグのオブジェクトからFlagStateを取得
                if(flagObj.TryGetComponent<FlagState>(out FlagState flagState))
                {
                    //もし自分の番号と旗の番号が一致したら必要なコンポーネントを取得
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
                    Debug.LogError("FlagStateのないFlagタグのオブジェクトが見つかりました");
                }
            }
        }

        public void RaiseAndLowerTheFlag(bool hasFlag)
        {
            pManager.controller.hasFlag = hasFlag;
            if (!hasFlag)
            {
                //持っていた旗のキネマティックをOFF
                if (myFlagRb.isKinematic)
                {
                    myFlagRb.isKinematic = false;
                }
            }
            myFlagState.SetFlagAnimation(hasFlag);
        }

        private void FixedUpdate()
        {
            //旗を持っている状態の処理
            if (pManager.controller.hasFlag)
            {
                //持っている間旗のキネマティックはON
                if (!myFlagRb.isKinematic)
                {
                    myFlagRb.isKinematic = true;
                }
                myFlagObj.transform.position = (Vector2)transform.position + new Vector2(0, 100);

                //旗が破壊されている場合
                if (myFlagState.isBreak)
                {
                    RaiseAndLowerTheFlag(false);
                }
            }
        }

        private void Update()
        {
            //旗を持っている状態の処理
            if (pManager.controller.hasFlag)
            {
                //旗が破壊されている場合
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
                //自分の旗と同じだったら旗内フラグON
                if (collision.gameObject == myFlagObj)
                {
                    inFlag = true;
                }
            }
            if (collision.CompareTag("FlagArea"))
            {
                //このオブジェクトの親が自分の旗と同じだったら旗エリア内フラグON
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
                //自分の旗と同じだったら旗内フラグOFF
                if (collision.gameObject == myFlagObj)
                {
                    inFlag = false;
                }
            }
            if (collision.CompareTag("FlagArea"))
            {
                //このオブジェクトの親が自分の旗と同じだったら旗エリア内フラグOFF
                if (collision.gameObject.transform.parent.gameObject == myFlagObj)
                {
                    inFlagArea = false;
                }
            }
        }
    }
}

