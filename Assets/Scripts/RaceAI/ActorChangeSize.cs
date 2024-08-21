using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Logging;

namespace UrchinGame.AI
{
    public class ActorChangeSize : MonoBehaviour
    {
        [SerializeField, Tooltip("How much the size of the urnchin will be increased by")]
        private float sizeMultiplier;

        private Rigidbody2D rb;
        private CircleCollider2D _collider2D;
        private SpriteRenderer bodyRenderer;

        [SerializeField] private bool isSizeIncreased; // Serialized for debugging

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            bodyRenderer = GetComponentInChildren<SpriteRenderer>();  
        }

        #region Called by Statemachine
        public void ChangeUrchinSize() {
            if (!isSizeIncreased)
                IncreaseSize();
            else
                DecreaseSize();
        }
        #endregion

        private void IncreaseSize() {
            isSizeIncreased = true;
            rb.mass *= sizeMultiplier;
            _collider2D.radius *= sizeMultiplier;
            bodyRenderer.transform.localScale *= sizeMultiplier;
            Log.Debug("Increased urchin Size");
        }
        private void DecreaseSize() {
            isSizeIncreased = false;
            rb.mass /= sizeMultiplier;
            _collider2D.radius /= sizeMultiplier;
            bodyRenderer.transform.localScale /= sizeMultiplier;
            Log.Debug("Decreased urchin Size");
        }
    }
}
