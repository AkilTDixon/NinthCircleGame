using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

// using UnityEngine.UIElements;

public class FadeInOut : MonoBehaviour {
    private float _alpha = 1;
    private bool _reverse = false;
    private bool _done = true;
    private Image img;
    [SerializeField] private float speed = 0.4f;

    private void Start() {
        img = GetComponent<Image>();
    }

    public void FadeIn() {
        _alpha = 1;
        _done = false;
        _reverse = true;
    }

    public void FadeOut() {
        _alpha = 0;
        _done = false;
        _reverse = false;
    }
    
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) { //todo remove this
            FadeIn();
        }
        if (Input.GetKeyDown(KeyCode.U)) { //todo remove this
            FadeOut();
        }

        if (!_done) {
            if (_reverse) {
                _alpha -= speed * Time.deltaTime;
                if (_alpha <= 0f) {
                    _alpha = 0f;
                    _done = true;
                }
            } else {
                _alpha += speed * Time.deltaTime;
                if (_alpha >= 1f) {
                    _alpha = 1f;
                    _done = true;
                }
            }
            img.color = new Color(img.color.r, img.color.g, img.color.b, _alpha);
        }
    }
}
