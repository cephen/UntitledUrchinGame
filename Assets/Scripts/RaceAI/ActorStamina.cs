using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrchinGame
{
    public class ActorStamina : MonoBehaviour
    {
        public delegate void ActorOutOfStaminaAction();
        public static event ActorOutOfStaminaAction OnActorStaminaDepleted;

        [SerializeField] private float stamina;
        private bool isSleeping = false;
        public void Sleep() {
            OnActorStaminaDepleted?.Invoke(); 
            isSleeping = true;
        }

        public bool GetIsSleeping() { return isSleeping; }
        private void
    }
}
