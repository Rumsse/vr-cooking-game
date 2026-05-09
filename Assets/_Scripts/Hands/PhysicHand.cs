using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicHand : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 50f;
    public float rotationSpeed = 50f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100f;
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        UpdateLinearVelocity();
        UpdateAngularVelocity();
    }

    private void UpdateLinearVelocity()
    {
        Vector3 positionDifference = target.position - transform.position;
        rb.linearVelocity = positionDifference * followSpeed;
    }

    private void UpdateAngularVelocity()
    {
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        if (float.IsInfinity(rotationAxis.x))
            return;

        if (angleInDegree > 180f)
            angleInDegree -= 360f;

        Vector3 angularDifference = angleInDegree * rotationAxis;
        rb.angularVelocity = (angularDifference * Mathf.Deg2Rad) * rotationSpeed;
    }
}