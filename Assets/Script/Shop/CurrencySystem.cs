using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    public int currentMoney = 0;

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"Added {amount} money. Current money: {currentMoney}");
    }
    public void RemoveMoney(int amount)
    {
        currentMoney -= amount;
        Debug.Log($"Removed {amount} money. Current money: {currentMoney}");
    }
}
