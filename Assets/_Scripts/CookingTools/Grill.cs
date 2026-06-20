using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Wastepunk.Interfaces;

namespace Wastepunk.Stations
{
    [RequireComponent(typeof(Collider))]
    public class Grill : MonoBehaviour
    {
        #region Fields & Properties

        [SerializeField] private VisualEffect fireVfx;
        [SerializeField] private string scaleProperty = "Intensity_Scale";
        [SerializeField] private float maxScale = 1f;
        [SerializeField] private string lifetimeProperty = "Intensity_Lifetime";
        [SerializeField] private float maxLifetime = 1.85f;
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float fuelConsumptionRate = 1f;

        private float currentFuel;
        private readonly List<ICookable> itemsOnGrill = new List<ICookable>();

        public float FireIntensity => Mathf.Clamp01(currentFuel / maxFuel);

        #endregion

        #region Unity Methods

        private void Awake() => currentFuel = maxFuel;

        private void Update()
        {
            UpdateFire();

            if (itemsOnGrill.Count == 0)
                return;

            if (FireIntensity <= 0f)
                return;

            for (int i = itemsOnGrill.Count - 1; i >= 0; i--)
                ProcessItem(itemsOnGrill[i]);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out ICookable cookable))
                return;

            if (!cookable.CanBeCooked())
                return;

            if (!itemsOnGrill.Contains(cookable))
                itemsOnGrill.Add(cookable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out ICookable cookable))
                return;

            if (itemsOnGrill.Contains(cookable))
                itemsOnGrill.Remove(cookable);
        }

        #endregion

        #region Logic

        private void UpdateFire()
        {
            if (currentFuel > 0f)
                currentFuel -= fuelConsumptionRate * Time.deltaTime;

            if (fireVfx == null)
                return;

            if (fireVfx.HasFloat(scaleProperty))
                fireVfx.SetFloat(scaleProperty, maxScale * FireIntensity);

            if (fireVfx.HasFloat(lifetimeProperty))
                fireVfx.SetFloat(lifetimeProperty, maxLifetime * FireIntensity);
        }

        private void ProcessItem(ICookable cookable)
        {
            cookable.CookingProgress += Time.deltaTime * FireIntensity;

            if (cookable.CookingProgress >= cookable.TimeToBurn)
                cookable.Burn();
            else if (cookable.CookingProgress >= cookable.TimeToCook)
                cookable.Cook();
        }

        public void AddFuel(float amount)
        {
            currentFuel = Mathf.Clamp(currentFuel + amount, 0f, maxFuel);
        }

        #endregion
    }
}