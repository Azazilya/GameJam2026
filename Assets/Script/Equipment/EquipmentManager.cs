using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

public float GetFinalStat(MaskData mask, StatType type)
{
    float value = 0;

    switch (type)
    {
        case StatType.Attack:
            value = mask.baseAttack;
            break;
        case StatType.Defense:
            value = mask.baseDefense;
            break;
        case StatType.MoveSpeed:
            value = mask.baseMoveSpeed;
            break;
        case StatType.AttackSpeed:
            value = mask.baseAttackSpeed;
            break;
    }

    foreach (var token in mask.equippedTokens)
    {
        if (token.statType == type)
            value += value * (token.percentValue / 100f);
    }

    return value;
}


    private float GetBaseStat(MaskData mask, StatType statType)
    {
        switch (statType)
        {
            case StatType.Attack: return mask.baseAttack;
            case StatType.AttackSpeed: return mask.baseAttackSpeed;
            case StatType.MoveSpeed: return mask.baseMoveSpeed;
            case StatType.Defense: return mask.baseDefense;
            default: return 0f;
        }
    }
}
