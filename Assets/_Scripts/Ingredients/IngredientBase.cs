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

            if (currentState == IngredientState.None)
                ChangeState(ingredientData.DefaultState);
        }

        public void Initialize(IngredientData data, IngredientState state, float progress)
        {
            ingredientData = data;
            currentState = state;
            CookingProgress = progress;
        }

        public void AddState(IngredientState stateFlag)
        {
            if (currentState.HasFlag(IngredientState.Burned))
                return;

            if (currentState.HasFlag(stateFlag))
                return;

            currentState |= stateFlag;
            OnStateChanged?.Invoke(currentState);
        }

        public void RemoveState(IngredientState stateFlag)
        {
            if (!currentState.HasFlag(stateFlag))
                return;

            currentState &= ~stateFlag;
            OnStateChanged?.Invoke(currentState);
        }

        public void ChangeState(IngredientState newState)
        {
            if (currentState == newState)
                return;

            if (currentState.HasFlag(IngredientState.Burned))
                return;

            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        public void Cook(CookingType cookingType)
        {
            RemoveState(IngredientState.Raw);

            if (cookingType == CookingType.Boiling)
                AddState(IngredientState.Boiled);
            else if (cookingType == CookingType.Frying)
                AddState(IngredientState.Fried);
        }

        public void Burn()
        {
            RemoveState(IngredientState.Raw);
            RemoveState(IngredientState.Boiled);
            RemoveState(IngredientState.Fried);

            AddState(IngredientState.Burned);
        }

        public void Season() => AddState(IngredientState.Seasoned);

        public bool CanBeSeasoned()
        {
            if (currentState.HasFlag(IngredientState.Dirty) || currentState.HasFlag(IngredientState.NotPeeled) || currentState.HasFlag(IngredientState.Burned))
                return false;

            if (currentState.HasFlag(IngredientState.Seasoned))
                return false;

            return true;
        }

        public bool CanBeCooked(CookingType cookingType)
        {
            if (currentState.HasFlag(IngredientState.Burned))
                return false;

            if (currentState.HasFlag(IngredientState.Dirty) || currentState.HasFlag(IngredientState.NotPeeled))
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