using UnityEngine;

namespace UrchinGame.Urchins {
    internal class UrchinAnimator : MonoBehaviour {
        [SerializeField] private UrchinAnimation _animData;
        [SerializeField] private SpriteRenderer _render;


#region Unity Lifecycle

        private void Awake() {
            if (_animData.IdleSprite.IsDone && _animData.IdleSprite.Asset is Sprite sprite) {
                _render.sprite = sprite;
                _render.color = Color.white;
                _render.size = Vector2.one;
            }
            else {
                _animData.IdleSprite.LoadAssetAsync().Completed += handle => {
                    _render.sprite = handle.Result;
                    _render.color = Color.white;
                    _render.size = Vector2.one;
                };
            }
        }

#endregion

    }
}