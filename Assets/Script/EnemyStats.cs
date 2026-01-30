using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Stats/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public float maxHP;
    public float movementSpeed;
    public float attackDamage;
    public float defense;
}