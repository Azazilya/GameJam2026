using UnityEngine;

public class TokenButton : MonoBehaviour
{
    public TokenData token;
    public MaskManager maskManager;

    public void OnClickEquip()
    {
        if (maskManager == null || token == null) return;

        TokenManager.Instance.EquipToken(
            maskManager.CurrentMask,
            token
        );
    }
}
