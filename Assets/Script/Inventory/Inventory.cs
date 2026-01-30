using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Singleton<Inventory>
{
    //public int maxSlots = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public GridLayoutGroup content;

    [SerializeField] private TextMeshProUGUI moneyDisplay;
    [SerializeField] private GameObject itemUIPrefab;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Awake()
    {
        if (content == null)
        {
            content = GetComponentInChildren<GridLayoutGroup>();
        }
    }

    public bool AddItem(ItemData item)
    {
        slots.Add(new InventorySlot { item = item});
        return true;
    }

    public bool RemoveItem(ItemData item)
    {
        if (slots.Exists(slot => slot.item == item))
        {
            InventorySlot itemToRemove = slots.Find(slot => slot.item == item);
            slots.Remove(itemToRemove);
            return true;
        }
        return false;
    }

    public void UpdateUI()
    {
        moneyDisplay.text = $"Koin: {CurrencySystem.instance.currentMoney.ToString()}";
        if (spawnedObjects.Count > 0)
        {
            foreach (var obj in spawnedObjects)
            {
                Destroy(obj);
            }
            spawnedObjects.Clear();
        }
        foreach (var item in slots)
        {
            GameObject spawned = Instantiate(itemUIPrefab, content.transform);
            spawned.GetComponent<InventoryItems>().itemData = item.item;
            spawnedObjects.Add(spawned);
        }
    }
}
