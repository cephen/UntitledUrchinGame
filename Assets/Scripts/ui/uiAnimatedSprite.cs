using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[Serializable]
public class uiAnimatedImage : MonoBehaviour {
    private Image image;
    public Sprite[] sprites;
    public float timeToSwitchSprite;
    private float spriteTimer;
    private int currentSpriteIndex;

    public uiAnimatedImage(Image _image, Sprite[] _sprites, float _timeToSwitchSprite) {
        image = _image;
        sprites = _sprites;
        timeToSwitchSprite = _timeToSwitchSprite;
    }

    public void Awake() {
        image = GetComponent<Image>();
    }

    public void Update() {
        spriteTimer += Time.deltaTime;
        if (spriteTimer < timeToSwitchSprite) { return; }
        spriteTimer = 0.0f;
        currentSpriteIndex = (currentSpriteIndex + 1 >= sprites.Length) ? 0 : (currentSpriteIndex + 1);
        image.sprite = sprites[currentSpriteIndex];
    }
}
