using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] GameObject FireTailEffect;
    private float lastSpawn = 0f;
    private GameObject obj = null;
    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastSpawn + 0.03f)
        {
            if (obj != null)
                Destroy(obj.gameObject);

            obj = Instantiate(FireTailEffect, transform.position, transform.rotation);
            
            lastSpawn = Time.time;
        }
    }
}
