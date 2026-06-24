using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Wastepunk.Cooking
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class CactusNeedle : MonoBehaviour
    {
        [SerializeField] private Cactus parentCactus;
        [SerializeField] private float pullDistanceThreshold = 0.15f;

        private XRSimpleInteractable interactable;
        private IXRSelectInteractor currentInteractor;
        private Vector3 initialHandPosition;

        private void Awake() => interactable = GetComponent<XRSimpleInteractable>();

        private void OnEnable()
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }

        private void OnDisable()
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }

        private void Update()
        {
            if (currentInteractor == null)
                return;

            if (Vector3.Distance(initialHandPosition, currentInteractor.transform.position) >= pullDistanceThreshold)
                RemoveNeedle();
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            currentInteractor = args.interactorObject;
            initialHandPosition = currentInteractor.transform.position;
        }

        private void OnSelectExited(SelectExitEventArgs args) => currentInteractor = null;

        private void RemoveNeedle()
        {
            if (parentCactus == null)
            {
                Debug.LogError("Parent Cactus reference is missing!");
                return;
            }

            parentCactus.RegisterNeedleRemoved();
            Destroy(gameObject);
        }
    }
}