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

        private void Start() {
            stamina = 10;
        }
        private void Update() {
            DepleteStaminaOverTime();
            if (stamina < 0) 
                OnActorStaminaDepleted?.Invoke();
        }
        public float GetStamina() { return stamina; }
        public void UseStamina(float staminaAmount) {
            stamina -= staminaAmount;
        }
        private void DepleteStaminaOverTime() {
            if (stamina > 0) {
                stamina -= 1 * Time.deltaTime;
            }
        }
    }
}
