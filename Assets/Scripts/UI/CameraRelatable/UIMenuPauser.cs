using UnityEngine;

public class UIMenuPauser : MonoBehaviour
{
    private void OnEnable() => PauseManager.Instance?.RequestPause();
    private void OnDisable() => PauseManager.Instance?.RequestResume();
}