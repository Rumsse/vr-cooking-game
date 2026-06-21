using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName;

    public void StartGame()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
            return;

        SceneManager.LoadSceneAsync(gameplaySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Executing QuitGame");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}