using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItems : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onLeftClick;
    public ItemData itemData;

    public Image icon;

    public bool isSelected = false;
    [SerializeField] private CanvasGroup dimmer;

    private void Awake()
    {
        if (icon == null)
        {
            icon = GetComponentInChildren<Image>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        icon.sprite = itemData.icon;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) & isSelected)
        {
            isSelected = false;
        }
        if (!isSelected)
        {
            dimmer.alpha = 0f;
        }
    }

    public void SelectItem()
    {
        isSelected = true;
        dimmer.alpha = 0.5f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left & !isSelected)
        {
            SelectItem();
            onLeftClick.Invoke();
        }
    }

}
