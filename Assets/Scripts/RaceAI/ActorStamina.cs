using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrchinGame.AI
{
    public class ActorStamina : MonoBehaviour
    {
        public delegate void ActorOutOfStaminaAction();
        public static event ActorOutOfStaminaAction OnActorStaminaDepleted;

        [SerializeField, Range(0f,100f), Tooltip("How long the actor can race for.")] private float stamina;

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
