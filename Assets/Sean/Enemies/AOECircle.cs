using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOECircle : MonoBehaviour
{
    public float defenseBuff;
    public float attackBuff;
    public float movementSpeedBuff;
    public float attackSpeedBuff;
    public int healthRegenAmt;
    public float healthRegenTick;
    float tick;

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("IN: " + other.name);
        if(other.tag == "Enemy")
        {
            Skeleton_Follow2 follow = other.GetComponent<Skeleton_Follow2>();

            if (defenseBuff != 0)
                follow.defense *= defenseBuff;

            if (attackBuff != 0)
                follow.DamageAmount = (int) (follow.DamageAmount * attackBuff);

            if (movementSpeedBuff != 0)
                follow.agent.speed *= movementSpeedBuff;

            if (attackSpeedBuff != 0)
            {
                follow.attackSpeed *= attackSpeedBuff;
                follow.animator.SetFloat("attackSpeed", follow.attackSpeed);
            }
                
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(tick == 0)
        {
            if (healthRegenAmt != 0 && other.tag == "Enemy")
            {
                Skeleton_Follow2 follow = other.GetComponent<Skeleton_Follow2>();
                follow.Heal(healthRegenAmt);
            }
        }  
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("OUT: " + other.name);
        if (other.tag == "Enemy")
        {
            Skeleton_Follow2 follow = other.GetComponent<Skeleton_Follow2>();

            if (defenseBuff != 0)
                follow.defense /= defenseBuff;

            if (attackBuff != 0)
                follow.DamageAmount = (int)(follow.DamageAmount / attackBuff);

            if (movementSpeedBuff != 0)
                follow.agent.speed /= movementSpeedBuff;

            if (attackSpeedBuff != 0)
            {
                follow.attackSpeed /= attackSpeedBuff;
                follow.animator.SetFloat("attackSpeed", follow.attackSpeed);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        tick += Time.deltaTime;

        if (tick >= healthRegenTick)
            tick = 0;
    }
}
