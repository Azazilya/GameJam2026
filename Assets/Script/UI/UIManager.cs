using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject warningTextPrefab;
    public GameObject warningTextHolder;
    public void ShowWarningText(string text = "")
    {
        Instantiate(warningTextPrefab, warningTextHolder.transform).GetComponent<WarningText>().Text = text;
    }
}
