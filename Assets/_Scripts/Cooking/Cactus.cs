using System;
using UnityEngine;

namespace Wastepunk.Cooking
{
    public class Cactus : MonoBehaviour
    {
        [SerializeField] private string sliceableLayerName = "Sliceable";
        [SerializeField] private int totalNeedles = 5;
        
        private int removedNeedles;

        public event Action OnFullyPeeled;

        public void RegisterNeedleRemoved()
        {
            removedNeedles++;

            if (removedNeedles < totalNeedles)
                return;

            gameObject.layer = LayerMask.NameToLayer(sliceableLayerName);
            OnFullyPeeled?.Invoke();
        }

        public bool IsFullyPeeled() => removedNeedles >= totalNeedles;
    }
}