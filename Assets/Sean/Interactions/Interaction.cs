using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactKeyText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private float viewLength = 3f;
    Vector3 viewPoint = new Vector3(0.5f, 0.5f, 0);
    RaycastHit hit;
    Ray ray;
    IInteractable interact;
    IInteractable lastInteract;

    void Awake()
    {
        interactKeyText.text = PlayerInfoScript.Instance.shopInteract.ToString();
        interactionUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;

        ray = cam.ViewportPointToRay(viewPoint);
        
        // Is the player looking at something close enough AND is it interactable?
        if (Physics.Raycast(ray, out hit, viewLength) && hit.transform.TryGetComponent(out interact))
        {
            interactText.text = interact.InteractPrompt();
            interactionUI.SetActive(true);
            
            // Do any hover over actions that the interactable might have
            interact.HoverOver();

            // Did the user interact?
            if (Input.GetKeyUp(PlayerInfoScript.Instance.shopInteract))
                interact.Interact();

            // Did we look at 1 interactable to another?
            if(lastInteract != interact && lastInteract != null)
                lastInteract.HoverOut();

            lastInteract = interact;
        }
        else
        {
            // Are we no longer looking at the interactable?
            if (lastInteract != null)
                lastInteract.HoverOut();
            interactionUI.SetActive(false);
        }
    }
}
