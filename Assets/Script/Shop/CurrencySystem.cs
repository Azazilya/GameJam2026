using System.Collections.Generic;
using UnityEngine;

public class CurrencySystem : Singleton<CurrencySystem>
{
    public int currentMoney = 0;
    public delegate void OnMoneyChange();
    public OnMoneyChange OnMoneyChanged;

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"Added {amount} money. Current money: {currentMoney}");
        OnMoneyChanged?.Invoke();
    }
    public void RemoveMoney(int amount)
    {
        currentMoney -= amount;
        Debug.Log($"Removed {amount} money. Current money: {currentMoney}");
        OnMoneyChanged?.Invoke();
    }
}
