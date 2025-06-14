using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBuriedCheck : MonoBehaviour
{
    [SerializeField] Collider2D groundCollider;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //地面の中に埋まっているときは判定を透過させる
        if (collision.CompareTag("Ground"))
        {
            groundCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //地面の中から出てきたときは判定を元に戻す
        if (collision.CompareTag("Ground"))
        {   
            groundCollider.isTrigger = false;
        }
    }
}
