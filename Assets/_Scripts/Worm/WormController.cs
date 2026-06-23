using System;
using UnityEngine;
using Wastepunk.Recipes;

namespace Wastepunk.Creatures
{
    public class WormController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float hungerDecreaseRate = 1.5f;
        [SerializeField] private float correctFoodReward = 35f;
        [SerializeField] private float wrongFoodPenalty = 20f;
        [SerializeField] private float timePerRequest = 40f;

        private float currentHunger;
        private float requestTimer;
        private RecipeData desiredRecipe;
        private bool isGameOver;

        public float HungerRatio => currentHunger / maxHunger;
        public RecipeData DesiredRecipe => desiredRecipe;

        public event Action OnMonsterStateChanged;
        public event Action OnGameOver;

        #endregion

        #region Unity Methods

        private void Start()
        {
            currentHunger = maxHunger;
            SelectNewRequest();
        }

        private void Update()
        {
            if (isGameOver)
                return;

            HandleHunger();
            HandleRequestTimer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isGameOver)
                return;

            if (!other.TryGetComponent(out FinishedDish dish))
                return;

            EvaluateFood(dish);
        }

        #endregion

        #region Logic

        private void HandleHunger()
        {
            currentHunger -= Time.deltaTime * hungerDecreaseRate;
            OnMonsterStateChanged?.Invoke();

            if (currentHunger > 0f)
                return;

            TriggerGameOver();
        }

        private void HandleRequestTimer()
        {
            requestTimer -= Time.deltaTime;

            if (requestTimer > 0f)
                return;

            SelectNewRequest();
        }

        private void SelectNewRequest()
        {
            if (RecipeManager.Instance == null)
                return;

            desiredRecipe = RecipeManager.Instance.GetRandomRecipe();
            requestTimer = timePerRequest;
            OnMonsterStateChanged?.Invoke();
        }

        private void EvaluateFood(FinishedDish dish)
        {
            if (dish.RecipeData == desiredRecipe)
            {
                currentHunger = Mathf.Clamp(currentHunger + correctFoodReward, 0f, maxHunger);
                Debug.Log("Monster fed correctly with: " + dish.RecipeData.RecipeName);
                SelectNewRequest();
            }
            else
            {
                currentHunger = Mathf.Clamp(currentHunger - wrongFoodPenalty, 0f, maxHunger);
                Debug.LogWarning("Monster fed WRONG food: " + dish.RecipeData.RecipeName);
            }

            Destroy(dish.gameObject);
            OnMonsterStateChanged?.Invoke();

            if (currentHunger > 0f)
                return;

            TriggerGameOver();
        }

        private void TriggerGameOver()
        {
            isGameOver = true;
            currentHunger = 0f;
            OnGameOver?.Invoke();
            Debug.LogError("Game Over! The monster starved.");
        }

        #endregion
    }
}