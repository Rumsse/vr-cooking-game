using EzySlice;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Wastepunk.Ingredients;

[RequireComponent(typeof(Rigidbody))]
public class SliceObject : MonoBehaviour
{
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private LayerMask sliceableLayer;
    [SerializeField] private Material defaultCrossSectionMaterial;
    [SerializeField] private float cutForce = 200f;
    [SerializeField] private float minVelocityToCut = 0.5f;

    private Rigidbody bladeRigidbody;
    private Vector3 previousPosition;
    private Vector3 estimatedVelocity;

    private void Awake()
    {
        bladeRigidbody = GetComponent<Rigidbody>();
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        CalculateManualVelocity();

        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);

        if (!hasHit)
            return;

        Slice(hit.transform.gameObject);
    }

    private void CalculateManualVelocity()
    {
        estimatedVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;
    }

    public void Slice(GameObject target)
    {
        if (estimatedVelocity.magnitude < minVelocityToCut)
            return;

        Vector3 bladeDirection = endSlicePoint.position - startSlicePoint.position;
        Vector3 planeNormal = Vector3.Cross(bladeDirection, estimatedVelocity).normalized;

        if (planeNormal == Vector3.zero)
            return;

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull == null)
            return;

        IngredientData extractedData = null;
        IngredientState extractedState = IngredientState.None;
        float extractedProgress = 0f;
        StateMaterial[] extractedVisuals = null;

        if (target.TryGetComponent(out IngredientBase originalIngredient))
        {
            extractedData = originalIngredient.Data;
            extractedState = originalIngredient.CurrentState;
            extractedProgress = originalIngredient.CookingProgress;
        }

        if (target.TryGetComponent(out IngredientVisuals originalVisuals))
            extractedVisuals = originalVisuals.StateMaterials;

        Material materialToUse = defaultCrossSectionMaterial;

        if (target.TryGetComponent(out SliceableProperties properties))
            materialToUse = properties.innerMaterial;

        GameObject upperHull = hull.CreateUpperHull(target, materialToUse);
        SetupSlicedComponent(upperHull, target, materialToUse, extractedData, extractedState, extractedProgress, extractedVisuals);

        GameObject lowerHull = hull.CreateLowerHull(target, materialToUse);
        SetupSlicedComponent(lowerHull, target, materialToUse, extractedData, extractedState, extractedProgress, extractedVisuals);

        Destroy(target);
    }

    private void SetupSlicedComponent(GameObject slicedObject, GameObject originalTarget, Material appliedMaterial, IngredientData data, IngredientState state, float progress, StateMaterial[] visualData)
    {
        slicedObject.transform.position = originalTarget.transform.position;
        slicedObject.transform.rotation = originalTarget.transform.rotation;
        slicedObject.transform.localScale = originalTarget.transform.lossyScale;

        slicedObject.layer = originalTarget.layer;

        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider meshCollider = slicedObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

        XRGrabInteractable grabInteractable = slicedObject.AddComponent<XRGrabInteractable>();
        grabInteractable.useDynamicAttach = true;

        if (originalTarget.TryGetComponent(out XRGrabInteractable originalGrab))
            grabInteractable.interactionLayers = originalGrab.interactionLayers;

        SliceableProperties newProperties = slicedObject.AddComponent<SliceableProperties>();
        newProperties.innerMaterial = appliedMaterial;

        if (data != null)
        {
            IngredientBase newIngredient = slicedObject.AddComponent<IngredientBase>();
            newIngredient.Initialize(data, state, progress);
            newIngredient.AddState(IngredientState.Sliced);
        }

        if (visualData != null)
        {
            IngredientVisuals newVisuals = slicedObject.AddComponent<IngredientVisuals>();
            newVisuals.Initialize(visualData);
        }

        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1f);
    }
}