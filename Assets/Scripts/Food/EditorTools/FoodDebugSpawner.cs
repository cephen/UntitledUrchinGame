using UnityEngine;
using UnityEngine.InputSystem;

namespace UrchinGame.Food.EditorTools {
    public sealed class FoodDebugSpawner : MonoBehaviour {
        [SerializeField] private FoodDB _db;
        [SerializeField] private FoodItem _foodPrefab;

        private Camera _cam;

        private void Start() {
            _cam = Camera.main;
        }


        private void Update() {
            if (!Keyboard.current[Key.LeftShift].isPressed
                || !Keyboard.current[Key.Digit1].wasPressedThisFrame) return;

            if (_cam is null) return;

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 spawnPos = _cam.ScreenToWorldPoint(mouseScreenPos);

            FoodItem food = Instantiate(_foodPrefab, spawnPos, Quaternion.identity);

            food.Init(_db.FoodData[0], _db.ContactFilter);
        }
    }
}
