using PlayerScript;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] GuiManager guiManager;
    [SerializeField] PlayerData playerData;
    [SerializeField] FighterData fighterData;
    [SerializeField] GameObject flagPf;

    [SerializeField, Header("コンポーネント")] GameObject[] playersObj;

    [SerializeField, Header("関数"), Tooltip("プレイ人数")] int playerCount = 0;
    [SerializeField, Tooltip("制限時間(s)")] int timeLimit = 0;
    [SerializeField, Tooltip("経過時間(s)")] float timeLimit_Counter = 0;
    [SerializeField, Tooltip("表示させる残り時間(s)")] List<int> displayTheTimeList = new();

    public int[] flagLevels = new int[4];

    public bool gameStart = false;

    private void Awake()
    {
        //ゲームシステム設定
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //初期化
        timeLimit_Counter = timeLimit;

        //プレイヤーデータからプレイ人数を取得
        playerCount = playerData.playerCount;

        //プレイヤーごとにファイターと旗を生成
        for (int i = 0; i < playerCount; i++) 
        {
            //ファイター生成&プレイヤー番号を指定
            PlayerManager playerManager 
                = Instantiate(
                    fighterData.allFightersList[playerData.playerFighters[i]],
                    playersObj[i].transform
                ).GetComponent<PlayerManager>();

            playerManager.playerNum = i + 1;

            //旗生成
            FlagState flagState = Instantiate(
                flagPf, playersObj[i].transform
                ).GetComponent<FlagState>();

            flagState.flagNum = i + 1;

        }
    }

    private void Start()
    {
        //ゲーム開始カウントダウン
        guiManager.PlayCountDownAnimation();
    }

    private void Update()
    {
        //タイムリミットカウントダウン
        if (gameStart)
        {
            //タイムリミットカウント
            timeLimit_Counter -= Time.deltaTime;
            //タイムアップ処理
            if (timeLimit_Counter <= 0)
            {
                timeLimit_Counter = 0;
                //タイムアップ処理
                FinishForTimeUp();
                return;
            }
            //時間表示リストに値が残っている時
            if (displayTheTimeList.Count > 0)
            {
                //リストのそれぞれの値を下回った時に起動
                if(timeLimit_Counter <= displayTheTimeList[0])
                {
                    //残り時間表示用の文字列を定義
                    string viewTimeCount;
                    //残り時間が60秒を超えていたら分単位の表示
                    if (displayTheTimeList[0] >= 60)
                    {
                        viewTimeCount = $"残り{displayTheTimeList[0] / 60}分";
                    }
                    //残り時間が60秒を下回ったら秒単位の表示
                    else
                    {
                        viewTimeCount = $"残り{displayTheTimeList[0]}秒";
                    }
                    //カウント表示アニメーション
                    guiManager.PlayNewsFlowAnimation(viewTimeCount);
                    //表示希望リストから表示した値を削除
                    displayTheTimeList.RemoveAt(0);
                }
            }
        }
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void SetGameStart()
    {
        gameStart = true;
    }

    /// <summary>
    /// プレイヤーいずれかが勝利した場合の処理
    /// </summary>
    public void AwardTheWinner(int winnerPlayerNum)
    {
        //ゲームを終了
        gameStart = false;
        //勝利したプレイヤーを配列に変形
        int[] winnerPlayers = new int[1] { winnerPlayerNum };
        //勝利アニメーション
        guiManager.PlayVictoryAnim(winnerPlayers);
    }

    /// <summary>
    /// タイムアップ時の勝者決め
    /// </summary>
    void FinishForTimeUp()
    {
        //ゲームを終了
        gameStart = false;
        //旗レベルの最大値を取得
        int maxLevel = flagLevels.Max();
        //勝者追加用のリストを定義
        List<int> winnerPlayersList = new();
        //旗レベルの最大値に達しているプレイヤーのナンバーをリストに追加
        for (int i = 0; i < playerCount; i++) 
        {
            if (flagLevels[i] == maxLevel)
            {
                winnerPlayersList.Add(i + 1);
            }
        }
        //勝利プレイヤーリストを配列に変形
        int[] winnerPlayers = winnerPlayersList.ToArray();
        //勝利アニメーション
        guiManager.PlayVictoryAnim(winnerPlayers,isTimeUp:true);
    }
}
