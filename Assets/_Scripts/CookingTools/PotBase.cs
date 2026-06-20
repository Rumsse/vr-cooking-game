using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Ingredients
{
    public class PotBase : MonoBehaviour, IFillable
    {
        [SerializeField] private float maxVolume = 100f;
        [SerializeField] private GameObject waterVisual;
        [SerializeField] private float waterRiseSpeed = 0.5f;

        public float MaxVolume => maxVolume;
        public float CurrentVolume { get; private set; }

        private Vector3 initialWaterScale;

        private void Start()
        {
            if (waterVisual != null)
            {
                initialWaterScale = waterVisual.transform.localScale;
                UpdateWaterVisual();
            }
        }

        public void Fill(float amount)
        {
            if (IsFull()) return;

            CurrentVolume += amount;
            CurrentVolume = Mathf.Clamp(CurrentVolume, 0f, MaxVolume); 
            UpdateWaterVisual();
        }

        public bool IsFull() => CurrentVolume >= MaxVolume;

        private void UpdateWaterVisual()
        {
            if (waterVisual == null) return;

            if (CurrentVolume <= 0f)
            {
                waterVisual.SetActive(false);
            }
            else
            {
                waterVisual.SetActive(true);

                float fillRatio = CurrentVolume / MaxVolume;
                Vector3 newScale = initialWaterScale;
                newScale.y = initialWaterScale.y * fillRatio;
                waterVisual.transform.localScale = newScale;
            }
        }
    }
}