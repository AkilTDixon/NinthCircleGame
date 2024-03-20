using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;
using Random = UnityEngine.Random;

public class PlayerInfoScript : MonoBehaviour, IDamageable
{

    //Singleton
    public static PlayerInfoScript Instance { get; private set; }

    public static bool easyMode = false;

    [Header("References")]
    //Keys used for each weapon
    /*
     Defaults
        Rifle   : KeyCode.Alpha1
        RPG     : KeyCode.Alpha2
        Shotgun : KeyCode.Alpha3
        Melee   : KeyCode.V
     */
    public WeaponList wepList;
    public TalismanList talList;
    public Camera mainCam;
    public GameObject shopMenu;
    public GameObject ChargingBox;
    public List<GameObject> TalismanDropItems;
    public GameObject theFloorObject;
    public GameObject SpawnObject;

    [Header("Keybinds")]
    [Tooltip("Element 0: Rifle | Element 1: RPG | Element 2: Shotgun | Element 3: Melee")]
    public List<KeyCode> IDToKeyCodes = new List<KeyCode>() { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.V };
    public KeyCode activateTalismanKey = KeyCode.Q;
    public KeyCode cycleTalismanKey = KeyCode.Tab;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode shopWeaponToggleRight = KeyCode.E;
    public KeyCode shopWeaponToggleLeft = KeyCode.Q;
    public KeyCode shopInteract = KeyCode.E;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode secondaryFireKey = KeyCode.Mouse1;

    private List<KeyCode> IDToKeyCodes_Default = new List<KeyCode>() { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.V };
    private KeyCode activateTalismanKey_Default = KeyCode.Q;
    private KeyCode cycleTalismanKey_Default = KeyCode.Tab;
    private KeyCode jumpKey_Default = KeyCode.Space;
    private KeyCode dashKey_Default = KeyCode.LeftShift;
    private KeyCode shopWeaponToggleRight_Default = KeyCode.E;
    private KeyCode shopWeaponToggleLeft_Default = KeyCode.Q;
    private KeyCode shopInteract_Default = KeyCode.E;
    private KeyCode reloadKey_Default = KeyCode.R;
    private KeyCode shootKey_Default = KeyCode.Mouse0;
    private KeyCode secondaryFireKey_Default = KeyCode.Mouse1;

    [Header("Health")]
    [SerializeField] private int hp = 100;
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int shieldHp = 0;
    [SerializeField] private int maxShieldHp = 0;

    [Header("Experience System")]
    [SerializeField] private int expAmount = 0;
    [SerializeField] int NumberOfContinues = 3;

    [Header("Stat Multipliers")]
    [SerializeField] private float defense = 1;
    [SerializeField] private float speed = 1;
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float expMultiplier = 1;
    [SerializeField] private float shopCostMultiplier = 1;
    public float speedBuffAdjustAbsolute = 0f; //set by buffs

    [Header("Weapon Stats")]
    [SerializeField] private List<int> AmmoPerID;
    [SerializeField] private List<int> AmmoBaseCapPerID;
    [SerializeField] private List<int> AmmoClipPerID;
    [SerializeField] private int RPGAmmo = 4;
    [SerializeField] private int RPGAmmoCap = 4;
    [SerializeField] private int RPGAmmoClip = 4;
    [SerializeField] private int RPGBaseAmmoCap = 4;
    [SerializeField] private int RifleAmmo = 60;
    [SerializeField] private int RifleAmmoCap = 180;
    [SerializeField] private int RifleBaseAmmoCap = 180;
    [SerializeField] private int RifleAmmoClip = 60;
    [SerializeField] private int ShotgunAmmo = 12;
    [SerializeField] private int ShotgunAmmoCap = 48;
    [SerializeField] private int ShotgunBaseAmmoCap = 48;
    [SerializeField] private int ShotgunAmmoClip = 12;


    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currentHPText;
    [SerializeField] private TextMeshProUGUI maxHPText;
    [SerializeField] private GameObject ShieldElementsHolder;
    [SerializeField] private TextMeshProUGUI currentShieldHPText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI expMultiplierText;
    [SerializeField] private TextMeshProUGUI currentWeaponText;
    [SerializeField] private TextMeshProUGUI currentWeaponAmmoText;
    [SerializeField] private TextMeshProUGUI maxCurrentWeaponAmmoText;
    [SerializeField] private TextMeshProUGUI RPGLabelText;
    [SerializeField] private TextMeshProUGUI RPGAmmoText;
    [SerializeField] private TextMeshProUGUI maxRPGAmmoText;
    [SerializeField] private TextMeshProUGUI RifleLabelText;
    [SerializeField] private TextMeshProUGUI RifleAmmoText;
    [SerializeField] private TextMeshProUGUI maxRifleAmmoText;
    [SerializeField] private TextMeshProUGUI ShotgunLabelText;
    [SerializeField] private TextMeshProUGUI ShotgunAmmoText;
    [SerializeField] private TextMeshProUGUI maxShotgunAmmoText;
    [SerializeField] private GameObject ReloadContainer;
    [SerializeField] private TextMeshProUGUI ReloadText;
    [SerializeField] private TextMeshProUGUI ReloadKeyText;
    [SerializeField] private GameObject DamagePaneContainer;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private TextMeshProUGUI ContinueText;
    public GameObject TalismanPanelHolder;
    public TextMeshProUGUI TalismanNameText;
    public TextMeshProUGUI TalismanDescriptionText;
    public GameObject WaveElementsHolder;
    public GameObject bossElementsHolder;
    public TextMeshProUGUI bossNameText;
    public GameObject crosshair;
    
