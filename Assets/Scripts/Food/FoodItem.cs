using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;

namespace UrchinGame.Food {
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public sealed class FoodItem : MonoBehaviour {
        private ContactFilter2D _contactFilter;
        private Rigidbody2D _body;
        private FoodData _data;
        private SpriteRenderer _renderer;
        private float _startTime;
        private float _startX;

        private FallState _state = FallState.Falling;

        public void Init(FoodData data, ContactFilter2D contactFilter) {
            _contactFilter = contactFilter;
            _data = data;
            _renderer.sprite = _data.Sprite;
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

            float phase = math.sin(_data.SwaySpeed * (Time.time - _startTime));

            float2 currPos = (Vector2)transform.position;

            float hPos = _startX + _data.SwayAmount * phase;
            float vMove = _data.FallSpeed * Time.deltaTime;
            float vPos = currPos.y - vMove;

            _body.MovePosition(new Vector2(hPos, vPos));
        }

        private void OnCollisionEnter2D(Collision2D other) {
            Log.Debug("Food {0} touched collider {1}", name, other.gameObject.name);

            bool isTerrain = _contactFilter.IsFilteringLayerMask(other.gameObject);

            if (isTerrain) _state = FallState.Resting;
        }

#endregion
    }
}
