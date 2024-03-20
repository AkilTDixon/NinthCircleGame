using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public ConsumableScriptableObject effect;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            effect.Apply(other.gameObject);

            // FIX LATER FOR POOLING
            Destroy(gameObject);
        }
    }
}
