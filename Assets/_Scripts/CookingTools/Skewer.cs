using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Wastepunk.Ingredients;

namespace Wastepunk.Tools
{
    public class Skewer : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform startPoint;
        [SerializeField] private float itemSpacing = 0.1f;
        [SerializeField] private int maxItems = 4;
        [SerializeField] private string firstItemId = "rat";
        [SerializeField] private string secondItemId = "peach";

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
            {
                ingredientTransform.localPosition = new Vector3(0, 0, impaledItems.Count * itemSpacing);
            }

            impaledItems.Add(ingredient);
            CheckRecipe();
        }

        private void CheckRecipe()
        {
            if (impaledItems.Count != maxItems)
                return;

            bool isCorrect = true;

            for (int i = 0; i < impaledItems.Count; i++)
            {
                IngredientBase item = impaledItems[i];
                bool isFirstItemType = item.Data.IngredientId == firstItemId;
                bool isSecondItemType = item.Data.IngredientId == secondItemId;

                if (i % 2 == 0)
                {
                    if (!isFirstItemType || !item.CurrentState.HasFlag(IngredientState.Fried))
                        isCorrect = false;
                }
                else
                {
                    if (!isSecondItemType || !item.CurrentState.HasFlag(IngredientState.Raw))
                        isCorrect = false;
                }
            }

            if (isCorrect)
                Debug.Log("Shashlik recipe completed successfully!");
            else
                Debug.LogWarning("Invalid ingredients or states on the skewer.");
        }

        #endregion
    }
}