using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SOs/ItemSO")]
public class ItemSO : ScriptableObject
{
    public int itemId = -1;
    public Sprite sprite = null;
    public GameObject itemPf;
}
