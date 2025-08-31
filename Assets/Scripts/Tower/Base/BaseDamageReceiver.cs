using UnityEngine;
using Ilumisoft.HealthSystem;
using System.Collections.Generic;

public class BaseDamageReceiver : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float checkRadius = 10f;
    [SerializeField] private LayerMask detectionLayer = ~0; // За замовчуванням всі шари
    [SerializeField] private float damageCheckInterval = 0.5f;
    
    [Header("Enemy Filtering")]
    [SerializeField] private List<string> enemyTags = new List<string> { "Enemy" };
    [Tooltip("Якщо увімкнено, буде враховуватися лише шар detectionLayer")]
    [SerializeField] private bool useLayerFiltering = true;
    
    private HealthComponent healthComponent;
    private float lastCheckTime;

    private void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        
        if (healthComponent == null)
        {
            Debug.LogError("HealthComponent not found on " + gameObject.name);
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
        Collider[] detectedColliders;
        
        if (useLayerFiltering)
        {
            // Використовуємо фільтрацію за шарами для оптимізації
            detectedColliders = Physics.OverlapSphere(transform.position, checkRadius, detectionLayer);
        }
        else
        {
            // Перевіряємо всі колайдери без фільтрації за шарами
            detectedColliders = Physics.OverlapSphere(transform.position, checkRadius);
        }
        
        foreach (var collider in detectedColliders)
        {
            // Фільтруємо за тегами
            if (!enemyTags.Contains(collider.tag)) continue;
            
            if (collider.TryGetComponent<EnemyAttack>(out var enemyAttack))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                
                if (distance <= enemyAttack.config.attackRange)
                {
                    ApplyDamageFromEnemy(enemyAttack);
                }
            }
        }
    }

    private void ApplyDamageFromEnemy(EnemyAttack enemyAttack)
    {
        // Перевіряємо, чи ворог може атакувати (не мертвий)
        if (enemyAttack.GetComponent<Health>() != null && 
            !enemyAttack.GetComponent<Health>().IsAlive)
            return;
            
        if (enemyAttack.config.projectilePrefab == null)
        {
            healthComponent.ApplyDamage(enemyAttack.config.damage * damageCheckInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
    
    // Методи для додавання/видалення тегів зі списку
    public void AddEnemyTag(string tag)
    {
        if (!enemyTags.Contains(tag))
        {
            enemyTags.Add(tag);
        }
    }
    
    public void RemoveEnemyTag(string tag)
    {
        if (enemyTags.Contains(tag))
        {
            enemyTags.Remove(tag);
        }
    }
    
    public void ClearEnemyTags()
    {
        enemyTags.Clear();
    }
    
    public List<string> GetEnemyTags()
    {
        return new List<string>(enemyTags);
    }
    
    // Властивості для доступу до налаштувань
    public bool UseLayerFiltering
    {
        get { return useLayerFiltering; }
        set { useLayerFiltering = value; }
    }
    
    public LayerMask DetectionLayer
    {
        get { return detectionLayer; }
        set { detectionLayer = value; }
    }
}