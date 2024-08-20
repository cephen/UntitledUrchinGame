using UnityEngine;

namespace UrchinGame.Game {
    public class Bank : ScriptableObject {
        public int Held { get; private set; } = 10;

        public bool Charge(int amount) {
            if (amount > Held) return false;

            Held -= amount;
            return true;
        }
    }
}