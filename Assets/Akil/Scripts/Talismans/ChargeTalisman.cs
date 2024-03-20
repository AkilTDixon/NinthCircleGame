using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeTalisman : TalismanBase
{
    private float ChargeDuration = 1f;
    private int DamageInflicted = 50;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        switch (rarity)
        {
            case Rarity.Low:
                ChargeDuration = 0.5f;
                DamageInflicted = 50;
                Cooldown = 20f;
                break;
            case Rarity.Medium:
                ChargeDuration = 1f;
                DamageInflicted = 75;
                Cooldown = 17f;
                break;
            case Rarity.High:
                ChargeDuration = 1.5f;
                DamageInflicted = 100;
                Cooldown = 13f;
                break;
            case Rarity.Ultra:
                ChargeDuration = 3f;
                DamageInflicted = 150;
                Cooldown = 10f;
                break;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (cooldownStartTime != 0)
        {
            if (Time.time > cooldownStartTime)
                GameEvents.current.chargeEnd();
        }

    }
   
    protected override void Activate()
    {
        /*
         Increase player speed
         Force player to move in forward direction
         Player is unable to slow down
         Player is immune to damage
         Player deals damage on hit with its collider
         */
        GameEvents.current.chargeStart(DamageInflicted);
        cooldownStartTime = Time.time + ChargeDuration;
    }
}
