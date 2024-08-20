using System;
using System.Collections.Generic;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UrchinGame.Food;

namespace UrchinGame.Urchins {
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    internal sealed class NurseryUrchin : MonoBehaviour {
        // A minimum is needed to ensure the urchin can move around the nursery
        private const float BASE_SPEED = 1f;
        private const float MIN_IDLE_TIME = 1f;
        private const float MAX_IDLE_TIME = 3f;

        // Distance from center to edge of tank
        private const float TANK_WIDTH = 8f;

        [SerializeField] private float _sizeScale = 0.4f; // Used for collider abd body size
        [SerializeField] private SpriteRenderer _bodyRenderer;
        [field: SerializeField] public UrchinData UrchinData { get; private set;} = new();

        private FoodItem _trackedFood = null;
        private State _state = State.Idle;
        private Rigidbody2D _body;
        private CircleCollider2D _collider;
        private Unity.Mathematics.Random _rng = new((uint)DateTimeOffset.UtcNow.GetHashCode());
        private Camera _trackedCamera;

        private float _lastStateChange = float.MinValue;
        private float _timeToIdle;
        private float _wanderTarget;

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

            if (_state is State.Idle) {
                _body.constraints = RigidbodyConstraints2D.None;
            }

            float distanceToFood = math.distance(transform.position, food.transform.position);
            _state = distanceToFood < MaxEatDistance ? State.Eat : State.MoveToFood;
        }

        internal void PickUp() {
            // _body.simulated = false;
            Log.Debug("[NurseryUrchin] Player picked up {0}", name);
            _state = State.Carried;
            _trackedCamera = Camera.main;
        }

        private void Drop() {
            var contacts = new List<Collider2D>();
            _body.GetContacts(contacts);

            foreach (Collider2D contact in contacts) {
                if (!contact.TryGetComponent(out Treadmill treadmill)) continue;

                Log.Debug("[NurseryUrchin] Player dropped {0} on treadmill", name);

                if (UrchinData.Stats.Weight < 1.5f) {
                    Log.Debug("[NurseryUrchin] {0} needs to eat more before they can start training!", name);
                    break;
                }

                if (treadmill.Activate(this)) {
                    ToTreadmill();
                    return;
                }
            }

            Log.Debug("[NurseryUrchin] Player dropped {0}", name);
            ToIdle();
        }

        internal void CompleteTraining() {
            UrchinData.CompleteTraining();
            ToIdle();
        }

#region Helpers

        private float MaxEatDistance => UrchinData.Stats.Size;

        private float DistanceToFood => _trackedFood switch {
            null => float.PositiveInfinity,
            _ => math.distance(transform.position, _trackedFood.transform.position),
        };

        private float MoveSpeed => BASE_SPEED + UrchinData.Stats.MaxSpeed;

        private void ApplyStats() {
            _body.mass = UrchinData.Stats.Weight;
            _collider.radius = UrchinData.Stats.Size * 0.5f * _sizeScale;
            _bodyRenderer.transform.localScale = UrchinData.Stats.Size * _sizeScale * Vector3.one;
        }

#endregion

#region Unity Lifecycle

        private void Awake() {
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        private void Start() {
            ApplyStats();
            ToIdle();
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

        /// <summary>
        /// Called when transitioning to idle state
        /// </summary>
        private void ToIdle() {
            Log.Debug("[NurseryUrchin] {0} started Idle", name);
            _body.gravityScale = 1f;
            _body.drag = 0.7f;
            _body.velocity = Vector2.zero;
            _body.constraints = RigidbodyConstraints2D.None;

            _lastStateChange = Time.time;
            _timeToIdle = _rng.NextFloat(MIN_IDLE_TIME, MAX_IDLE_TIME);
            _state = State.Idle;
        }

        private void IdleState() {
            if (!(Time.time >= _lastStateChange + _timeToIdle)) return;

            State next = _rng.NextBool() ? State.Wander : State.Rest;

            switch (next) {
                case State.Wander:
                    ToWander();
                    break;
                case State.Rest:
                    break;
                default:
                    // Unreachable
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ToWander() {
            _wanderTarget = _rng.NextFloat(-TANK_WIDTH, TANK_WIDTH);
            Log.Debug("[NurseryUrchin] {0} started Wander to {1}", name, _wanderTarget);
            _lastStateChange = Time.time;
            _state = State.Wander;
        }

        private void WanderState() {
            float distanceToTarget = math.distance(_wanderTarget, _body.position.x);

            if (distanceToTarget <= 0.1f) {
                Log.Debug("[NurseryUrchin] {0} reached wander target", name);
                ToIdle();
                return;
            }

            float moveDirection = math.sign(_wanderTarget - _body.position.x);
            _body.AddForce(new Vector2(moveDirection, 0) * MoveSpeed, ForceMode2D.Force);
            _body.velocity = Vector3.ClampMagnitude(_body.velocity, MoveSpeed);
        }

        private void RestState() { }

        private void MoveToFoodState() {
            if (DistanceToFood < MaxEatDistance) {
                _body.velocity = Vector3.zero;
                _state = State.Eat;
                return;
            }

            float2 toFood = ((float3)(_trackedFood.transform.position - transform.position)).xy;
            float2 direction = math.normalize(toFood);
            _body.AddForce(direction * MoveSpeed, ForceMode2D.Force);
            _body.velocity = Vector3.ClampMagnitude(_body.velocity, MoveSpeed);
        }

        private void EatState() {
            Log.Debug("[NurseryUrchin] {0} is eating food {1}", name, _trackedFood.name);
            UrchinData.Stats.Weight += _trackedFood.Data.Weight;
            ApplyStats();
            _trackedFood.Consume();
            _trackedFood = null;

            ToIdle();
        }

        private void CarryState() {
            if (!Mouse.current.leftButton.isPressed) {
                Drop();
                return;
            }

            float2 mousePos = Mouse.current.position.ReadValue();
            float3 worldPos = _trackedCamera.ScreenToWorldPoint(math.float3(mousePos, 0f));
            _body.MovePosition(worldPos.xy);
        }

        private void ToTreadmill() {
            _body.constraints = RigidbodyConstraints2D.FreezePosition;
            _body.velocity = Vector2.zero;
            _state = State.Treadmill;
        }

        private void TreadmillState() {
            _body.AddTorque(-UrchinData.Stats.MaxSpeed, ForceMode2D.Force);
            _body.angularVelocity = math.clamp(_body.angularVelocity, -UrchinData.Stats.MaxSpeed * 100, 0f);
        }

#endregion
    }
}
