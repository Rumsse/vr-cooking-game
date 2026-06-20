using UnityEngine;

namespace Wastepunk.Tools
{
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private ParticleSystem saltParticleSystem;
        [SerializeField] private float tiltThreshold = 0.5f;

        private bool isPouring;

        private void Update()
        {
            float dotProduct = Vector3.Dot(transform.up, Vector3.down);

            if (dotProduct > tiltThreshold)
                StartPouring();
            else
                StopPouring();
        }

        private void StartPouring()
        {
            if (isPouring)
                return;

            isPouring = true;
            saltParticleSystem.Play();
        }

        private void StopPouring()
        {
            if (!isPouring)
                return;

            isPouring = false;
            saltParticleSystem.Stop();
        }
    }
}