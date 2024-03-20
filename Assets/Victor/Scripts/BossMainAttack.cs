using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMainAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
    }

    void Update() {
        
    }
    void OnDestroy() {
        GameEvents.current.kaeneMainAttackHit(this.gameObject.transform.position);
    }
}
