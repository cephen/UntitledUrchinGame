using System;
using UnityEngine;

namespace UrchinGame.Urchins {
    [Serializable]
    public sealed class UrchinData {
        [field: SerializeField] public StatBlock Stats { get; private set; } = StatBlock.Default();
        [field: SerializeField] public UrchinAnimation Animation { get; private set; }


        public void AddWeight(float amount) {
            Stats.Weight += amount;
            
        }

        public void CompleteTraining()  {
            Stats.Weight -= 1f;
            Stats.MaxStamina += 1;
        }
    }
}