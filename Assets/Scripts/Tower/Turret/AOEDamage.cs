using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    private float damage;
    private float radius;
    private float lifeTime = 0.5f;

    public void Init(ProjectileConfig config, float aoeRadius)
    {
        if (config != null) damage = config.damage;
        radius = aoeRadius;
        Invoke(nameof(DoAOE), 0.01f);
        Invoke(nameof(SelfDestroy), config != null ? config.lifeTime : lifeTime);
    }

    private void DoAOE()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<IDamageable>(out var d))
            {
                d.TakeDamage(damage);
            }
        }
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}