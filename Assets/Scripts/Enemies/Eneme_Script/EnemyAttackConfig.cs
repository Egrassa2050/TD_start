using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackConfig", menuName = "Configs/EnemyAttackConfig")]
public class EnemyAttackConfig : ScriptableObject
{
    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float attackRange = 2f;
    public string attackAnimation = "Attack";

    public GameObject projectilePrefab;        // для дальника
    public ProjectileConfig projectileConfig;  // SO снаряда
}