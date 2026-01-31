using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public int itemPrice = 0;
    public enum StatIncreaseType
    {
        Attack,
        Defense,
        Speed
    }
    public StatIncreaseType statIncreaseType;
    public int statIncreaseAmount = 0;
}
