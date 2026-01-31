using DG.Tweening;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;

    public ShopSlot currentShopSlot;

    public void ShowPanel()
    {
        Debug.Log($"curren shop slot = {currentShopSlot}");
        UpdateUI();
        Sequence tween = DOTween.Sequence();
        tween
            .Append(GetComponentInParent<CanvasGroup>().DOFade(1, 0.25f))
            .Join(transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutBack))
            .AppendCallback(() => GetComponentInParent<CanvasGroup>().interactable = true)
            .JoinCallback(() => GetComponentInParent<CanvasGroup>().blocksRaycasts = true);
    }

    public void HidePanel()
    {
        Sequence tween = DOTween.Sequence();
        tween
            .AppendCallback(() => GetComponentInParent<CanvasGroup>().interactable = false)
            .Append(GetComponentInParent<CanvasGroup>().DOFade(0, 0.25f))
            .Join(transform.DOScale(Vector3.zero, 0.25f).From(Vector3.one).SetEase(Ease.OutBack))
            .AppendCallback(() => GetComponentInParent<CanvasGroup>().blocksRaycasts = false);
    }

    public void ConfirmPurchase()
    {
        if (CurrencySystem.instance.currentMoney < currentShopSlot.item.itemPrice)
        {
            Debug.Log("Not enough money!");
            UIManager.instance.ShowWarningText("Not enough money!");
            HidePanel();
            return;
        }
        Inventory.instance.AddItem(currentShopSlot.item);
        currentShopSlot.isPurchased = true;
        CurrencySystem.instance.RemoveMoney(currentShopSlot.item.itemPrice);
        currentShopSlot.UpdateSlotUI();
        HidePanel();
    }

    public void UpdateUI()
    {
        itemImage.sprite = currentShopSlot.item.icon;
        itemPrice.text = currentShopSlot.item.itemPrice.ToString();
        string increaseType = "";
        switch (currentShopSlot.item.statIncreaseType)
        {
            case ItemData.StatIncreaseType.Attack:
                increaseType = "Attack";
                break;
            case ItemData.StatIncreaseType.Defense:
                increaseType = "Defense";
                break;
            case ItemData.StatIncreaseType.Speed:
                increaseType = "Speed";
                break;
            default:
                itemPrice.text += "Missing Reference err.";
                break;
        }
        itemDescription.text = $"{increaseType} +{currentShopSlot.item.statIncreaseAmount}";
    }

}
