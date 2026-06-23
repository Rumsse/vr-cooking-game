using System;
using System.Collections.Generic;
using UnityEngine;
using Wastepunk.Ingredients;

namespace Wastepunk.Recipes
{
    public enum RecipeType
    {
        Boiling,
        Assembly
    }

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
        [SerializeField] private string recipeName;
        [SerializeField] private RecipeType recipeType;
        [SerializeField] private Sprite recipeIcon;
        [SerializeField] private List<RecipeRequirement> requiredIngredients;
        [SerializeField] private GameObject resultPrefab;
        [SerializeField] private float processingTime = 5f;

        public string RecipeId => recipeId;
        public string RecipeName => recipeName;
        public RecipeType RecipeType => recipeType;
        public Sprite RecipeIcon => recipeIcon;
        public List<RecipeRequirement> RequiredIngredients => requiredIngredients;
        public GameObject ResultPrefab => resultPrefab;
        public float ProcessingTime => processingTime;
    }
}