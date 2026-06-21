using EzySlice;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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

        Material materialToUse = defaultCrossSectionMaterial;

        if (target.TryGetComponent(out SliceableProperties properties))
            materialToUse = properties.innerMaterial;

        GameObject upperHull = hull.CreateUpperHull(target, materialToUse);
        SetupSlicedComponent(upperHull, target, materialToUse);

        GameObject lowerHull = hull.CreateLowerHull(target, materialToUse);
        SetupSlicedComponent(lowerHull, target, materialToUse);

        Destroy(target);
    }

    private void SetupSlicedComponent(GameObject slicedObject, GameObject originalTarget, Material appliedMaterial)
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

        SliceableProperties newProperties = slicedObject.AddComponent<SliceableProperties>();
        newProperties.innerMaterial = appliedMaterial;

        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1f);
    }
}