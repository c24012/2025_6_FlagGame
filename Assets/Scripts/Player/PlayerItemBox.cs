using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerItemBox : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        [SerializeField] SpriteRenderer sr;
        [SerializeField] Transform srTf;

        [SerializeField] Transform haveItemTf = null;
        public bool inItemBox = false;
        ItemBox sumpleItemBox = null;
        Transform sumpleItemBoxTf = null;
        ItemSO haveItemSO = null;

        private void Awake()
        {
            //アイテムスプライトを空にしておく
            sr.sprite = null;
        }

        public void SetSpriteDirection(bool isLeft)
        {
            if (isLeft)
            {
                sr.flipX = false;
                srTf.localPosition *= -1;
            }
            else
            {
                sr.flipX = true;
                srTf.localPosition *= -1;
            }
        }

        /// <summary>
        /// アイテムボックスからアイテムを取得
        /// </summary>
        public void GetItemForItemBox()
        {
            //箱の中のアイテムSOを取得
            haveItemSO = sumpleItemBox.inItemSO;
            //箱を空にする
            sumpleItemBox.SetEmptyBox();
            //アイテムスプライトの更新
            sr.sprite = haveItemSO.sprite;
        }

        /// <summary>
        /// 持っているアイテムPrefabを生成する
        /// </summary>
        public void InstantiateHaveItem()
        {
            //生成＆使ったプレイヤーの登録
            ItemController itemController =
                Instantiate(haveItemSO.itemPf, pManager.transform).GetComponent<ItemController>();
            itemController.SendMasageToItemSc(pManager);

            //アイテムスプライトを空にする
            sr.sprite = null;
        }

        /// <summary>
        /// アイテムボックスをあさる
        /// </summary>
        public void TryGetItemFromItemBox()
        {
            //アイテムボックスのスプライト変更
            sumpleItemBox.OpenBox();
            //プレイヤーの場所をアイテムボックスの前に来るよう移動
            pManager.transform.position = sumpleItemBoxTf.position;
            pManager.controller.SetIdle();
        }

        /// <summary>
        /// アイテムボックスあさりをキャンセル
        /// </summary>
        public void CancelGetItemFromItemBox()
        {
            sumpleItemBox.CloseBox();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("ItemBox"))
            {
                sumpleItemBox = collision.gameObject.GetComponent<ItemBox>();
                //アイテムボックスの中身があればフラグをON
                if (sumpleItemBox.inItemId != -1)
                {
                    inItemBox = true;
                    sumpleItemBoxTf = collision.transform;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("ItemBox"))
            {
                inItemBox = false;
                sumpleItemBox = null;
                sumpleItemBoxTf = null;
            }
        }

    }
}
