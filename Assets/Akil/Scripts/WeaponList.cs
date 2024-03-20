using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{

    /*
    Weapon  -   ID

    Rifle   -   0
    RPG     -   1
    Shotgun -   2
    Melee   -   3
    */

    public List<GameObject> ListOfWeapons;
    
    [SerializeField] GameObject shopMenu;
    public bool Tutorial = false;
    private int currentlyActive = 0;
    private bool Dead = false;
    private int cycleIndex = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;
        shopMenu = GameObject.Find("ShopCanvas");
        if (!Tutorial && !PlayerInfoScript.Instance.onIntro)
            ListOfWeapons[0].SetActive(true);
        else
            ListOfWeapons[0].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            if (!shopMenu.activeSelf)
            {
                if (Input.GetKeyDown(PlayerInfoScript.Instance.IDToKeyCodes[0]) && PlayerInfoScript.Instance.weaponUnlocked[0])
                    SetActiveWeapon(0);
                else if (Input.GetKeyDown(PlayerInfoScript.Instance.IDToKeyCodes[1]) && PlayerInfoScript.Instance.weaponUnlocked[1])
                    SetActiveWeapon(1);
                else if (Input.GetKeyDown(PlayerInfoScript.Instance.IDToKeyCodes[2]) && PlayerInfoScript.Instance.weaponUnlocked[2])
                    SetActiveWeapon(2);
                else if (Input.GetKeyDown(PlayerInfoScript.Instance.IDToKeyCodes[3]) && PlayerInfoScript.Instance.weaponUnlocked[3])
                    SetActiveWeapon(3);
            }
            else
            {
                if (Input.GetKeyDown(PlayerInfoScript.Instance.shopWeaponToggleRight))
                {
                    cycleIndex = GetActiveWeaponId();

                    cycleIndex++;
                    if (cycleIndex > 3)
                        cycleIndex = 0;
                    if (PlayerInfoScript.Instance.weaponUnlocked[cycleIndex])
                    {
                        SetActiveWeapon(cycleIndex);
                        GameEvents.current.weaponSwapped(cycleIndex);
                    }
                    else
                    {
                        cycleIndex = FindNextAvailableRight(cycleIndex - 1);
                        SetActiveWeapon(cycleIndex);
                        GameEvents.current.weaponSwapped(cycleIndex);
                    }

                }
                else if (Input.GetKeyDown(PlayerInfoScript.Instance.shopWeaponToggleLeft))
                {
                    cycleIndex = GetActiveWeaponId();

                    cycleIndex--;
                    if (cycleIndex < 0)
                        cycleIndex = 3;
                    if (PlayerInfoScript.Instance.weaponUnlocked[cycleIndex])
                    {
                        SetActiveWeapon(cycleIndex);
                        GameEvents.current.weaponSwapped(cycleIndex);
                    }
                    else
                    {
                        cycleIndex = FindNextAvailableLeft(cycleIndex - 1);
                        SetActiveWeapon(cycleIndex);
                        GameEvents.current.weaponSwapped(cycleIndex);
                    }
                }
            }
        }
    }
    private int FindNextAvailableLeft(int index)
    {
        int found = index;
        int count;
        if (index - 1 >= 0)
            count = index - 1;
        else
            count = 3;
        while (count != index)
        {
            if (PlayerInfoScript.Instance.weaponUnlocked[count])
            {
                found = count;
                break;
            }

            if (count - 1 < 0)
                count = 4;
            else
                count = count - 1 % 4;
        }

        return found;
    }
    private int FindNextAvailableRight(int index)
    {
        int found = index;
        int count = index+1;
        while (count != index)
        {
            if (PlayerInfoScript.Instance.weaponUnlocked[count])
            {   found = count;
                break;
            }

            count = count + 1 % 4;
        }

       return found;
    }
    public void GlobalDamageIncrease(float value)
    {
        for (int i = 0; i < ListOfWeapons.Count; i++)
            ListOfWeapons[i].GetComponent<WeaponScript>().IncreaseDamage(value);
    }
    public void SetActiveWeapon(int value)
    {
        if (currentlyActive != value || !ListOfWeapons[currentlyActive].activeSelf)
        {

            ListOfWeapons[currentlyActive].SetActive(false);
            currentlyActive = value;
            ListOfWeapons[currentlyActive].SetActive(true);

            //If switching to melee, attack
            if (currentlyActive == 3)
                ListOfWeapons[currentlyActive].GetComponent<MeleeScript>().TryAttack();
        }
    }
    public GameObject GetActiveWeapon()
    {
        return ListOfWeapons[currentlyActive];
    }

    public int GetActiveWeaponId()
    {
        return currentlyActive;
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
