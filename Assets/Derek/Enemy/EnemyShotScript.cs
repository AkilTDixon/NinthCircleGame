using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotScript : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem Projectile;
    public ParticleSystem Core;
    public float speed;
    public float duration;
    void Start()
    {
        ParticleSystem sys = GetComponent<ParticleSystem>();  
        ParticleSystem.MainModule mainP;

        mainP = sys.main;
        mainP.simulationSpeed = speed;
 

    }


}
