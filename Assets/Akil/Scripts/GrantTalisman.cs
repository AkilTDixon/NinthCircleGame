using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantTalisman : MonoBehaviour
{

    [SerializeField] int TalismanID = 0;
    [TextArea] [SerializeField] string TalismanName;
    [TextArea] [SerializeField] string TalismanDescription;
    [SerializeField] Color TalismanColor;
    private bool AllowDestroy = false;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerInfoScript.Instance.TalismanPanelHolder.SetActive(true);
            PlayerInfoScript.Instance.TalismanNameText.text = TalismanName;
            PlayerInfoScript.Instance.TalismanNameText.color = TalismanColor;
            TalismanDescription = TalismanDescription.Replace("&activateKeybind", PlayerInfoScript.Instance.activateTalismanKey.ToString());
            TalismanDescription = TalismanDescription.Replace("&cycleKeybind", PlayerInfoScript.Instance.cycleTalismanKey.ToString());
            PlayerInfoScript.Instance.TalismanDescriptionText.text = TalismanDescription;
            
            GameEvents.current.itemPickedUp();
            Time.timeScale = 0;
            AllowDestroy = true;
            
        }
    }
    void Update()
    {
        if (AllowDestroy)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                GameEvents.current.infoSkipped();
                PlayerInfoScript.Instance.TalismanPanelHolder.SetActive(false);
                Time.timeScale = 1;
                PlayerInfoScript.Instance.talismanUnlocked[TalismanID] = true;
                Destroy(gameObject);
            }
    }
    /*

Talisman    -   ID

Shield      -   0
Heal        -   1
Charge      -   2          
Grapple     -   3
 */

}
