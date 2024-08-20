using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UrchinGame.Food;
using Random = System.Random;

namespace UrchinGame.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        public Bank bank { get; private set; } = ScriptableObject.CreateInstance<Bank>();
        public bool paused { get; private set; }
        public Volume globalVolume { get; private set; }
        public System.Random random { get; private set; }

        [SerializeField] private string persistentManagersSceneName = "PersistentManagers";
        [SerializeField] private AssetReference nurseryScene;

        private void Awake() {
            instance = this;
            random = new Random();
            globalVolume = GetComponent<Volume>();
        }
        void Start()
        {
            #if UNITY_EDITOR
            if (unloadAllNonPMScenesOnStart) { UnloadAllNonPMScenesOnStart(persistentManagersSceneName); }
            #endif
        }

        void Update()
        {

        }

        public void Play() {
            Addressables.LoadScene(nurseryScene, LoadSceneMode.Additive);
        }

        #region Unload all non persistent manager scenes on start if in editor
        #if UNITY_EDITOR
                [SerializeField] bool unloadAllNonPMScenesOnStart = true;
                List<UnityEngine.SceneManagement.Scene> scenes = new List<UnityEngine.SceneManagement.Scene> ();
                void UnloadAllNonPMScenesOnStart(string _persistentManagersSceneName)
                {
                    for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }
                    string output = string.Empty;
                    foreach (UnityEngine.SceneManagement.Scene scene in scenes)
                    {
                        if (scene.name != _persistentManagersSceneName)
                        {
                            output += scene.name + ", ";
                            #pragma warning disable CS0618 // Type or member is obsolete
                            SceneManager.UnloadScene(scene);
                            #pragma warning restore CS0618 // Type or member is obsolete
                        }
                    }
                    if (output != string.Empty) { Log.Warning("The scenes " + output + "were found on startup and were unloaded."); }
                }
        #endif
        #endregion
    }
}
