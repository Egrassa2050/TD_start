using UnityEngine;
using Ilumisoft.HealthSystem;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private ProjectileConfig config;
    private Rigidbody rb;

    public void Init(Transform targetTransform, ProjectileConfig projectileConfig)
    {
        target = targetTransform;
        config = projectileConfig;
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();
            
        Destroy(gameObject, config.lifetime);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(config.speed * Time.deltaTime * direction, Space.World);
        transform.LookAt(target);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HitTarget(other.gameObject);
        }
    }

    private void HitTarget(GameObject targetObject)
    {
        // Шукаємо HealthComponent в дітях цільового об'єкта
        HealthComponent health = targetObject.GetComponentInChildren<HealthComponent>();
        
        if (health != null)
        {
            health.ApplyDamage(config.damage);
        }

        if (config.hitEffect != null)
            Instantiate(config.hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}