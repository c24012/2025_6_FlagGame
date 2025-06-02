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
        //ランダムなアイテムを挿入
        SetItemInBox();
    }

    /// <summary>
    ///ランダムなアイテムを挿入
    /// </summary>
    void SetItemInBox()
    {
        col.enabled = true;
        inItemId = Random.Range(0, itemsData.allItemList.Length);
        inItemSO = itemsData.allItemList[inItemId];
        CloseBox();
    }

    /// <summary>
    /// 中身を空にする
    /// </summary>
    public void SetEmptyBox()
    {
        col.enabled = false;
        inItemId = -1;
        inItemSO = null;
        OpenBox();

        //一定時間後に宝箱が復活
        Invoke(nameof(SetItemInBox), Random.Range(restoredTime_min, restoredTime_max));
    }

    /// <summary>
    /// 箱の見た目を開いている状態にする
    /// </summary>
    public void OpenBox()
    {
        sr.sprite = boxSprite_open;
    }

    /// <summary>
    /// 箱の見た目を閉じている状態にする
    /// </summary>
    public void CloseBox()
    {
        sr.sprite = boxSprite_close;
    }
}
