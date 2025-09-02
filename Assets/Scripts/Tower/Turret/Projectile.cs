using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    private float damage;
    private Transform target;
    private Vector3 direction;
    private bool directionBased;
    private float speed = 12f;
    private float lifeTime = 5f;

    // runtime state
    private int remainingPierce = 0;
    private bool useHoming = false;
    private float homingTurnSpeed = 720f;
    private bool isAOE = false;
    private float aoeRadius = 3f;
    private bool aoeOnImpact = true;

    public void Init(Transform targetTransform, float dmg, bool dirBased = false, Vector3 dir = default)
    {
        ApplyBaseSettings(dmg, 12f, 5f, dirBased, dir);
        target = targetTransform;
    }

    public void Init(Transform targetTransform, ProjectileConfig config, Vector3 dir = default)
    {
        if (config == null) { Init(targetTransform, 0f); return; }

        damage = config.damage;
        speed = config.speed;
        lifeTime = config.lifeTime;
        directionBased = config.directionBased;
        direction = dir.normalized;

        remainingPierce = config.type == ProjectileConfig.ProjectileType.Piercing ? Mathf.Max(1, config.maxPierce) : 1;
        useHoming = config.useHoming && config.type == ProjectileConfig.ProjectileType.Homing;
        homingTurnSpeed = config.homingTurnSpeed;
        isAOE = config.isAOE || config.type == ProjectileConfig.ProjectileType.AOE;
        aoeRadius = config.aoeRadius;
        aoeOnImpact = config.aoeOnImpact;

        target = targetTransform;

        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);

        if (isAOE && !aoeOnImpact)
        {
            DoAOE();
            ReturnToPool();
        }
    }

    private void ApplyBaseSettings(float dmg, float spd, float life, bool dirBased, Vector3 dir)
    {
        damage = dmg;
        speed = spd;
        lifeTime = life;
        directionBased = dirBased;
        direction = dir.normalized;
        remainingPierce = 1;
        useHoming = false;
        isAOE = false;
        aoeRadius = 0f;
        aoeOnImpact = true;
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void Update()
    {
        if (directionBased)
        {
            transform.position += direction * speed * Time.deltaTime;
            return;
        }

        if (useHoming)
        {
            if (target == null) { ReturnToPool(); return; }
            Vector3 toTarget = (target.position - transform.position).normalized;
            float maxRadiansDelta = homingTurnSpeed * Mathf.Deg2Rad * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, toTarget, maxRadiansDelta, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }

        if (target == null) { ReturnToPool(); return; }
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform) return;

        if (isAOE && aoeOnImpact)
        {
            DoAOE();
            ReturnToPool();
            return;
        }

        if (other.TryGetComponent<IDamageable>(out var dmgable))
        {
            dmgable.TakeDamage(damage);
            remainingPierce--;
            if (remainingPierce <= 0) ReturnToPool();
        }
        else
        {
            // якщо влучили в щось не живе — просто знищити (або можна пропускати)
            ReturnToPool();
        }
    }

    private void DoAOE()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<IDamageable>(out var d)) d.TakeDamage(damage);
        }
    }

    protected virtual void ReturnToPool()
    {
        CancelInvoke();
        if (ProjectilePool.Instance != null) ProjectilePool.Instance.Despawn(this);
        else Destroy(gameObject);
    }
}
