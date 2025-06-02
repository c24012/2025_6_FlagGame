using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBuriedCheck : MonoBehaviour
{
    [SerializeField] Collider2D groundCollider;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //�n�ʂ̒��ɖ��܂��Ă���Ƃ��͔���𓧉߂�����
        if (collision.CompareTag("Ground"))
        {
            groundCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�n�ʂ̒�����o�Ă����Ƃ��͔�������ɖ߂�
        if (collision.CompareTag("Ground"))
        {   
            groundCollider.isTrigger = false;
        }
    }
}
