using UnityEngine;

namespace Wastepunk.Environment
{
    public class Faucet : MonoBehaviour
    {
        #region Variables

        [SerializeField] private ParticleSystem waterParticles;
        [SerializeField] private float maxEmissionRate = 50f;

        private ParticleSystem.EmissionModule waterEmission;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (waterParticles == null)
                return;

            waterEmission = waterParticles.emission;
            waterEmission.enabled = true;
            waterEmission.rateOverTime = 0f;
            waterParticles.Stop();
        }

        #endregion

        #region Logic

        public void SetWaterFlow(float percent)
        {
            if (waterParticles == null)
                return;

            percent = Mathf.Clamp01(percent);

            if (percent <= 0.05f)
            {
                if (waterParticles.isPlaying)
                    waterParticles.Stop();

                waterEmission.rateOverTime = 0f;
            }
            else
            {
                if (!waterParticles.isPlaying)
                    waterParticles.Play();

                waterEmission.rateOverTime = percent * maxEmissionRate;
            }
        }

        #endregion
    }
}