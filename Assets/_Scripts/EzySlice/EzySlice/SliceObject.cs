using EzySlice;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SliceObject : MonoBehaviour
{
    [Header("Slice Points")]
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;

    [Header("Settings")]
    [SerializeField] private LayerMask sliceableLayer;
    [SerializeField] private Material crossSectionMaterial;
    [SerializeField] private float cutForce = 2000f;
    
    [Header("VR Velocity Settings")]
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
        Vector3 displacement = transform.position - previousPosition;

        estimatedVelocity = displacement / Time.fixedDeltaTime;

        previousPosition = transform.position;
    }

    public void Slice(GameObject target)
    {
        Vector3 velocity = estimatedVelocity;

        if (velocity.magnitude < minVelocityToCut)
            return;

        Vector3 bladeDirection = endSlicePoint.position - startSlicePoint.position;
        Vector3 planeNormal = Vector3.Cross(bladeDirection, velocity).normalized;
        
        if (planeNormal == Vector3.zero)
        {
            Debug.LogWarning("[Slice Debug] Normalna płaszczyzny to zero. Ruch jest równoległy do ostrza.");
            return;
        }

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull == null)
        {
            Debug.LogError($"[Slice Debug] Slice failed on {target.name}. Upewnij się, że obiekt ma MeshFilter i zaznaczone 'Read/Write Enabled' w ustawieniach modelu!");
            return;
        }

        GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
        SetupSlicedComponent(upperHull);

        GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
        SetupSlicedComponent(lowerHull);

        Destroy(target);
    }

    private void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider meshCollider = slicedObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1f);
    }
}