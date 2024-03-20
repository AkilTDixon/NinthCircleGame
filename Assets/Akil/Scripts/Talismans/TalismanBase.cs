using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanBase : MonoBehaviour
{
    public enum Rarity
    {
        Low,
        Medium,
        Good,
        High,
        Ultra

    }

    public Rarity rarity = Rarity.Low;
    protected GameObject shopMenu;
    protected float cooldownStartTime = 0f;
    protected float Cooldown = 100f;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (cooldownStartTime == 0)
        {
            if (Input.GetKeyDown(PlayerInfoScript.Instance.activateTalismanKey) && !PlayerInfoScript.Instance.shopMenu.activeSelf)
            {
                Activate();
            }
        }
        else
        {
            if (!PlayerInfoScript.Instance.CooldownElementsHolder.activeSelf)
                PlayerInfoScript.Instance.CooldownElementsHolder.SetActive(true);

            PlayerInfoScript.Instance.currentCooldownText.text = ((int)((cooldownStartTime + Cooldown) - Time.time)).ToString();
            if (Time.time > cooldownStartTime + Cooldown)
            {
                PlayerInfoScript.Instance.CooldownElementsHolder.SetActive(false);
                cooldownStartTime = 0; 
            }
        }
    }
    protected virtual void OnDisable()
    {
        if (PlayerInfoScript.Instance.CooldownElementsHolder.activeSelf)
            PlayerInfoScript.Instance.CooldownElementsHolder.SetActive(false);
    }
    protected virtual void Activate()
    {
        Debug.Log("Talisman effect");
    }
}
