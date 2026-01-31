using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerState", menuName = "Player/State Data")]
public class PlayerStateData : ScriptableObject
{
    [Header("Combat Stats")]
    public float attackDamage;
    public float knockbackForce = 5f;
    public float attackSpeed;
    public float defense;

    [Header("HP Passive Regen")]
    public float hpRegenRate = 1f; // Jumlah HP per detik
    public float hpRegenDelay = 5f; // Waktu tunggu setelah terkena damage sebelum regen mulai

    [Header("Movement Stats")]
    public float movementSpeed;
    [Range(0, 1)] public float chargeMovementPenalty = 0.5f;

    [Header("Head Animations")]
    public Sprite[] headIdle;
    public Sprite[] headWalkDepan;
    public Sprite[] headWalkBelakang;
    public Sprite[] headWalkSamping;

    [Header("Hand Animations (Single Side)")]
    public Sprite[] handIdle;
    public Sprite[] handAttack;

    // Tambahkan di dalam class PlayerStateData
    [Header("Attack Type Settings")]
    public bool useRotateAttack; // Checkbox: Rotate vs Animation Based
    public float rotateAngle = 90f; // Seberapa jauh ayunan rotasi (derajat)
    public float rotateDuration = 0.1f; // Kecepatan ayunan

    [Header("Charge Attack Settings")]
    public float chargeThreshold = 0.5f;
    public float superChargeThreshold = 7f;
    public int holdFrameIndex = 1;
    public Color flickerColor = Color.white;
    public float baseFlickerSpeed = 15f;      
    public float maxFlickerSpeed = 50f;
    public float chargeDamageMultiplier = 2f; 
    public float superChargeMultiplier = 4f; 
}