using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    public Slider HPMeter;
    public Image StaminaBar;
    public TextMeshProUGUI moneyHUD;

    [SerializeField] private Sprite emptyStaminaFillImage;
    [SerializeField] private Sprite oneStamina;
    [SerializeField] private Sprite twoStamina;
    [SerializeField] private Sprite threeStamina;
    [SerializeField] private Sprite fullStamina;

    private void Start()
    {
        CurrencySystem.instance.OnMoneyChanged += UpdateMoneyHUD;
        UpdateMoneyHUD();
    }

    private void OnDestroy()
    {
        CurrencySystem.instance.OnMoneyChanged -= UpdateMoneyHUD;
    }

    private void UpdateMoneyHUD()
    {
        string text = $": {CurrencySystem.instance.currentMoney}";
        moneyHUD.text = text;
    }
}
