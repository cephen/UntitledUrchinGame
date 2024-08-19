using SideFX.Events;
using Unity.Logging;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace UrchinGame.Food {
    /// <summary>
    /// Raised when a food item touches the floor of the tank,
    /// signifying it can be reached by urchins.
    /// </summary>
    public readonly struct FoodReady : IEvent {
        public readonly FoodItem Food;
        public FoodReady(FoodItem food) => Food = food;
    }

    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public sealed class FoodItem : MonoBehaviour {
        internal FoodData Data { get; private set; }

        private ContactFilter2D _contactFilter;
        private Rigidbody2D _body;
        private Collider2D _collider;
        private SpriteRenderer _renderer;
        private float _startTime;
        private float _startX;
        private float _phaseShift;

        private State _state = State.Falling;
        private const float DespawnTime = 2f;
        private float _despawnStartTime;

        private enum State { Falling, Resting, Despawning }

        public void Init(FoodData data, ContactFilter2D contactFilter) {
            Data = data;
            name = Data.Name;
            _renderer.sprite = Data.Sprite;
            _contactFilter = contactFilter;
            _phaseShift = UnityEngine.Random.value * 360f;
        }

        public void Consume() {
            Log.Debug("[FoodItem] {0} consumed", Data.Name);
            _state = State.Despawning;
            _despawnStartTime = Time.time;
            _body.simulated = false;
            _collider.enabled = false;
        }


#region Unity Lifecycle

        private void Awake() {
            _startX = transform.position.x;
            _startTime = Time.time;
            _renderer = GetComponent<SpriteRenderer>();
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }


        private void FixedUpdate() {
            switch (_state) {
                case State.Falling:
                    float phase = math.sin(Data.SwaySpeed * (Time.time - _startTime + _phaseShift));

                    float2 currPos = (Vector2)transform.position;

                    float hPos = _startX + Data.SwayAmount * phase;
                    float vMove = Data.FallSpeed * Time.deltaTime;
                    float vPos = currPos.y - vMove;

                    _body.MovePosition(new Vector2(hPos, vPos));
                    return;
                case State.Despawning: // fade out the renderer
                    float elapsedRatio = math.unlerp(_despawnStartTime, _despawnStartTime + DespawnTime, Time.time);
                    if (elapsedRatio >= 1.1f) { // add a slight delay after animation completes
#if UNITY_EDITOR
                        DestroyImmediate(gameObject);
#else
                        Destroy(gameObject);
#endif
                        return;
                    }

                    float alpha = 1f - math.clamp(elapsedRatio, 0f, 1f);
                    _renderer.color = _renderer.color.WithAlpha(alpha);
                    return;
                default:
                    return;
            }
        }

        private void OnCollisionEnter2D(Collision2D other) {
            bool isTerrain = _contactFilter.IsFilteringLayerMask(other.gameObject);
            if (isTerrain) return;

            Log.Debug("Food {0} touched collider {1}, entering Resting state", name, other.gameObject.name);
            _state = State.Resting;

            EventBus<FoodReady>.Raise(new FoodReady(this));
        }

#endregion
    }
}
