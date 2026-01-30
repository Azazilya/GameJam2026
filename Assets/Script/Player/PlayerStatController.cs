using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [Header("Player Base Stats")]
    [SerializeField] private float baseAttack = 10f;
    [SerializeField] private float baseDefense = 3f;
    [SerializeField] private float baseMoveSpeed = 9.5f;
    [SerializeField] private float baseAttackSpeed = 2.0f;

    float attack;
    float defense;
    float moveSpeed;
    float attackSpeed;

    public float Attack => attack;
    public float Defense => defense;
    public float MoveSpeed => moveSpeed;
    public float AttackSpeed => attackSpeed;

    public void ApplyMask(MaskData mask)
    {
        if (mask == null)
        {
            ResetToBase();
            return;
        }

        // Base + Mask
        attack = baseAttack + mask.baseAttack;
        defense = baseDefense + mask.baseDefense;
        moveSpeed = baseMoveSpeed + mask.baseMoveSpeed;
        attackSpeed = baseAttackSpeed + mask.baseAttackSpeed;

        // Apply Tokens (%)
        foreach (var token in mask.equippedTokens)
        {
            ApplyToken(token);
        }
    }

    void ApplyToken(TokenData token)
    {
        float value = token.percentValue / 100f;

        switch (token.statType)
        {
            case StatType.Attack:
                attack += attack * value;
                break;
            case StatType.Defense:
                defense += defense * value;
                break;
            case StatType.MoveSpeed:
                moveSpeed += moveSpeed * value;
                break;
            case StatType.AttackSpeed:
                attackSpeed += attackSpeed * value;
                break;
        }
    }

    void ResetToBase()
    {
        attack = baseAttack;
        defense = baseDefense;
        moveSpeed = baseMoveSpeed;
        attackSpeed = baseAttackSpeed;
    }
}
