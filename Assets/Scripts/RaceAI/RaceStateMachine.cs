using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;
using UnityEngine.InputSystem;
using UrchinGame.Urchins;

namespace UrchinGame.AI
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class RaceStateMachine : MonoBehaviour
    {
        [SerializeField, Tooltip("Tick to make actor playable")] private bool isPlayable;

        [SerializeField, Tooltip("Used for collider abd body size")]
        private float sizeScale = 0.4f;
        [SerializeField] private UrchinData data;
        private enum StateMachince { Running, NoStamina, Jump }
        private StateMachince stateMachine;

        private ActorMove actorMove;
        private ActorJump actorJump;
        private ActorStamina actorStamina;
        private ActorChangeSize actorChangeSize;

        private Rigidbody2D rb;
        private CircleCollider2D _collider2D;
        [SerializeField, Tooltip("Apply body renderer")]
        private SpriteRenderer bodyRenderer;

        private float speed;
        private bool noStamina = false;

        private void Start() {
            actorMove = GetComponent<ActorMove>();
            actorJump = GetComponent<ActorJump>();
            actorStamina = GetComponent<ActorStamina>();
            actorChangeSize = GetComponent<ActorChangeSize>();
            rb = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            bodyRenderer = GetComponentInChildren<SpriteRenderer>();
            if (isPlayable) ApplyPlayerUrchinStats();
            if (!isPlayable) ApplyRandomStats();
        }
        
        #region Racing AI Statemachine
        void FixedUpdate() {
            switch (stateMachine) {
                case StateMachince.Running:
                    if (actorStamina.GetStamina() < 0)
                        stateMachine = StateMachince.NoStamina;
                    actorMove.Move();
                    break;

                case StateMachince.Jump:
                    actorJump.Jump();
                    stateMachine = StateMachince.Running;
                    break;

                case StateMachince.NoStamina:
                    Log.Debug($"{gameObject.name} is out of stamina and fell asleep");
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Player Input
        private void Update() {
            if (Keyboard.current[Key.Space].wasPressedThisFrame && isPlayable) // Need to change input system
                actorJump.Jump();
            if (Keyboard.current[Key.Enter].wasPressedThisFrame && isPlayable) { // <3 if Statements
                actorChangeSize.ChangeUrchinSize();
            }
        }
        #endregion

        #region    Get Stats

        public void GetData(UrchinData data) {
            this.data = data;
        }

        private void ApplyPlayerUrchinStats() { // switch to UrchinData class stats
            //float placeholderWeight = 1;
            //float placeholderSize = 1;
            rb.mass = data.Stats.Weight;
            _collider2D.radius = data.Stats.Size * 0.5f * sizeScale;
            bodyRenderer.transform.localScale = data.Stats.Size * sizeScale * Vector3.one;
            // Get sprite data
        }
        private void ApplyRandomStats() {
            // Randomise Stats for AI racers
        }
        #endregion
    }
}
