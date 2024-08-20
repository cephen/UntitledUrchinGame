using System;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UrchinGame.Urchins {
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class Treadmill : MonoBehaviour {
        [field: SerializeField] public Transform TrainingPosition { get; private set; }
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private int _animationFPS = 4;
        [SerializeField] private float _trainingDuration = 5f;
        [SerializeField] private AssetReferenceSprite _inactiveSprite;
        [SerializeField] private AssetReferenceSprite[] _activeSprites;

        private int _activeSpriteIndex = 0;
        private float _lastSpriteChange = float.MinValue;
        private float _startedTrainingAt;
        private NurseryUrchin _trainee;

        private State _state = State.Inactive;
        private enum State { Inactive, Active, }

        internal bool Activate(NurseryUrchin urchin) {
            if (_state is State.Active) {
                Log.Debug("[Treadmill] Only one urchin can use the treadmill at a time");
                return false;
            }

            urchin.transform.SetParent(TrainingPosition);
            _trainee = urchin;
            _startedTrainingAt = Time.time;
            _state = State.Active;
            return true;
        }

#region Helpers

        private float TimeBetweenFrames => 1f / _animationFPS;

        private bool IsTrainingFinished() => _startedTrainingAt + _trainingDuration < Time.time;

#endregion


#region Unity Lifecycle

        private void Awake() {
            _inactiveSprite.LoadAssetAsync().Completed += handle => {
                if (handle.Status is AsyncOperationStatus.Succeeded)
                    _renderer.sprite = handle.Result;
                else
                    Log.Error("[Treadmill] Failed to load treadmill inactive sprite: {0}", handle.OperationException);
            };
            foreach (AssetReferenceSprite sprite in _activeSprites) { sprite.LoadAssetAsync(); }
        }

        private void Update() {
            switch (_state) {
                case State.Inactive: {
                    if (_inactiveSprite.IsDone && _inactiveSprite.Asset is Sprite sprite) _renderer.sprite = sprite;

                    break;
                }
                case State.Active: {
                    if (IsTrainingFinished()) {
                        _trainee.transform.SetParent(null, worldPositionStays: true);
                        _trainee.CompleteTraining();
                        _trainee = null;
                        _state = State.Inactive;
                    }
                    float timeSinceLastFrame = math.distance(_lastSpriteChange, Time.time);
                    if (timeSinceLastFrame < TimeBetweenFrames) return;

                    _activeSpriteIndex++; // go to next frame
                    _activeSpriteIndex %= _activeSprites.Length; // wrap around if all frames viewed

                    AssetReferenceSprite spriteHandle = _activeSprites[_activeSpriteIndex];

                    if (spriteHandle.IsDone && spriteHandle.Asset is Sprite sprite) {
                        _renderer.sprite = sprite;
                        _lastSpriteChange = Time.time;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#endregion
    }
}
