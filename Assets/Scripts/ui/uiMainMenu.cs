using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Logging;
using UnityEngine;
using UnityEngine.UI;
using UrchinGame.Game;

namespace UrchinGame.UI {
    public class uiMainMenu : MonoBehaviour {
        public List<uiAnimatedButton> uiAnimatedButtons = new List<uiAnimatedButton>();

        [Serializable] public struct uiAnimatedButton {
            public uiAnimatedButton(Button _button, List<Sprite> _sprites, float _timeToSwitchSprite = 0.05f) {
                button = _button;
                sprites = _sprites;
                timeToSwitchSprite = _timeToSwitchSprite;
                spriteTimer = _timeToSwitchSprite;
                currentSpriteIndex = 0;
            }

            public Button button;
            public List<Sprite> sprites;
            public float timeToSwitchSprite;
            public float spriteTimer;
            public int currentSpriteIndex;
        }
        void Update() {
            for (int i = 0; i < uiAnimatedButtons.Count; i++) {
                uiAnimatedButton uiAnimatedButton = uiAnimatedButtons[i];
                uiAnimatedButton.spriteTimer += Time.deltaTime;
                if (uiAnimatedButton.spriteTimer >= uiAnimatedButton.timeToSwitchSprite) {
                    uiAnimatedButton.spriteTimer = 0.0f;
                    uiAnimatedButton.currentSpriteIndex = (uiAnimatedButton.currentSpriteIndex + 1 >= uiAnimatedButton.sprites.Count) ? 0 : (uiAnimatedButton.currentSpriteIndex + 1);
                    uiAnimatedButton.button.image.sprite = uiAnimatedButton.sprites[uiAnimatedButton.currentSpriteIndex];
                }
                uiAnimatedButtons[i] = uiAnimatedButton;
            }
        }
    }
}
