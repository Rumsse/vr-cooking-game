using UnityEngine;

namespace Wastepunk.Environment
{
    public class Faucet : MonoBehaviour
    {
        [SerializeField] private ParticleSystem waterParticles;
        private bool isWaterFlowing;

        public void ToggleWater()
        {
            isWaterFlowing = !isWaterFlowing;
            if (isWaterFlowing)
            {
                waterParticles.Play();
            }
            else
            {
                waterParticles.Stop();
            }
        }
    }
}