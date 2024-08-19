using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;
using UrchinGame;

namespace UrchinGame
{
    public class ActorMove : MonoBehaviour
    {
        [SerializeField, Range(0f,20f)] private float startSpeed;
        [SerializeField, Range(0f, 20f), Tooltip("Set the amount speed will be set to when going downhill")]
        private float downHillMaxSpeed;
        [SerializeField, Range(0f, 20f), Tooltip("Set the amount speed will be set to when going uphill")]
        private float upHillMaxSpeed;
        [SerializeField, Tooltip("Set layer mask of Race Track to check for grounded collisions")]
        private LayerMask groundLayerMask;
        private float weight; // To do
        private float size; // To do
        private float speed;
        [SerializeField] private bool goingUpHill; // Serialized for debugigng
        [SerializeField] private bool goingDownHill; // Serialized for debugigng - using seperate bools so we know if we are on flat land
        private CircleCollider2D collider2D;
        private Rigidbody2D rb;
        private Vector3 yOffsetRight, yOffsetLeft;
        private ActorJump actorJump;
        private bool changeSpeedOnce = false;

        private void Awake() {
            collider2D = GetComponent<CircleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            actorJump = GetComponent<ActorJump>();
        }
        private void Start() {
            // Set the position of the raycasts
            yOffsetRight = new Vector3(-collider2D.radius/2, collider2D.radius /2, 0f);
            yOffsetLeft = new Vector3(collider2D.radius/2, collider2D.radius /2, 0f);
            speed = startSpeed;
        }
        public void Move() {
            Vector2 sufraceNormal = SurfaceNormalVector();
            //Vector2 moveDir = sufraceNormal + Vector2.right; not working - trying to use surface normal stick to the floor and stop jittering
            Vector2 moveDir = Vector2.right;
            rb.transform.Translate(moveDir * speed * Time.deltaTime);
            
        }

        private void Update() {
            CheckTrackAngle();
            FrontRayCast();
            BackRayCast();
            //AdjustSpeedOnHill();
            //Log.Debug($"currente speed = {speed}");
        }

        private void CheckTrackAngle() {
            
            if (FrontRayCast()) {
                float trackAngle = Vector2.Angle(FrontRayCast().normal, Vector2.up);
                //Log.Debug($"{gameObject.name}'s Current slope angle = {trackAngle}");
                goingUpHill = true;
            } else
                goingUpHill = false;
            if (BackRayCast()) {
                float trackAngle = Vector2.Angle(BackRayCast().normal, Vector2.up);
                goingDownHill = true;
            } else
                goingDownHill= false;
            //else Log.Debug($"No slope found");
        }

        private RaycastHit2D FrontRayCast() {
            float frontRayDistance = .5f;
            RaycastHit2D hit = Physics2D.Raycast(collider2D.bounds.center - yOffsetRight , Vector2.right, frontRayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(collider2D.bounds.center - yOffsetRight, Vector2.right *frontRayDistance, Color.yellow);
            return hit;
        }

        private RaycastHit2D BackRayCast() {
            float backRayDistance = .5f;
            RaycastHit2D hit = Physics2D.Raycast(collider2D.bounds.center - yOffsetLeft, Vector2.left, backRayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(collider2D.bounds.center - yOffsetLeft, Vector2.left * backRayDistance, Color.red);
            return hit;
        }

        private Vector2 SurfaceNormalVector() {
            float rayDistance = 1000f;
            Vector2 surfaceNomral;
            RaycastHit2D hit = Physics2D.Raycast(collider2D.bounds.center, Vector2.down, rayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(collider2D.bounds.center, Vector2.down * rayDistance, Color.magenta);
            if (hit) {
                surfaceNomral = hit.normal;
            }
            else {
                surfaceNomral = Vector2.zero;
                Log.Debug($"Actor {gameObject.name} can't find suface normal");
            }
                
            return surfaceNomral;
        }

        private void AdjustSpeedOnHill() {
            if (goingUpHill && actorJump.GetGroundedStatus()) {
                speed = upHillMaxSpeed;
                //Log.Debug("Speed Changed");
            }
            if (goingDownHill && actorJump.GetGroundedStatus()) {
                speed = downHillMaxSpeed;
                //Log.Debug("Speed Changed");
            }
            if (!goingDownHill && !goingUpHill)
                speed = startSpeed;
        }
    }
}
