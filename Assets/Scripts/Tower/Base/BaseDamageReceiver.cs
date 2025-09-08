using UnityEngine;
using Ilumisoft.HealthSystem;
using System.Collections.Generic;

public class BaseDamageReceiver : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float checkRadius = 10f;
    [SerializeField] private LayerMask detectionLayer = ~0;
    [SerializeField] private float damageCheckInterval = 0.5f;

    [Header("Enemy Filtering")]
    [SerializeField] private List<string> enemyTags = new List<string> { "Enemy" };
    [SerializeField] private bool useLayerFiltering = true;

    [Header("Debug Settings")]
    [SerializeField] private float extraRange = 2f; // додатковий радіус для тесту

    private HealthComponent healthComponent;
    private float lastCheckTime;

    private void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent == null)
        {
            Debug.LogError("HealthComponent не знайдено на " + gameObject.name);
            enabled = false;
        }
    }

    private void Update()
    {
        if (healthComponent == null || !healthComponent.IsAlive) return;

        if (Time.time - lastCheckTime >= damageCheckInterval)
        {
            CheckForEnemies();
            lastCheckTime = Time.time;
        }
    }

    private void CheckForEnemies()
    {
        Collider[] detectedColliders = useLayerFiltering
            ? Physics.OverlapSphere(transform.position, checkRadius, detectionLayer)
            : Physics.OverlapSphere(transform.position, checkRadius);

        Debug.Log($"[{gameObject.name}] Знайдено {detectedColliders.Length} колайдерів всього");

        foreach (var collider in detectedColliders)
        {
            if (!enemyTags.Contains(collider.tag)) continue;

            if (!collider.TryGetComponent<EnemyAttack>(out var enemyAttack))
            {
                Debug.Log($"Ворог {collider.name} не має EnemyAttack");
                continue;
            }

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            float effectiveRange = enemyAttack.config.attackRange + extraRange;

            Health enemyHealth = enemyAttack.GetComponent<Health>();
            bool enemyAlive = enemyHealth == null || enemyHealth.IsAlive;

            Debug.Log($"Ворог: {collider.name}, Відстань: {distance:F2}, EffectiveRange: {effectiveRange}, Ворог живий: {enemyAlive}");

            if (!enemyAlive)
            {
                Debug.Log("Ворог мертвий, ApplyDamage не викликається");
                continue;
            }

            if (distance > effectiveRange)
            {
                Debug.Log("Ворог поза effectiveRange, ApplyDamage не викликається");
                continue;
            }

            Debug.Log($"ApplyDamage викликається на {collider.name}");
            ApplyDamageFromEnemy(enemyAttack);
        }
    }

    private void ApplyDamageFromEnemy(EnemyAttack enemyAttack)
    {
        if (enemyAttack.config.projectilePrefab == null)
        {
            float damageAmount = enemyAttack.config.damage * damageCheckInterval;
            healthComponent.ApplyDamage(damageAmount);
            Debug.Log($"Нанесено {damageAmount} шкоди, HP бази: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
        }
        else
        {
            Debug.Log("Ворог використовує снаряд, прямий урон не застосовується");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
