using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;
    private bool isOpen;

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        isOpen = true;
        EventCenter.CallBaseBagOpenEvent(SlotType.Shop, shopData);
        EventCenter.CallUpdateGameStateEvent(GameState.Pause);
    }

    public void CloseShop()
    {
        isOpen = false;
        EventCenter.CallBaseBagCloseEvent(SlotType.Shop, shopData);
        EventCenter.CallUpdateGameStateEvent(GameState.Gameplay);
    }
}
