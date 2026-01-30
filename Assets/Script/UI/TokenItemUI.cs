using UnityEngine;
using UnityEngine.UI;

public class TokenItemUI : MonoBehaviour
{
    public int slotIndex;
    public Image icon;

    public void Refresh(MaskData mask)
    {
        if (mask.equippedTokens.Count > slotIndex)
        {
            icon.enabled = true;
            icon.sprite = mask.equippedTokens[slotIndex].icon;
        }
        else
        {
            icon.enabled = false;
            icon.sprite = null;
        }
    }

    public void OnClick()
    {
        InventoryMaskController inv =
            GetComponentInParent<InventoryMaskController>();

        if (inv == null) return;

        TokenManager.Instance.UnequipToken(
            inv.CurrentMask, slotIndex);

        Refresh(inv.CurrentMask);
    }
}
