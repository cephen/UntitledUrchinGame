using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;
using UrchinGame;

namespace UrchinGame
{
    public class ActorMove : MonoBehaviour
    {
        [SerializeField, Range(0f,20f)] private float speed;
        [SerializeField, Range(0f, 5f), Tooltip("Set the amount speed will be multiplied by when going downhill")]
        private float speedIncreaseMultiplier;
        [SerializeField, Range(0f, 5f), Tooltip("Set the amount speed will be divided by when going uphill")]
        private float speedDecreaseMultiplier;
        [SerializeField, Tooltip("Set layer mask of Race Track to check for grounded collisions")]
        private LayerMask groundLayerMask;
        private float weight; // To do
        private float size; // To do
        [SerializeField] private bool goingUpHill; // Serialized for debugigng
        private CircleCollider2D collider2D;
        private Vector3 yOffset;

        private void Awake() {
            collider2D = GetComponent<CircleCollider2D>();
        }
        private void Start() {
            // Set the position of the raycasts
            yOffset = new Vector3(0f, collider2D.radius /2, 0f);
        }
        public void Move() { 
            Vector2 moveDir = Vector2.right;
            transform.Translate(moveDir * speed * Time.deltaTime);
        }

        private void Update() {
            CheckTrackAngle();
            FrontRayCast();
            BackRayCast();
        }

        private void CheckTrackAngle() {
            
            if (FrontRayCast()) {
                float trackAngle = Vector2.Angle(FrontRayCast().normal, Vector2.up);
                //Log.Debug($"{gameObject.name}'s Current slope angle = {trackAngle}");
                goingUpHill = true;
            }
            if (BackRayCast()) {
                float trackAngle = Vector2.Angle(BackRayCast().normal, Vector2.up);
                goingUpHill = false;
            }
            //else Log.Debug($"No slope found");
        }

        private RaycastHit2D FrontRayCast() {
            float frontRayDistance = 1f;
            RaycastHit2D hit = Physics2D.Raycast(collider2D.bounds.center - yOffset , Vector2.right, frontRayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(collider2D.bounds.center - yOffset, Vector2.right, Color.yellow);
            return hit;
        }

        private RaycastHit2D BackRayCast() {
            float backRayDistance = 1f;
            RaycastHit2D hit = Physics2D.Raycast(collider2D.bounds.center - yOffset, Vector2.left, backRayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(collider2D.bounds.center - yOffset, Vector2.left, Color.red);
            return hit;
        }

        private void AdjustSpeedOnHill() {
            if (goingUpHill) {
                speed /= speedDecreaseMultiplier;
            }
            if (!goingUpHill) {
                speed *= speedIncreaseMultiplier;
            }
        }
    }
}
