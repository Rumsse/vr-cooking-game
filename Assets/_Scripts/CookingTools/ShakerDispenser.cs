using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Tools
{
    public class ShakerDispenser : MonoBehaviour
    {
        private void OnParticleCollision(GameObject other)
        {
            if (!other.TryGetComponent(out ISeasonable seasonable))
                return;

            if (!seasonable.CanBeSeasoned())
                return;

            seasonable.Season();
            Debug.Log("Ingredient seasoned");
        }
    }
}