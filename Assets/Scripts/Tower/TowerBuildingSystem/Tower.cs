using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Параметри вежі")]
    [SerializeField] private int cost = 50;
    [SerializeField] private float range = 8f;
    [SerializeField] private float fireRate = 1f;

    public int Cost => cost;  // Публічний геттер
    public float Range => range;
    public float FireRate => fireRate;
}