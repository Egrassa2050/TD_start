using UnityEngine;
using Ilumisoft.HealthSystem;

public class EnemyDeathNotifier : MonoBehaviour
{
    public System.Action<GameObject> OnDeath;

    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnHealthEmpty += HandleDeath;
        }
    }

    void HandleDeath()
    {
        OnDeath?.Invoke(gameObject);
        health.OnHealthEmpty -= HandleDeath;
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthEmpty -= HandleDeath;
        }
    }
}