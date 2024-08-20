using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;
using UnityEngine.InputSystem;

namespace UrchinGame.AI
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
