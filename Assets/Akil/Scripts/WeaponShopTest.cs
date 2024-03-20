using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class WeaponShopTest : MonoBehaviour, IInteractable
{
    struct TemporaryItemData
    {
        public string name;
        public string desc;
        public int cost;
        public bool available;

        //Akil - boolean to check if the shop item is a global bonus for all weapons or not
        public bool global;
        public Action<float> FunctionPointer;   //Action<float> meaning this will hold a function pointer to any function that returns void and takes a float as parameter
        public float FunctionParameterFloat;    //This will hold the variable to be sent to the function
        //

        public TemporaryItemData(string n, string d, int c, Action<float> action, float f, bool g)
        {
            name = n;
            desc = d;
            cost = c;
            available = true;
            FunctionPointer = action;
            FunctionParameterFloat = f;
            global = g;
        }

        //Akil - Call any function that returns void but takes one float as a parameter
        public void CallFunc(Action<float> action, float p) => action(p);
        //
    }

    [SerializeField] private TemporaryItemData[] itemData;
    [SerializeField] private string prompt;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private TextMeshProUGUI[] itemUINames = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] itemUIDescs = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] itemUICosts = new TextMeshProUGUI[3];
    [SerializeField] private GameObject[] itemUIBlock = new GameObject[3];

    //Akil - holder for the current active weapon object
    private GameObject CurrentWeapon;
    //

    private ShopkeeperMovement skm;

    // Start is called before the first frame update
    void Start()
    {
        //Akil - Get current weapon. This is a placeholder so that the array of ItemData can be created for now
        CurrentWeapon = Camera.main.GetComponent<WeaponList>().GetActiveWeapon();
        //

        //Akil - constructor
        itemData = new TemporaryItemData[3] { new TemporaryItemData("Damage", "50% Increase to Weapon Damage of All Weapons", 500, Camera.main.GetComponent<WeaponList>().GlobalDamageIncrease, 0.5f, true), new TemporaryItemData("Fire Rate", "25% Increase to Fire Rate of Current Weapon", 9999, CurrentWeapon.GetComponent<WeaponScript>().IncreaseFireRate, 0.25f, false), new TemporaryItemData("Fire Rate", "25% Increase to Fire Rate of Current Weapon", 200, CurrentWeapon.GetComponent<WeaponScript>().IncreaseFireRate, 0.25f, false) };
        //

        skm = GetComponent<ShopkeeperMovement>(); 
        shopMenu.SetActive(false);

        // Set up the shop
        
    }

    public void HoverOut()
    {
        //Debug.Log("Goodbye, shopkeeper.");
    }

    public void HoverOver()
    {
        //Debug.Log("This is the shopkeeper.");
    }

    public void Interact()
    {
        //Debug.Log("You are buying something?");

        // *** OPEN THE SHOP MENU ***
        if (!shopMenu.activeSelf)
        {

            //Akil - Get the currently active weapon at the time of opening the shop - is this what we want?
            CurrentWeapon = Camera.main.GetComponent<WeaponList>().GetActiveWeapon();
            //

            for (int i = 0; i < itemData.Length; i++)
            {
                itemUINames[i].text = itemData[i].name;
                itemUIDescs[i].text = itemData[i].desc;
                itemUICosts[i].text = itemData[i].cost.ToString() + " XP";

                //Akil - If this isn't a global bonus, change the function pointer to the current weapon, and the appropriate function
                if (!itemData[i].global)
                {   if (itemData[i].name == "Fire Rate")
                        itemData[i].FunctionPointer = CurrentWeapon.GetComponent<WeaponScript>().IncreaseFireRate;
                }
                //

                if (itemData[i].available)
                    itemUIBlock[i].SetActive(false);
                else
                    itemUIBlock[i].SetActive(true);
            }

            shopMenu.SetActive(true);
            skm.PauseMovement();
        }
    }

    public string InteractPrompt()
    {
        return prompt;
    }

    // Update is called once per frame
    void Update()
    {
        // Shop is open
        if (shopMenu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                shopMenu.SetActive(false);
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    BuyItem(0);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    BuyItem(1);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    BuyItem(2);
            }
        }        
    }

    public void BuyItem(int index)
    {
        if (itemData[index].available)
        {
            Debug.Log("BUYING: " + itemData[index].name);
            //itemData[index].available = false;
            //itemUIBlock[index].SetActive(true);

            //Akil - When bought, call the shop item's function and do whatever was described (increase damage, increase fire rate, etc)
            itemData[index].CallFunc(itemData[index].FunctionPointer,itemData[index].FunctionParameterFloat);
            //
        }
        else
            Debug.Log("CANNOT BUY: " + itemData[index].name + " (SOLD OUT)");
    }

    public void CloseShop()
    {
        if (shopMenu.activeSelf)
        {
            shopMenu.SetActive(false);
            skm.ContinueMovement();
        }
    }
}
