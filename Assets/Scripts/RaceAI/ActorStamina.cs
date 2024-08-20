using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrchinGame.Urchins;

namespace UrchinGame.AI
{
    public class ActorStamina : MonoBehaviour
    {
        public delegate void ActorOutOfStaminaAction();
        public static event ActorOutOfStaminaAction OnActorStaminaDepleted;

        [SerializeField] private UrchinData data;
        private ActorMove actorMove;
        private float stamina;

        private void Awake() {
            actorMove = GetComponent<ActorMove>();
        }

        private void Start() {
            ApplyStats();
        }

        private void Update() {
            DepleteStaminaOverTime();
            if (stamina < 0) 
                OnActorStaminaDepleted?.Invoke(); // Not set to anything yet
        }
        #region    Get Stats
        public void GetData(UrchinData data) {
            this.data = data;
        }
        private void ApplyStats() { // Need to move to state machine
            stamina = data.Stats.MaxStamina;
            // Get sprite
        }
        #endregion
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
