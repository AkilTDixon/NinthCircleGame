using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemTextContainer;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro descText;
    [SerializeField] private string prompt;
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;

    // *** ITEM'S SPECIFIC PROPERTIES GO HERE (MUST LIKELY USING SCRIPTABLE OBJECTS) ***

    void Awake()
    {
        nameText.text = itemName;
        descText.text = itemDescription;
        itemTextContainer.SetActive(false);
    }

    public void SpawnItem()
    {
        // *** SET UP ITEM'S PROPERTIES (MUST LIKELY USING SCRIPTABLE OBJECTS) ***

        ///

        gameObject.SetActive(true);
    }


    public string InteractPrompt()
    {
        return prompt;
    }

    public void HoverOver()
    {
        print("I'm looking at item: " + name);
        itemTextContainer.SetActive(true);
    }

    public void HoverOut()
    {
        print("I'm no longer looking at item: " + name);
        itemTextContainer.SetActive(false);
    }

    public void Interact()
    {
        print("Interacting with item: " + name);
       
        // *** OBTAIN THE ITEM HERE ***

        gameObject.SetActive(false);
    }
}
