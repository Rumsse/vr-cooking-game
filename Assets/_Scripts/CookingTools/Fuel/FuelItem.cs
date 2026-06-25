using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Items
{
    public class FuelItem : MonoBehaviour, IFuel
    {
        [SerializeField] private float fuelAmount = 25f;

        public float FuelAmount => fuelAmount;

        public void Consume()
        {
            Debug.Log("Fuel item consumed by fire.");
            Destroy(gameObject);
        }
    }
}