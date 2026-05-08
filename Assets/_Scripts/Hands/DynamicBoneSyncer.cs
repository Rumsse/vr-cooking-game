using UnityEngine;

public class DynamicBoneSyncer : MonoBehaviour
{
    [SerializeField] private string ghostHandSpawnName = "LeftHandQuestVisual(Clone)";
    [SerializeField] private Transform physicalWrist;

    private Transform[] ghostBones;
    private Transform[] physicalBones;
    private bool isInitialized;

    private void Awake() => isInitialized = false;

    private void LateUpdate()
    {
        if (!isInitialized)
        {
            InitializeBones();
            return;
        }

        SyncRotations();
    }

    private void InitializeBones()
    {
        GameObject ghostObj = GameObject.Find(ghostHandSpawnName);
        if (ghostObj == null)
            return;

        Transform ghostWrist = ghostObj.transform.GetChild(0);
        ghostBones = ghostWrist.GetComponentsInChildren<Transform>();

        if (physicalWrist == null)
        {
            Debug.LogError("Physical wrist not assigned");
            return;
        }

        physicalBones = physicalWrist.GetComponentsInChildren<Transform>();
        isInitialized = true;
    }

    private void SyncRotations()
    {
        int limit = Mathf.Min(ghostBones.Length, physicalBones.Length);
        
        for (int i = 0; i < limit; i++)
            physicalBones[i].localRotation = ghostBones[i].localRotation;
    }
}