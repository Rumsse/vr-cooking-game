using UnityEngine;
using UnityEngine.SceneManagement;
using Wastepunk.Creatures;

public class GameOverManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private WormController wormController;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Transform vrCamera;
    [SerializeField] private GameObject locomotionObject;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private string mainMenuSceneName;

    #endregion

    #region Unity Methods

    private void OnEnable() => SubscribeEvents();

    private void OnDisable() => UnsubscribeEvents();

    #endregion

    #region Logic

    public void RestartGame() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

    public void ReturnToMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
            return;

        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }

    private void SubscribeEvents()
    {
        if (wormController == null)
            return;

        wormController.OnGameOver += ShowGameOverScreen;
    }

    private void UnsubscribeEvents()
    {
        if (wormController == null)
            return;

        wormController.OnGameOver -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        Debug.Log("Displaying Game Over UI and freezing player locomotion");
        FreezePlayerMovement();

        if (gameOverUI == null)
            return;

        PositionUIInFrontOfPlayer();
        gameOverUI.SetActive(true);
    }

    private void FreezePlayerMovement()
    {
        if (locomotionObject == null)
            return;

        locomotionObject.SetActive(false);
        Debug.Log("Locomotion object disabled");
    }

    private void PositionUIInFrontOfPlayer()
    {
        if (vrCamera == null)
            vrCamera = Camera.main != null ? Camera.main.transform : null;

        if (vrCamera == null)
            return;

        Vector3 targetPosition = vrCamera.position + vrCamera.forward * spawnDistance;
        gameOverUI.transform.position = targetPosition;

        Vector3 lookAtTarget = targetPosition - vrCamera.position;
        lookAtTarget.y = 0f;

        if (lookAtTarget == Vector3.zero)
            return;

        gameOverUI.transform.rotation = Quaternion.LookRotation(lookAtTarget);
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

    #endregion
}