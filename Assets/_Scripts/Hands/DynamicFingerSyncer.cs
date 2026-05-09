using UnityEngine;

[RequireComponent(typeof(HandData))]
public class DynamicFingerSync : MonoBehaviour
{
    public Transform xrOriginRoot;
    public string visualWristName = "L_Wrist";
    public bool isRightHand;

    [Header("Check this box in Play Mode to calibrate")]
    public bool calibrateNow;

    #region Private Fields

    private HandData handData;
    private Transform visualWrist;
    private Transform physicsWrist;
    private Transform[] visualBones;
    private Quaternion[] initVisRel;
    private Quaternion[] initPhysRel;
    private bool isInitialized;

    #endregion

    #region Initialization

    private void Awake()
    {
        handData = GetComponent<HandData>();
        physicsWrist = handData.root;
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

        if (calibrateNow)
        {
            CalibrateFlatHands();
            calibrateNow = false;
        }

        for (int i = 0; i < handData.fingerBones.Length; i++)
        {
            if (visualBones[i] == null || handData.fingerBones[i] == null)
                continue;

            Quaternion currentVisRel = Quaternion.Inverse(visualWrist.rotation) * visualBones[i].rotation;
            Quaternion relDelta = currentVisRel * Quaternion.Inverse(initVisRel[i]);
            Quaternion currentPhysRel = relDelta * initPhysRel[i];

            handData.fingerBones[i].rotation = physicsWrist.rotation * currentPhysRel;
        }
    }

    private void TryInitializeBones()
    {
        if (xrOriginRoot == null)
            return;

        visualWrist = FindChildRecursive(xrOriginRoot, visualWristName);
        if (visualWrist == null)
            return;

        if (TryGetComponent(out PhysicHand physicsHand))
            physicsHand.target = visualWrist;

        int boneCount = handData.fingerBones.Length;
        visualBones = new Transform[boneCount];
        initVisRel = new Quaternion[boneCount];
        initPhysRel = new Quaternion[boneCount];

        bool foundAny = false;
        string prefix = isRightHand ? "R_" : "L_";

        for (int i = 0; i < boneCount; i++)
        {
            Transform oculusBone = handData.fingerBones[i];
            if (oculusBone == null)
                continue;

            visualBones[i] = MapOculusToXRBone(visualWrist, oculusBone.name, prefix);

            if (visualBones[i] != null)
                foundAny = true;
        }

        if (foundAny)
        {
            CalibrateFlatHands();
            isInitialized = true;
            Debug.Log($"Bones mapped for {visualWristName}");
        }
    }

    #endregion

    #region Calibration & Mapping

    [ContextMenu("Calibrate Flat Hands")]
    public void CalibrateFlatHands()
    {
        if (visualWrist == null || visualBones == null)
            return;

        for (int i = 0; i < handData.fingerBones.Length; i++)
        {
            if (visualBones[i] == null || handData.fingerBones[i] == null)
                continue;

            initVisRel[i] = Quaternion.Inverse(visualWrist.rotation) * visualBones[i].rotation;
            initPhysRel[i] = Quaternion.Inverse(physicsWrist.rotation) * handData.fingerBones[i].rotation;
        }

        Debug.Log("Hand pose manually calibrated.");
    }

    private Transform MapOculusToXRBone(Transform wrist, string oculusName, string prefix)
    {
        string finger = "";
        if (oculusName.Contains("index")) finger = "Index";
        else if (oculusName.Contains("middle")) finger = "Middle";
        else if (oculusName.Contains("ring")) finger = "Ring";
        else if (oculusName.Contains("pinky")) finger = "Little";
        else if (oculusName.Contains("thumb")) finger = "Thumb";

        string joint = "";

        if (finger == "Thumb")
        {
            if (oculusName.Contains("1")) joint = "Metacarpal";
            else if (oculusName.Contains("2")) joint = "Proximal";
            else if (oculusName.Contains("3")) joint = "Distal";
        }
        else if (finger == "Little")
        {
            if (oculusName.Contains("0")) joint = "Metacarpal";
            else if (oculusName.Contains("1")) joint = "Proximal";
            else if (oculusName.Contains("2")) joint = "Intermediate";
            else if (oculusName.Contains("3")) joint = "Distal";
        }
        else
        {
            if (oculusName.Contains("1")) joint = "Proximal";
            else if (oculusName.Contains("2")) joint = "Intermediate";
            else if (oculusName.Contains("3")) joint = "Distal";
        }

        if (string.IsNullOrEmpty(finger) || string.IsNullOrEmpty(joint))
            return null;

        return FindChildRecursive(wrist, prefix + finger + joint);
    }

    private Transform FindChildRecursive(Transform parent, string targetName)
    {
        if (parent.name.Contains(targetName) && !parent.IsChildOf(transform))
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, targetName);
            if (result != null)
                return result;
        }

        return null;
    }

    #endregion
}