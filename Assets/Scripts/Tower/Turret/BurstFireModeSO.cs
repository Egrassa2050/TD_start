using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "FireModes/Burst")]
public class BurstFireModeSO : FireModeSO
{
    public Projectile projectile;
    public int burstCount = 3;
    public float burstInterval = 0.12f;

    public override void Fire(Tower tower)
    {
        var t = tower.GetClosestTarget();
        if (t == null) return;
        tower.StartCoroutine(Burst(t, tower));
    }

    private IEnumerator Burst(Transform target, Tower tower)
    {
        for (int i = 0; i < burstCount; i++)
        {
            var p = ProjectilePool.Instance.Spawn(projectile, tower.FirePointPosition, Quaternion.LookRotation(target.position - tower.FirePointPosition));
            p.Init(target, tower.Damage);
            yield return new WaitForSeconds(burstInterval);
        }
    }
}