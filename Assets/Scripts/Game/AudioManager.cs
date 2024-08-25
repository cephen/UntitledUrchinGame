using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UrchinGame.Game;
using UrchinGame.UI;

namespace UrchinGame.Game {
    public class AudioManager : MonoBehaviour {
        public static AudioManager instance { get; private set; }
        private AudioSource audioSource;
        [SerializeField] private AssetLabelReference addressableAudioFolder;
        private Dictionary<string, List<AudioClip>> sounds = new();
        public List<AudioSource> loopsPlaying = new();
        public List<AudioSource> musicIntrosPlaying = new();
        public bool debugAudioLogs;
        #if UNITY_EDITOR
        public Sounds testSound;
        public List<string> keys = new();
        #endif
        public enum Sounds {
            none,
            sound_urchinMunch,
            sound_urchinBodySlap,
            sound_urchinSpikesOut,
            sound_urchinSpikesIn,
            sound_waterPlop,
            sound_uiSelect,
            loop_treadmill,
            music_urchinSafegrounds,
            music_mirrorball
        }

        private void Awake() {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        void Start() {
            Addressables.LoadAssetsAsync<AudioClip>(addressableAudioFolder, LoadAudioClipComplete).Completed += LoadAudioComplete;
        }

        void Update() {
            #if UNITY_EDITOR
            if (Keyboard.current[Key.F].wasPressedThisFrame) { PlaySound(testSound); }
            if (Keyboard.current[Key.G].wasPressedThisFrame) { StopLoopsBySound(testSound); }
            if (Keyboard.current[Key.H].wasPressedThisFrame) { StopAllLoops(); }
            #endif
        }

        void LoadAudioClipComplete(AudioClip sound) {
            int underscoreCount = sound.name.CountOf('_');
            bool isVariant = underscoreCount > 1;

            if (underscoreCount is < 1 or > 2) {
                Log.Error(sound.name + " is not labelled correctly, (example: sound_urchinMunch_1)");
                return;
            }

            if (isVariant) {
                string soundBaseName = sound.name[..sound.name.LastIndexOf('_')];
                string soundVariant = sound.name[(sound.name.LastIndexOf('_') + 1)..];
                if (sounds.TryGetValue(soundBaseName, out List<AudioClip> soundEntry)) {
                    Log.Debug(soundBaseName + " new variant: " + soundVariant);
                    soundEntry.Add(sound);
                }
                else {
                    Log.Debug("new sound base: " + soundBaseName);
                    sounds.Add(soundBaseName, new List<AudioClip>() { sound });
                }
            }
            else {
                Log.Debug("non-variant sound added: " + sound.name);
                sounds.Add(sound.name, new List<AudioClip>{ sound });
            }
        }

        void LoadAudioComplete(AsyncOperationHandle<IList<AudioClip>> asyncOperationHandle) {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded) {
                #if UNITY_EDITOR
                foreach (var entry in sounds) { keys.Add(entry.Key + ", " +  entry.Value.Count); }
                #endif
                Log.Debug("Audio Directory Load Succeeded");
                return;
            }
            Log.Error("Audio Directory Load Failed: " + asyncOperationHandle.OperationException);
        }

        public void PlaySound(Sounds sound, bool randomise = true, int variant = 0) {
            if (sound == Sounds.none) {
                Log.Warning("[AudioManager] Tried to play 'none' sound");
                return;
            }
            string soundName = Enum.GetName(typeof(Sounds), sound);
            // assumes enum contains an underscore
            string soundType = soundName[..soundName.IndexOf('_')];

            switch (soundType) {
                case "sound": {
                    if (debugAudioLogs) { Log.Debug("[AudioManager] Playing Sound " + soundName); }
                    PlaySound(soundName, randomise, variant);
                    break;
                }
                case "loop": {
                    if (debugAudioLogs) { Log.Debug("[AudioManager] Playing Loop " + soundName); }
                    PlayLoop(soundName);
                    break;
                }
                case "music": {
                    if (debugAudioLogs) { Log.Debug("[AudioManager] Playing Music " + soundName); }
                    PlayMusic(soundName);
                    break;
                }
                default: { // unknown
                    Log.Warning($"[AudioManager] Unrecognised audio type, playing '{soundName}' as sound");
                    break;
                }
            }
           // PlaySound(Enum.GetName(typeof(Sounds), sound));
        }

