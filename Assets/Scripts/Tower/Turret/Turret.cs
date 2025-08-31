using System.Collections.Generic;
using UnityEngine;

public enum FireMode
{
    Single,   // по одному ворогу
    Multi,    // кілька ворогів
    AOE       // зона ураження
}

public class Turret : MonoBehaviour
{
    [Header("Основні параметри")]
    public FireMode fireMode = FireMode.Single;
    public Transform firePoint;
    public float fireRate = 1f;
    public float range = 10f;

    [Header("Снаряди")]
    public GameObject projectilePrefab;
    public ProjectileConfig projectileConfig;

    [Header("Параметри режимів")]
    public int multiTargetsCount = 3;
    public float aoeRadius = 3f;

    private float fireCooldown;
    private List<GameObject> targets = new List<GameObject>();

    void Awake()
    {
        if (firePoint == null)
            firePoint = transform.Find("FirePoint");
    }

    void Update()
    {
        FindTargets();

        if (targets.Count == 0) return;

        RotateTowards(targets[0].transform.position);

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = 1f / fireRate;
        }
    }

    void FindTargets()
    {
        targets.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                targets.Add(hit.gameObject);
        }

        targets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
    }

    void RotateTowards(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        if (dir == Vector3.zero) return;

        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 360 * Time.deltaTime);
    }

    void Fire()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                SpawnProjectile(targets[0]);
                break;

            case FireMode.Multi:
                int count = Mathf.Min(multiTargetsCount, targets.Count);
                for (int i = 0; i < count; i++)
                    SpawnProjectile(targets[i]);
                break;

            case FireMode.AOE:
                if (projectilePrefab && firePoint)
                {
                    GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                    AOEDamage aoeDamage = proj.GetComponent<AOEDamage>();
                    if (aoeDamage != null)
                    {
                        aoeDamage.Init(projectileConfig, aoeRadius);
                    }
                    else
                    {
                        // Якщо компонент AOEDamage відсутній, створюємо його
                        aoeDamage = proj.AddComponent<AOEDamage>();
                        aoeDamage.Init(projectileConfig, aoeRadius);
                    }
                }
                break;
        }
    }

    void SpawnProjectile(GameObject target)
    {
        if (!projectilePrefab || !firePoint || !projectileConfig) return;

        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(target.transform, projectileConfig);
    }
}