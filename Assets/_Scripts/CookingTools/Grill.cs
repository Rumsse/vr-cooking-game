using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Wastepunk.Ingredients;
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
        private readonly List<IHeatable> containersOnGrill = new List<IHeatable>();

        public float FireIntensity => Mathf.Clamp01(currentFuel / maxFuel);

        #endregion

        #region Unity Methods

        private void Awake() => currentFuel = maxFuel;

        private void Update()
        {
            UpdateFire();

            if (FireIntensity <= 0f)
                return;

            for (int i = itemsOnGrill.Count - 1; i >= 0; i--)
                ProcessItem(itemsOnGrill[i]);

            for (int i = containersOnGrill.Count - 1; i >= 0; i--)
                containersOnGrill[i].Heat(FireIntensity);
        }

        private void OnTriggerEnter(Collider other)
        {
            IFuel fuel = other.GetComponentInParent<IFuel>();
            if (fuel != null)
            {
                TryConsumeFuel(fuel);
                return;
            }

            ICookable cookable = other.GetComponentInParent<ICookable>();
            if (cookable != null)
                TryAddItem(cookable);

            IHeatable heatable = other.GetComponentInParent<IHeatable>();
            if (heatable != null)
                TryAddContainer(heatable);
        }

        private void OnTriggerExit(Collider other)
        {
            ICookable cookable = other.GetComponentInParent<ICookable>();
            if (cookable != null)
                TryRemoveItem(cookable);

            IHeatable heatable = other.GetComponentInParent<IHeatable>();
            if (heatable != null)
                TryRemoveContainer(heatable);
        }

        #endregion

        #region Logic

        private void TryConsumeFuel(IFuel fuel)
        {
            AddFuel(fuel.FuelAmount);
            fuel.Consume();
            Debug.Log("Fuel consumed and added to grill.");
        }

        private void TryAddItem(ICookable cookable)
        {
            if (!cookable.CanBeCooked(CookingType.Frying))
                return;

            if (!itemsOnGrill.Contains(cookable))
                itemsOnGrill.Add(cookable);
        }

        private void TryAddContainer(IHeatable heatable)
        {
            if (!containersOnGrill.Contains(heatable))
                containersOnGrill.Add(heatable);
        }

        private void TryRemoveItem(ICookable cookable)
        {
            if (itemsOnGrill.Contains(cookable))
                itemsOnGrill.Remove(cookable);
        }

        private void TryRemoveContainer(IHeatable heatable)
        {
            if (containersOnGrill.Contains(heatable))
                containersOnGrill.Remove(heatable);
        }

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
                cookable.Cook(CookingType.Frying);
        }

        public void AddFuel(float amount) => currentFuel = Mathf.Clamp(currentFuel + amount, 0f, maxFuel);

        #endregion
    }
}