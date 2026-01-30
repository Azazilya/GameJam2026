using UnityEngine;

public class TokenManager : MonoBehaviour
{
    public static TokenManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public bool EquipToken(MaskData mask, TokenData token)
    {
        if (mask.equippedTokens.Count >= 4)
            return false;

        mask.equippedTokens.Add(token); // BOLEH SAMA
        return true;
    }

    public void UnequipToken(MaskData mask, int index)
    {
        if (index < 0 || index >= mask.equippedTokens.Count) return;
        mask.equippedTokens.RemoveAt(index);
    }
}
