using UnityEngine;

public enum HandModelType
{
    Left,
    Right
}

public class HandData : MonoBehaviour
{
    public HandModelType handType;
    public Transform root;
    public Transform[] fingerBones;
}