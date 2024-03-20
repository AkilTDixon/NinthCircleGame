using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowUpDown : MonoBehaviour
{

    public float maxY = 5f;
    public float minY = 0f;
    private Vector3 basePos;
    private Vector3 newPos;
    private bool fall = false;

    // Start is called before the first frame update
    void Start()
    {
        basePos = transform.position;
        newPos = transform.position + new Vector3(0,maxY,0);
        transform.position = newPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (newPos.y >= basePos.y + maxY)
            fall = true;
        else if (newPos.y <= basePos.y)
            fall = false;


        if (fall)
        {
            newPos.y -= 0.1f;
            transform.position = newPos;
        }
        else
        {
            newPos.y += 0.1f;
            transform.position = newPos;
        }


    }
}
