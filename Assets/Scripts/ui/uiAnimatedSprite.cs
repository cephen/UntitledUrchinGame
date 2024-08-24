using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable] public class uiAnimatedImage {
    public Image image;
    public List<Sprite> sprites;
    public float timeToSwitchSprite;
    private float spriteTimer;
    private int currentSpriteIndex;

    public uiAnimatedImage(Image _image, List<Sprite> _sprites, float _timeToSwitchSprite) {
        image = _image;
        sprites = _sprites;
        timeToSwitchSprite = _timeToSwitchSprite;
    }

    public void UpdateImage() {
        spriteTimer += Time.deltaTime;
        if (spriteTimer < timeToSwitchSprite) { return; }
        spriteTimer = 0.0f;
        currentSpriteIndex = (currentSpriteIndex + 1 >= sprites.Count) ? 0 : (currentSpriteIndex + 1);
        image.sprite = sprites[currentSpriteIndex];
    }
}
