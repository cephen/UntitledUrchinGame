using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrchinGame.Urchins;
using Unity.Logging;

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
            ApplyStaminaStats();
        }

        private void Update() {
            DepleteStaminaOverTime();
            if (stamina < 0) {
                OnActorStaminaDepleted?.Invoke(); // Not set to anything yet
                Log.Debug("out of stamina");
            }
                
        }
        #region    Get Stats

        public void GetData(UrchinData data) {
            this.data = data;
        }
        
        private void ApplyStaminaStats() { // change to data stats
            float placeholderStamina = 80;
            stamina = placeholderStamina;
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
