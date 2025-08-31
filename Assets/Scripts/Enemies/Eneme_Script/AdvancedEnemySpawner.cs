using UnityEngine;
using System.Collections;

public class AdvancedEnemySpawner : MonoBehaviour
{
    [Header("Основні налаштування")]
    public GameObject enemyPrefab;
    public Vector3 spawnArea = new Vector3(5f, 0f, 5f);
    
    [Header("Контроль спавну")]
    public float initialSpawnRate = 1f; // Ворогів на секунду на початку
    public float maxSpawnRate = 5f; // Максимальна кількість ворогів на секунду
    public float spawnRateIncreaseInterval = 30f; // Інтервал збільшення складності (секунди)
    public float spawnRateIncreaseAmount = 0.5f; // На скільки збільшувати спавн rate
    
    [Header("Обмеження")]
    public int maxEnemies = 20; // Максимальна кількість ворогів одночасно
    
    [Header("Gizmos")]
    public bool showSpawnArea = true;
    public Color spawnAreaColor = Color.red;
    
    private float currentSpawnRate;
    private int currentEnemies = 0;
    private float nextSpawnTime = 0f;
    private float difficultyTimer = 0f;

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        difficultyTimer = spawnRateIncreaseInterval;
    }

    void Update()
    {
        // Збільшення складності з часом
        difficultyTimer -= Time.deltaTime;
        if (difficultyTimer <= 0f)
        {
            IncreaseDifficulty();
            difficultyTimer = spawnRateIncreaseInterval;
        }
        
        // Спавн ворогів згідно поточного спавн рейту
        if (currentEnemies < maxEnemies && Time.time >= nextSpawnTime)
        {
            SpawnSingleEnemy();
            nextSpawnTime = Time.time + 1f / currentSpawnRate;
        }
    }
    
    void IncreaseDifficulty()
    {
        if (currentSpawnRate < maxSpawnRate)
        {
            currentSpawnRate += spawnRateIncreaseAmount;
            currentSpawnRate = Mathf.Min(currentSpawnRate, maxSpawnRate);
            Debug.Log($"Складність збільшена! Поточний спавн рейт: {currentSpawnRate} ворогів/секунду");
        }
    }
    
    void SpawnSingleEnemy()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0f,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        GameObject enemy = Instantiate(
            enemyPrefab,
            transform.position + randomPosition,
            Quaternion.identity
        );

        // Додаємо/знаходимо компонент для відстеження смерті ворога
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            enemyHealth = enemy.AddComponent<EnemyHealth>();
        }
        
        enemyHealth.OnDeath += HandleEnemyDeath;
        currentEnemies++;
    }

    void HandleEnemyDeath()
    {
        currentEnemies--;
    }
    
    // Метод для зміни спавн рейту ззовні (наприклад, через UI)
    public void SetSpawnRate(float newRate)
    {
        currentSpawnRate = Mathf.Clamp(newRate, 0.1f, maxSpawnRate);
    }
    
    // Метод для зміни максимальної кількості ворогів
    public void SetMaxEnemies(int newMax)
    {
        maxEnemies = newMax;
    }

    void OnDrawGizmosSelected()
    {
        if (showSpawnArea)
        {
            Gizmos.color = spawnAreaColor;
            Gizmos.DrawWireCube(transform.position, spawnArea);
        }
    }
}

// Допоміжний клас для здоров'я ворога
public class EnemyHealth : MonoBehaviour
{
    public System.Action OnDeath;
    public int health = 100;
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}