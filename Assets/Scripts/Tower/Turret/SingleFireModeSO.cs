using UnityEngine;

[CreateAssetMenu(menuName = "FireModes/Single")]
public class SingleFireModeSO : FireModeSO
{
    public Projectile projectile;
    public override void Fire(Tower tower)
    {
        var t = tower.GetClosestTarget();
        if (t == null) return;
        var p = ProjectilePool.Instance.Spawn(projectile, tower.FirePointPosition, Quaternion.LookRotation(t.position - tower.FirePointPosition));
        p.Init(t, tower.Damage); // <-- другий аргумент float
    }
}