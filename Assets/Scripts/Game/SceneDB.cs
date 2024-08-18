using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UrchinGame.Game {
    // [CreateAssetMenu(fileName = "SceneDB", menuName = "Urchin/SceneDB")]
    internal sealed class SceneDB : ScriptableObject {
        // Gameplay Scenes
        [field: SerializeField] public AssetReference MainMenu { get; private set; }
        [field: SerializeField] public AssetReference Nursery { get; private set; }
        [field: SerializeField] public AssetReference Racetrack { get; private set; }
        // Manager Scenes
        [field: SerializeField] public AssetReference PersistentManagers { get; private set; }
        [field: SerializeField] public AssetReference GameplayManagers { get; private set; }
    }
}
