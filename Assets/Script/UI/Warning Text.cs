using DG.Tweening;
using TMPro;
using UnityEngine;

public class WarningText : MonoBehaviour
{
    public string Text;
    public TextMeshProUGUI textDisplay;

    private void Awake()
    {
        if (textDisplay == null)
        {
            textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        textDisplay.text = Text;
        Sequence sequence = DOTween.Sequence();
        sequence
            .PrependInterval(3f)
            .Append(textDisplay.DOFade(0f, 1f))
            .AppendCallback(() => Destroy(gameObject));
    }
}
