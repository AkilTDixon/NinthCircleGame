using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health Item", menuName = "Scriptable Object/Consumable Item/Health Item")]
public class HealthScriptableObject : ConsumableScriptableObject
{
    public int amount;

    public override void Apply(GameObject target)
    {
        //target.GetComponent<PlayerStats>().Heal(amount);
    }
}
