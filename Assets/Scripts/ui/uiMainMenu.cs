using System.Collections.Generic;
using UnityEngine;

namespace UrchinGame.UI {
    public class uiMainMenu : MonoBehaviour {
        public List<uiAnimatedImage> animatedImages = new();
        private void Update() {
            foreach (uiAnimatedImage animatedImage in animatedImages) { animatedImage.UpdateImage(); }
        }
    }
}
