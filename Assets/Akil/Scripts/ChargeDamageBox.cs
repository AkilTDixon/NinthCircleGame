using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDamageBox : MonoBehaviour
{
    [HideInInspector] public int Damage;
    private BoxCollider bc;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Skeleton_Follow2 sf = other.gameObject.GetComponent<Skeleton_Follow2>();
            Vector3 dir = transform.position - other.gameObject.transform.position;
            dir.y += 2;
            sf.TakeDamageWithForce(Damage, dir *5f);
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        //gameObject.layer = LayerMask.NameToLayer("whatIsGround");
        bc = gameObject.AddComponent<BoxCollider>();
        bc.size = new Vector3(3f, 3f, 3f);
        bc.isTrigger = true;
    }

    // Update is called once per frame
    void OnDisable()
    {
        //gameObject.layer = LayerMask.NameToLayer("Player");
        Destroy(bc);

    }
}
