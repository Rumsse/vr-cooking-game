using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Ingredients
{
    public class PotBase : MonoBehaviour, IFillable
    {
        [SerializeField] private float maxVolume = 100f;
        [SerializeField] private Transform waterVisual;
        [SerializeField] private float minLocalY = -0.5f;
        [SerializeField] private float maxLocalY = 0.5f;

        public float MaxVolume => maxVolume;
        public float CurrentVolume { get; private set; }

        private void Start() => UpdateWaterVisual();

        public void Fill(float amount)
        {
            if (IsFull())
                return;

            CurrentVolume += amount;
            CurrentVolume = Mathf.Clamp(CurrentVolume, 0f, MaxVolume);
            UpdateWaterVisual();
        }

        public bool IsFull() => CurrentVolume >= MaxVolume;

        private void UpdateWaterVisual()
        {
            if (waterVisual == null)
                return;

            if (CurrentVolume <= 0f)
            {
                waterVisual.gameObject.SetActive(false);
                return;
            }

            if (!waterVisual.gameObject.activeSelf)
                waterVisual.gameObject.SetActive(true);

            float fillRatio = CurrentVolume / MaxVolume;
            Vector3 newPos = waterVisual.localPosition;
            newPos.y = Mathf.Lerp(minLocalY, maxLocalY, fillRatio);
            waterVisual.localPosition = newPos;
        }
    }
}