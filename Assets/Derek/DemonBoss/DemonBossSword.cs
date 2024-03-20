using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBossSword : MonoBehaviour {

    [SerializeField] private DemonBoss scriptDemonBoss;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            scriptDemonBoss.swordStrikingPlayer();
        }
    }


}
