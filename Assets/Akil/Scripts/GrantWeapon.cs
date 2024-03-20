using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantWeapon : MonoBehaviour
{
    public static int NextKey = 1;
    [SerializeField] int WeaponID = 0;
    [TextArea][SerializeField] string WeaponName;
    [TextArea][SerializeField] string WeaponDescription;
    [SerializeField] Color WeaponColor;
    private bool AllowDestroy = false;
    public enum Weapons
    {
        Rifle,
        RPG,
        Shotgun,
        Melee
    }
    [SerializeField] Weapons WeaponToUnlock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            Grant();
            PlayerInfoScript.Instance.TalismanPanelHolder.SetActive(true);
            PlayerInfoScript.Instance.TalismanNameText.text = WeaponName;
            PlayerInfoScript.Instance.TalismanNameText.color = WeaponColor;           
            WeaponDescription = WeaponDescription.Replace("&weaponKey", PlayerInfoScript.Instance.KeycodeNumToString(PlayerInfoScript.Instance.IDToKeyCodes[WeaponID]));
            PlayerInfoScript.Instance.TalismanDescriptionText.text = WeaponDescription;

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
                Destroy(gameObject);
            }
    }
    void Grant()
    {

        bool[] unlocked = PlayerInfoScript.Instance.weaponUnlocked;
        KeyCode newKey = KeyCode.None;

        for (int i = 0; i < unlocked.Length; i++)
            if (!unlocked[i])
            {
                switch (NextKey)
                {
                    case 0:
                        newKey = KeyCode.Alpha1;
                        break;
                    case 1:
                        newKey = KeyCode.Alpha2;
                        break;
                    case 2:
                        newKey = KeyCode.Alpha3;
                        break;
                    case 3:
                        newKey = KeyCode.Alpha4;
                        break;
                }
                break;
            }
        
        Debug.Log((int)WeaponToUnlock);
        
        if (newKey != KeyCode.None)
        {
            GameEvents.current.weaponUnlock((int)WeaponToUnlock);
            PlayerInfoScript.Instance.weaponUnlocked[(int)WeaponToUnlock] = true;
            PlayerInfoScript.Instance.IDToKeyCodes[(int)WeaponToUnlock] = newKey;
            NextKey++;
        }
        else
            Debug.Log("WEAPONS ALREADY UNLOCKED");
 
    }

}
