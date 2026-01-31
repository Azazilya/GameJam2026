using UnityEngine;

public class TokenListUI : MonoBehaviour
{
    private InventoryMaskController inventory;

    void Awake()
    {
        inventory = GetComponentInParent<InventoryMaskController>();
    }

    public void Refresh()
    {
        if (inventory == null) return;

        TokenItemUI[] slots =
            GetComponentsInChildren<TokenItemUI>(true);

        foreach (var slot in slots)
        {
            slot.Refresh(inventory.CurrentMask);
        }
    }
}
