using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public bool isEmpty => item == null;
}
