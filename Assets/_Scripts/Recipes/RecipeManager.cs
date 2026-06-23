using System.Collections.Generic;
using UnityEngine;

namespace Wastepunk.Recipes
{
    public class RecipeManager : MonoBehaviour
    {
        public static RecipeManager Instance { get; private set; }

        [SerializeField] private List<RecipeData> allRecipes;

        private RecipeData currentActiveRecipe;

        private void Awake() => Instance = this;

        public RecipeData GetRandomRecipe()
        {
            if (allRecipes == null || allRecipes.Count == 0)
                return null;

            int randomIndex = Random.Range(0, allRecipes.Count);
            currentActiveRecipe = allRecipes[randomIndex];

            Debug.Log("New active recipe selected: " + currentActiveRecipe.RecipeName);
            return currentActiveRecipe;
        }

        public RecipeData GetRandomRecipeByType(RecipeType type)
        {
            List<RecipeData> filtered = allRecipes.FindAll(r => r.RecipeType == type);

            if (filtered.Count == 0)
                return null;

            return filtered[Random.Range(0, filtered.Count)];
        }

        public bool IsCurrentRecipe(RecipeData recipe) => currentActiveRecipe == recipe;
    }
}