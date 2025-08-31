using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Pause Settings")]
public class PauseSettings : ScriptableObject
{
    [Header("Cursor Settings")]
    public bool hideCursorOnResume = true;
    public CursorLockMode resumeCursorLock = CursorLockMode.Locked;
    
    [Header("Time Settings")]
    public float pausedTimeScale = 0f;
    public float normalTimeScale = 1f;
}