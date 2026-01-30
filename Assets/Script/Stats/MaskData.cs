using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Mask")]
public class MaskData : ScriptableObject
{
    [Header("Identity")]
    public string maskName;

    [Header("Visual")]
    public Sprite icon;  
    [Header("Mask Base Stats")]
    public float baseAttack;
    public float baseDefense;
    public float baseMoveSpeed;
    public float baseAttackSpeed;

    [Header("Equipped Tokens (Max 4)")]
    public List<TokenData> equippedTokens = new List<TokenData>();

    public bool CanEquipToken()
    {
        return equippedTokens.Count < 4;
    }
}
