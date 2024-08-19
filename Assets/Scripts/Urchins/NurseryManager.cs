using System;
using System.Collections.Generic;
using SideFX.Events;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UrchinGame.Food;

namespace UrchinGame.Urchins {
    public sealed class NurseryManager : MonoBehaviour {
        #if UNITY_EDITOR
        [SerializeField, Tooltip("Prefab used in editor to spawn urchins at runtime")]
        private NurseryUrchin _urchinPrefab;
        #endif
        private Transform sceneRoot;
        private HashSet<NurseryUrchin> _urchins = new();

#region Unity Lifecycle

        private void Awake() {
            sceneRoot = gameObject.scene.GetRootGameObjects()[0].transform;
        }

        private void OnEnable() {
            RegisterEventHandlers();
        }

        private void OnDisable() {
            DeregisterEventHandlers();
        }

        private void Update() {
#if UNITY_EDITOR
            Keyboard keyboard = Keyboard.current;
            bool modifier = keyboard.leftShiftKey.isPressed;
            bool activator = keyboard.uKey.wasPressedThisFrame;

            if (modifier && activator) {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector3 spawnPos = Camera.main.ScreenToWorldPoint(mousePos).With(z: 0f);
                NurseryUrchin urchin = Instantiate(_urchinPrefab, sceneRoot);
                urchin.transform.position = spawnPos;
                _urchins.Add(urchin);
            }
#endif
        }

#endregion

#region Event Handlers

        private void RegisterEventHandlers() {
            _foodReadyBinding = new EventBinding<FoodReady>(OnFoodReady);
            EventBus<FoodReady>.Register(_foodReadyBinding);
        }

        private void DeregisterEventHandlers() {
            EventBus<FoodReady>.Deregister(_foodReadyBinding);
        }

        private EventBinding<FoodReady> _foodReadyBinding;

        private void OnFoodReady(FoodReady e) {
            float3 foodPos = e.Food.transform.position;

            NurseryUrchin closest = null;
            float closestDist = float.PositiveInfinity;

            foreach (NurseryUrchin urchin in _urchins) {
                float dist = math.distance(urchin.transform.position, foodPos);

                if (!(dist < closestDist)) continue;

                closest = urchin;
                closestDist = dist;
            }

            if (closest is null) {
                Log.Error("[NurseryManager] No urchins found to go eat food");
                return;
            }

            closest.GoEat(e.Food);
        }

#endregion
    }
}
