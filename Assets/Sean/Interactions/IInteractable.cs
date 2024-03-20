using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string InteractPrompt();
    void HoverOver();
    void HoverOut();
    void Interact();
}
