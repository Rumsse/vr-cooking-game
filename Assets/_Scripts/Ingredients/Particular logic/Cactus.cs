using System;
using UnityEngine;
using Wastepunk.Ingredients;

namespace Wastepunk.Cooking
{
    [RequireComponent(typeof(IngredientBase))]
    public class Cactus : MonoBehaviour
    {
        [SerializeField] private string sliceableLayerName = "Sliceable";

        private int totalNeedles;
        private int removedNeedles;
        private IngredientBase ingredientBase;

        public event Action OnFullyPeeled;

        private void Awake()
        {
            ingredientBase = GetComponent<IngredientBase>();
            totalNeedles = GetComponentsInChildren<CactusNeedle>().Length;
        }

        public void RegisterNeedleRemoved()
        {
            removedNeedles++;

            if (removedNeedles < totalNeedles)
                return;

            gameObject.layer = LayerMask.NameToLayer(sliceableLayerName);
            ingredientBase.ChangeState(IngredientState.Raw);
            OnFullyPeeled?.Invoke();
        }

        public bool IsFullyPeeled() => removedNeedles >= totalNeedles;
    }
}