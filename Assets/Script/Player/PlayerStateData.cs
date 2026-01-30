using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerState", menuName = "Player/State Data")]
public class PlayerStateData : ScriptableObject
{
    [Header("Combat Stats")]
    public float attackDamage;
    public float attackSpeed;
    public float defense;

    [Header("Movement Stats")]
    public float movementSpeed;

    [Header("Head Animations")]
    public Sprite[] headIdle;
    public Sprite[] headWalkDepan;
    public Sprite[] headWalkBelakang;
    public Sprite[] headWalkSamping;

    [Header("Hand Animations (Single Side)")]
    public Sprite[] handIdle;
    public Sprite[] handAttack;
}