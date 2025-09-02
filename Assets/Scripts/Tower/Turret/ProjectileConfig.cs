using UnityEngine;

[CreateAssetMenu(menuName = "Projectiles/Config")]
public class ProjectileConfig : ScriptableObject
{
    public enum ProjectileType
    {
        Basic,
        Piercing,
        Homing,
        Directional,
        AOE
    }

    [Header("Основні")]
    [Tooltip("Тип снаряда")]
    public ProjectileType type = ProjectileType.Basic;

    [Tooltip("Дамаг, наноситься при попаданні або AOE")]
    public float damage = 10f;

    [Tooltip("Швидкість снаряду")]
    public float speed = 12f;

    [Tooltip("Час життя снаряду")]
    public float lifeTime = 5f;

    [Header("Piercing")]
    [Tooltip("Максимальна кількість ворогів, через яких може пройти снаряд (piercing)")]
    public int maxPierce = 1;

    [Header("Homing")]
    [Tooltip("Чи самонаводиться снаряд")]
    public bool useHoming = false;
    [Tooltip("Швидкість повороту (градуси в секунду) для homing")]
    public float homingTurnSpeed = 720f;

    [Header("Directional / Spread")]
    [Tooltip("Чи рухається снаряд по напрямку (direction-based) замість слідування за ціллю")]
    public bool directionBased = false;

    [Header("AOE")]
    [Tooltip("Чи завдає снаряд AOE-урон")]
    public bool isAOE = false;
    [Tooltip("Радіус AOE")]
    public float aoeRadius = 3f;
    [Tooltip("Якщо true — AOE відбувається при зіткненні; якщо false — при спавні")]
    public bool aoeOnImpact = true;
}