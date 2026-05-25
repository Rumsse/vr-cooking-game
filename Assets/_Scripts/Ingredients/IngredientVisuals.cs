using System;
using UnityEngine;

namespace Wastepunk.Ingredients
{
    [Serializable]
    public struct StateMaterial
    {
        public IngredientState State;
        public Material Material;
    }

    [RequireComponent(typeof(IngredientBase), typeof(Renderer))]
    public class IngredientVisuals : MonoBehaviour
    {
        [SerializeField] private StateMaterial[] stateMaterials;

        private IngredientBase ingredient;
        private Renderer meshRenderer;

        private void Awake()
        {
            ingredient = GetComponent<IngredientBase>();
            meshRenderer = GetComponent<Renderer>();
        }

        private void OnEnable() => ingredient.OnStateChanged += UpdateMaterial;

        private void OnDisable() => ingredient.OnStateChanged -= UpdateMaterial;

        private void Start() => UpdateMaterial(ingredient.CurrentState);

        private void UpdateMaterial(IngredientState state)
        {
            foreach (var stateMat in stateMaterials)
            {
                if (stateMat.State != state)
                    continue;

                meshRenderer.material = stateMat.Material;
                return;
            }

            Debug.LogWarning("Material for state " + state + " not found on " + gameObject.name);
        }
    }
}