using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shopkeeper : MonoBehaviour, IInteractable
{
    public static bool shopping = false;
    private GameObject player;
    private PlayerInfoScript playerStats;
    private ShopkeeperMovement skm;

    public int locks = 0;
    [SerializeField] private GameObject chains;

    [Header("Shop Items")]
    [SerializeField] private ShopItemScriptableObject[] permanentItemInventory;
    [SerializeField] private ShopItemScriptableObject[] randomItemInventory;
    [SerializeField] private ShopItemScriptableObject[] meleeUpgradeInventory;
    [SerializeField] private ShopItemScriptableObject[] rifleUpgradeInventory;
    [SerializeField] private ShopItemScriptableObject[] rpgUpgradeInventory;
    [SerializeField] private ShopItemScriptableObject[] shotgunUpgradeInventory;


    [SerializeField] private List<ShopItemScriptableObject> currentItems;

    [Header("UI")]
    [SerializeField] private string prompt;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private TextMeshProUGUI[] itemUINames = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] itemUIDescs = new TextMeshProUGUI[3];
    [SerializeField] private TextMeshProUGUI[] itemUICosts = new TextMeshProUGUI[3];
    [SerializeField] private Image[] itemUIThumbnails = new Image[3];
    [SerializeField] private GameObject[] itemUIBlock = new GameObject[3];

    [Header("CurrentWeapon")]
    [SerializeField] private GameObject CurrentWeaponObject;
    [SerializeField] private int CurrentWeaponID;

    public bool Tutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerInfoScript>();
        skm = GetComponent<ShopkeeperMovement>(); 
        shopMenu.SetActive(false);
        chains.SetActive(false);

        //Akil - close shop when player dies
        GameEvents.current.onPlayerDeath += CloseShop;
        //Akil - keep track of current weapon
        GameEvents.current.onWeaponSwap += WeaponSwappedEvent;
        //

        // Set up the shop
        RestockEntireShop();
    }

    public void RestockEntireShop()
    {
        // Check which weapon the player is currently holding
        int wepIndex = 0;

        // Restock permanent items 
        for(int i = 0; i < 3; i++)
        {
            currentItems[i] = permanentItemInventory[i + wepIndex * 3];
        }

        // Restock random items 
        for (int i = 3; i < 7; i++)
        {
            SetRandomItem(i);
        }
    }
    void SetItem(int index1, int index2)
    {
        ShopItemScriptableObject item;

        int cap = randomItemInventory.Length;

        if (playerStats.GetShopCostMultiplier() <= 0.501f)
            cap -= 1;

        // Always get a unique item

        item = randomItemInventory[index2];


        currentItems[index1] = item;
    }
    public void SetRandomItem(int index)
    {
        ShopItemScriptableObject randomItem;

        int cap = randomItemInventory.Length;

        if (playerStats.GetShopCostMultiplier() <= 0.501f)
            cap -= 1;

        // Always get a unique item
        while (true)
        {
            randomItem = randomItemInventory[Random.Range(0, cap)];
            if (!currentItems.Contains(randomItem))
                break;
        }

        currentItems[index] = randomItem;
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

        if (locks > 0)
            return;

        if (Tutorial)
            TextController.Instance.openedShop = true;

        // *** OPEN THE SHOP MENU ***
        if (!shopMenu.activeSelf)
        {
            CurrentWeaponObject = PlayerInfoScript.Instance.wepList.GetActiveWeapon();
            CurrentWeaponID = PlayerInfoScript.Instance.wepList.GetActiveWeaponId();

            GameEvents.current.shopMenuOpened();
            shopping = true;

            switch (CurrentWeaponID)
            {
                case 0:
                    if (rifleUpgradeInventory != null)
                        currentItems[2] = rifleUpgradeInventory[0];
                    else
                        currentItems[2] = permanentItemInventory[2];
                    break;
                case 1:
                    if (rpgUpgradeInventory != null)
                        currentItems[2] = rpgUpgradeInventory[0];
                    else
                        currentItems[2] = permanentItemInventory[2];
                    break;
                case 2:
                    if (shotgunUpgradeInventory != null)
                        currentItems[2] = shotgunUpgradeInventory[0];
                    else
                        currentItems[2] = permanentItemInventory[2];
                    break;
                case 3:
                    if (meleeUpgradeInventory != null)
                        currentItems[2] = meleeUpgradeInventory[0];
                    else
                        currentItems[2] = permanentItemInventory[2];
                    break;
            }

            if (Tutorial)
            {
                SetItem(3, 1);
                SetItem(4, 3);
                SetItem(5, 6);
                SetItem(6, 9);
            }
            RefreshInventory();



            shopMenu.SetActive(true);
            skm.PauseMovement();
        }
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < currentItems.Count; i++)
        {

            int cost = Mathf.RoundToInt(currentItems[i].cost * playerStats.GetShopCostMultiplier());
            itemUINames[i].text = currentItems[i].name;
            itemUIDescs[i].text = currentItems[i].desc;
            itemUICosts[i].text = cost + " XP";
            itemUIThumbnails[i].sprite = currentItems[i].thumbnail;

            if(playerStats.GetExpCount() < cost)
                itemUIBlock[i].SetActive(true);
            else
                itemUIBlock[i].SetActive(false);

            if (CurrentWeaponID == 3)
            {
                if (meleeUpgradeInventory == null)
                    if (i == 2)
                        itemUIBlock[i].SetActive(true);
                if (i == 1)
                    itemUIBlock[i].SetActive(true);
            }
            /*if (currentItems[i].available)
                itemUIBlock[i].SetActive(false);
            else
                itemUIBlock[i].SetActive(true);*/
        }
    }

    public string InteractPrompt()
    {
        return prompt;
    }

    public void ModifyLocks(int amt)
    {
        locks += amt;

        if (locks > 0)
        {
            shopMenu.SetActive(false);
            chains.SetActive(true);
            skm.PauseMovement();
        }
        else
        {
            chains.SetActive(false);
            skm.ContinueMovement();
        }          
    }

    // Update is called once per frame
    void Update()
    {
        // Shop is open
        if (shopMenu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Vector3.Distance(player.transform.position, transform.position) > 9f)
            {
                GameEvents.current.shopMenuClosed();
                CloseShop();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    BuyItem(0);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    BuyItem(1);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    BuyItem(2);
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    BuyItem(3);
                if (Input.GetKeyDown(KeyCode.Alpha5))
                    BuyItem(4);
                if (Input.GetKeyDown(KeyCode.Alpha6))
                    BuyItem(5);
                if (Input.GetKeyDown(KeyCode.Alpha7))
                    BuyItem(6);
            }
        }        
    }

    public void BuyItem(int index)
    {
        int cost = Mathf.RoundToInt(currentItems[index].cost * playerStats.GetShopCostMultiplier());
        // Can the player afford this item?
        if (playerStats.GetExpCount() >= cost)
        {

            if (CurrentWeaponID == 3 && meleeUpgradeInventory == null && index == 2)
                return;
            if (CurrentWeaponID == 3 && index == 1)
                return;

            Debug.Log("BUYING: " + currentItems[index].name);
            playerStats.SpendExp(cost);
            currentItems[index].Equip(player);

            if (index == 2)
            {
                switch (CurrentWeaponID)
                {
                    case 0:
                        ResizeArray(ref rifleUpgradeInventory);
                        break;
                    case 1:
                        ResizeArray(ref rpgUpgradeInventory);
                        
                        break;
                    case 2:
                        ResizeArray(ref shotgunUpgradeInventory);
                        
                        break;
                    case 3:
                        ResizeArray(ref meleeUpgradeInventory);
                        
                        break;
                }
            }
            // Random item was bought and must be replaced
            if(index >= 3 && !Tutorial)
                SetRandomItem(index);

            RefreshInventory();
        }
        else
        {
            Debug.Log("YOU ARE TOO POOR TO BUY THIS");
        }


        /*if (itemData[index].available)
        {
            Debug.Log("BUYING: " + itemData[index].name);
            itemData[index].available = false;
            itemUIBlock[index].SetActive(true);
        }
        else
            Debug.Log("CANNOT BUY: " + itemData[index].name + " (SOLD OUT)");*/
    }

    public void CloseShop()
    {
        if (shopMenu.activeSelf)
        {
            shopping = false;
            shopMenu.SetActive(false);
            skm.ContinueMovement();

            if (Tutorial)
                TextController.Instance.spokenToShop = true;
        }
    }

    void ResizeArray(ref ShopItemScriptableObject[] upgradeInventory)
    {
        if (upgradeInventory != null)
        {
            if (upgradeInventory.Length > 1)
            {
                ShopItemScriptableObject[] resize = new ShopItemScriptableObject[upgradeInventory.Length - 1];
                for (int i = 1; i < upgradeInventory.Length; i++)
                    resize[i - 1] = upgradeInventory[i];

                upgradeInventory = resize;
                currentItems[2] = upgradeInventory[0];

            }
            else if (upgradeInventory.Length == 1)
            {
                upgradeInventory = null;
                currentItems[2] = permanentItemInventory[2];
            }
        }
    }

    //Akil - tell shop that equipped weapon changed
    /*
    Weapon  -   ID

    Rifle   -   0
    RPG     -   1
    Shotgun -   2
    Melee   -   3
     */
    public void WeaponSwappedEvent()
    {
        CurrentWeaponObject = PlayerInfoScript.Instance.wepList.GetActiveWeapon();
        CurrentWeaponID = PlayerInfoScript.Instance.wepList.GetActiveWeaponId();


        switch (CurrentWeaponID)
        {
            case 0:
                if (rifleUpgradeInventory != null)
                    currentItems[2] = rifleUpgradeInventory[0];
                else
                    currentItems[2] = permanentItemInventory[2];

                RefreshInventory();
                break;
            case 1:
                if (rpgUpgradeInventory != null) 
                    currentItems[2] = rpgUpgradeInventory[0];               
                else
                    currentItems[2] = permanentItemInventory[2];
                RefreshInventory();
                break;
            case 2:
                if (shotgunUpgradeInventory != null)
                    currentItems[2] = shotgunUpgradeInventory[0];
                else
                    currentItems[2] = permanentItemInventory[2];

                RefreshInventory();
                break;
            case 3:
                if (meleeUpgradeInventory != null)
                    currentItems[2] = meleeUpgradeInventory[0];
                else
                    currentItems[2] = permanentItemInventory[2];

                RefreshInventory();
                break;
        }
    }
    //
}
