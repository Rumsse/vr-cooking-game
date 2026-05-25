using UnityEngine;

namespace Wastepunk.Ingredients
{
    [CreateAssetMenu(fileName = "NewIngredientData", menuName = "Wastepunk/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
        [SerializeField] private string ingredientId;
        [SerializeField] private string ingredientName;
        [SerializeField] private IngredientState defaultState = IngredientState.Raw;
        [SerializeField] private float timeToCook = 5f;
        [SerializeField] private float timeToBurn = 15f;

        public string IngredientId => ingredientId;
        public string IngredientName => ingredientName;
        public IngredientState DefaultState => defaultState;
        public float TimeToCook => timeToCook;
        public float TimeToBurn => timeToBurn;
    }
}