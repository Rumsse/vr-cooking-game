using System.Collections.Generic;
using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Ingredients
{
    public class PotBase : MonoBehaviour, IFillable, IHeatable
    {
        #region Fields & Properties

        [SerializeField] private float maxVolume = 100f;
        [SerializeField] private Transform waterVisual;
        [SerializeField] private float minLocalY = -0.5f;
        [SerializeField] private float maxLocalY = 0.5f;
        [SerializeField] private float waterRequiredForCooking = 15f;

        private readonly List<ICookable> itemsInPot = new List<ICookable>();

        public float MaxVolume => maxVolume;
        public float CurrentVolume { get; private set; }
        public bool HasEnoughWater => CurrentVolume >= waterRequiredForCooking;

        #endregion

        #region Unity Methods

        private void Start() => UpdateWaterVisual();

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out ICookable cookable))
                return;

            if (!cookable.CanBeCooked())
                return;

            if (!itemsInPot.Contains(cookable))
                itemsInPot.Add(cookable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out ICookable cookable))
                return;

            if (itemsInPot.Contains(cookable))
                itemsInPot.Remove(cookable);
        }

        #endregion

        #region Logic

        public void Fill(float amount)
        {
            if (IsFull())
                return;

            CurrentVolume += amount;
            CurrentVolume = Mathf.Clamp(CurrentVolume, 0f, maxVolume);
            UpdateWaterVisual();
        }

        public bool IsFull() => CurrentVolume >= maxVolume;

        public void Heat(float intensity)
        {
            if (!HasEnoughWater)
                return;

            for (int i = itemsInPot.Count - 1; i >= 0; i--)
                ProcessItem(itemsInPot[i], intensity);
        }

        private void ProcessItem(ICookable cookable, float intensity)
        {
            cookable.CookingProgress += Time.deltaTime * intensity;

            if (cookable.CookingProgress >= cookable.TimeToBurn)
                cookable.Burn();
            else if (cookable.CookingProgress >= cookable.TimeToCook)
                cookable.Cook();
        }

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

            float fillRatio = CurrentVolume / maxVolume;
            Vector3 newPos = waterVisual.localPosition;
            newPos.y = Mathf.Lerp(minLocalY, maxLocalY, fillRatio);
            waterVisual.localPosition = newPos;
        }

        #endregion
    }
}