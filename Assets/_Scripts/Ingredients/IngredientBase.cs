using System;
using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Ingredients
{
    public class IngredientBase : MonoBehaviour, ICookable, ISeasonable
    {
        [SerializeField] private IngredientData ingredientData;
        [SerializeField] private IngredientState currentState;

        public IngredientState CurrentState => currentState;
        public IngredientData Data => ingredientData;

        public float CookingProgress { get; set; }
        public float TimeToCook => ingredientData.TimeToCook;
        public float TimeToBurn => ingredientData.TimeToBurn;

        public event Action<IngredientState> OnStateChanged;

        private void Start()
        {
            if (ingredientData == null)
            {
                Debug.LogError("IngredientData is missing on " + gameObject.name);
                return;
            }

            ChangeState(ingredientData.DefaultState);
        }

        public void ChangeState(IngredientState newState)
        {
            if (currentState == newState)
                return;

            if (currentState == IngredientState.Burned)
                return;

            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        public void Cook(CookingType cookingType)
        {
            if (cookingType == CookingType.Boiling)
                ChangeState(IngredientState.Boiled);
            else if (cookingType == CookingType.Frying)
                ChangeState(IngredientState.Fried);
        }

        public void Burn() => ChangeState(IngredientState.Burned);

        public void Season() => ChangeState(IngredientState.Seasoned);

        public bool CanBeSeasoned()
        {
            if (currentState == IngredientState.Dirty || currentState == IngredientState.NotPeeled || currentState == IngredientState.Burned)
                return false;

            if (currentState == IngredientState.Seasoned)
                return false;

            return true;
        }

        public bool CanBeCooked(CookingType cookingType)
        {
            if (currentState == IngredientState.Burned)
                return false;

            if (currentState == IngredientState.Dirty || currentState == IngredientState.NotPeeled)
            {
                Debug.LogWarning("Ingredient is not ready to be cooked on " + gameObject.name);
                return false;
            }

            if (cookingType == CookingType.Boiling && !ingredientData.CanBeBoiled)
            {
                Debug.LogWarning("This ingredient cannot be boiled: " + gameObject.name);
                return false;
            }

            if (cookingType == CookingType.Frying && !ingredientData.CanBeFried)
            {
                Debug.LogWarning("This ingredient cannot be fried: " + gameObject.name);
                return false;
            }

            return true;
        }
    }
}