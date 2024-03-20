using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Buff : MonoBehaviour{

    public enum BuffType {
        FREEZE, XP_MULTIPLIER, HEAL, INCREASE_MAX_HP
    }
    public BuffType buffType = BuffType.FREEZE;
    public bool isHelpful = true;  //buff or debuff?
    public float duration = 3.0f; // how long does it last?
    public bool isActivated = false;
    public float value = 0f;
    public float tick = 0f; // set to zero to only apply effect once, otherwise every tick
    public string text = "Buff text here";
    private float activationTime = 0f;
    private float nextTickTime = 0f;

    private PlayerInfoScript playerScript;

    public void copyFrom(Buff other) {
        isHelpful = other.isHelpful;
        duration = other.duration;
        isActivated = other.isActivated;
        value = other.value;
        tick = other.tick;
        buffType = other.buffType;
        text = other.text;
    }
    
    
    public void Activate() {
        isActivated = true;
        activationTime = Time.time;
        nextTickTime = Time.time + tick;
        playerScript = PlayerInfoScript.Instance;
        applyEffect();
    }

    private void Update() {
        if (isActivated) {
            if (tick!=0f) {
                //check for extra ticks
                if (Time.time >= nextTickTime) {
                    applyEffect();
                    nextTickTime = Time.time + tick;
                }
            }
        } 
    }

    private void applyEffect() {
        switch (buffType) {
            case BuffType.FREEZE:
                playerScript.speedBuffAdjustAbsolute -= value;
                GameEvents.current.playerFreeze();
                break;
            case BuffType.XP_MULTIPLIER:
                playerScript.ChangeExpMultiplier(value);
                break;
            case BuffType.INCREASE_MAX_HP:
                playerScript.ChangeMaxHP((int)value);
                break;
            
        }
        
    } //called at Activate and every tick (if enabled)

    private void removeEffect() {
        switch (buffType) {
            case BuffType.FREEZE:
                playerScript.speedBuffAdjustAbsolute += value;
                break;
            case BuffType.XP_MULTIPLIER:
                playerScript.ChangeExpMultiplier(value * -1);
                break;
            case BuffType.INCREASE_MAX_HP:
                playerScript.ChangeMaxHP((int)value * -1);
                break;
            
        }
    }

    private void OnDestroy() {
        if (isActivated)
            removeEffect();
    }
}
