using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Configs/ProjectileConfig")]
public class ProjectileConfig : ScriptableObject
{
    public int damage = 1;
    public float speed = 10f;
    public float lifetime = 4f;
    public string targetTag = "Base"; // "Base" або "Enemy"
    public GameObject hitEffect;      // ефект при влучанні
}