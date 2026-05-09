using UnityEngine;

public class BoneSyncer : MonoBehaviour
{
    [SerializeField] private Transform sourceRoot;
    [SerializeField] private Transform targetRoot;

    private Transform[] sourceBones;
    private Transform[] targetBones;

    private void Awake() => CacheBones();

    private void Update()
    {
        if (sourceBones == null || targetBones == null)
            return;

        if (sourceBones.Length != targetBones.Length)
        {
            Debug.LogError("Bone hierarchy mismatch.");
            return;
        }

        for (int i = 0; i < sourceBones.Length; i++)
            targetBones[i].localRotation = sourceBones[i].localRotation;
    }

    private void CacheBones()
    {
        sourceBones = sourceRoot.GetComponentsInChildren<Transform>();
        targetBones = targetRoot.GetComponentsInChildren<Transform>();
    }
}