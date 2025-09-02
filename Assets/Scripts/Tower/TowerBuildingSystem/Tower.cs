using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Tower : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Tooltip("Ціна")] private int cost = 50;
    [SerializeField, Tooltip("Дальність")] private float range = 8f;
    [SerializeField, Tooltip("Швидкість стрільби")] private float fireRate = 1f;
    [SerializeField, Tooltip("Дамаг")] private float damage = 10f;
    [Header("Setup")]
    [SerializeField, Tooltip("Точка вистрілу")] private Transform firePointTransform;
    [SerializeField, Tooltip("Режим стрільби")] private FireModeSO fireMode;

    private readonly List<Transform> targets = new List<Transform>();
    private float nextFire;
    private SphereCollider sc;

    public Vector3 FirePointPosition => firePointTransform != null ? firePointTransform.position : transform.position;
    public float Damage => damage;
    public int Cost => cost; // <-- публічний геттер

    void Awake()
    {
        sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = range;
        if (firePointTransform == null) firePointTransform = transform;
    }

    void Update()
    {
        if (Time.time >= nextFire)
        {
            if (fireMode != null) fireMode.Fire(this);
            nextFire = Time.time + fireRate;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform) return;
        if (other.TryGetComponent<IDamageable>(out var d)) if (!targets.Contains(other.transform)) targets.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other.transform)) targets.Remove(other.transform);
    }

    public Transform GetClosestTarget()
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        var pos = transform.position;
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            var t = targets[i];
            if (t == null) { targets.RemoveAt(i); continue; }
            float sq = (t.position - pos).sqrMagnitude;
            if (sq < bestSqr) { bestSqr = sq; best = t; }
        }
        return best;
    }
}
