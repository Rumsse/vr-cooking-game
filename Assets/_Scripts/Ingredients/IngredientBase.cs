using System;
using UnityEngine;

namespace Wastepunk.Ingredients
{
    public class IngredientBase : MonoBehaviour
    {
        [SerializeField] private IngredientData ingredientData;

        public IngredientState CurrentState { get; private set; }
        public IngredientData Data => ingredientData;

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
    }
}
