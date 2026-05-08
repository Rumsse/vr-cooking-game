using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsHand : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float positionSpeed = 20f;
    [SerializeField] private float rotationSpeed = 100f;

    private Rigidbody rb;

    private void Awake() => rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (target == null)
            return;

        MoveToTarget();
        RotateToTarget();
    }

    private void MoveToTarget()
    {
        Vector3 positionDifference = target.position - transform.position;
        rb.linearVelocity = positionDifference * positionSpeed;
    }

    private void RotateToTarget()
    {
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        if (angleInDegree > 180f)
            angleInDegree -= 360f;

        if (Mathf.Abs(angleInDegree) < Mathf.Epsilon)
            return;

        Vector3 angularVelocity = (angleInDegree * rotationAxis.normalized * rotationSpeed * Mathf.Deg2Rad) / Time.fixedDeltaTime;

        if (float.IsNaN(angularVelocity.x))
            return;

        rb.angularVelocity = angularVelocity;
    }
}