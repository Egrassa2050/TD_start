using UnityEngine;

[CreateAssetMenu(menuName = "FireModes/Spread")]
public class SpreadFireModeSO : FireModeSO
{
    public Projectile projectile;
    public int pellets = 5;
    public float spreadAngle = 12f;

    public override void Fire(Tower tower)
    {
        var t = tower.GetClosestTarget();
        if (t == null) return;
        var baseDir = (t.position - tower.FirePointPosition).normalized;
        float start = -spreadAngle * 0.5f;
        float step = spreadAngle / Mathf.Max(1, pellets - 1);
        for (int i = 0; i < pellets; i++)
        {
            var angle = start + step * i;
            var dir = Quaternion.Euler(0, angle, 0) * baseDir;
            var rot = Quaternion.LookRotation(dir);
            var p = ProjectilePool.Instance.Spawn(projectile, tower.FirePointPosition, rot);
            p.Init(null, tower.Damage, true, dir); // <-- direction-based, target=null
        }
    }
}