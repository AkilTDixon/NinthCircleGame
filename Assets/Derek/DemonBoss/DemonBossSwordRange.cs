using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBossSwordRange : MonoBehaviour {

    [SerializeField] private DemonBoss bossScript;


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            bossScript.inSwordStrikeRange = true;
        }
            
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            bossScript.inSwordStrikeRange = false;
            
        }
            
    }
}
