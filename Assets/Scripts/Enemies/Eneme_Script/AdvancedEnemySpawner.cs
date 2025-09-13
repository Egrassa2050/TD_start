using UnityEngine;
using System.Collections;

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

    [Header("Контроль спавну (базові значення)")]
    [Tooltip("Базовий спавн рейт (ворогів/сек) — множиться на значення з spawnRateCurve для поточної хвилі")]
    public float baseSpawnRate = 1f;
    [Tooltip("Максимальний спавн рейт (ворогів/сек) — остаточне обмеження")]
    public float maxSpawnRate = 10f;

    [Header("Хвилі")]
    [Tooltip("Загальна кількість хвиль")]
    public int totalWaves = 5;
    [Tooltip("Базова кількість ворогів у хвилі — множиться на значення з enemyCountCurve для поточної хвилі")]
    public int enemiesPerWave = 10;
    [Tooltip("Мінімальна затримка між хвилями (сек)")]
    public float minWaveDelay = 5f;
    [Tooltip("Максимальна затримка між хвилями (сек)")]
    public float maxWaveDelay = 10f;
    [Tooltip("Анімаційна крива множника кількості ворогів по хвилах. X: 0..1 (перша->остання хвиля), Y: множник")]
    public AnimationCurve enemyCountCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 2f);
    [Tooltip("Анімаційна крива множника спавн-рейту по хвилах. X: 0..1 (перша->остання хвиля), Y: множник")]
    public AnimationCurve spawnRateCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 2f);

    [Header("Бос")]
    [Tooltip("Інтервал між появами боса (сек)")]
    public float bossSpawnInterval = 60f;
    [Tooltip("Якщо true — бос може спавнитися навіть при досягненні ліміту ворогів")]
    public bool bossIgnoresMaxEnemies = false;

    [Header("Обмеження")]
    [Tooltip("Максимальна кількість ворогів одночасно")]
    public int maxEnemies = 30;

    [Header("Gizmos")]
    [Tooltip("Показувати область спавну в сцені")]
    public bool showSpawnArea = true;
    [Tooltip("Колір області спавну")]
    public Color spawnAreaColor = Color.red;

    // runtime
    private float currentSpawnRate;
    private int currentEnemies = 0;
    private float nextSpawnTime = 0f;
    private int currentWave = 0;
    private int enemiesSpawnedInWave = 0;
    private int enemiesToSpawnThisWave = 0;
    private bool isSpawningBoss = false;
    private float nextBossTime = 0f;

    void Start()
    {
        currentWave = 1;
        PrepareWave(currentWave);
        nextBossTime = Time.time + bossSpawnInterval;
        Debug.Log($"Початок хвилі {currentWave} — заплановано спавн {enemiesToSpawnThisWave} ворогів");
    }

    void Update()
    {
        if (currentWave > totalWaves) return;

        if (Time.time >= nextBossTime && !isSpawningBoss)
        {
            TrySpawnBoss();
            nextBossTime = Time.time + bossSpawnInterval;
        }

        if (currentEnemies < maxEnemies && Time.time >= nextSpawnTime && enemiesSpawnedInWave < enemiesToSpawnThisWave)
        {
            SpawnRandomEnemy();
            enemiesSpawnedInWave++;
            nextSpawnTime = Time.time + 1f / currentSpawnRate;
        }

        CheckWaveCompletion();
    }

    void PrepareWave(int waveNumber)
    {
        // normalized X для кривих: 0 для першої хвилі, 1 для останньої
        float normalized = (totalWaves <= 1) ? 0f : (waveNumber - 1) / (float)(totalWaves - 1);

        // enemy count based on curve (плавно, не по прямій)
        float enemyMultiplier = (enemyCountCurve != null) ? enemyCountCurve.Evaluate(normalized) : 1f;
        enemiesToSpawnThisWave = Mathf.Max(1, Mathf.RoundToInt(enemiesPerWave * enemyMultiplier));

        // spawn rate based on curve
        float spawnMultiplier = (spawnRateCurve != null) ? spawnRateCurve.Evaluate(normalized) : 1f;
        currentSpawnRate = Mathf.Clamp(baseSpawnRate * spawnMultiplier, 0.01f, maxSpawnRate);

        // reset wave spawn timers/counters
        enemiesSpawnedInWave = 0;
        nextSpawnTime = Time.time; // стартувати зразу коли дозволить логіка
        Debug.Log($"Підготовка хвилі {waveNumber}. Mножник ворогів: {enemyMultiplier:F2}, ворогів для спавну: {enemiesToSpawnThisWave}. Спавн-рейт: {currentSpawnRate:F2} в/сек.");
    }

    void CheckWaveCompletion()
    {
        // коли всі вороги хвилі заспавнені і поточних ворогів 0 — хвиля завершена
        if (enemiesSpawnedInWave >= enemiesToSpawnThisWave && currentEnemies == 0)
        {
            if (currentWave < totalWaves)
            {
                StartCoroutine(StartNextWaveRoutine());
            }
            else
            {
                GameManager.Instance.OnGameWon();
            }
        }
    }

    IEnumerator StartNextWaveRoutine()
    {
        float delay = Random.Range(minWaveDelay, maxWaveDelay);
        Debug.Log($"Хвиля {currentWave} завершена. Наступна хвиля {currentWave + 1} через {delay:F1} сек.");
        yield return new WaitForSeconds(delay);

        currentWave++;
        PrepareWave(currentWave);
        Debug.Log($"Початок хвилі {currentWave}!");
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
        AttachDeathNotifier(enemy);
        currentEnemies++;
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

        isSpawningBoss = true;

        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0f,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );

        GameObject boss = Instantiate(bossPrefab, transform.position + randomPosition, Quaternion.identity);
        AttachDeathNotifier(boss);
        currentEnemies++;
        isSpawningBoss = false;
    }

    void AttachDeathNotifier(GameObject obj)
    {
        EnemyDeathNotifier notifier = obj.GetComponent<EnemyDeathNotifier>();
        if (notifier == null) notifier = obj.AddComponent<EnemyDeathNotifier>();
        notifier.OnDeath += HandleEnemyDeath;
    }

    void HandleEnemyDeath(GameObject enemy)
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);

        EnemyDeathNotifier notifier = enemy.GetComponent<EnemyDeathNotifier>();
        if (notifier != null)
        {
            notifier.OnDeath -= HandleEnemyDeath;
        }
    }

    public void SetBaseSpawnRate(float newRate)
    {
        baseSpawnRate = Mathf.Max(0.01f, newRate);
        currentSpawnRate = Mathf.Clamp(baseSpawnRate, 0.01f, maxSpawnRate);
    }

    public void SetMaxEnemies(int newMax)
    {
        maxEnemies = Mathf.Max(1, newMax);
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
