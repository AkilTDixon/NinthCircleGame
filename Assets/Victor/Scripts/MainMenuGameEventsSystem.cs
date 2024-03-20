using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuGameEventsSystem : MonoBehaviour {
    public static MainMenuGameEventsSystem current;
    // Start is called before the first frame update
    private void Awake() {
        current = this;
    }

    public event Action onHoverEnter;

    public void hoverEnter() {
        if (onHoverEnter != null) {
            onHoverEnter();
        }
    }

    public event Action onClick;

    public void click() {
        if (onClick != null) {
            onClick();
        }
    }
}
