using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Налаштування сцен")]
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";
    public string nextLevelSceneName = "NextLevel";
    
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
        LoadScene(loseSceneName);
    }

    public void OnGameWon()
    {
        if (gameEnded) return;
        gameEnded = true;
        
        Debug.Log("Гра виграна! Завантаження сцени перемоги.");
        LoadScene(winSceneName);
    }

    public void LoadNextLevel()
    {
        if (gameEnded) return;
        
        Debug.Log("Завантаження наступного рівня.");
        LoadScene(nextLevelSceneName);
    }

    private void LoadScene(string sceneName)
    {
        // Знищуємо Wallet.Instance перед завантаженням нової сцени
        if (Wallet.Instance != null)
        {
            Destroy(Wallet.Instance.gameObject);
            Wallet.Instance = null;
        }
        
        // Завантажуємо сцену
        SceneManager.LoadScene(sceneName);
    }
}