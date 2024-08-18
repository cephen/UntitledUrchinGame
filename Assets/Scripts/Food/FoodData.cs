using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UrchinGame.Food {
    [Serializable]
    public sealed class FoodData {
        public string Name;
        public Sprite Sprite;
        public AssetReferenceT<Sprite> UIShopSprite;
        public int Cost = 5;
        public float FallSpeed = 1f;
        public float SwayAmount = 1f;
        public float SwaySpeed = 1f;
        public float Weight = 1f;
    }
}
