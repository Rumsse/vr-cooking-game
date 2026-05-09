using UnityEngine;
using System.Reflection;

namespace Wastepunk.Cooking
{
    [RequireComponent(typeof(Rigidbody))]
    public class CactusNeedle : MonoBehaviour
    {
        [SerializeField] private Cactus parentCactus;
        [SerializeField] private float pullDistanceThreshold = 0.15f;

        private bool isRemoved;
        private FixedJoint joint;
        private Component grabInteractable;
        private PropertyInfo isSelectedProp;
        private Vector3 localSocketPosition;

        private void Awake()
        {
            joint = GetComponent<FixedJoint>();
            
            if (parentCactus != null)
                localSocketPosition = parentCactus.transform.InverseTransformPoint(transform.position);

            var xriGrabType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable, Unity.XR.Interaction.Toolkit") 
                            ?? System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable, Unity.XR.Interaction.Toolkit");
                            
            if (xriGrabType == null)
                return;

            grabInteractable = GetComponent(xriGrabType);
            
            if (grabInteractable != null)
                isSelectedProp = xriGrabType.GetProperty("isSelected");
        }

        private void Update()
        {
            if (isRemoved)
                return;

            if (grabInteractable == null || isSelectedProp == null || parentCactus == null)
                return;

            if (!(bool)isSelectedProp.GetValue(grabInteractable))
                return;

            if ((transform.position - parentCactus.transform.TransformPoint(localSocketPosition)).sqrMagnitude > pullDistanceThreshold * pullDistanceThreshold)
                DetachNeedle();
        }

        private void DetachNeedle()
        {
            isRemoved = true;
            transform.SetParent(null);
            
            if (joint != null)
                Destroy(joint);

            TriggerHapticFeedback();
            
            if (parentCactus != null)
                parentCactus.RegisterNeedleRemoved();
        }

        private void TriggerHapticFeedback()
        {
            if (grabInteractable == null)
                return;
            
            try
            {
                var interactorsProp = grabInteractable.GetType().GetProperty("interactorsSelecting");
                if (interactorsProp == null)
                    return;

                if (interactorsProp.GetValue(grabInteractable) is not System.Collections.IEnumerable interactors)
                    return;

                foreach (var interactor in interactors)
                {
                    var sendHapticMethod = interactor.GetType().GetMethod("SendHapticImpulse", new[] { typeof(float), typeof(float) });
                    if (sendHapticMethod != null)
                        sendHapticMethod.Invoke(interactor, new object[] { 1f, 0.2f });
                }
            }
            catch
            {
            }
        }
    }
}
