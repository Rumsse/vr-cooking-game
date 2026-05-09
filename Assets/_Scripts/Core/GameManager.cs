using UnityEngine;

namespace Wastepunk.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start() => ChangeState(GameState.Playing);

        private void OnEnable() => GameEvents.OnGameOver += HandleGameOver;

        private void OnDisable() => GameEvents.OnGameOver -= HandleGameOver;

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) 
                return;

            CurrentState = newState;
            GameEvents.TriggerGameStateChanged(CurrentState);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.Playing:
                    break;
                case GameState.GameOver:
                    HandleGameOver();
                    break;
            }
        }

        private void HandleGameOver()
        {
            if (CurrentState == GameState.GameOver)
                return;

            ChangeState(GameState.GameOver);
            Debug.Log("Game Over");
        }
    }
}
