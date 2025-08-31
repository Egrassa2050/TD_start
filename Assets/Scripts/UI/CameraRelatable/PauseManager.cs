using UnityEngine;
using System;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private PauseSettings pauseSettings;
    
    public bool IsGamePaused { get; private set; }
    public event Action<bool> OnPauseStateChanged;

    private List<IPausable> pausableObjects = new List<IPausable>();
    private int pauseRequests = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (pauseSettings == null)
            Debug.LogWarning("PauseSettings not assigned!");
    }

    public void RegisterPausable(IPausable pausable)
    {
        if (!pausableObjects.Contains(pausable))
            pausableObjects.Add(pausable);
    }

    public void UnregisterPausable(IPausable pausable)
    {
        if (pausableObjects.Contains(pausable))
            pausableObjects.Remove(pausable);
    }

    public void RequestPause()
    {
        pauseRequests++;
        UpdatePauseState();
    }

    public void RequestResume()
    {
        pauseRequests = Mathf.Max(0, pauseRequests - 1);
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        bool shouldPause = pauseRequests > 0;

        if (IsGamePaused != shouldPause)
        {
            IsGamePaused = shouldPause;
            UpdateTimeScale();
            UpdateCursorState();
            NotifyPausableObjects();
            OnPauseStateChanged?.Invoke(shouldPause);
            
            Debug.Log("Game " + (shouldPause ? "paused" : "resumed"));
        }
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = IsGamePaused ? 
            pauseSettings.pausedTimeScale : 
            pauseSettings.normalTimeScale;
    }

    private void UpdateCursorState()
    {
        if (IsGamePaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (pauseSettings.hideCursorOnResume)
        {
            Cursor.visible = false;
            Cursor.lockState = pauseSettings.resumeCursorLock;
        }
    }

    private void NotifyPausableObjects()
    {
        foreach (var pausable in pausableObjects)
        {
            if (IsGamePaused)
                pausable.OnPause();
            else
                pausable.OnResume();
        }
    }

    public void ForceResume()
    {
        pauseRequests = 0;
        UpdatePauseState();
    }
}

// Інтерфейс для всіх об'єктів, які мають реагувати на паузу
public interface IPausable
{
    void OnPause();
    void OnResume();
}