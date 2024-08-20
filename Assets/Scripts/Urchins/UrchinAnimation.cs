using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UrchinGame.Urchins {
    [CreateAssetMenu(fileName = "Urchin Animation", menuName = "Urchins/Animation")]
    public class UrchinAnimation : ScriptableObject {
        [field: SerializeField] public AssetReferenceSprite IdleSprite { get; private set; }
        [field: SerializeField] public AssetReferenceSprite[] AnimationSprites { get; private set; }
        [field: SerializeField] public uint AnimationFPS { get; private set; } = 4;
    }
}