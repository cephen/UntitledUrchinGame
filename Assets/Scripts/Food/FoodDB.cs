using System.Collections.Generic;
using UnityEngine;

namespace UrchinGame.Food {
    public sealed class FoodDB : ScriptableObject {
        public ContactFilter2D ContactFilter;
        public List<FoodData> FoodData = new();
    }
}
