using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Logging;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UrchinGame.Food;

namespace UrchinGame.UI
{
    public class uiShop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private AssetReferenceT<FoodDB> foodDBAsset;
        [SerializeField] private GameObject shopButtonPrefab;
        [SerializeField] private float shopAnimationSpeed = 2500.0f;
        [SerializeField] private bool autoSetOpenClosePositions = true;
        [SerializeField] private Vector2 openPosition;
        [SerializeField] private Vector2 closedPosition;
        private bool isOpen;
        private bool isOpening;
        private RectTransform rootTransform;
        private Vector2 targetPosition;
        private ScrollRect uiShopScrollRect;
        private GameObject uiShopScrollRectObject;
        private RectTransform uiShopScrollRectTransform;
        private FoodDB foodDB;

        private void Awake() {
            rootTransform = GetComponent<RectTransform>();
            uiShopScrollRectObject = transform.GetChild(0).gameObject;
            uiShopScrollRect = uiShopScrollRectObject.GetComponent<ScrollRect>();
            uiShopScrollRectTransform = uiShopScrollRectObject.GetComponent<RectTransform>();
        }

        private void Start() {
            foodDBAsset.LoadAssetAsync().Completed += LoadFoodDBComplete;
            if (autoSetOpenClosePositions) {
                openPosition = new Vector2();
                closedPosition = new Vector2(-rootTransform.sizeDelta.x * 0.75f, 0.0f);
            }
            rootTransform.anchoredPosition = closedPosition;
        }

        void LoadFoodDBComplete(AsyncOperationHandle<FoodDB> asyncOperationHandle) {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded) {
                PopulateShop(asyncOperationHandle.Result);
                return;
            }
            Log.Error("Failed to load FoodDB");
        }

        void PopulateShop(FoodDB _foodDB) {
            foodDB = _foodDB;
            foreach (FoodData foodData in foodDB.FoodData) {
                GameObject newButtonObject = Instantiate(shopButtonPrefab, uiShopScrollRect.content.transform);
                uiShopButton newButton = newButtonObject.GetComponent<uiShopButton>();
                newButton.Init(foodData, foodDB.ContactFilter);
            }
        }
        private void Update() {
            targetPosition = isOpening ? openPosition : closedPosition;
            isOpen = isOpening && rootTransform.anchoredPosition == openPosition;
            PositionUpdate();
        }

        public void OnPointerEnter(PointerEventData pointerEventData) {
            isOpening = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            isOpening = false;
        }

        private void PositionUpdate() {
            rootTransform.anchoredPosition = Vector2.MoveTowards(
                rootTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * shopAnimationSpeed);
        }
    }
}
