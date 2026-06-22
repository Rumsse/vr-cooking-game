using System.Collections.Generic;
using UnityEngine;
using Wastepunk.Interfaces;
using Wastepunk.Recipes;

namespace Wastepunk.Ingredients
{
    public class PotBase : MonoBehaviour, IFillable, IHeatable
    {
        #region Fields & Properties

        [SerializeField] private float maxVolume = 100f;
        [SerializeField] private Transform waterVisual;
        [SerializeField] private float minLocalY = -0.5f;
        [SerializeField] private float maxLocalY = 0.5f;
        [SerializeField] private float waterRequiredForCooking = 15f;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private List<RecipeData> availableRecipes;

        [SerializeField] private Transform detectionCenter;
        [SerializeField] private Vector3 detectionExtents = new Vector3(0.3f, 0.2f, 0.3f);

        private readonly List<ICookable> itemsInPot = new List<ICookable>();
        private readonly List<ICookable> matchedRecipeItems = new List<ICookable>();
        private readonly Collider[] overlapResults = new Collider[30];
        private RecipeData currentRecipe;
        private float recipeProgress;

        public float MaxVolume => maxVolume;
        public float CurrentVolume { get; private set; }
        public bool HasEnoughWater => CurrentVolume >= waterRequiredForCooking;

        #endregion

        #region Unity Methods

        private void Start() => UpdateWaterVisual();

        private void OnDrawGizmosSelected()
        {
            if (detectionCenter == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(detectionCenter.position, detectionCenter.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, detectionExtents * 2f);
        }

        #endregion

        #region Logic

        public void Fill(float amount)
        {
            if (IsFull())
                return;

            CurrentVolume += amount;
            CurrentVolume = Mathf.Clamp(CurrentVolume, 0f, maxVolume);
            UpdateWaterVisual();
        }

        public bool IsFull() => CurrentVolume >= maxVolume;

        public void Heat(float intensity)
        {
            if (!HasEnoughWater)
            {
                Debug.LogWarning("Heating failed. Not enough water!");
                return;
            }

            ScanPotContents();
            EvaluateRecipes();

            if (currentRecipe != null)
            {
                ProcessRecipe(intensity);
                return;
            }

            for (int i = itemsInPot.Count - 1; i >= 0; i--)
                ProcessItem(itemsInPot[i], intensity);
        }

        private void ScanPotContents()
        {
            itemsInPot.Clear();

            if (detectionCenter == null)
                return;

            int count = Physics.OverlapBoxNonAlloc(detectionCenter.position, detectionExtents, overlapResults, detectionCenter.rotation);

            for (int i = 0; i < count; i++)
            {
                ICookable cookable = overlapResults[i].GetComponentInParent<ICookable>();

                if (cookable == null)
                    continue;

                if (!cookable.CanBeCooked(CookingType.Boiling))
                    continue;

                if (itemsInPot.Contains(cookable))
                    continue;

                itemsInPot.Add(cookable);
            }
        }

        private void ProcessItem(ICookable cookable, float intensity)
        {
            cookable.CookingProgress += Time.deltaTime * intensity;

            if (cookable.CookingProgress >= cookable.TimeToBurn)
                cookable.Burn();
            else if (cookable.CookingProgress >= cookable.TimeToCook)
                cookable.Cook(CookingType.Boiling);
        }

        private void ProcessRecipe(float intensity)
        {
            recipeProgress += Time.deltaTime * intensity;

            Debug.Log("Cooking recipe: " + currentRecipe.RecipeId + " Progress: " + recipeProgress);

            if (recipeProgress < currentRecipe.TimeToCookRecipe)
                return;

            CompleteRecipe();
        }

        private void EvaluateRecipes()
        {
            RecipeData previousRecipe = currentRecipe;
            currentRecipe = null;
            matchedRecipeItems.Clear();

            if (itemsInPot.Count == 0)
            {
                recipeProgress = 0f;
                return;
            }

            foreach (RecipeData recipe in availableRecipes)
            {
                if (!TryMatchRecipe(recipe))
                    continue;

                currentRecipe = recipe;
                break;
            }

            if (currentRecipe != previousRecipe)
                recipeProgress = 0f;
        }

        private void CompleteRecipe()
        {
            Debug.Log("Recipe completed! Spawning prefab.");

            foreach (ICookable item in matchedRecipeItems)
            {
                itemsInPot.Remove(item);

                if (item is MonoBehaviour mb)
                    Destroy(mb.gameObject);
            }

            matchedRecipeItems.Clear();

            if (currentRecipe.ResultPrefab != null)
            {
                Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.up * 0.3f;
                Instantiate(currentRecipe.ResultPrefab, spawnPos, Quaternion.identity);
            }

            currentRecipe = null;
            recipeProgress = 0f;
        }

        private bool TryMatchRecipe(RecipeData recipe)
        {
            int totalRequiredAmount = 0;

            foreach (RecipeRequirement req in recipe.RequiredIngredients)
                totalRequiredAmount += req.Amount;

            if (itemsInPot.Count != totalRequiredAmount)
                return false;

            List<ICookable> availableItems = new List<ICookable>(itemsInPot);
            List<ICookable> tempMatchedItems = new List<ICookable>();

            foreach (RecipeRequirement req in recipe.RequiredIngredients)
            {
                int amountFound = 0;

                for (int i = availableItems.Count - 1; i >= 0; i--)
                {
                    if (!(availableItems[i] is IngredientBase ing))
                        continue;

                    if (ing.Data != req.IngredientData)
                        continue;

                    if (!ing.CurrentState.HasFlag(req.RequiredState))
                        continue;

                    tempMatchedItems.Add(availableItems[i]);
                    availableItems.RemoveAt(i);
                    amountFound++;

                    if (amountFound == req.Amount)
                        break;
                }

                if (amountFound < req.Amount)
                    return false;
            }

            matchedRecipeItems.AddRange(tempMatchedItems);
            return true;
        }

        private void UpdateWaterVisual()
        {
            if (waterVisual == null)
                return;

            if (CurrentVolume <= 0f)
            {
                waterVisual.gameObject.SetActive(false);
                return;
            }

            if (!waterVisual.gameObject.activeSelf)
                waterVisual.gameObject.SetActive(true);

            float fillRatio = CurrentVolume / maxVolume;
            Vector3 newPos = waterVisual.localPosition;
            newPos.y = Mathf.Lerp(minLocalY, maxLocalY, fillRatio);
            waterVisual.localPosition = newPos;
        }

        #endregion
    }
}