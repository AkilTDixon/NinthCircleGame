using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopRange : MonoBehaviour
{
    [SerializeField] private Shopkeeper shopkeeper;

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            shopkeeper.CloseShop();
    }
}
