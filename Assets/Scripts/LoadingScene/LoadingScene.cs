using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // назва сцени куди перейти
    [SerializeField] private float delay = 3f; // час очікування

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }
}