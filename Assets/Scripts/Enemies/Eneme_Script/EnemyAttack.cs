using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public EnemyAttackConfig config;
    private float nextAttackTime;
    private Animator animator;
    private Transform baseTarget;

    private void Start()
    {
        animator = GetComponent<Animator>();
        baseTarget = GameObject.FindGameObjectWithTag("Base")?.transform;
    }

    private void Update()
    {
        if (baseTarget == null || config == null) return;

        float distance = Vector3.Distance(transform.position, baseTarget.position);

        if (distance <= config.attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + config.attackCooldown;

            if (animator != null && !string.IsNullOrEmpty(config.attackAnimation))
                animator.SetTrigger(config.attackAnimation);

            if (config.projectilePrefab != null)
                ShootProjectile();
            // Пряму атаку тепер обробляє BaseDamageReceiver
        }
    }

    private void ShootProjectile()
    {
        GameObject projGO = Instantiate(config.projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(baseTarget, config.projectileConfig);
    }
}