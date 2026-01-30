using UnityEngine;
using UnityEngine.UI;

public class InventoryMaskController : MonoBehaviour
{
    public MaskData[] masks;
    public Image maskImage;
    public Sprite[] maskSprites;

    public int currentIndex;
    public MaskData CurrentMask => masks[currentIndex];

    void Start()
    {
        RefreshUI();
    }

    public void Next()
    {
        currentIndex = (currentIndex + 1) % masks.Length;
        RefreshUI();
    }

    public void Previous()
    {
        currentIndex = (currentIndex - 1 + masks.Length) % masks.Length;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (maskSprites.Length > currentIndex)
            maskImage.sprite = maskSprites[currentIndex];

        foreach (var slot in GetComponentsInChildren<TokenItemUI>())
            slot.Refresh(CurrentMask);
    }
}
