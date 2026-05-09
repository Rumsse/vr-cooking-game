using UnityEngine;

[RequireComponent(typeof(HandData), typeof(PhysicHand))]
public class DynamicFingerSync : MonoBehaviour
{
    public Transform xrOriginRoot;
    public string visualWristName;

    #region Private Fields

    private HandData handData;
    private PhysicHand physicsHand;
    private Transform[] visualBones;
    private bool isInitialized;

    #endregion

    #region Initialization

    private void Awake()
    {
        handData = GetComponent<HandData>();
        physicsHand = GetComponent<PhysicHand>();
    }

    #endregion

    #region Sync Logic

    private void Update()
    {
        if (!isInitialized)
        {
            TryInitializeBones();
            return;
        }

        for (int i = 0; i < handData.fingerBones.Length; i++)
        {
            if (visualBones[i] != null)
                handData.fingerBones[i].localRotation = visualBones[i].localRotation;
        }
    }

    private void TryInitializeBones()
    {
        if (xrOriginRoot == null)
            return;

        Transform visualWrist = FindBoneByName(xrOriginRoot, visualWristName);

        if (visualWrist == null)
            return;

        physicsHand.target = visualWrist;
        visualBones = new Transform[handData.fingerBones.Length];
        bool foundAll = true;

        for (int i = 0; i < handData.fingerBones.Length; i++)
        {
            visualBones[i] = FindBoneByName(visualWrist, handData.fingerBones[i].name);

            if (visualBones[i] == null)
                foundAll = false;
        }

        if (foundAll)
        {
            isInitialized = true;
            Debug.Log("Visual hand matched and target updated safely.");
        }
    }

    private Transform FindBoneByName(Transform parent, string boneName)
    {
        if (parent.name == boneName && !parent.IsChildOf(transform))
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindBoneByName(child, boneName);

            if (result != null)
                return result;
        }

        return null;
    }

    #endregion
}