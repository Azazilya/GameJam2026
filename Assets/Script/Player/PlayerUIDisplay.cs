using UnityEngine;
using TMPro; // Penting untuk TextMeshPro

public class PlayerUIDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats; // Drag file ScriptableObject PlayerStats ke sini
    [SerializeField] private TextMeshProUGUI hpText;   // Drag objek Text (TMP) ke sini

    [Header("Settings")]
    [SerializeField] private string prefix = "HP: ";
    [SerializeField] private bool showMaxHP = true;

    void Update()
    {
        UpdateHPDisplay();
    }

    private void UpdateHPDisplay()
    {
        if (playerStats == null || hpText == null) return;

        // Membulatkan angka agar tidak ada koma yang panjang (misal: 95.444)
        int currentHP = Mathf.RoundToInt(playerStats.currentHP);
        int maxHP = Mathf.RoundToInt(playerStats.maxHP);

        if (showMaxHP)
        {
            hpText.text = $"{prefix}{currentHP} / {maxHP}";
        }
        else
        {
            hpText.text = $"{prefix}{currentHP}";
        }
    }
}