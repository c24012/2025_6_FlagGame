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
            //�A�C�e���X�v���C�g����ɂ��Ă���
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
        /// �A�C�e���{�b�N�X����A�C�e�����擾
        /// </summary>
        public void GetItemForItemBox()
        {
            //���̒��̃A�C�e��SO���擾
            haveItemSO = sumpleItemBox.inItemSO;
            //������ɂ���
            sumpleItemBox.SetEmptyBox();
            //�A�C�e���X�v���C�g�̍X�V
            sr.sprite = haveItemSO.sprite;
        }

        /// <summary>
        /// �����Ă���A�C�e��Prefab�𐶐�����
        /// </summary>
        public void InstantiateHaveItem()
        {
            //�������g�����v���C���[�̓o�^
            ItemController itemController =
                Instantiate(haveItemSO.itemPf, pManager.transform).GetComponent<ItemController>();
            itemController.SendMasageToItemSc(pManager);

            //�A�C�e���X�v���C�g����ɂ���
            sr.sprite = null;
        }

        /// <summary>
        /// �A�C�e���{�b�N�X��������
        /// </summary>
        public void TryGetItemFromItemBox()
        {
            //�A�C�e���{�b�N�X�̃X�v���C�g�ύX
            sumpleItemBox.OpenBox();
            //�v���C���[�̏ꏊ���A�C�e���{�b�N�X�̑O�ɗ���悤�ړ�
            pManager.transform.position = sumpleItemBoxTf.position;
            pManager.controller.SetIdle();
        }

        /// <summary>
        /// �A�C�e���{�b�N�X��������L�����Z��
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
                //�A�C�e���{�b�N�X�̒��g������΃t���O��ON
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
