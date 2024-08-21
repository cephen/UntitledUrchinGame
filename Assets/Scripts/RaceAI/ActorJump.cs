using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;

namespace UrchinGame.AI
{
    public class ActorJump : MonoBehaviour
    {
        [SerializeField] private float jumpPower;
        [SerializeField] private float jumpStaminaAmount;
        private ActorStamina actorStamina;
        private Rigidbody2D rb;
        private const int maxJumpCount = 1;
        private int jumpCount = 0;
        [SerializeField]private bool isGrounded;
        
        private void Start() {
            rb = GetComponent<Rigidbody2D>();
            actorStamina = GetComponent<ActorStamina>();
        }
        public void Jump() {
            if (jumpCount < maxJumpCount && isGrounded) {
                jumpCount++;
                isGrounded = false;
                Vector2 jumpDir = Vector2.up;
                rb.AddForce(jumpDir * jumpPower, ForceMode2D.Impulse);
                actorStamina.UseStamina(jumpStaminaAmount);
                Log.Debug($"Jumped");
            }
        }

        public bool GetGroundedStatus() {
            return isGrounded;
        }
        private void ResetJump() {
            jumpCount = 0;
            isGrounded = true;
        }
        private void OnCollisionEnter2D(Collision2D collision) {
            if (!isGrounded && collision.gameObject.GetComponent<TrackFloorTag>()) {
                ResetJump();
            }
        }
    }
}
