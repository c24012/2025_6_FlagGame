using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerSpawn : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        public GameObject spawnPointObj;
        [SerializeField] float respawnTime = 2f;
        float respawnTimeCounter = 0;

        public void LateStart()
        {
            //自分のスポーンポイントを取得
            GameObject[] spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (GameObject spawnPointObj in spawnPoint)
            {
                //SpawnPointタグのオブジェクトからSpawnPointStateを取得
                if (spawnPointObj.TryGetComponent<SpawnPointState>(out SpawnPointState spawnPointState))
                {
                    //もし自分の番号と旗の番号が一致したら必要なコンポーネントを取得
                    if (spawnPointState.spawnPointNum == pManager.playerNum)
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

            //スポーンポイント初期移動
            transform.position = spawnPointObj.transform.position;
        }

        private void Update()
        {
            //死んでいたらリスポーンカウント
            if (pManager.controller.isDeid)
            {
                respawnTimeCounter += Time.deltaTime;
                if (respawnTimeCounter > respawnTime)
                {
                    transform.position = spawnPointObj.transform.position;
                    pManager.status.SetMaxHp();
                    pManager.controller.DiedOrAlive(isDeid:false);
                    respawnTimeCounter = 0;
                }
            }
        }
    }
}

