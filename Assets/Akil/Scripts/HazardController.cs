using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HazardController : MonoBehaviour
{
    public int Level = 1;
    public GameObject[] Hazard;
    public GameObject[] HazardIndicator;
    public float HazardSpawnInterval = 5f;
    private Bounds FloorBounds;
    private float lastSpawned = 0f;
    


    // Start is called before the first frame update
    void Start()
    {
        FloorBounds = PlayerInfoScript.Instance.theFloorObject.GetComponent<Renderer>().bounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (Level > 1)
        {
            if (Time.time > lastSpawned + HazardSpawnInterval)
            {
                NavMeshHit nhit = new NavMeshHit();


                float x = Random.Range(FloorBounds.min.x, FloorBounds.max.x);
                float z = Random.Range(FloorBounds.min.z, FloorBounds.max.z);

                NavMesh.SamplePosition(new Vector3(x, 0, z), out nhit, 100f, -1);
                if (nhit.hit)
                {
                    GameObject indicator = Instantiate(HazardIndicator[Level-2], nhit.position, HazardIndicator[Level-2].transform.rotation);
                    indicator.GetComponent<HazardSpawn>().Hazard = Hazard[Level-2];
                }
                lastSpawned = Time.time;
            }
        }
    }
}
