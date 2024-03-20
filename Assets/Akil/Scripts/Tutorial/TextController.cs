using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class TextController : MonoBehaviour
{

    public static TextController Instance { get; private set; }

    public GameObject shopkeep;
    public Transform shopSpawn;
    public Transform shopWait;
    public SkinnedMeshRenderer shopkeeperRenderer;
    public MeshRenderer backpackRenderer;
    public GameObject skeleton;
    public GameObject arrow;
    public Transform spawnPoint;
    public GameObject pressEnter;
    public GlorySystem gs;
    public TextMeshProUGUI messageBox;
    [TextArea] public string[] messages;
    private int index = 1;
    private float lastEnter = 0f;
    private GameObject enemyInstance;
    private bool spawned = false;
    private bool waitForInput = false;
    private bool gloryTime = false;
    private bool shopTime = false;
    private bool movedShop = false;
    public bool openedShop = false;
    public bool spokenToShop = false;
    private bool endTutorial = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        PlayerInfoScript.Instance.weaponUnlocked[3] = false;
        PlayerInfoScript.Instance.weaponUnlocked[0] = false;
        shopkeeperRenderer.enabled = false;
        backpackRenderer.enabled = false;
        gs = GameObject.Find("GlorySystem").GetComponent<GlorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!endTutorial)
        {
            if (!shopTime)
                shopkeep.transform.position = shopWait.position;

            if (index == 11 && shopTime)
            {
                if (!movedShop)
                {
                    shopkeep.transform.position = shopSpawn.position;
                    movedShop = true;
                }
                if (openedShop)
                {
                    messageBox.enabled = false;
                }
                if (spokenToShop)
                {

                    messageBox.enabled = true;
                    pressEnter.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        shopTime = false;
                        index++;
                        NextText();
                        pressEnter.SetActive(false);
                        lastEnter = Time.time;
                    }
                }
            }
            else if (index == 10 && shopTime)
            {

                pressEnter.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    index++;
                    NextText();
                    pressEnter.SetActive(false);
                    lastEnter = Time.time;
                }
            }
            else if (index == 9 && gloryTime)
            {
                gs.MaximumGlory();
                if (Time.time > lastEnter + 5f)
                {
                    pressEnter.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        index++;
                        gloryTime = false;
                        arrow.SetActive(false);
                        NextText();
                        pressEnter.SetActive(false);
                        lastEnter = Time.time;
                    }
                }
            }
            else if (index == 6 && waitForInput)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
                {
                    waitForInput = false;
                    index++;
                }
            }
            else if ((index == 3 || index == 4) && spawned)
            {
                if (enemyInstance == null)
                {
                    if (index == 3)
                    {
                        PlayerInfoScript.Instance.wepList.GetActiveWeapon().SetActive(false);
                        PlayerInfoScript.Instance.weaponUnlocked[0] = false;
                    }
                    index++;
                    spawned = false;
                }
            }
            else if (Time.time > lastEnter + 2f)
            {
                pressEnter.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (index == 14)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                        endTutorial = true;
                        return;
                    }
                    NextText();
                    pressEnter.SetActive(false);
                    lastEnter = Time.time;
                }
            }
        }
    }

    void NextText()
    {
        if (index == 3)
        {
            string key;
            if (PlayerInfoScript.Instance.isAlphaKey(PlayerInfoScript.Instance.IDToKeyCodes[0]))
                key = PlayerInfoScript.Instance.KeycodeNumToString(PlayerInfoScript.Instance.IDToKeyCodes[0]);
            else
                key = PlayerInfoScript.Instance.IDToKeyCodes[0].ToString();
            messages[index] = messages[index].Replace("&rifleKey", key);
            messages[index] = messages[index].Replace("&shootKey", PlayerInfoScript.Instance.MouseKeytoString(PlayerInfoScript.Instance.shootKey));

        }
        else if (index == 4)
        {
            string key;
            if (PlayerInfoScript.Instance.isAlphaKey(PlayerInfoScript.Instance.IDToKeyCodes[3]))
                key = PlayerInfoScript.Instance.KeycodeNumToString(PlayerInfoScript.Instance.IDToKeyCodes[3]);
            else
                key = PlayerInfoScript.Instance.IDToKeyCodes[3].ToString();
            messages[index] = messages[index].Replace("&meleeKey", key);
        }
        else if (index == 6)
        {
            string key;
            if (PlayerInfoScript.Instance.isAlphaKey(PlayerInfoScript.Instance.dashKey))
                key = PlayerInfoScript.Instance.KeycodeNumToString(PlayerInfoScript.Instance.dashKey);
            else
                key = PlayerInfoScript.Instance.dashKey.ToString();
            messages[index] = messages[index].Replace("&dashKey", key);
        }
        messageBox.text = messages[index];
        if (index == 10)
        {
            shopTime = true;
            return;
        }
        if (index == 3 || index == 4)
        {
            if (index == 3)
                PlayerInfoScript.Instance.weaponUnlocked[0] = true;
            else
                PlayerInfoScript.Instance.weaponUnlocked[3] = true;

            spawned = true;

            enemyInstance = Instantiate(skeleton, spawnPoint.position, skeleton.transform.rotation);
            return;
        }
        if (index == 11)
        {
            shopkeeperRenderer.enabled = true;
            backpackRenderer.enabled = true;
            return;
        }
        if (index == 6)
        {
            waitForInput = true;
            return;
        }
        if (index == 8)
        {
            arrow.SetActive(true);

        }
        if (index == 9)
        {
            gloryTime = true;
            gs.MaximumGlory();
            
            return;
        }
        index++;
    }
}