        public void PlayLoop(string soundBaseName) {
            if (sounds.TryGetValue(soundBaseName, out List<AudioClip> soundVariants)) {
                AudioSource newLoop = gameObject.AddComponent<AudioSource>();
                newLoop.loop = true;
                newLoop.clip = soundVariants[0];
                newLoop.Play();
                loopsPlaying.Add(newLoop);
                return;
            }
            Log.Error("[AudioManager] Could not find loop " + soundBaseName);

        }

        public void PlayMusic(string musicName) {
            if (sounds.TryGetValue(musicName, out List<AudioClip> soundVariants)) {
                AudioClip[] introClips = soundVariants.Where(static clip => clip.name[clip.name.LastIndexOf('_')..] == "_intro").ToArray();
                if (introClips.Length < 1) {
                    PlayLoop(musicName);
                    return;
                }
                AudioSource newIntro = gameObject.AddComponent<AudioSource>();
                newIntro.clip = introClips[0];
                newIntro.Play();
                StartCoroutine(MusicIntroEnd(newIntro));
                musicIntrosPlaying.Add(newIntro);
                return;
            }
            Log.Error("[AudioManager] Could not find music " + musicName);
        }

        private void PlaySound(string soundBaseName,  bool randomise = true, int variant = 0, bool loop = false) {
            if (sounds.TryGetValue(soundBaseName, out List<AudioClip> soundVariants)) {
                if (randomise) { variant = GameManager.instance.random.Next(soundVariants.Count); }
                audioSource.PlayOneShot(soundVariants[variant]);
                return;
            }
            Log.Error("[AudioManager] Could not play sound " + soundBaseName);
        }

        IEnumerator MusicIntroEnd(AudioSource musicIntroSource) {
            yield return new WaitForSeconds(musicIntroSource.clip.length);
            musicIntroSource.Stop();

            if (!sounds.TryGetValue(musicIntroSource.clip.name[..musicIntroSource.clip.name.LastIndexOf('_')],
                    out List<AudioClip> audioClips)) { yield break; }

            if (audioClips.Count < 1) { yield break; }

            AudioClip[] loopClips = audioClips.Where(static clip => clip.name[clip.name.LastIndexOf('_')..] == "_loop").ToArray();
            if (loopClips.Length == 0) {
                Log.Error("[AudioManager] No loop found for " + musicIntroSource.clip.name);
                yield break;
            }
            musicIntroSource.clip = loopClips[0];
            musicIntroSource.loop = true;
            musicIntrosPlaying.Remove(musicIntroSource);
            loopsPlaying.Add(musicIntroSource);
            musicIntroSource.Play();
            if (debugAudioLogs) { Log.Debug("[AudioManager] Music Changed to loop: " + loopClips[0].name); }
        }

        public void StopAllLoops() {
            foreach (AudioSource loop in loopsPlaying) {
                loop.Stop();
                Destroy(loop);
            }
            loopsPlaying.Clear();
        }

        public void StopLoopsBySound(Sounds sound) {
            for (int i = loopsPlaying.Count - 1; i >= 0; i--) {
                AudioSource loop = loopsPlaying[i];
                string loopBaseName = loop.clip.name;
                if (loop.clip.name.CountOf('_') > 1) {
                    loopBaseName = loop.clip.name[loop.clip.name.LastIndexOf('_')..];
                }
                if (sound.ToString() == loopBaseName) {
                    loop.Stop();
                    loopsPlaying.RemoveAt(i);
                    Destroy(loop);
                    if (debugAudioLogs) { Log.Debug("[AudioManager] Stopped Loop " + loopBaseName); }
                    continue;
                }
                Log.Warning("[AudioManager] Tried to stop loop '" + loopBaseName + "' but no loop found");
            }
        }
    }
}
