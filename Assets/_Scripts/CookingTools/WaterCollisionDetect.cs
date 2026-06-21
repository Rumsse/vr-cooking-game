using UnityEngine;
using Wastepunk.Interfaces;

namespace Wastepunk.Environment
{
    public class WaterCollisionDetect : MonoBehaviour
    {
        [SerializeField] private float fillAmountPerParticle = 0.5f;

        private void OnParticleCollision(GameObject other)
        {
            IFillable fillable = other.GetComponentInParent<IFillable>();

            if (fillable == null)
                return;

            fillable.Fill(fillAmountPerParticle);
        }
    }
}