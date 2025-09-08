using UnityEngine;

public class AdvancedEnemySpawner : MonoBehaviour
{
    [Header("Основні налаштування")]
    [Tooltip("Префаб ворога тип A")]
    public GameObject enemyPrefabA;
    [Tooltip("Префаб ворога тип B")]
    public GameObject enemyPrefabB;
    [Tooltip("Префаб боса")]
    public GameObject bossPrefab;
    [Tooltip("Розмір області спавну (X, Y, Z)")]
    public Vector3 spawnArea = new Vector3(5f, 0f, 5f);

    [Header("Контроль спавну")]
    [Tooltip("Початковий спавн рейт (ворогів/сек)")]
    public float initialSpawnRate = 1f;
    [Tooltip("Максимальний спавн рейт (ворогів/сек)")]
    public float maxSpawnRate = 5f;
    [Tooltip("Інтервал збільшення складності (сек)")]
    public float spawnRateIncreaseInterval = 30f;
    [Tooltip("На скільки збільшувати спавн рейт при кожному інтервалі")]
    public float spawnRateIncreaseAmount = 0.5f;

    [Header("Бос")]
    [Tooltip("Інтервал між появами боса (сек)")]
    public float bossSpawnInterval = 60f;
    [Tooltip("Якщо true — бос може спавнитися навіть при досягненні ліміту ворогів")]
    public bool bossIgnoresMaxEnemies = false;

    [Header("Обмеження")]
    [Tooltip("Максимальна кількість ворогів одночасно")]
    public int maxEnemies = 20;

    [Header("Gizmos")]
    [Tooltip("Показувати область спавну в сцені")]
    public bool showSpawnArea = true;
    [Tooltip("Колір області спавну")]
    public Color spawnAreaColor = Color.red;

    private float currentSpawnRate;
    private int currentEnemies = 0;
    private float nextSpawnTime = 0f;
    private float difficultyTimer = 0f;
    private float nextBossTime = 0f;

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        difficultyTimer = spawnRateIncreaseInterval;
        nextBossTime = Time.time + bossSpawnInterval;
    }

    void Update()
    {
        difficultyTimer -= Time.deltaTime;
        if (difficultyTimer <= 0f)
        {
            IncreaseDifficulty();
            difficultyTimer = spawnRateIncreaseInterval;
        }

        if (Time.time >= nextBossTime)
        {
            TrySpawnBoss();
            nextBossTime = Time.time + bossSpawnInterval;
        }

        if (currentEnemies < maxEnemies && Time.time >= nextSpawnTime)
        {
            SpawnRandomEnemy();
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

    void SpawnRandomEnemy()
    {
        GameObject prefab = ChooseRandomEnemyPrefab();
        if (prefab == null) return;

        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0f,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        GameObject enemy = Instantiate(prefab, transform.position + randomPosition, Quaternion.identity);
        AttachHealthAndSubscribe(enemy);
        currentEnemies++;
        Debug.Log("Spawned enemy: " + enemy.name);
    }

    GameObject ChooseRandomEnemyPrefab()
    {
        if (enemyPrefabA == null && enemyPrefabB == null) return null;
        if (enemyPrefabA == null) return enemyPrefabB;
        if (enemyPrefabB == null) return enemyPrefabA;

        return Random.value < 0.5f ? enemyPrefabA : enemyPrefabB;
    }

    void TrySpawnBoss()
    {
        if (bossPrefab == null) return;
        if (!bossIgnoresMaxEnemies && currentEnemies >= maxEnemies) return;

        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0f,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        GameObject boss = Instantiate(bossPrefab, transform.position + randomPosition, Quaternion.identity);
        AttachHealthAndSubscribe(boss);
        currentEnemies++;
        Debug.Log("Бос з'явився!");
    }

    void AttachHealthAndSubscribe(GameObject obj)
    {
        EnemyHealth eh = obj.GetComponent<EnemyHealth>();
        if (eh == null) eh = obj.AddComponent<EnemyHealth>();
        eh.OnDeath += HandleEnemyDeath;
    }

    void HandleEnemyDeath()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
        Debug.Log("Ворог помер. Поточна кількість ворогів: " + currentEnemies);
    }

    public void SetSpawnRate(float newRate)
    {
        currentSpawnRate = Mathf.Clamp(newRate, 0.1f, maxSpawnRate);
    }

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

public class EnemyHealth : MonoBehaviour
{
    public System.Action OnDeath;
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " отримав " + damage + " шкоди. HP: " + health);
        if (health <= 0) Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " помер.");
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
