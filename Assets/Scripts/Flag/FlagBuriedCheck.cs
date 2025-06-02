using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBuriedCheck : MonoBehaviour
{
    [SerializeField] Collider2D groundCollider;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //’n–Ê‚Ì’†‚É–„‚Ü‚Á‚Ä‚¢‚é‚Æ‚«‚Í”»’è‚ğ“§‰ß‚³‚¹‚é
        if (collision.CompareTag("Ground"))
        {
            groundCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //’n–Ê‚Ì’†‚©‚ço‚Ä‚«‚½‚Æ‚«‚Í”»’è‚ğŒ³‚É–ß‚·
        if (collision.CompareTag("Ground"))
        {   
            groundCollider.isTrigger = false;
        }
    }
}
