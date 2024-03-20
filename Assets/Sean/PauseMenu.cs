using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject menu;

    [Header("Key Binds")]
    bool detectingKeys = false;
    KeyCode key;
    int whichKey = 0;
    private string inputMsg = "Please enter a new key to replace this action with...";
    private string warnMsg = "<color=red>This key is already being used. The only duplicates acceptable are the shop weapon swapping keys.</color>";
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private TextMeshProUGUI[] keyTexts = new TextMeshProUGUI[14];

    // Start is called before the first frame update
    void Start()
    {
        UpdateKeyUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Shopkeeper.shopping && !PlayerInfoScript.Instance.finalBossKilled)
        {
            if (!isPaused)
                Pause();
            else
                Unpause();
        }

        if (detectingKeys)
        {
            foreach (KeyCode vkey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vkey))
                {
                    if (vkey != KeyCode.Return && vkey != KeyCode.Escape)
                    {
                        if(KeyAlreadyInUse(vkey, whichKey))
                        {
                            inputText.text = warnMsg;
                            return;
                        }    

                        key = vkey;
                        ChangeKey(whichKey);
                    }

                    detectingKeys = false;
                    inputText.text = "";
                }
            }
        }      
    }

    public bool KeyAlreadyInUse(KeyCode k, int which)
    {
        PlayerInfoScript pis = PlayerInfoScript.Instance;
        if (which != 8 && which != 9 &&
            (k == pis.IDToKeyCodes[0] || k == pis.IDToKeyCodes[1] || k == pis.IDToKeyCodes[2] || k == pis.IDToKeyCodes[3] || 
               k == pis.activateTalismanKey || k == pis.cycleTalismanKey || k == pis.jumpKey || k == pis.dashKey ||
               k == pis.shopInteract || k == pis.reloadKey || k == pis.shootKey || k == pis.secondaryFireKey))
            return true;

        // Exception for shop weapon swapping where they can have different results
        if (k == pis.shopWeaponToggleLeft && which == 9 || k == pis.shopWeaponToggleRight && which == 8)
            return true;

        return false;
    }

    public string KeyCodeToString(KeyCode k)
    {
        string s = "";

        if (PlayerInfoScript.Instance.isAlphaKey(k))
            s = PlayerInfoScript.Instance.KeycodeNumToString(k);
        else if (k == KeyCode.Mouse0 || k == KeyCode.Mouse1 || k == KeyCode.Mouse2)
            s = PlayerInfoScript.Instance.MouseKeytoString(k);
        else
            s = k.ToString();

        return s;
    }

    public void ChangeKey(int which)
    {
        PlayerInfoScript.Instance.SetKey(which, key);
        keyTexts[which].text = KeyCodeToString(key);
    }

    public void RebindKey(int which)
    {
        whichKey = which;
        detectingKeys = true;
        inputText.text = inputMsg;
    }

    public void ResetKeys()
    {
        PlayerInfoScript.Instance.ResetKeys();
        UpdateKeyUI();
    }

    public void UpdateKeyUI()
    {
        PlayerInfoScript pis = PlayerInfoScript.Instance;

        // UPDATE UI
        for (int i = 0; i < 4; i++)
        {
            keyTexts[i].text = KeyCodeToString(pis.IDToKeyCodes[i]);
        }

        keyTexts[4].text = KeyCodeToString(pis.activateTalismanKey);
        keyTexts[5].text = KeyCodeToString(pis.cycleTalismanKey);
        keyTexts[6].text = KeyCodeToString(pis.jumpKey);
        keyTexts[7].text = KeyCodeToString(pis.dashKey);
        keyTexts[8].text = KeyCodeToString(pis.shopWeaponToggleLeft);
        keyTexts[9].text = KeyCodeToString(pis.shopWeaponToggleRight);
        keyTexts[10].text = KeyCodeToString(pis.shopInteract);
        keyTexts[11].text = KeyCodeToString(pis.reloadKey);
        keyTexts[12].text = KeyCodeToString(pis.shootKey);
        keyTexts[13].text = KeyCodeToString(pis.secondaryFireKey);
    }

    public void Pause()
    {
        GameObject.Find("PlayerContainer").transform.Find("Player").GetComponent<PlayerInfoScript>().textIntoOutro.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        Time.timeScale = 0;
        menu.SetActive(true);
        
    }

    public void Unpause()
    {
        GameObject.Find("PlayerContainer").transform.Find("Player").GetComponent<PlayerInfoScript>().textIntoOutro.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void Restart(string scene)
    {
        // Restart Game...
        Unpause();
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    public void Quit(string scene)
    {
        // Quit Game...
        Unpause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
