using PlayerScript;
using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public PlayerManager pManager = null;

    public void SendMasageToItemSc(PlayerManager playerManager)
    {
        pManager = playerManager;
        gameObject.SendMessage("SetPlayerManager", playerManager);
    } 
}
