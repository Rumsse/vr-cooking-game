using UnityEngine;
using Wastepunk.Recipes;

namespace Wastepunk.Recipes
{
    public class FinishedDish : MonoBehaviour
    {
        [SerializeField] private RecipeData recipeData;

        public RecipeData RecipeData => recipeData;
    }
}