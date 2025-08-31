using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalPauseHandler : MonoBehaviour
{
    [Header("Налаштування")]
    public KeyCode pauseKey = KeyCode.Escape;
    public GameObject pauseMenuPanel;

    [Header("Стан курсора")]
    public bool hideCursorOnPause = true;
    public CursorLockMode pausedCursorState = CursorLockMode.None;

    private bool isPaused = false;
    private CursorLockMode previousCursorState;
    private bool wasCursorVisible;

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (IsAnyUIOpen())
            {
                CloseTopmostUI();
            }
            else
            {
                TogglePause();
            }
        }
    }

    private bool IsAnyUIOpen()
    {
        // Перевіряємо чи є активні UI елементи крім паузи
        return EventSystem.current.currentSelectedGameObject != null &&
               EventSystem.current.currentSelectedGameObject != pauseMenuPanel;
    }

    private void CloseTopmostUI()
    {
        // Знаходимо найвищий UI елемент і закриваємо його
        GameObject topmostUI = FindTopmostUI();
        if (topmostUI != null)
        {
            topmostUI.SetActive(false);
        }
    }

    private GameObject FindTopmostUI()
    {
        // Пошук активних UI елементів
        var uiElements = FindObjectsOfType<GameObject>();
        foreach (var element in uiElements)
        {
            if (element.activeInHierarchy && element != pauseMenuPanel && element.CompareTag("UI"))
            {
                return element;
            }
        }
        return null;
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        // Зберігаємо стан курсора
        previousCursorState = Cursor.lockState;
        wasCursorVisible = Cursor.visible;

        // Встановлюємо паузу
        PauseManager.Instance.RequestPause();
        
        // Налаштовуємо курсор
        if (hideCursorOnPause)
        {
            Cursor.visible = true;
            Cursor.lockState = pausedCursorState;
        }

        // Активуємо меню паузи
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }

    private void ResumeGame()
    {
        // Відновлюємо гру
        PauseManager.Instance.RequestResume();
        
        // Відновлюємо курсор
        Cursor.visible = wasCursorVisible;
        Cursor.lockState = previousCursorState;

        // Деактивуємо меню паузи
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Підписуємось на події паузи
        PauseManager.Instance.OnPauseStateChanged += HandlePauseStateChange;
    }

    private void OnDisable()
    {
        // Відписуємось від подій паузи
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OnPauseStateChanged -= HandlePauseStateChange;
        }
    }

    private void HandlePauseStateChange(bool isPaused)
    {
        // Синхронізуємо внутрішній стан з менеджером паузи
        this.isPaused = isPaused;
    }
}