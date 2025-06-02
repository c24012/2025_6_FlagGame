using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite boxSprite_open;
    [SerializeField] Sprite boxSprite_close;
    [SerializeField] ItemsData itemsData;
    [SerializeField] Collider2D col;

    [SerializeField] float restoredTime_min = 1f;
    [SerializeField] float restoredTime_max = 2f;

    public int inItemId = -1;
    public ItemSO inItemSO;

    private void Start()
    {
        //�����_���ȃA�C�e����}��
        SetItemInBox();
    }

    /// <summary>
    ///�����_���ȃA�C�e����}��
    /// </summary>
    void SetItemInBox()
    {
        col.enabled = true;
        inItemId = Random.Range(0, itemsData.allItemList.Length);
        inItemSO = itemsData.allItemList[inItemId];
        CloseBox();
    }

    /// <summary>
    /// ���g����ɂ���
    /// </summary>
    public void SetEmptyBox()
    {
        col.enabled = false;
        inItemId = -1;
        inItemSO = null;
        OpenBox();

        //��莞�Ԍ�ɕ󔠂�����
        Invoke(nameof(SetItemInBox), Random.Range(restoredTime_min, restoredTime_max));
    }

    /// <summary>
    /// ���̌����ڂ��J���Ă����Ԃɂ���
    /// </summary>
    public void OpenBox()
    {
        sr.sprite = boxSprite_open;
    }

    /// <summary>
    /// ���̌����ڂ���Ă����Ԃɂ���
    /// </summary>
    public void CloseBox()
    {
        sr.sprite = boxSprite_close;
    }
}
