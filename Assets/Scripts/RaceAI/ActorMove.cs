using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UrchinGame;

namespace UrchinGame
{
    public class ActorMove : MonoBehaviour
    {
        [SerializeField] private float speed;
        private float weight; // To do
        private float size; // To do
        public void Move() {
            Vector2 moveDir = Vector2.right;
            transform.Translate(moveDir * speed * Time.deltaTime);
        }
    }
}
