using UnityEngine;
using Ilumisoft.HealthSystem;

public class DamageRadius : MonoBehaviour
{
    public int damage = 2;
    public float radius = 3f;
    public float lifetime = 0.5f;

    void Start()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            // Отримуємо компонент Health
            HealthComponent health = hit.GetComponent<HealthComponent>();
            
            if (health != null)
            {
                // Завдаємо шкоду через систему здоров'я
                health.ApplyDamage(damage);
            }
        }

        Destroy(gameObject, lifetime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}