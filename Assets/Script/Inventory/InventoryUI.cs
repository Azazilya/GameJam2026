using DG.Tweening;
using UnityEngine;

public class InventoryUI : Singleton<InventoryUI>
{
    public CanvasGroup inventoryHUD;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private float fadeDuration = 0.25f;

    private void Awake()
    {
        if (inventoryHUD == null)
        {
            inventoryHUD = GetComponentInChildren<CanvasGroup>();
        }
        if (Inventory == null)
        {
            Inventory = GetComponentInChildren<Inventory>();
        }
    }

    public void OpenShop()
    {
        Inventory.UpdateUI();
        Sequence tween = DOTween.Sequence();
        tween
            .Append(inventoryHUD.DOFade(1, fadeDuration))
            .Join(Inventory.transform.DOScale(Vector3.one, fadeDuration).From(Vector3.zero).SetEase(Ease.OutBack))
            .AppendCallback(() => inventoryHUD.interactable = true)
            .JoinCallback(() => inventoryHUD.blocksRaycasts = true);
        // TODO: add player movement disable
    }

    public void CloseShop()
    {
        Sequence tween = DOTween.Sequence();
        tween
            .AppendCallback(() => inventoryHUD.interactable = false)
            .Append(inventoryHUD.DOFade(0, fadeDuration))
            .Join(Inventory.transform.DOScale(Vector3.zero, fadeDuration).From(Vector3.one).SetEase(Ease.OutBack))
            .AppendCallback(() => inventoryHUD.blocksRaycasts = false);
        // TODO: add player movement enable
    }
}
