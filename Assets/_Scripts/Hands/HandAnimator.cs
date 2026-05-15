using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private InputActionReference gripReference;
    [SerializeField] private InputActionReference triggerReference;

    private Animator animator;
    private static readonly int GripParam = Animator.StringToHash("Grip");
    private static readonly int TriggerParam = Animator.StringToHash("Trigger");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator missing on the hand model.");
    }

    private void Update()
    {
        if (animator == null)
            return;

        AnimateHand();
    }

    private void AnimateHand()
    {
        float gripValue = gripReference.action?.ReadValue<float>() ?? 0f;
        float triggerValue = triggerReference.action?.ReadValue<float>() ?? 0f;

        animator.SetFloat(GripParam, gripValue);
        animator.SetFloat(TriggerParam, triggerValue);
    }
}