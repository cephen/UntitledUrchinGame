﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;

namespace UrchinGame.AI
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class ActorMove : MonoBehaviour
    {
        [SerializeField] private float startSpeed;
        [SerializeField, Range(0f, 30f), Tooltip("Set the amount speed will be set to when going downhill")]
        private float downHillMaxSpeed;
        [SerializeField, Range(0f, 30f), Tooltip("Set the amount speed will be set to when going uphill")]
        private float upHillMaxSpeed;
        [SerializeField, Tooltip("Set layer mask of Race Track to check for grounded collisions")]
        private LayerMask groundLayerMask;
        private float weight; // To do
        private float size; // To do
        private float speed;
        [SerializeField] private bool goingUpHill; // Serialized for debugigng
        [SerializeField] private bool goingDownHill; // Serialized for debugigng - using seperate bools so we know if we are on flat land
        private CircleCollider2D _collider2D;
        private Rigidbody2D rb;
        private Vector3 yOffsetRight, yOffsetLeft;
        private ActorJump actorJump;
        private bool changeSpeedOnce = false;

        private void Awake() {
            _collider2D = GetComponent<CircleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            actorJump = GetComponent<ActorJump>();
        }
        private void Start() {
            // Set the position of the raycasts
            yOffsetRight = new Vector3(-_collider2D.radius/2, _collider2D.radius /2, 0f);
            yOffsetLeft = new Vector3(_collider2D.radius/2, _collider2D.radius /2, 0f);
            speed = startSpeed;
        }
        private void Update() {
            FrontRayCast();
            BackRayCast();
            CheckTrackAngle();
            //AdjustSpeedOnHill(); - Relying on physics only?
            //Log.Debug($"currente speed = {speed}");
        }
        #region Called in State Machine
        public void Move() {
            //Vector2 sufraceNormal = SurfaceNormalVector(); not working - trying to use surface normal stick to the floor and stop jittering
            Vector2 moveDir = Vector2.right;
            rb.AddForce(moveDir * speed * Time.fixedDeltaTime, ForceMode2D.Force);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
        }
        #endregion
        #region Raycasts
        private RaycastHit2D FrontRayCast() {
            float frontRayDistance = .5f;
            RaycastHit2D hit = Physics2D.Raycast(_collider2D.bounds.center - yOffsetRight , Vector2.right, frontRayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(_collider2D.bounds.center - yOffsetRight, Vector2.right *frontRayDistance, Color.yellow);
            return hit;
        }

        private RaycastHit2D BackRayCast() {
            float backRayDistance = .5f;
            RaycastHit2D hit = Physics2D.Raycast(_collider2D.bounds.center - yOffsetLeft, Vector2.left, backRayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(_collider2D.bounds.center - yOffsetLeft, Vector2.left * backRayDistance, Color.red);
            return hit;
        }

        private Vector2 SurfaceNormalVector() {
            float rayDistance = 1000f;
            Vector2 surfaceNomral;
            RaycastHit2D hit = Physics2D.Raycast(_collider2D.bounds.center, Vector2.down, rayDistance, groundLayerMask);
            UnityEngine.Debug.DrawRay(_collider2D.bounds.center, Vector2.down * rayDistance, Color.magenta);
            if (hit) {
                surfaceNomral = hit.normal;
            }
            else {
                surfaceNomral = Vector2.zero;
                Log.Debug($"Actor {gameObject.name} can't find suface normal");
            }
                
            return surfaceNomral;
        }
        #endregion
        private void CheckTrackAngle() {

            if (FrontRayCast()) {
                float trackAngle = Vector2.Angle(FrontRayCast().normal, Vector2.up);
                //Log.Debug($"{gameObject.name}'s Current slope angle = {trackAngle}");
                goingUpHill = true;
            }
            else
                goingUpHill = false;
            if (BackRayCast()) {
                float trackAngle = Vector2.Angle(BackRayCast().normal, Vector2.up);
                goingDownHill = true;
            }
            else
                goingDownHill = false;
            //else Log.Debug($"No slope found");
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
