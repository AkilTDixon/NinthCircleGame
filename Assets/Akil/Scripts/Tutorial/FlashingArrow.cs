using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlashingArrow : MonoBehaviour
{
    public RawImage img;
    public Color c;
    private bool rising = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (rising)
        {
            c = img.color;
            c.a += 0.02f;
            img.color = c;
            if (img.color.a >= 1)
                rising = false;
        }
        else
        {
            c = img.color;
            c.a -= 0.02f;
            img.color = c;
            if (img.color.a <= 0)
                rising = true;
        }
    }
}