    public GameObject CooldownElementsHolder;
    public TextMeshProUGUI currentCooldownText;
    [SerializeField] GameObject ChargeUIEffect;
    
    [Header("Intro/Outro")]
    [SerializeField] private FadeInOut fadeInOutScript;
    [SerializeField] private Transform startPosition;
    [SerializeField] private PathCreator pathIntro;
    [SerializeField] private float pathY = 7.0f;
    [SerializeField] private float pathSpeed = 1f;
    [SerializeField] public TextMeshProUGUI textIntoOutro;
    public bool onIntro = true;
    public bool finalBossKilled = false;
    private float distanceAlongPath = 0f;
    private bool introStartRotationSet = false;
    private PathCreator currentPath;
    private GameObject gameUI;
    private GameObject glorySystem;
    public GameObject uiWaveIndicator;
    public GameObject uiWaveInfo;
    public GameObject uiLevelIndicator;
    public bool Tutorial = false;


    [Header("Other")]

    float hpFlickerAmt = 1f;
    float hpFlickerCap = 1f;
    
    private bool fadeInStarted = false;

    public bool GodMode = false;
    public bool finalBossIntro = false;

    
    // OTHERS WILL GO HERE

    private int CurrentWeaponClipSize = 0;
    private int CurrentWeaponBaseAmmoCap = 0;
    private int ammoCount = 0;
    private int ammoCap = 0;
    public GameObject PlayerObject;
    public PlayerMovement movementScript;
    public Transform player;
    public bool Dead = false;
    // public bool win = false;

    /*
    Weapon  -   ID

    Rifle   -   0
    RPG     -   1
    Shotgun -   2
    Melee   -   3
    */
    public bool[] weaponUnlocked = { true, false, false, true }; //rifle is unlocked, melee is unlocked

