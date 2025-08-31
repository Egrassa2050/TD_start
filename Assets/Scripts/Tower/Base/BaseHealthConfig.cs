using UnityEngine;

[CreateAssetMenu(fileName = "BaseHealthConfig", menuName = "Configs/BaseHealthConfig")]
public class BaseHealthConfig : ScriptableObject
{
    public int maxHealth = 100;
    public float healthRegenRate = 0f;
    public bool showHealthbarAlways = false;
}