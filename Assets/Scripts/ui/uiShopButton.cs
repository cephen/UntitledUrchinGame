using System;
using System.Collections;
using System.Collections.Generic;
using SideFX.Events;
using TMPro;
using Unity.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UrchinGame.Food;

namespace UrchinGame.UI
{
    public readonly struct OnShopPurchase : IEvent {
        public readonly FoodData food;
        public readonly ContactFilter2D contactFilter;
        public OnShopPurchase(FoodData _food, ContactFilter2D _contactFilter) {
            food = _food;
            contactFilter = _contactFilter;
        }
    }
    public class uiShopButton : MonoBehaviour {
        private Button button;
        private FoodData foodData;
        private ContactFilter2D defaultContactFilter;
        [SerializeField] private Image foodIcon;
        [SerializeField] private TextMeshProUGUI foodName;

        private void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);
        }

        public void Init(FoodData _foodData, ContactFilter2D _defaultContactFilter) {
            foodData = _foodData;
            defaultContactFilter = _defaultContactFilter;
            foodData.UIShopSprite.LoadAssetAsync().Completed += LoadImageComplete;
            foodName.text = foodData.Name;
        }

        public void LoadImageComplete(AsyncOperationHandle<Sprite> asyncOperationHandle) {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded) {
                foodIcon.sprite = asyncOperationHandle.Result;
                return;
            }
            Log.Error("Failed to load UIShopSprite of " + foodData.Name);
        }

        public void Click() {
            uiMessage.instance.New("Bought " + foodData.Name, "uiShopButton");
            EventBus<OnShopPurchase>.Raise(new OnShopPurchase(foodData, defaultContactFilter));
        }
    }
}
