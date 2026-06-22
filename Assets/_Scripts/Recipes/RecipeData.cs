using System;
using System.Collections.Generic;
using UnityEngine;
using Wastepunk.Ingredients;

namespace Wastepunk.Recipes
{
    [Serializable]
    public struct RecipeRequirement
    {
        public IngredientData IngredientData;
        public IngredientState RequiredState;
        [Min(1)] public int Amount;
    }

    [CreateAssetMenu(fileName = "NewRecipeData", menuName = "Wastepunk/Recipe Data")]
    public class RecipeData : ScriptableObject
    {
        [SerializeField] private string recipeId;
        [SerializeField] private List<RecipeRequirement> requiredIngredients;
        [SerializeField] private GameObject resultPrefab;
        [SerializeField] private float timeToCookRecipe = 10f;

        public string RecipeId => recipeId;
        public List<RecipeRequirement> RequiredIngredients => requiredIngredients;
        public GameObject ResultPrefab => resultPrefab;
        public float TimeToCookRecipe => timeToCookRecipe;
    }
}