using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;
using UnityEngine.InputSystem;

namespace UrchinGame
{
    public class RaceStateMachine : MonoBehaviour
    {
        [SerializeField] private bool isPlayable;
        private enum StateMachince { Running, NoStamina, Jump }
        private StateMachince stateMachine;

        private ActorMove actorMove;
        private ActorJump actorJump;
        private ActorStamina actorStamina;


        private void Start() {
            actorMove = GetComponent<ActorMove>();
            actorJump = GetComponent<ActorJump>();
            actorStamina = GetComponent<ActorStamina>();
        }

        void Update() {
            switch (stateMachine) {
                case StateMachince.Running:
                    actorMove.Move();
                    if (Keyboard.current[Key.Space].wasPressedThisFrame && isPlayable)
                        stateMachine = StateMachince.Jump;
                    break;

                case StateMachince.Jump:
                    actorJump.Jump();
                    stateMachine = StateMachince.Running;
                    break;

                case StateMachince.NoStamina:
                    actorStamina.Sleep();
                    Log.Debug($"{gameObject.name} is out of stamina and fell asleep");
                    break;

                default:
                    break;
            }
        }

        private void DepleteStaminaOverTime() {

        }
    }
}
