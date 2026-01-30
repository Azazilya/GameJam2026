using UnityEngine;
using UnityEngine.UI;

public class TokenSlotUI : MonoBehaviour
{
    public TokenData token;
    public Image icon;

    public MaskManager maskManager;

    bool equipped;

    void Start()
    {
        icon.sprite = token.icon;
        icon.color = Color.black;
    }

    public void OnClick()
    {
        MaskData mask = maskManager.CurrentMask;

        if (!equipped)
        {
            if (!mask.CanEquipToken())
                return;

            mask.equippedTokens.Add(token);
            icon.color = Color.red;
            equipped = true;
        }
        else
        {
            mask.equippedTokens.Remove(token);
            icon.color = Color.black;
            equipped = false;
        }

        maskManager.SendMessage("ApplyCurrentMask", SendMessageOptions.DontRequireReceiver);
    }
}
