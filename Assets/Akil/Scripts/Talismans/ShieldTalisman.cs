using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTalisman : TalismanBase
{

    [SerializeField] int RegenPerTick = 2;
    [SerializeField] float RegenPauseTime = 10;
    private float ShieldPercent = 0.3f;
    private float lastRegen = 0f;
    private float pauseStartTime = 0f;
    // Start is called before the first frame update
    protected override void Start()
    {
        GameEvents.current.onShieldBreak += StartCooldown;
        GameEvents.current.onPlayerHit += DamageTakenEvent;
        base.Start();
        switch (rarity)
        {
            case Rarity.Low:
                ShieldPercent = 0.3f;
                Cooldown = 100f;
                break;
            case Rarity.Medium:
                ShieldPercent = 0.45f;
                Cooldown = 80f;
                break;
            case Rarity.High:
                ShieldPercent = 0.60f;
                Cooldown = 60f;
                break;
            case Rarity.Ultra:
                ShieldPercent = 1f;
                Cooldown = 30f;
                break;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (PlayerInfoScript.Instance.GetShieldHP() > 0)
        {
            cooldownStartTime = Time.time;
            PlayerInfoScript.Instance.SetShield(0);
        }
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Time.time > pauseStartTime + RegenPauseTime && Time.time > lastRegen + 0.5f && PlayerInfoScript.Instance.GetShieldHP() > 0)
        {
            Regenerate();
        }
    }

    private void Regenerate()
    {
        PlayerInfoScript.Instance.HealShield(RegenPerTick);
        lastRegen = Time.time;
    }
    protected override void Activate()
    {
        if (PlayerInfoScript.Instance.GetShieldHP() == 0)
            PlayerInfoScript.Instance.SetShield(Mathf.CeilToInt(PlayerInfoScript.Instance.GetMaxHP() * ShieldPercent));
    }
    public void StartCooldown()
    {
        if (cooldownStartTime == 0)
            cooldownStartTime = Time.time;
      
    }

    public void DamageTakenEvent(int unused)
    {
        pauseStartTime = Time.time;
    }
}
