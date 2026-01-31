using UnityEngine;

public class InventoryUIdeprecated : MonoBehaviour
{
    public GameObject inventoryPanel;

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
