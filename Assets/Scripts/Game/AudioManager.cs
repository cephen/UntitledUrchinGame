using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UrchinGame.Game;

namespace UrchinGame {

    [System.Serializable]
    public class ListWrapper
    {
        public List<AudioClip> myList;
    }


    public class AudioManager : MonoBehaviour {
        public static AudioManager instance { get; private set; }
        [SerializeField] private AssetLabelReference addressableAudioFolder;
        private HashSet<AudioClip> loadedAudio = new HashSet<AudioClip>();
        private Dictionary<string, List<AudioClip>> sounds = new();
        public List<ListWrapper> foo= new List<ListWrapper>();
        public enum Sounds {
            none,
            sound_urchinMunch,
            sound_urchinBodySlap,
            sound_urchinSpikeOut,
            sound_urchinSpikeIn,
            sound_waterPlop,
            sound_uiSelect,
            loop_treadmill,
            music_urchinSafeGround,
            music_mirrorball
        }

        private void Awake() {
            instance = this;
        }

        void Start() {
            Addressables.LoadAssetsAsync<AudioClip>(addressableAudioFolder, LoadAudioClipComplete).Completed += LoadAudioComplete;
        }

        void LoadAudioClipComplete(AudioClip sound) {
            int underscoreCount = sound.name.CountOf('_');
            bool isVariant = underscoreCount > 1;

            if (underscoreCount is < 1 or > 2) {
                Log.Error(sound.name + " is not labelled correctly, (example: sound_urchinMunch_1)");
                return;
            }

            string soundBaseName = sound.name[..sound.name.LastIndexOf('_')];
            if (isVariant)
            {
                string soundVariant = sound.name[(sound.name.LastIndexOf('_') + 1)..];
                if (sounds.TryGetValue(soundBaseName, out List<AudioClip> soundEntry)) { Log.Debug(soundBaseName + " new variant " + soundVariant); soundEntry.Add(sound); }
                else { Log.Debug("new sound base " + soundBaseName); sounds.Add(soundBaseName, new List<AudioClip>() { sound }); }
            }
            else {
                sounds.Add(soundBaseName, new List<AudioClip>{ sound });
            }
        }

        void Update() { }

        void LoadAudioComplete(AsyncOperationHandle<IList<AudioClip>> asyncOperationHandle) {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded) {
                Log.Debug("Audio Directory Load Succeeded");
                return;
            }
            Log.Error("Audio Directory Load Failed: " + asyncOperationHandle.OperationException);
        }

        public void PlaySound(Sounds sound, bool randomise = true) {

        }

        public void PlayLoop(Sounds sounds) {
        }

        public void PlayMusic(Sounds music) {

        }

        private void PlaySound(string soundBaseName, bool randomise = true, bool loop = false) {
            if (sounds.TryGetValue(soundBaseName, out List<AudioClip> soundVariants)) {
                soundVariants[GameManager.instance.random.Next(soundVariants.Count)];
            }
        }
    }
}