    /*

    Talisman    -   ID

    Shield      -   0
    Heal        -   1
    Charge      -   2          
    Grapple     -   3
     */
    public bool[] talismanUnlocked = { true, true, false, false };
    [HideInInspector] public bool ChargeActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.Log("Destroying extra instance of PlayerInfoScript (This is a Singleton!)");
            return;
        }

        Instance = this;
        PlayerObject = GameObject.Find("PlayerContainer/Player");
        player = PlayerObject.GetComponent<Transform>();
        movementScript = PlayerObject.GetComponent<PlayerMovement>();
        mainCam = Camera.main;
        shopMenu = GameObject.Find("ShopCanvas");
        
    }
    void Start()
    {
        GameEvents.current.onPlayerHit += TakeDamage;
        GameEvents.current.onWeaponUnlock += WeaponUnlocked;
        GameEvents.current.onChargeActivated += ChargeActivated;
        GameEvents.current.onChargeDeactivated += ChargeDeactivated;
        hp = 100;
        maxHp = 100;
        for (int i = 0; i < wepList.ListOfWeapons.Count; i++)
        {
            WeaponScript ws = wepList.ListOfWeapons[i].GetComponent<WeaponScript>();
            InitializeAmmoInfo(i, ws.GetAmmoCount(), ws.GetAmmoCapCount());
            AmmoPerID.Add(ws.GetAmmoCount());
            AmmoBaseCapPerID.Add(ws.GetAmmoCapCount());
            AmmoClipPerID.Add(ws.GetAmmoCount());
        }

        LoadSavedKeys();

        textIntoOutro.enabled = false;
        
        gameUI = GameObject.Find("GameplayUI");
        glorySystem = GameObject.Find("GlorySystem");
        if (onIntro) {
            currentPath = pathIntro;
            textIntoOutro.text = "Wake up....";
            textIntoOutro.enabled = true;
            //todo disable UI elements here
            hideUIElementsForIntroOrOutro();
            
        }

    }


    
    public void Update()
    {

        if (onIntro) {
            handleIntroOutroSteps();
            return;
        }

        if (finalBossKilled) {
            return;
        }

        // if (win) { // if win bool is turned on
        //     Win();
        // }


        
        if ((float)hp / maxHp <= 0.25f)
        {
            if (hpFlickerAmt < 0)
                hpFlickerAmt = hpFlickerCap;

            hpFlickerAmt -= Time.deltaTime * 1.2f;
            currentHPText.color = new Color(hpFlickerAmt, 0, 0);
        }
        else
            currentHPText.color = Color.white;

        currentHPText.text = hp.ToString();
        maxHPText.text = maxHp.ToString();
        expText.text = expAmount.ToString() + " XP";
        expMultiplierText.text = "XP MULTIPLIER: <color=orange>x" + expMultiplier.ToString("F2") + "</color>";

        if (shieldHp > 0)
        {
            if (!ShieldElementsHolder.activeSelf)
                ShieldElementsHolder.SetActive(true);

            currentShieldHPText.text = shieldHp.ToString();
        }
        else if (ShieldElementsHolder.activeSelf)
            ShieldElementsHolder.SetActive(false);


        /*
        Weapon  -   ID

        Rifle   -   0
        RPG     -   1
        Shotgun -   2
        Melee   -   3


        Keys to ID should be dynamic and not decided here. Pressing 1 shouldn't necessarily be ID 0
        */

        switch (wepList.GetActiveWeaponId())
        {
            case 0:
                ammoCount = RifleAmmo;
                ammoCap = RifleAmmoCap;

                currentWeaponText.text = "Rifle";
                currentWeaponAmmoText.text = ammoCount.ToString();
                maxCurrentWeaponAmmoText.text = ammoCap.ToString();
                CurrentWeaponClipSize = RifleAmmoClip;
                CurrentWeaponBaseAmmoCap = RifleBaseAmmoCap;
                break;
            case 1:
                ammoCount = RPGAmmo;
                ammoCap = RPGAmmoCap;

                currentWeaponText.text = "RPG";
                currentWeaponAmmoText.text = ammoCount.ToString();
                maxCurrentWeaponAmmoText.text = "0";
                CurrentWeaponClipSize = RPGAmmoClip;
                CurrentWeaponBaseAmmoCap = RPGBaseAmmoCap;
                break;
            case 2:
                ammoCount = ShotgunAmmo;
                ammoCap = ShotgunAmmoCap;

                currentWeaponText.text = "Shotgun";
                currentWeaponAmmoText.text = ammoCount.ToString();
                maxCurrentWeaponAmmoText.text = ammoCap.ToString();
                CurrentWeaponClipSize = ShotgunAmmoClip;
                CurrentWeaponBaseAmmoCap = ShotgunBaseAmmoCap;
                break;
            case 3:
                currentWeaponText.text = "Melee";
                currentWeaponAmmoText.text = "∞";
                maxCurrentWeaponAmmoText.text = "∞";
                break;
        }
        if (currentWeaponText.text != "Melee")
        {
            if (ammoCount <= (CurrentWeaponClipSize * 0.2f))
            {
                ReloadContainer.SetActive(true);
                if (maxCurrentWeaponAmmoText.text != "0")
                {                 
                    ReloadKeyText.text = reloadKey.ToString();
                    ReloadText.text = "<color=orange>RELOAD</color>";
                }
                else
                {
                    ReloadKeyText.text = reloadKey.ToString();
                    ReloadText.text = "<color=red>CANNOT RELOAD</color>";
                }
            }
            else if (ammoCount > (CurrentWeaponClipSize * 0.2f) && ReloadContainer.activeSelf)
                ReloadContainer.SetActive(false);

        }
        else if (ReloadContainer.activeSelf)
            ReloadContainer.SetActive(false);


        RifleAmmoText.text = RifleAmmo.ToString();
        maxRifleAmmoText.text = RifleAmmoCap.ToString();

        if (weaponUnlocked[1])
            RPGAmmoText.text = RPGAmmo.ToString();


        if (weaponUnlocked[2])
        {
            ShotgunAmmoText.text = ShotgunAmmo.ToString();
            maxShotgunAmmoText.text = ShotgunAmmoCap.ToString();
        }
    }

    private void handleIntroOutroSteps() {
        if (!fadeInStarted) {
            fadeInOutScript.FadeIn();
            fadeInStarted = true;
        }
        
        if (currentPath != null) {
            distanceAlongPath += pathSpeed * Time.deltaTime;
            float percentComplete = distanceAlongPath / currentPath.path.length;
            Vector3 pos = currentPath.path.GetPointAtDistance(distanceAlongPath);
            pos.y = pathY;
            if (percentComplete < 1f)
                player.position = pos;
            if (!introStartRotationSet) {
                introStartRotationSet = true;
                GameObject demonBoss = GameObject.Find("DemonBossChair");
                if (demonBoss != null)
                    player.rotation = Quaternion.LookRotation(demonBoss.GetComponent<Transform>().position);
            }
            
            // transform.rotation = pla
            // transform.rotation = currentPath.path.GetRotationAtDistance(distanceAlongPath);

            
            switch (percentComplete) {
                case >=1:
                    textIntoOutro.text = "This is your moment!  Begin!";
                    StartCoroutine(hideIntroText());
                    endIntroAndStartGame();
                    break;
                case >= 0.95f:
                    textIntoOutro.text = "Get Ready....";
                    break;
                case >= 0.75f:
                    textIntoOutro.text = "The crowd wants blood...  Give them what they want and you may survive...";
                    break;
                case >= 0.5f:
                    textIntoOutro.text = "This arena has seen countless souls suffer...";
                    break;
                case >= 0.25f:
                    textIntoOutro.text = "This is where you will prove yourself... Worthy or Unworthy...";
                    break;
                default:
                    textIntoOutro.text = "Welcome....  Look around....";
                    break;
            }
        }

        IEnumerator hideIntroText() {
            yield return new WaitForSeconds(3);
            textIntoOutro.enabled = false;
        }
            
    }
    
    public void hideUIElementsForIntroOrOutro() {
        weaponUnlocked[0] = false;
        weaponUnlocked[3] = false;
        wepList.GetActiveWeapon().SetActive(false);
        ReloadContainer.SetActive(false);
        gameUI.SetActive(false);
        glorySystem.SetActive(false);
        SpawnObject.SetActive(false);
        crosshair.SetActive(false);
    }

    public void endIntroAndStartGame() {
        onIntro = false;
        GetComponent<PlayerMovement>().rb.isKinematic = false;
        weaponUnlocked[0] = true;
        weaponUnlocked[3] = true;
        ReloadContainer.SetActive(true);
        wepList.SetActiveWeapon(0); //equip the rifle
        
        gameUI.SetActive(true);
        glorySystem.SetActive(true);
        SpawnObject.SetActive(true);
        crosshair.SetActive(true);
        
        uiWaveIndicator.SetActive(true);
        uiWaveInfo.SetActive(true);
        uiLevelIndicator.SetActive(true);

    }
    public void SetShield(int value)
    {
        shieldHp = value;
        maxShieldHp = value;
    }
    public int GetShieldHP()
    {
        return shieldHp;
    }
    public void HealShield(int heal)
    {
        shieldHp += heal;

        if (shieldHp > maxShieldHp)
            shieldHp = maxShieldHp;
    }
    public void Heal(int heal)
    {
        hp += heal;

        if (hp > maxHp)
            hp = maxHp;
    }
    public void ChargeActivated(int damage)
    {
        ChargeActive = true;
        ChargeUIEffect.SetActive(true);
        ChargingBox.GetComponent<ChargeDamageBox>().Damage = damage;
        ChargingBox.GetComponent<ChargeDamageBox>().enabled = true;
    }
    public void ChargeDeactivated()
    {
        ChargeActive = false;
        ChargeUIEffect.SetActive(false);
        ChargingBox.GetComponent<ChargeDamageBox>().enabled = false;
    }
    public void TakeDamage(int damage)
    {
        if (finalBossIntro)
        {
            GameEvents.current.shieldAbsorb();
            return;
        }
        if (easyMode)
            damage = Mathf.FloorToInt(damage * 0.5f);

        if (ChargeActive)
        {
            damage = 0;
            GameEvents.current.shieldAbsorb();
        }

        if (shieldHp > 0)
        {
            int newShieldVal = shieldHp - damage;
            damage -= shieldHp;
            if (damage < 0)
            {
                damage = 0;
                GameEvents.current.shieldAbsorb();
            }
            if (newShieldVal <= 0)
            {
                shieldHp = 0;
                GameEvents.current.shieldBroken();
            }
            else
            {
                shieldHp = newShieldVal;
            }
        }
        
        hp -= (int)(damage / defense);

        if (hp <= 0)
        {

            hp = 0;
            if (!Dead)
                Death();
        }
    }

    private IEnumerator ContinueDelay(int seconds)
    {

        for (int i = seconds; i > 0; i--)
        { ContinueText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        ContinueText.text = "CONTINUE";
        ContinueButton.interactable = true;

    }
    public void Death()
    {
        WaveElementsHolder.SetActive(false);
        Dead = true;
        GameEvents.current.playerDeath();
        Debug.Log("YOU ARE DEAD.");
        expAmount = 0;
        GameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (easyMode)
            NumberOfContinues = 1;

        if (NumberOfContinues > 0)
        {
            NumberOfContinues--;
            StartCoroutine(ContinueDelay(3));
        }
        else
            ContinueText.text = "";


    }

    // public void Win()
    // {
    //     WaveElementsHolder.SetActive(false);
    //     win = true;
    //     GameEvents.current.playerDeath();
    //     Debug.Log("PLAYER WON");
    //     GameWinPanel.SetActive(true);
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    // }

    public void goToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void Revive()
    {
        WaveElementsHolder.SetActive(true);
        ContinueButton.interactable = false;
        GameOverPanel.SetActive(false);
        GameEvents.current.playerRevive();
        hp = maxHp;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Dead = false;

        if (easyMode)
        {
            for (int i = 0; i < weaponUnlocked.Length; i++)
                if (weaponUnlocked[i])
                {
                    switch (i)
                    {
                        case 0:
                            MaximizeWeaponAmmo(0, GetRifleClipSize(), GetRifleBaseCap());
                            break;
                        case 1:
                            MaximizeWeaponAmmo(1, GetRPGClipSize(), GetRPGBaseCap());
                            break;
                        case 2:
                            MaximizeWeaponAmmo(2, GetShotgunClipSize(), GetShotgunBaseCap());
                            break;
                    }
                }
        }
    }
    void InitializeAmmoInfo(int id, int ammoCount, int ammoCap)
    {
        switch (id)
        {
            case 0:
                //Rifle
                RifleAmmo = ammoCount;
                RifleAmmoCap = ammoCap;
                RifleBaseAmmoCap = ammoCap;
                break;
            case 1:
                //RPG
                RPGAmmo = ammoCount;
                RPGAmmoCap = ammoCap;
                RPGBaseAmmoCap = ammoCap;
                break;
            case 2:
                //Shotgun
                ShotgunAmmo = ammoCount;
                ShotgunAmmoCap = ammoCap;
                ShotgunBaseAmmoCap = ammoCap;
                break;
            case 3:
                //Melee
                break;
        }
    }

    public void IncreaseAmmoInfo(int id, int ammoAmt)
    {
        WeaponScript ws = wepList.ListOfWeapons[id].GetComponent<WeaponScript>();
        switch (id)
        {
            case 0:
                //Rifle
                RifleAmmo += ammoAmt;
                RifleAmmoClip += ammoAmt;
                RifleAmmoCap += ammoAmt * 3;
                RifleBaseAmmoCap += ammoAmt * 3;

                ws.SetAmmo(RifleAmmo, RifleAmmoCap, RifleAmmoClip);

                break;
            case 1:
                //RPG
                RPGAmmo += ammoAmt;
                RPGAmmoClip += ammoAmt;

                ws.SetAmmo(RPGAmmo, RPGAmmoCap, RPGAmmoClip);
                break;
            case 2:
                //Shotgun
                ShotgunAmmo += ammoAmt;
                ShotgunAmmoClip += ammoAmt;
                ShotgunAmmoCap += ammoAmt * 3;
                ShotgunBaseAmmoCap += ammoAmt * 3;

                ws.SetAmmo(ShotgunAmmo, ShotgunAmmoCap, ShotgunAmmoClip);
                break;
            case 3:
                //Melee
                break;
        }
    }
    public void SetAmmoInfo(int id, int ammoCount, int ammoCap)
    {
        switch(id)
        {
            case 0:
                //Rifle
                RifleAmmo = ammoCount;
                RifleAmmoCap = ammoCap;
                break;
            case 1:
                //RPG
                RPGAmmo = ammoCount;
                RPGAmmoCap = ammoCap;
                break;
            case 2:
                //Shotgun
                ShotgunAmmo = ammoCount;
                ShotgunAmmoCap = ammoCap;
                break;
            case 3:
                //Melee
                break;
        }
    }

    //--------------------
    //RPG methods
    public int GetRPGAmmoCount()
    {
        return RPGAmmo;
    }
    public int GetRPGAmmoCap()
    {
        return RPGAmmoCap;
    }
    public int GetRPGBaseCap()
    {
        return RPGBaseAmmoCap;
    }
    public int GetRPGClipSize()
    {
        return RPGAmmoClip;
    }

    //Rifle methods
    public int GetRifleAmmoCount()
    {
        return RifleAmmo;
    }
    public int GetRifleAmmoCap()
    {
        return RifleAmmoCap;
    }
    public int GetRifleBaseCap()
    {
        return RifleBaseAmmoCap;
    }
    public int GetRifleClipSize()
    {
        return RifleAmmoClip;
    }

    //Shotgun methods
    public int GetShotgunAmmoCount()
    {
        return ShotgunAmmo;
    }
    public int GetShotgunAmmoCap()
    {
        return ShotgunAmmoCap;
    }
    public int GetShotgunBaseCap()
    {
        return ShotgunBaseAmmoCap;
    }
    public int GetShotgunClipSize()
    {
        return ShotgunAmmoClip;
    }
    //---------------


    public int GetCurrentBaseAmmoCap()
    {
        return CurrentWeaponBaseAmmoCap;
    }
    public int GetCurrentAmmoClipSize()
    {
        return CurrentWeaponClipSize;
    }
    public int GetCurrentAmmoCapCount()
    {
        return int.Parse(maxCurrentWeaponAmmoText.text);
    }
    public int GetCurrentAmmoCount()
    {
        return int.Parse(currentWeaponAmmoText.text);
    }
    public int GetExpCount()
    {
        return expAmount;
    }

    /// <summary>
    /// STAT CHANGES
    /// </summary>

    public float GetSpeed()
    {
        return speed;
    }

    public float GetJumpForce()
    {
        return jumpForce;
    }

    public float GetShopCostMultiplier()
    {
        return shopCostMultiplier;
    }

    public void ChangeMaxHP(int value)
    {
        maxHp += value;
        hp += value;
        if (hp <= 0) // decrease of max p
            hp = 1;



    }

    public void ChangeDefense(float value)
    {
        defense += value;
    }

    public void ChangeSpeed(float value)
    {
        speed += value;
    }

    public void ChangeJumpForce(float value)
    {
        jumpForce += value;
    }

    public void ChangeExpMultiplier(float value)
    {
        expMultiplier += value;
    }

    public void ChangeShopCostMultiplier(float value)
    {
        shopCostMultiplier += value;
    }

    public void ChangeExp(int value)
    {
        expAmount += Mathf.RoundToInt(value * expMultiplier);
    }

    public void SpendExp(int value)
    {
        expAmount -= value;
    }

    public void MaximizeWeaponAmmo(int id, int clipSize, int baseCap)
    {
        wepList.ListOfWeapons[id].GetComponent<WeaponScript>().SetAmmo(clipSize, baseCap, clipSize);

        SetAmmoInfo(id, clipSize, baseCap);
    }

    public void ReplenishWeaponAmmo()
    {
        float ran = Random.Range(0.05f,0.08f);
        //int clipRestoreAmount = ran * 

        for (int i = 0; i < 3; i++)
        {
            //skipping rpg
            if (i == 1)
                continue;
            WeaponScript ws = wepList.ListOfWeapons[i].GetComponent<WeaponScript>();
            int currentAmmoCount = ws.GetAmmoCount();
            int currentAmmoCap = ws.GetAmmoCapCount();
            int clipRestoreAmount = Mathf.CeilToInt(ran * AmmoClipPerID[i]) + currentAmmoCount;
            int capRestoreAmount = currentAmmoCap;
            if (clipRestoreAmount > AmmoClipPerID[i])
            {
                capRestoreAmount = clipRestoreAmount - AmmoClipPerID[i];
                clipRestoreAmount = AmmoClipPerID[i];
                if (currentAmmoCap < AmmoBaseCapPerID[i])
                {
                    capRestoreAmount += currentAmmoCap;
                    if (capRestoreAmount > AmmoBaseCapPerID[i])
                        capRestoreAmount = AmmoBaseCapPerID[i];
                }
                else
                    capRestoreAmount = AmmoBaseCapPerID[i];
            }

            ws.SetAmmo(clipRestoreAmount, capRestoreAmount, ws.GetAmmoClipSize());

            SetAmmoInfo(i, clipRestoreAmount, capRestoreAmount);
        }
    }
    public int GetHP()
    {
        return hp;
    }
    public int GetMaxHP()
    {
        return maxHp;
    }
    public void WeaponUnlocked(int id)
    {
        switch (id)
        {
            case 0:
                RifleLabelText.text = "Rifle";
                RifleAmmoText.text = RifleAmmo.ToString();
                maxRifleAmmoText.text = RifleAmmoCap.ToString();
                break;
            case 1:
                RPGLabelText.text = "RPG";
                RPGAmmoText.text = RPGAmmo.ToString();
                maxRPGAmmoText.text = "0";
                break;
            case 2:
                ShotgunLabelText.text = "Shotgun";
                ShotgunAmmoText.text = ShotgunAmmo.ToString();
                maxShotgunAmmoText.text = ShotgunAmmoCap.ToString();
                break;
            case 3:
                break;
        }
    }
    public KeyCode IntToKeyCode(int num)
    {
        KeyCode ret = KeyCode.None;

        switch (num)
        {
            case 1: ret = KeyCode.Alpha1;             
                break;
            case 2:
                ret = KeyCode.Alpha2;
                break;
            case 3:
                ret = KeyCode.Alpha3;
                break;
            case 4:
                ret = KeyCode.Alpha4;
                break;
            case 5:
                ret = KeyCode.Alpha5;
                break;
            case 6:
                ret = KeyCode.Alpha6;
                break;
            case 7:
                ret = KeyCode.Alpha7;
                break;
            case 8:
                ret = KeyCode.Alpha8;
                break;
            case 9:
                ret = KeyCode.Alpha9;
                break;
            case 0:
                ret = KeyCode.Alpha0;
                break;
        }
        return ret;

    }
    public string MouseKeytoString(KeyCode code)
    {
        string ret = "";

        switch (code)
        {
            case KeyCode.Mouse0: ret = "Left Mouse Button";
                break;
            case KeyCode.Mouse1: ret = "Right Mouse Button";
                break;
            case KeyCode.Mouse2: ret = "Middle Mouse Button";
                break;
  
        }
        return ret;
    }
    public string KeycodeNumToString(KeyCode code)
    {
        string ret = "";

        switch (code)
        {
            case KeyCode.Alpha1: ret = "1";
                break;
            case KeyCode.Alpha2: ret = "2";
                break;
            case KeyCode.Alpha3: ret = "3";
                break;
            case KeyCode.Alpha4: ret = "4";
                break;
            case KeyCode.Alpha5: ret = "5";
                break;
            case KeyCode.Alpha6: ret = "6";
                break;
            case KeyCode.Alpha7: ret = "7";
                break;
            case KeyCode.Alpha8: ret = "8";
                break;
            case KeyCode.Alpha9: ret = "9";
                break;
            case KeyCode.Alpha0: ret = "0";
                break;
        }
        return ret;

    }
    public bool isAlphaKey(KeyCode code)
    {
        bool ret = false;
        switch (code)
        {
            case KeyCode.Alpha1:
                ret = true;
                break;
            case KeyCode.Alpha2:
                ret = true;
                break;
            case KeyCode.Alpha3:
                ret = true;
                break;
            case KeyCode.Alpha4:
                ret = true;
                break;
            case KeyCode.Alpha5:
                ret = true;
                break;
            case KeyCode.Alpha6:
                ret = true;
                break;
            case KeyCode.Alpha7:
                ret = true;
                break;
            case KeyCode.Alpha8:
                ret = true;
                break;
            case KeyCode.Alpha9:
                ret = true;
                break;
            case KeyCode.Alpha0:
                ret = true;
                break;
        }
        return ret;
    }

    // Key Bindings

    public void LoadSavedKeys()
    {
        IDToKeyCodes[0] = SetSavedKeys("Key0", IDToKeyCodes_Default[0]);
        IDToKeyCodes[1] = SetSavedKeys("Key1", IDToKeyCodes_Default[1]);
        IDToKeyCodes[2] = SetSavedKeys("Key2", IDToKeyCodes_Default[2]);
        IDToKeyCodes[3] = SetSavedKeys("Key3", IDToKeyCodes_Default[3]);
        activateTalismanKey = SetSavedKeys("Key4", activateTalismanKey_Default);
        cycleTalismanKey = SetSavedKeys("Key5", cycleTalismanKey_Default);
        jumpKey = SetSavedKeys("Key6", jumpKey_Default);
        dashKey = SetSavedKeys("Key7", dashKey_Default);
        shopWeaponToggleLeft = SetSavedKeys("Key8", shopWeaponToggleLeft_Default);
        shopWeaponToggleRight = SetSavedKeys("Key9", shopWeaponToggleRight_Default);
        shopInteract = SetSavedKeys("Key10", shopInteract_Default);
        reloadKey = SetSavedKeys("Key11", reloadKey_Default);
        shootKey = SetSavedKeys("Key12", shootKey_Default);
        secondaryFireKey = SetSavedKeys("Key13", secondaryFireKey_Default);
    }

    private KeyCode SetSavedKeys(string savedKeyName, KeyCode defaultKey)
    {
        KeyCode playerKey;

        if (PlayerPrefs.HasKey(savedKeyName))
        {
            playerKey = (KeyCode)PlayerPrefs.GetInt(savedKeyName);
            Debug.Log(playerKey);
        }      
        else
        {
            playerKey = defaultKey;
            PlayerPrefs.SetInt(savedKeyName, (int) playerKey);
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.HasKey(savedKeyName) + " - " + savedKeyName);
        }

        return playerKey;
    }

    public void SetKey(int which, KeyCode k)
    {
        switch (which)
        {
            case 0: IDToKeyCodes[0] = k; break;
            case 1: IDToKeyCodes[1] = k; break;
            case 2: IDToKeyCodes[2] = k; break;
            case 3: IDToKeyCodes[3] = k; break;
            case 4: activateTalismanKey = k; break;
            case 5: cycleTalismanKey = k; break;
            case 6: jumpKey = k; break;
            case 7: dashKey = k; break;
            case 8: shopWeaponToggleLeft = k; break;
            case 9: shopWeaponToggleRight = k; break;
            case 10: shopInteract = k; break;
            case 11: reloadKey = k; break;
            case 12: shootKey = k; break;
            case 13: secondaryFireKey = k; break;
        }
        PlayerPrefs.SetInt("Key"+which, (int)k);
        PlayerPrefs.Save();
    }

    public void ResetKeys()
    {
        for(int i = 0; i < IDToKeyCodes.Count; i++)
        {
            IDToKeyCodes[i] = IDToKeyCodes_Default[i];
            PlayerPrefs.GetInt("Key"+i, (int) IDToKeyCodes[i]);
        }

        activateTalismanKey = activateTalismanKey_Default;
        cycleTalismanKey = cycleTalismanKey_Default;
        jumpKey = jumpKey_Default;
        dashKey = dashKey_Default;
        shopWeaponToggleLeft = shopWeaponToggleLeft_Default;
        shopWeaponToggleRight = shopWeaponToggleRight_Default;
        shopInteract = shopInteract_Default;
        reloadKey = reloadKey_Default;
        shootKey = shootKey_Default;
        secondaryFireKey = secondaryFireKey_Default;

        PlayerPrefs.SetInt("Key4", (int)activateTalismanKey);
        PlayerPrefs.SetInt("Key5", (int)cycleTalismanKey);
        PlayerPrefs.SetInt("Key6", (int)jumpKey);
        PlayerPrefs.SetInt("Key7", (int)dashKey);
        PlayerPrefs.SetInt("Key8", (int)shopWeaponToggleLeft);
        PlayerPrefs.SetInt("Key9", (int)shopWeaponToggleRight);
        PlayerPrefs.SetInt("Key10", (int)shopInteract);
        PlayerPrefs.SetInt("Key11", (int)reloadKey);
        PlayerPrefs.SetInt("Key12", (int)shootKey);
        PlayerPrefs.SetInt("Key13", (int)secondaryFireKey);
        PlayerPrefs.Save();
    }
}
