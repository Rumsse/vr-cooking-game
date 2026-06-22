using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Wastepunk.Ingredients;
using Wastepunk.Recipes;

namespace Wastepunk.Tools
{
    public class Skewer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform startPoint;
        [SerializeField] private float itemSpacing = 0.1f;
        [SerializeField] private int maxItems = 4;
        [SerializeField] private RecipeData skewerRecipe;

        private readonly List<IngredientBase> impaledItems = new List<IngredientBase>();

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if (impaledItems.Count >= maxItems)
                return;

            if (!other.TryGetComponent(out IngredientBase ingredient))
                return;

            if (!ingredient.CurrentState.HasFlag(IngredientState.Sliced))
                return;

            Impale(ingredient);
        }

        #endregion

        #region Logic

        private void Impale(IngredientBase ingredient)
        {
            if (impaledItems.Contains(ingredient))
                return;

            if (ingredient.TryGetComponent(out XRGrabInteractable grab))
                grab.enabled = false;

            if (ingredient.TryGetComponent(out Rigidbody rb))
                rb.isKinematic = true;

            Collider[] colliders = ingredient.GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
                col.enabled = false;

            Transform ingredientTransform = ingredient.transform;
            ingredientTransform.SetParent(startPoint);

            ingredientTransform.localRotation = Quaternion.identity;

            if (ingredient.TryGetComponent(out MeshFilter meshFilter))
            {
                Vector3 localCenter = meshFilter.sharedMesh.bounds.center;
                Vector3 scaledCenter = Vector3.Scale(localCenter, ingredientTransform.localScale);
                Vector3 targetPos = new Vector3(0, 0, impaledItems.Count * itemSpacing);
                ingredientTransform.localPosition = targetPos - scaledCenter;
            }
            else
                ingredientTransform.localPosition = new Vector3(0, 0, impaledItems.Count * itemSpacing);

            impaledItems.Add(ingredient);
            CheckRecipe();
        }

        private void CheckRecipe()
        {
            if (skewerRecipe == null)
                return;

            if (impaledItems.Count != maxItems)
                return;

            if (!ValidateSkewerIngredients())
                return;

            CompleteSkewer();
        }

        private bool ValidateSkewerIngredients()
        {
            if (skewerRecipe.RequiredIngredients.Count < 2)
                return false;

            RecipeRequirement req1 = skewerRecipe.RequiredIngredients[0];
            RecipeRequirement req2 = skewerRecipe.RequiredIngredients[1];

            for (int i = 0; i < impaledItems.Count; i++)
            {
                IngredientBase item = impaledItems[i];
                RecipeRequirement targetReq = i % 2 == 0 ? req1 : req2;

                if (item.Data != targetReq.IngredientData)
                    return false;

                if (!item.CurrentState.HasFlag(targetReq.RequiredState))
                    return false;
            }

            return true;
        }

        private void CompleteSkewer()
        {
            Debug.Log("Skewer recipe completed: " + skewerRecipe.RecipeName);

            foreach (IngredientBase item in impaledItems)
                Destroy(item.gameObject);

            impaledItems.Clear();

            if (skewerRecipe.ResultPrefab != null)
                Instantiate(skewerRecipe.ResultPrefab, startPoint.position, Quaternion.identity);
        }

        #endregion
    }
}