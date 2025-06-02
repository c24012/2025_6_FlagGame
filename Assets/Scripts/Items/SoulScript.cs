using UnityEngine;
using PlayerScript;
using UnityEngine.U2D.IK;

[RequireComponent(typeof(ItemController))]
public class SoulScript : ItemController
{
    [SerializeField] float speedFactor = 1f;
    [SerializeField] float attackFactor = 1f;
    [SerializeField] float damageFactor = 1f;
    [SerializeField] float jumpPowFactor = 1f;

    [SerializeField] float effectTime = 8f;
    float effectTime_counter = 0;

    public void SetPlayerManager(PlayerManager playerManager)
    {
        pManager = playerManager;
    }

    private void Start()
    {
        pManager.status.speedFactor *= speedFactor;
        pManager.status.attackFactor *= attackFactor;
        pManager.status.damageFactor *= damageFactor;
        pManager.status.jumpPowFactor *= jumpPowFactor;
    }

    public void FinishEffect()
    {
        pManager.status.speedFactor /= speedFactor;
        pManager.status.attackFactor /= attackFactor;
        pManager.status.damageFactor /= damageFactor;
        pManager.status.jumpPowFactor /= jumpPowFactor;
    }

    private void Update()
    {
        effectTime_counter += Time.deltaTime;
        if(effectTime_counter > effectTime)
        {
            FinishEffect();
            Destroy(gameObject);
        }
        else if (pManager.controller.isDeid)
        {
            FinishEffect();
            Destroy(gameObject);
        }
    }
}
