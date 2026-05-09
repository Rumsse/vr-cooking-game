using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GrabHandPose : MonoBehaviour
{
    public HandData rightHandPose;

    #region Private Fields

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;
    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;

    #endregion

    #region Initialization

    private void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable is missing.");
            return;
        }

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnsetPose);

        if (rightHandPose != null)
            rightHandPose.gameObject.SetActive(false);
    }

    #endregion

    #region Pose Logic

    public void SetupPose(SelectEnterEventArgs arg)
    {
        HandLink link = arg.interactorObject.transform.GetComponentInParent<HandLink>();

        if (link == null || link.physicsHand == null)
        {
            Debug.LogWarning("HandLink or HandData missing on interactor.");
            return;
        }

        HandData handData = link.physicsHand;

        if (handData.TryGetComponent(out DynamicFingerSync sync))
            sync.enabled = false;

        SetHandDataValues(handData, rightHandPose);
        SetHandData(handData, finalHandPosition, finalHandRotation, finalFingerRotations);
    }

    public void UnsetPose(SelectExitEventArgs arg)
    {
        HandLink link = arg.interactorObject.transform.GetComponentInParent<HandLink>();

        if (link == null || link.physicsHand == null)
            return;

        HandData handData = link.physicsHand;

        if (handData.TryGetComponent(out DynamicFingerSync sync))
            sync.enabled = true;

        SetHandData(handData, startingHandPosition, startingHandRotation, startingFingerRotations);
    }

    #endregion

    #region Data Assignment

    private void SetHandDataValues(HandData h1, HandData h2)
    {
        startingHandPosition = new Vector3(
            h1.root.localPosition.x / h1.root.localScale.x,
            h1.root.localPosition.y / h1.root.localScale.y,
            h1.root.localPosition.z / h1.root.localScale.z);

        finalHandPosition = new Vector3(
            h2.root.localPosition.x / h2.root.localScale.x,
            h2.root.localPosition.y / h2.root.localScale.y,
            h2.root.localPosition.z / h2.root.localScale.z);

        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;
        startingFingerRotations = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations = new Quaternion[h1.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;
        }
    }

    private void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
            h.fingerBones[i].localRotation = newBonesRotation[i];
    }

    #endregion
}