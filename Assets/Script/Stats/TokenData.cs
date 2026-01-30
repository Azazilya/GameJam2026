using UnityEngine;

[CreateAssetMenu(menuName = "Game/Token")]
public class TokenData : ScriptableObject
{
    public string tokenName;

    [Header("Visual")]
    public Sprite icon;

    [Header("Stat Modifier")]
    public StatType statType;

    [Tooltip("Contoh: 8 = +8%")]
    public float percentValue;

    public int price;
}
