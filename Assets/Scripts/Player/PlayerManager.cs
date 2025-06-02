using UnityEngine;

namespace PlayerScript
{
    public class PlayerManager : MonoBehaviour
    {
        public GameManager gameManager;

        public PlayerController controller;
        public PlayerHaveFlag haveFlag;
        public PlayerGUIsManager guiManager;
        public PlayerGroundCheck groundChack;
        public PlayerBody body;
        public PlayerAttack attack;
        public PlayerJumpAttack jumpAttack;
        public PlayerStatus status;
        public PlayerSpawn spawn;
        public PlayerGoalManager goal;
        public PlayerItemBox itemBox;

        [SerializeField,Header("レイヤー変更Obj")] GameObject bodyObj;
        [SerializeField] GameObject objectColObj;
        [SerializeField] GameObject attackObj;
        [SerializeField] GameObject jumpAttackObj;


        [SerializeField, Header("変数"), Tooltip("Player番号")]
        public int playerNum = 0;
        public int layerNum = 0;

        private void Start()
        {
            //初期化
            SetComponent();

            //レイヤーをプレイヤー番号を参照して設定
            switch (playerNum)
            {
                case 0:
                    layerNum = LayerMask.NameToLayer("Player");//Debug
                    break;
                case 1:
                    layerNum = LayerMask.NameToLayer("1P");
                    break;
                case 2:
                    layerNum = LayerMask.NameToLayer("2P");
                    break;
                case 3:
                    layerNum = LayerMask.NameToLayer("3P");
                    break;
                case 4:
                    layerNum = LayerMask.NameToLayer("4P");
                    break;
                default:
                    Debug.LogError("プレイヤー番号が範囲外です");
                    break;
            }
            bodyObj.layer = layerNum;
            objectColObj.layer = layerNum;
            attackObj.layer = layerNum;
            jumpAttackObj.layer = layerNum;
        }

        private void SetComponent()
        {
            //GameManagerを探して登録
            GameObject gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
            gameManager = gameManagerObj.GetComponent<GameManager>();

            //各々のスクリプトにマネージャーを登録
            controller.pManager = this;
            haveFlag.pManager = this;
            guiManager.pManager = this;
            groundChack.pManager = this;
            body.pManager = this;
            attack.pManager = this;
            jumpAttack.pManager = this;
            status.pManager = this;
            spawn.pManager = this;
            goal.pManager = this;
            itemBox.pManager = this;

            //各々のスクリプトの初期化関数を起動
            controller.LateStart();
            haveFlag.LateStart();
            guiManager.LateStart();
            spawn.LateStart();
            goal.LateStart();
        }
    }
}

