using UnityEngine;
using UnityEngine.UI;

public class MaskIconUI : MonoBehaviour
{
    public Image maskImage;
    public MaskManager maskManager;

    void Update()
    {
        maskImage.sprite = maskManager.CurrentMask.icon;
    }
}
