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
            //�����̃X�|�[���|�C���g���擾
            GameObject[] spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (GameObject spawnPointObj in spawnPoint)
            {
                //SpawnPoint�^�O�̃I�u�W�F�N�g����SpawnPointState���擾
                if (spawnPointObj.TryGetComponent<SpawnPointState>(out SpawnPointState spawnPointState))
                {
                    //���������̔ԍ��Ɗ��̔ԍ�����v������K�v�ȃR���|�[�l���g���擾
                    if (spawnPointState.spawnPointNum == pManager.playerNum)
                    {
                        this.spawnPointObj = spawnPointObj;
                        break;
                    }
                }
                else
                {
                    Debug.LogError("SpawnPointState�̂Ȃ�SpawnPoint�^�O�̃I�u�W�F�N�g��������܂���");
                }
            }

            //�X�|�[���|�C���g�����ړ�
            transform.position = spawnPointObj.transform.position;
        }

        private void Update()
        {
            //����ł����烊�X�|�[���J�E���g
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

