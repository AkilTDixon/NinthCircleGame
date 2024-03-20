using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealTalisman : TalismanBase
{
    public TextMeshProUGUI hpText;
    private Color healColor = new Color(0, 0.8f, 0);
    private float HealRate = 0.01f;
    private float lastHeal = 0f;
    // Start is called before the first frame update
    protected override void Start()
    {
        //this talisman is passive, therefore no need to check if shop is open, or check for a cooldown
        //base.Start();
        switch (rarity)
        {
            case Rarity.Low:
                HealRate = 0.01f;
                break;
            case Rarity.Medium:
                HealRate = 0.02f;
                break;
            case Rarity.High:
                HealRate = 0.03f;
                break;
            case Rarity.Ultra:
                HealRate = 0.04f;
                break;
        }
        lastHeal = Time.time;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        hpText.color = healColor;
        //this talisman is passive, therefore no need to check if shop is open, or check for a cooldown
        //base.Update();
        if (Time.time > lastHeal + 2.5f)
        {
            PlayerInfoScript.Instance.Heal(Mathf.CeilToInt(PlayerInfoScript.Instance.GetMaxHP() * HealRate));
            lastHeal = Time.time;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        hpText.color = Color.white;
    }

    protected override void Activate()
    {
        Debug.Log("Heal Talisman is passive");
    }
}
