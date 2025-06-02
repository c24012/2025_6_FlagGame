using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class FlagState : MonoBehaviour,IPlayerDamage
{
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Sprite[] flagSprites;

    GameObject spawnPointObj;

    [Header("ステータス"),Tooltip("旗プレイヤー番号")]public int flagNum = 0;
    [Tooltip("最大耐久値")] public int maxDurability = 100;
    [SerializeField, Tooltip("ゴールにかかる時間")] public float[] goalTimes = new float[3] { 5, 7, 9 };
    [SerializeField, Tooltip("リスポーン時間")] float respawnTime = 2f;
    [Tooltip("現在旗レベル")]public int flagLevel = 0;

    [SerializeField,Header("確認用"), Tooltip("現在耐久値")] 
    int durability = 100;
    public bool isBreak = false;
    public bool isPutGoal = false;
    

    private void Start()
    {
        //初期化
        Init();

        //旗の色を番号に沿ってに変更
        spriteRenderer.sprite = flagSprites[flagNum - 1];

        //自分のスポーンポイントを取得
        GameObject[] spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject spawnPointObj in spawnPoint)
        {
            //SpawnPointタグのオブジェクトからSpawnPointStateを取得
            if (spawnPointObj.TryGetComponent<SpawnPointState>(out SpawnPointState spawnPointState))
            {
                //もし自分の番号と旗の番号が一致したら必要なコンポーネントを取得
                if (spawnPointState.spawnPointNum == flagNum)
                {
                    this.spawnPointObj = spawnPointObj;
                    break;
                }
            }
            else
            {
                Debug.LogError("SpawnPointStateのないSpawnPointタグのオブジェクトが見つかりました");
            }
        }

        //初期ポジションに移動
        transform.position = spawnPointObj.transform.position;
    }

    private void FixedUpdate()
    {
        if (anim == null) return;
        anim.SetFloat("x",Input.GetAxis(flagNum + "P_Horizontal"));
    }

    /// <summary>
    /// 関数初期化
    /// </summary>
    void Init()
    {
        durability = maxDurability;
    }

    /// <summary>
    /// 持たれている旗の揺れるアニメーションフラグ指定処理
    /// </summary>
    public void SetFlagAnimation(bool hasFlag)
    {
        anim.SetBool("HasFlag", hasFlag);
    }

    /// <summary>
    /// プレイヤーからのダメージ処理
    /// </summary>
    public void IDamage(int damage,float force = 0,bool isLeft = false)
    {
        //耐久値が0以下になる場合は耐久値を0に指定
        if ((durability - damage) <= 0)
        {
            durability = 0;
            BreakAndRespawn(isBreak: true);
        }
        //それ以外は耐久値をDamage分減らす
        else
        {
            durability -= damage;
        }
    }

    /// <summary>
    /// 旗の破壊＆リスポーン処理
    /// </summary>
    public void BreakAndRespawn(bool isBreak)
    {
        //破壊時
        if (isBreak)
        {
            //RigidBodyをOFF
            rb.simulated = false;
            //SpriteRendererをOFF
            spriteRenderer.enabled = false;
            //ポジションを変更
            transform.position = spawnPointObj.transform.position;
            //アニメーションを再生
            StartCoroutine(RespawnAnimation());
            //レベルに合わせたSpriteに変更
            spriteRenderer.sprite = flagSprites[(flagNum - 1) + (flagLevel * 4)];
            //旗破壊フラグをON
            this.isBreak = true;
        }
        //復活時
        else
        {
            //耐久値を最大まで回復
            durability = maxDurability;
            //RigidBodyをON
            rb.simulated = true;
            //SpriteRendererをON
            spriteRenderer.enabled = true;
            //旗破壊フラグをOFF
            this.isBreak = false;
        }
    }

    /// <summary>
    /// リスポーンアニメーション
    /// </summary>
    IEnumerator RespawnAnimation()
    {
        yield return new WaitForSeconds(respawnTime);
        WaitForSeconds wait = new(0.08f);
        for(int i = 0; i < 6; i++)
        {
            spriteRenderer.enabled = true;
            yield return wait;
            spriteRenderer.enabled = false;
            yield return wait;
        }
        BreakAndRespawn(false);
    }

}
