using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UrchinGame.Game;

namespace UrchinGame.UI {
    public class uiManager : MonoBehaviour {
        public static uiManager instance { get; private set; }
        public Button initialButtonToFocus;

        public bool
            uiFadeToBlack;

        [SerializeField] Image
            uiFade;

        [SerializeField] float
            uiFadeInSpeed,
            uiFadeOutSpeed;

        public float uiFadeAlpha;

        void Awake() {
            instance = this;
        }

        void Start() {
            initialButtonToFocus.Select();
        }

        void Update() {
            //if (Input.GetKeyDown(KeyCode.Escape)) { settings.gameObject.SetActive(!settings.gameObject.activeSelf); }
            uiFadeUpdate();
        }

        public void Play() {
            GameManager.instance.Play();
        }

        public void Quit() {
            UrchinGame.Debug.uiDebugConsole.instance.InternalCommandCall("quit");
        }

        /// <summary>
        /// Updates the color of the ui fade element on screen used to hide the screen, uiFadeToBlack controls the direction of the fade
        /// </summary>
        void uiFadeUpdate() {
            uiFade.color = new Color(
                0,
                0,
                0,
                System.Math.Clamp(
                    uiFade.color.a
                    + (uiFadeToBlack ? Time.deltaTime * uiFadeInSpeed : -Time.deltaTime * uiFadeOutSpeed),
                    0f,
                    1f
                )
            );
            uiFadeAlpha = uiFade.color.a;
        }

        public void uiFadeAlphaSet(float alpha) => uiFade.color = new Color(0, 0, 0, alpha);
    }
}
