using UnityEngine;
using Ilumisoft.HealthSystem;

public class EnemyDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestEnemyHealth();
        }
    }

    void TestEnemyHealth()
    {
        // Шукаємо Health компонент в дітях
        Health health = GetComponentInChildren<Health>();
        if (health != null)
        {
            Debug.Log($"=== ТЕСТ ХП ===");
            Debug.Log($"Об'єкт: {health.gameObject.name}");
            Debug.Log($"Поточне ХП: {health.CurrentHealth}/{health.MaxHealth}");
            Debug.Log($"Живий: {health.IsAlive}");
            
            health.ApplyDamage(25);
            Debug.Log($"Нанесено 25 урону, нове ХП: {health.CurrentHealth}");
        }
        else
        {
            Debug.LogError("Health компонент не знайдено в дітях!");
        }
    }
}