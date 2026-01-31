using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData item;
    public bool isPurchased = false;
    public ConfirmPanel confirmationPanel;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private CanvasGroup purchasedOverlay;


    private void Awake()
    {
        if (confirmationPanel == null)
        {
            confirmationPanel = ShopUI.instance.confirmationCanvas.GetComponent<ConfirmPanel>();
        }
    }
    private void Start()
    {
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (isPurchased)
        {
            purchasedOverlay.alpha = 1;
            purchasedOverlay.blocksRaycasts = true;
        }
        else
        {
            purchasedOverlay.alpha = 0;
            purchasedOverlay.blocksRaycasts = false;
        }
        itemImage.sprite = item.icon;
        itemPriceText.text = item.itemPrice.ToString();
    }

    public void Confirmation(ItemData item)
    {
        ShopUI.instance.confirmationCanvas.GetComponentInChildren<ConfirmPanel>().currentShopSlot = this;
        ShopUI.instance.confirmationCanvas.GetComponentInChildren<ConfirmPanel>().ShowPanel();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!isPurchased) Confirmation(item);
        }
    }
}
