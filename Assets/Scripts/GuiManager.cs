using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class GuiManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI newsText;
    [SerializeField] TextMeshProUGUI victoryText;

    [SerializeField] PlayableDirector countDownTL;
    [SerializeField] PlayableDirector newsTL;
    [SerializeField] PlayableDirector timeUpTL;
    [SerializeField] PlayableDirector aloneTL;

    /// <summary>
    /// ゲーム開始カウントダウン
    /// </summary>
    public void PlayCountDownAnimation()
    {
        countDownTL.Play();
    }

    /// <summary>
    /// 残り時間表示テキストアニメーション
    /// </summary>
    public void PlayNewsFlowAnimation(string timeStr)
    {
        newsText.text = timeStr;
        newsTL.Play();
    }

    /// <summary>
    /// ゲーム終了時の勝者表示
    /// </summary>
    public void PlayVictoryAnim(int[] winnerPlayerNum, bool isTimeUp = false)
    {
        //表示用の文字列を定義
        string viewWinnerStr = null;
        //勝利したプレイヤー全員を文字列に入れていく
        for (int i = 0; i < winnerPlayerNum.Length; i++) 
        {
            viewWinnerStr += $"{winnerPlayerNum[i]}P";
            //表示される最後のプレイヤー以外は間に[空白]を入れる
            if(i != winnerPlayerNum.Length - 1) viewWinnerStr += " ";
        }
        //最後に改行してWINを追加
        viewWinnerStr += "\nWIN";

        //完成した文字列を表示テキストに代入
        victoryText.text = viewWinnerStr;

        //終了の理由がタイムアップの場合のアニメーション
        if (isTimeUp)
        {
            timeUpTL.Play();
        }
        //プレイヤーのいずれかがゴールした場合のアニメーション
        else
        {
            aloneTL.Play();
        }
    }
}
