using System;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerBody : MonoBehaviour, IPlayerDamage
    {
        [NonSerialized] public PlayerManager pManager;

        public void IDamage(int damage, float force, bool isLeft)
        {
            //‡ŠÔˆÈŠO‚Í•Ô‹p
            if (!pManager.gameManager.gameStart) return;

            if (pManager.controller.isInvincible) return;

            pManager.status.ReduceHpForDamage((int)(damage * pManager.status.damageFactor));

            pManager.controller.DamageMotion(force, isLeft);
        }
    }
}
