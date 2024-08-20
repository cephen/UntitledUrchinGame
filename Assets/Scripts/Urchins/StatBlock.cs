using System;

namespace UrchinGame.Urchins {
    [Serializable]
    public class StatBlock {
        public float Weight;
        public float MaxStamina;
        public float MaxSpeed;
        public float Size;

        public static StatBlock Default() => new StatBlock {
            Weight = 1f,
            MaxStamina = 1f,
            MaxSpeed = 1f,
            Size = 1f,
        };
    }
}
