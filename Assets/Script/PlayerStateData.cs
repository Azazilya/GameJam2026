using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerState", menuName = "Player/State Data")]
public class PlayerStateData : ScriptableObject
{
    [Header("Combat Stats")]
    public float attackDamage;
    public float knockbackForce = 5f;
    public float attackSpeed;
    public float defense;

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