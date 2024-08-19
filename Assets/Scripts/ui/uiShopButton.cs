using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UrchinGame.Food;

namespace UrchinGame.UI
{
    public class uiShopButton : MonoBehaviour {
        private Button button;
        private FoodData foodData;
        private void Awake() {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);
        }

        public void Init(FoodData _foodData) {
            foodData = _foodData;
            foodData.UIShopSprite.LoadAssetAsync().Completed += LoadImageComplete;
        }

        public void Click() {
            uiMessage.instance.New("Bought " + foodData.Name, "uiShopButton");
        }

        public void LoadImageComplete(AsyncOperationHandle<Sprite> asyncOperationHandle) {
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded) {
                button.image.sprite = asyncOperationHandle.Result;
                return;
            }
            Log.Error("Failed to load UIShopSprite of " + foodData.Name);
        }
    }
}
