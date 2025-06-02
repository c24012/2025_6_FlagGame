using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerGroundCheck : MonoBehaviour
    {
        [NonSerialized] public PlayerManager pManager;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //è„è∏íÜÇ≈Ç»Ç¢èÍçáÇÃÇ›ínñ îªíË
            if (!pManager.controller.isRise)
            {
                pManager.controller.SetIsGround(collision, isIn: true);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            //è„è∏íÜÇ≈Ç»Ç¢èÍçáÇÃÇ›ínñ îªíË
            if (!pManager.controller.isRise)
            {
                pManager.controller.SetIsGround(collision, isIn: true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            pManager.controller.SetIsGround(collision, isIn: false);
        }
    }
}

