using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Wastepunk.Environment
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class FaucetHandle : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Faucet faucetMain;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;
        [SerializeField] private float closedAngle = 0f;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float smoothness = 15f;

        private XRGrabInteractable grabInteractable;
        private Quaternion targetRotation;
        private IXRInteractor currentInteractor;
        private Transform originalPivot;

        private float currentLeverAngle;
        private float grabAngleOffset;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            rotationAxis.Normalize();
            targetRotation = transform.rotation;
            currentLeverAngle = closedAngle;

            grabInteractable.trackPosition = false;
            grabInteractable.trackRotation = false;

            originalPivot = new GameObject($"Pivot_{name}").transform;

            if (transform.parent != null)
                originalPivot.SetParent(transform.parent, false);

            originalPivot.position = transform.position;
            originalPivot.rotation = transform.rotation;
        }

        private void OnEnable()
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }

        private void OnDisable()
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }

        private void LateUpdate()
        {
            transform.position = originalPivot.position;
            CalculateTargetRotation();
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothness * Time.deltaTime);
            UpdateFaucetFlow();
        }

        #endregion

        #region Logic

        private void OnGrab(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject;

            Vector3 interactorLocalPos = originalPivot.InverseTransformPoint(currentInteractor.transform.position);
            Vector3 projectedPos = Vector3.ProjectOnPlane(interactorLocalPos, rotationAxis).normalized;
            float handAngle = Vector3.SignedAngle(Vector3.up, projectedPos, rotationAxis);

            grabAngleOffset = handAngle - currentLeverAngle;
        }

        private void OnRelease(SelectExitEventArgs args) => currentInteractor = null;

        private void CalculateTargetRotation()
        {
            if (currentInteractor == null)
                return;

            Vector3 interactorLocalPos = originalPivot.InverseTransformPoint(currentInteractor.transform.position);
            Vector3 projectedPos = Vector3.ProjectOnPlane(interactorLocalPos, rotationAxis).normalized;

            if (projectedPos == Vector3.zero)
                return;

            float handAngle = Vector3.SignedAngle(Vector3.up, projectedPos, rotationAxis);
            float rawTargetAngle = handAngle - grabAngleOffset;

            float minBound = Mathf.Min(closedAngle, openAngle);
            float maxBound = Mathf.Max(closedAngle, openAngle);
            currentLeverAngle = Mathf.Clamp(rawTargetAngle, minBound, maxBound);

            targetRotation = originalPivot.rotation * Quaternion.AngleAxis(currentLeverAngle, rotationAxis);
        }

        private void UpdateFaucetFlow()
        {
            if (faucetMain == null)
                return;

            float percentOpen = Mathf.InverseLerp(closedAngle, openAngle, currentLeverAngle);
            faucetMain.SetWaterFlow(percentOpen);
        }

        #endregion
    }
}