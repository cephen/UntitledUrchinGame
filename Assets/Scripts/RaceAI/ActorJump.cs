using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrchinGame
{
    public class ActorJump : MonoBehaviour
    {
        [SerializeField] private float jumpPower;
        [SerializeField] private int jumpCount = 0;
        private Rigidbody2D rb;
        [SerializeField]private bool isGrounded;


        private void Start() {
            rb = GetComponent<Rigidbody2D>();
        }
        public void Jump() {
            if (jumpCount < 1 && isGrounded) {
                jumpCount++;
                isGrounded = false;
                rb.velocity = Vector2.up * jumpPower;
            }
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
