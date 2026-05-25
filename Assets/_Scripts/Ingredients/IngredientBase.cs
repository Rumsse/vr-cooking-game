using System;
using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Ingredients
{
    public class IngredientBase : MonoBehaviour, ICookable
    {
        [SerializeField] private IngredientData ingredientData;

        public IngredientState CurrentState { get; private set; }
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
            if (CurrentState == newState)
                return;

            if (CurrentState == IngredientState.Burned)
                return;

            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void Cook() => ChangeState(IngredientState.Cooked);

        public void Burn() => ChangeState(IngredientState.Burned);

        public bool CanBeCooked()
        {
            if (CurrentState == IngredientState.Burned)
                return false;

            if (CurrentState == IngredientState.Dirty || CurrentState == IngredientState.NotPeeled)
            {
                Debug.LogWarning("Ingredient is not ready to be cooked on " + gameObject.name);
                return false;
            }

            return true;
        }
    }
}