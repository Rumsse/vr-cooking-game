using UnityEngine;

namespace Wastepunk.Tools
{
    public class Shaker : MonoBehaviour
    {
        #region Variables

        [SerializeField] private ParticleSystem saltParticleSystem;
        [SerializeField] private float tiltThreshold = 0.2f;
        [SerializeField] private float shakeThreshold = 15f;
        [SerializeField] private float shakeCooldown = 0.15f;
        [SerializeField] private int particlesPerShake = 15;

        private Vector3 lastPosition;
        private Vector3 lastVelocity;
        private float cooldownTimer;

        #endregion

        #region Unity Methods

        private void Start() => lastPosition = transform.position;

        private void Update()
        {
            cooldownTimer -= Time.deltaTime;

            Vector3 currentVelocity = Time.deltaTime > 0f ? (transform.position - lastPosition) / Time.deltaTime : Vector3.zero;
            Vector3 currentAcceleration = Time.deltaTime > 0f ? (currentVelocity - lastVelocity) / Time.deltaTime : Vector3.zero;

            lastPosition = transform.position;
            lastVelocity = currentVelocity;

            float dotProduct = Vector3.Dot(transform.up, Vector3.down);
            if (dotProduct < tiltThreshold)
                return;

            if (cooldownTimer > 0f)
                return;

            if (currentAcceleration.magnitude < shakeThreshold)
                return;

            EmitSalt();
        }

        #endregion

        #region Logic

        private void EmitSalt()
        {
            cooldownTimer = shakeCooldown;
            saltParticleSystem.Emit(particlesPerShake);
        }

        #endregion
    }
}