using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Logging;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace UrchinGame.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        public bool paused { get; private set; }
        public Volume globalVolume { get; private set; }
        public System.Random random { get; private set; }
        private void Awake() {
            instance = this;
            random = new Random();
            globalVolume = GetComponent<Volume>();
            #if UNITY_EDITOR
            if (unloadAllNonPMScenesOnStart) { UnloadAllNonPMScenesOnStart(); }
            #endif
        }
        void Start()
        {

        }

        void Update()
        {

        }

        public void Play() {
            SceneManager.LoadScene("Nursery");
        }

        #region Unload all non persistent manager scenes on start if in editor
        #if UNITY_EDITOR
                [SerializeField] bool unloadAllNonPMScenesOnStart = true;
                List<UnityEngine.SceneManagement.Scene> scenes = new List<UnityEngine.SceneManagement.Scene> ();
                void UnloadAllNonPMScenesOnStart()
                {
                    for (int i = 0; i < SceneManager.sceneCount; i++) { scenes.Add(SceneManager.GetSceneAt(i)); }
                    string output = string.Empty;
                    foreach (UnityEngine.SceneManagement.Scene scene in scenes)
                    {
                        if (scene.name is not "PersistentManagers")
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
