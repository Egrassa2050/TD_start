using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Налаштування сцен")]
    [Tooltip("Сцена, яка показується при перемозі (екран перемоги)")]
    public string winSceneName = "WinScene";
    [Tooltip("Сцена яка завантажується при програші")]
    public string loseSceneName = "LoseScene";
    [Tooltip("Сцена наступного рівня")]
    public string nextLevelSceneName = "NextLevel";

    [Header("Поведінка після перемоги")]
    [Tooltip("Якщо true — спочатку завантажиться сцена перемоги, потім через winDisplayDuration відбудеться перехід на наступний рівень")]
    public bool showWinSceneImmediately = true;
    [Tooltip("Час в секундах який має пройти перед переходом (або перед завантаженням сцени перемоги, залежно від showWinSceneImmediately)")]
    public float winDisplayDuration = 5f;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnGameLost()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Гра програна! Завантаження сцени програшу.");
        StartCoroutine(LoadSceneRoutine(loseSceneName, 0f));
    }

    public void OnGameWon()
    {
        if (gameEnded) return;
        gameEnded = true;

        StartCoroutine(DelayedWinRoutine());
    }

    private IEnumerator DelayedWinRoutine()
    {
        Debug.Log("Гравець виграв. Обробка переходу...");

        if (showWinSceneImmediately)
        {
            // Показуємо сцену перемоги одразу
            yield return StartCoroutine(LoadSceneRoutine(winSceneName, 0f));

            // Чекаємо реального часу (не залежить від timescale)
            yield return new WaitForSecondsRealtime(winDisplayDuration);

            // Переходимо на наступний рівень
            Debug.Log("Час показу пройшов — завантаження наступного рівня.");
            yield return StartCoroutine(LoadSceneRoutine(nextLevelSceneName, 0f));
        }
        else
        {
            // Спочатку пауза (наприклад показати анімацію на поточній сцені)
            yield return new WaitForSecondsRealtime(winDisplayDuration);

            // Потім завантажуємо сцену перемоги (якщо треба, можна потім ще додати перехід)
            Debug.Log("Пауза завершена — завантаження сцени перемоги.");
            yield return StartCoroutine(LoadSceneRoutine(winSceneName, 0f));
        }
    }

    public void LoadNextLevel()
    {
        if (gameEnded) return;

        Debug.Log("Завантаження наступного рівня (викликано вручну).");
        StartCoroutine(LoadSceneRoutine(nextLevelSceneName, 0f));
    }

    // Універсальний рутін для завантаження — знищує Wallet якщо він є і використовує async завантаження
    private IEnumerator LoadSceneRoutine(string sceneName, float delayBeforeLoad)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("LoadSceneRoutine: пусте ім'я сцени.");
            yield break;
        }

        if (delayBeforeLoad > 0f)
            yield return new WaitForSecondsRealtime(delayBeforeLoad);

        // Якщо є Wallet — знищуємо його перед завантаженням
        if (Wallet.Instance != null)
        {
            Destroy(Wallet.Instance.gameObject);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        if (asyncLoad == null)
        {
            Debug.LogError($"Не вдалося асинхронно завантажити сцену: {sceneName}");
            yield break;
        }

        // Чекаємо поки сцена завантажиться
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
