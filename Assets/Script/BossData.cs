using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Boss/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Base Stats")]
    public string bossName;
    public float maxHP;
    public float defense;

    [Header("Combat Stats")]
    public float attackDamage;
    public float attackSpeed;
    public float knockbackForce = 10f; // Boss biasanya memiliki knockback lebih kuat

    [Header("Movement Stats")]
    public float movementSpeed;

    [Header("Head Animations")]
    public Sprite[] headIdle;
    public Sprite[] headWalkDepan;
    public Sprite[] headWalkBelakang;
    public Sprite[] headWalkSamping;

    [Header("Body Animations (Handheld)")]
    // Karena boss hanya 1 state, kita bisa langsung masukkan animasi tangan/senjata di sini
    public Sprite[] handIdle;
    public Sprite[] handAttack;

    [Header("Special Settings")]
    public Color bossThemeColor = Color.white; // Bisa digunakan untuk UI atau partikel boss
}