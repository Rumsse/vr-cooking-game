using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Wastepunk.Cooking
{
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
    public class CactusNeedle : MonoBehaviour
    {
        [SerializeField] private Cactus parentCactus;
        [SerializeField] private float pullDistanceThreshold = 0.15f;

        private bool isRemoved;
        private Rigidbody rb;
        private XRGrabInteractable grabInteractable;
        private Vector3 localSocketPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grabInteractable = GetComponent<XRGrabInteractable>();
            
            rb.isKinematic = true;

            if (parentCactus != null)
                localSocketPosition = parentCactus.transform.InverseTransformPoint(transform.position);
        }

        private void Update()
        {
            if (isRemoved)
                return;

            if (!grabInteractable.isSelected)
                return;

            if (parentCactus == null)
            {
                Debug.LogWarning("CactusNeedle is missing parentCactus reference!");
                return;
            }

            if ((transform.position - parentCactus.transform.TransformPoint(localSocketPosition)).sqrMagnitude > pullDistanceThreshold * pullDistanceThreshold)
                DetachNeedle();
        }

        private void DetachNeedle()
        {
            isRemoved = true;
            rb.isKinematic = false;
            transform.SetParent(null);

            //TriggerHapticFeedback();

            if (parentCactus != null)
                parentCactus.RegisterNeedleRemoved();
        }

        private void TriggerHapticFeedback()
        {
            foreach (var interactor in grabInteractable.interactorsSelecting)
            {
                // if (interactor is XRBaseInteractor baseInteractor)
                //     baseInteractor.SendHapticImpulse(1f, 0.2f);
            }
        }
    }
}