using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankChargeBox : MonoBehaviour
{
    [SerializeField] Skeleton_Follow2 MovementScript;
    [SerializeField] BoxCollider bc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            bc.enabled = false;

            Vector3 dir = other.gameObject.transform.position - transform.position;
            dir.y += 2;
            PlayerInfoScript.Instance.PlayerObject.GetComponent<Rigidbody>().AddForce(dir * 5f, ForceMode.Impulse);
            GameEvents.current.playerHit(MovementScript.DamageAmount * 2);
        }
    }

    void OnEnable()
    {
        bc.enabled = true;
        //bc = gameObject.AddComponent<BoxCollider>();
        /*bc.center = new Vector3(0.3123454f, 1.769491f, 1.791399f);
        bc.size = new Vector3(2.368678f, 3.376459f, 6.350291f);
        bc.isTrigger = true;*/
    }
    void OnDisable()
    {
        bc.enabled = false;

    }
}
