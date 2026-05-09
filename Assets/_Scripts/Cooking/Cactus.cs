using System;
using UnityEngine;

namespace Wastepunk.Cooking
{
    public class Cactus : MonoBehaviour
    {
        [SerializeField] private int totalNeedles = 5;
        private int removedNeedles;

        public event Action OnFullyPeeled;

        public void RegisterNeedleRemoved()
        {
            removedNeedles++;
            
            if (removedNeedles >= totalNeedles)
                OnFullyPeeled?.Invoke();
        }

        public bool IsFullyPeeled() => removedNeedles >= totalNeedles;
    }
}
