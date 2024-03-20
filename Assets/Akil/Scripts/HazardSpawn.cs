using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawn : MonoBehaviour
{

    public GameObject Hazard;
    // Start is called before the first frame update
    void OnDisable()
    {
        Instantiate(Hazard, transform.position, Hazard.transform.rotation);
        Destroy(gameObject);
    }


}
