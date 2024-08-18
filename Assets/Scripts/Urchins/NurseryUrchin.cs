using System;
using Unity.Mathematics;
using UnityEngine;
using UrchinGame.Food;

namespace UrchinGame.Urchins {
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    internal sealed class NurseryUrchin : MonoBehaviour {
        // A minimum is needed to ensure the urchin can move around the nursery
        private const float BASE_SPEED = 1f;

        [SerializeField] private StatBlock _stats;
        [SerializeField] private SpriteRenderer _bodyRenderer;

        private FoodItem _trackedFood = null;
        private State _state = State.Idle;
        private Rigidbody2D _body;
        private CircleCollider2D _collider;

        private enum State {
            Idle,
            Wander,
            Rest,
            MoveToFood,
            Eat,
            Carried,
            Treadmill,
        }

        /// <summary>
        /// Called by the <see cref="NurseryManager"/>
        /// when a food item has been placed near the urchin.
        /// </summary>
        internal void GoEat(FoodItem food) {
            _trackedFood = food;

            float distanceToFood = math.distance(transform.position, food.transform.position);
            _state = distanceToFood < MaxEatDistance ? State.Eat : State.MoveToFood;
        }

#region Helpers

        private float MaxEatDistance => _stats.Size * 1.2f;

        private float DistanceToFood => _trackedFood switch {
            null => float.PositiveInfinity,
            _ => math.distance(transform.position, _trackedFood.transform.position),
        };

#endregion

#region Unity Lifecycle

        private void Awake() {
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        private void Start() {
            _body.mass = _stats.Weight;
            _collider.radius = _stats.Size * 0.5f;
            _bodyRenderer.size = Vector2.one * _stats.Size;
        }

        private void FixedUpdate() {
            Action handler = _state switch {
                State.Idle => IdleState,
                State.Wander => WanderState,
                State.Rest => RestState,
                State.MoveToFood => MoveToFoodState,
                State.Eat => EatState,
                State.Carried => CarryState,
                State.Treadmill => TreadmillState,
                _ => throw new ArgumentOutOfRangeException(),
            };

            handler();
        }

#endregion

#region StateMachine

        private void IdleState() { }

        private void WanderState() { }

        private void RestState() { }

        private void MoveToFoodState() {
            if (DistanceToFood < MaxEatDistance) {
                _body.simulated = false;
                _state = State.Eat;
                return;
            }

            float2 toFood = ((float3)(_trackedFood.transform.position - transform.position)).xy;
            float2 direction = math.normalize(toFood);
            _body.AddForce(direction * (BASE_SPEED + _stats.MaxSpeed), ForceMode2D.Force);
        }

        private void EatState() {
            _stats.Weight += _trackedFood.Data.Weight;
            _body.mass = _stats.Weight;
            _trackedFood.Consume();
            _trackedFood = null;
            _body.simulated = true;
            _state = State.Idle;
        }

        private void CarryState() { }
        private void TreadmillState() { }

#endregion
    }
}
