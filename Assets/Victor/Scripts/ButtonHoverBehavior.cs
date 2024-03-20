using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class ButtonHoverBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public Color normalColor;
    public Color hoverColor;

    TextMeshProUGUI childText;

    private void Start() {
        childText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        childText.color = hoverColor;
        MainMenuGameEventsSystem.current.hoverEnter();
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        childText.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData) {
        MainMenuGameEventsSystem.current.click();
    }
}
