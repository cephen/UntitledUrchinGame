using SideFX.Events;
using Unity.Logging;
using Unity.Mathematics;
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
        private SpriteRenderer _renderer;
        private float _startTime;
        private float _startX;
        private float _phaseShift;

        private FallState _state = FallState.Falling;

        public void Init(FoodData data, ContactFilter2D contactFilter) {
            Data = data;
            _renderer.sprite = Data.Sprite;
            _contactFilter = contactFilter;
            _phaseShift = UnityEngine.Random.value * 360f;
        }

        private enum FallState { Falling, Resting }

#region Unity Lifecycle

        private void Awake() {
            _startX = transform.position.x;
            _startTime = Time.time;
            _renderer = GetComponent<SpriteRenderer>();
            _body = GetComponent<Rigidbody2D>();
        }


        private void FixedUpdate() {
            if (_state is FallState.Resting) return;

            float phase = math.sin(Data.SwaySpeed * (Time.time - _startTime + _phaseShift));

            float2 currPos = (Vector2)transform.position;

            float hPos = _startX + Data.SwayAmount * phase;
            float vMove = Data.FallSpeed * Time.deltaTime;
            float vPos = currPos.y - vMove;

            _body.MovePosition(new Vector2(hPos, vPos));
        }

        private void OnCollisionEnter2D(Collision2D other) {
            bool isTerrain = _contactFilter.IsFilteringLayerMask(other.gameObject);
            if (isTerrain) return;

            Log.Debug("Food {0} touched collider {1}, entering Resting state", name, other.gameObject.name);
            _state = FallState.Resting;

            EventBus<FoodReady>.Raise(new FoodReady(this));
        }

#endregion
    }
}
