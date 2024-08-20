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

        private Rigidbody2D rb;
        private CircleCollider2D _collider2D;
        [SerializeField, Tooltip("Apply body renderer")]
        private SpriteRenderer bodyRenderer;

        private float speed;


        private void Start() {
            actorMove = GetComponent<ActorMove>();
            actorJump = GetComponent<ActorJump>();
            actorStamina = GetComponent<ActorStamina>();
            ApplyStats();
        }
        #region    Get Stats
        public void GetData(UrchinData data) {
            this.data = data;
        }
        private void ApplyStats() { // Need to move to state machine
            rb.mass = data.Stats.Weight;
            _collider2D.radius = data.Stats.Size * 0.5f * sizeScale;
            bodyRenderer.transform.localScale = data.Stats.Size * sizeScale * Vector3.one;
            // Get sprite
        }
        #endregion
        void FixedUpdate() {
            switch (stateMachine) {
                case StateMachince.Running:
                    if (actorStamina.GetStamina() < 0)
                        stateMachine = StateMachince.NoStamina;
                    actorMove.Move();
                    if (Keyboard.current[Key.Space].wasPressedThisFrame && isPlayable) // Need to change input system
                        stateMachine = StateMachince.Jump;
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
    }
}
