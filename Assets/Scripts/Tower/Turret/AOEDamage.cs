using UnityEngine;
using Ilumisoft.HealthSystem;

public class AOEDamage : MonoBehaviour
{
    private ProjectileConfig config;
    private float radius;

    public void Init(ProjectileConfig projectileConfig, float damageRadius)
    {
        config = projectileConfig;
        radius = damageRadius;
        Destroy(gameObject, config.lifetime);
    }

    void Start()
    {
        // Запускаємо вибух одразу або через деякий час
        Explode();
    }

    void Explode()
    {
        // Знаходимо всіх ворогів у радіусі
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                HealthComponent health = hit.GetComponent<HealthComponent>();
                if (health != null)
                {
                    health.ApplyDamage(config.damage);
                }
            }
        }

        // Створюємо ефект вибуху
        if (config.hitEffect != null)
            Instantiate(config.hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // Візуалізація радіусу у редакторі
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}