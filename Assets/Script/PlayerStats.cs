using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float regenDelay = 2f; // Waktu tunggu sebelum regen mulai
}