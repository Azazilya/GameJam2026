using DG.Tweening;
using UnityEngine;

public class ShopUI : Singleton<ShopUI>
{
    public CanvasGroup shopHUD;
    public CanvasGroup confirmationCanvas;
    [SerializeField] private Shop shop;
    [SerializeField] private float fadeDuration = 0.25f;

    private void Awake()
    {
        if (shopHUD == null)
        {
            shopHUD = GetComponentInChildren<CanvasGroup>();
        }
        if (shop == null)
        {
            shop = GetComponentInChildren<Shop>();
        }
    }

    public void OpenShop()
    {
        Sequence tween = DOTween.Sequence();
        tween
            .Append(shopHUD.DOFade(1, fadeDuration))
            .Join(shop.transform.DOScale(Vector3.one, fadeDuration).From(Vector3.zero).SetEase(Ease.OutBack))
            .AppendCallback(() => shopHUD.interactable = true)
            .JoinCallback(() => shopHUD.blocksRaycasts = true);
        // TODO: add player movement disable
    }

    public void CloseShop()
    {
        Sequence tween = DOTween.Sequence();
        tween
            .AppendCallback(() => shopHUD.interactable = false)
            .Append(shopHUD.DOFade(0, fadeDuration))
            .Join(shop.transform.DOScale(Vector3.zero, fadeDuration).From(Vector3.one).SetEase(Ease.OutBack))
            .AppendCallback(() => shopHUD.blocksRaycasts = false);
        // TODO: add player movement enable
    }
}
