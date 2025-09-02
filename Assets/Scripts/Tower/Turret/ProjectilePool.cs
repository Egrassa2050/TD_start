using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }

    [System.Serializable]
    public class PoolEntry
    {
        public Projectile prefab;
        public int initial = 10;
    }

    [SerializeField] private PoolEntry[] entries;
    private readonly Dictionary<Projectile, Queue<Projectile>> pools = new Dictionary<Projectile, Queue<Projectile>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        foreach (var e in entries)
        {
            var q = new Queue<Projectile>();
            for (int i = 0; i < e.initial; i++)
            {
                var go = Instantiate(e.prefab.gameObject, transform);
                go.SetActive(false);
                q.Enqueue(go.GetComponent<Projectile>());
            }
            pools[e.prefab] = q;
        }
    }

    public Projectile Spawn(Projectile prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return null;
        if (!pools.TryGetValue(prefab, out var q) || q.Count == 0)
        {
            var go = Instantiate(prefab.gameObject, pos, rot);
            return go.GetComponent<Projectile>();
        }
        var p = q.Dequeue();
        p.transform.position = pos;
        p.transform.rotation = rot;
        p.gameObject.SetActive(true);
        return p;
    }

    public void Despawn(Projectile p)
    {
        p.gameObject.SetActive(false);
        foreach (var key in pools.Keys)
        {
            if (key.GetType() == p.GetType())
            {
                pools[key].Enqueue(p);
                return;
            }
        }
    }
}