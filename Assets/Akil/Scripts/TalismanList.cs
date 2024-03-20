using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanList : MonoBehaviour
{
    public List<GameObject> ListOfTalismans;

    [SerializeField] GameObject shopMenu;
    public KeyCode switchTalismanKey;
    private int currentlyActive = 0;
    private bool Dead = false;


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;
        
        shopMenu = PlayerInfoScript.Instance.shopMenu;
        //ListOfTalismans[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            if (!shopMenu.activeSelf)
            {
                if (Input.GetKeyDown(PlayerInfoScript.Instance.cycleTalismanKey) && !PlayerInfoScript.Instance.ChargeActive)
                {
                    SetActiveTalisman(FindNextAvailableRight(currentlyActive));
                }             
            }
        }

    }
    public void SetActiveTalisman(int value)
    {
        if (currentlyActive != value && currentlyActive >= 0)
        {
            ListOfTalismans[currentlyActive].SetActive(false);
            currentlyActive = value;
            ListOfTalismans[currentlyActive].SetActive(true);
        }
    }
    private int FindNextAvailableRight(int index)
    {
        int found = index;
        int count = index + 1;
        if (count > ListOfTalismans.Count - 1)
            count = 0;
        
        while (count != index)
        {
            if (PlayerInfoScript.Instance.talismanUnlocked[count])
            {
                found = count;
                break;
            }

            count = (count + 1) % ListOfTalismans.Count;
        }
        if (found == index)
            if (!ListOfTalismans[found].activeSelf && PlayerInfoScript.Instance.talismanUnlocked[found])
                ListOfTalismans[currentlyActive].SetActive(true);

        return found;
    }
    public void PlayerDeathEvent()
    {
        Dead = true;
    }
    public void PlayerReviveEvent()
    {
        Dead = false;
    }
}
