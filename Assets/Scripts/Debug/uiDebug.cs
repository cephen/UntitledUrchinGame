using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UrchinGame.UI;

namespace UrchinGame.Debug {
    public class uiDebug : MonoBehaviour {
        public static uiDebug instance { get; private set; }

        [SerializeField] TextMeshProUGUI
            uiFPS,
            uiRes,
            uiStatsPlayer,
            uiStatsMiscellaneous,
            uiVersion;

        [SerializeField] GameObject
            uiDebugGroup;

        public bool debugMode { get; private set; }
        public bool debugLines { get; private set; }
        public bool uiFPSEnabled;
        public float noclipSpeed = 10f;
        [Tooltip("Lower is faster")] public float statsRepeatRate = 0.02f;
        bool noclipEnabled, godEnabled;
        LensDistortion lensDistortion;
        int fps;

        public const string
            // player
            str_playerTitle = "<u>Player/Game;</u>",
            str_targetFPS = "\ntargetFPS = ",
            str_vSync = ", vSync = ",
            str_mouseRotation = "\nmouseRotation = ",
            str_lookSensitivity = "\nlookSensitivity = ",
            str_playerDimensions = "\nplayerRadius = ",
            str_multiply = " * ",
            // level
            str_levelTitle = "<u>Level;</u>\ninLevel = ",
            str_assetKey = "\nassetKey = ",
            str_inGameName = "\ninGameName = ",
            str_objectives = "\nobjectives; ",
            str_playerStartPos = "\nplayerStartPos = ",
            // stealth
            str_stealthTitle = "<u>Stealth;</u>",
            str_stealthLevel = "\nstealthLevel = ",
            str_stealthTimer = "\nstealthTimer = ",
            str_playerVisible = "\nplayerVisible = ",
            str_toLevelSpotted = "\ntoLevelSpotted = ",
            str_toLevelFullAlert = "\ntoLevelFullAlert = ",
            str_fromLevelSpotted = "\nfromLevelSpotted = ",
            str_toGameOver = "\ntoGameOver = ",
            str_toGameOverSequnceRunning = "\ntoGameOverSequnceRunning = ",
            //other
            str_x = "x",
            str_v = "v",
            str_dash = " - ",
            str_equals = " = ",
            str_divide = " / ",
            str_NewLine = "\n",
            str_NewLineDash = "\n - ",
            str_Hz = "Hz",
            str_ResFormat = "{0}:{1}",
            str_noclip = "noclip ",
            str_god = "god mode ",
            str_enabled = "enabled",
            str_disabled = "disabled",
            str_notesTitle = "<u>Notes:</u>",
            str_todoTitle = "<u>\nTo do:</u>",
            // class names
            str_uiDebug = "uiDebug",
            str_uiDebugConsole = "uiDebugConsole",
            str_StealthHandler = "StealthHandler",
            str_Level = "Level",
            str_LevelGoal = "LevelGoal",
            str_LevelLoader = "LevelLoader",
            str_MenuLevel = "MenuLevel";

        void Awake() {
            instance = this;
            uiVersion.text = str_v + Application.version;
#if UNITY_EDITOR
            debugMode = true;
#endif
        }

        void Start() {
            uiDebugConsole.instance.Start();
            StartRepeating();
            InvokeRepeating(nameof(GetStatsAlways), 0f, statsRepeatRate);
        }

        void StartRepeating() {
            InvokeRepeating(nameof(GetRes), 0f, 1f);
            InvokeRepeating(nameof(GetStats), 0f, statsRepeatRate);
        }

        void StopRepeating() {
            CancelInvoke(nameof(GetRes));
            CancelInvoke(nameof(GetStats));
        }

        public void RefreshRepeating() {
            CancelInvoke();
            StartRepeating();
            CancelInvoke(nameof(GetStatsAlways));
            InvokeRepeating(nameof(GetStatsAlways), 0f, statsRepeatRate);
        }

        void Update() {
            if (Keyboard.current[Key.F3].wasPressedThisFrame) {
                debugMode = !debugMode;
                if (debugMode) { StartRepeating(); }
                else { StopRepeating(); }

                if (!uiFPSEnabled) uiFPS.gameObject.SetActive(debugMode);
            }

            uiDebugGroup.SetActive(debugMode);
            if (debugMode & !uiDebugConsole.instance.gameObject.activeSelf) { Controls(); }

            /* toggles debug console -> */
            if (Keyboard.current[Key.Backquote].wasPressedThisFrame && debugMode) {
                uiDebugConsole.instance.gameObject.SetActive(!uiDebugConsole.instance.gameObject.activeSelf);
            }

            //Noclip();
            //Player.instance.moveActive = !noclipEnabled;
            GetFPS();
        }

        void GetRes() // gets the current resolution, refresh rate and aspect ratio
        {
            float gcd = Extensions.CalcGCD(Screen.width, Screen.height);
            uiRes.text = Screen.width.ToString()
                         + str_x
                         + Screen.height.ToString()
                         + str_NewLine
                         + Screen.currentResolution.refreshRateRatio
                         + str_Hz
                         + str_NewLine
                         + (string.Format(str_ResFormat, Screen.width / gcd, Screen.height / gcd));
        }

        void GetFPS() // fps counter
        {
            fps = (int)(1.0f / Time.unscaledDeltaTime);
        }

        void
            GetStats() // contructs all stats for the debug overlay, uses stringbuilder & append to slightly improve performance
        {
            if (!debugMode) {
                uiStatsPlayer.text = string.Empty;
                uiStatsMiscellaneous.text = string.Empty;
                return;
            }

            //uiStatsPlayer.text = Player.instance.debugGetStats().ToString();
            uiStatsMiscellaneous.text = new StringBuilder("<u>Miscellaneous;</u>")
                .Append("\nuiFadeAlpha = ")
                .Append(uiManager.instance.uiFadeAlpha)
                .ToString();
        }

        void GetStatsAlways() {
            uiFPS.text = fps.ToString();
        }

        void Controls() {
            if (Keyboard.current[Key.F4].wasPressedThisFrame) { debugLines = debugMode && !debugLines; }

            if (Keyboard.current[Key.F5].wasPressedThisFrame) { uiDebugConsole.instance.InternalCommandCall("reset"); }

            if (Keyboard.current[Key.F6].wasPressedThisFrame) { ToggleNoclip(); }
        }

        public void ToggleNoclip() {
            noclipEnabled = !noclipEnabled;
            uiMessage.instance.New(str_noclip + (noclipEnabled ? str_enabled : str_disabled), str_uiDebug);
        }

        public void ToggleGod() {
            godEnabled = !godEnabled;
            uiMessage.instance.New(str_god + (godEnabled ? str_enabled : str_disabled), str_uiDebug);
        }

        public void ToggleFPS() {
            uiFPSEnabled = !uiFPSEnabled;
            if (!uiFPSEnabled && debugMode) { return; }

            uiFPS.gameObject.SetActive(uiFPSEnabled);
        }
    }
}
