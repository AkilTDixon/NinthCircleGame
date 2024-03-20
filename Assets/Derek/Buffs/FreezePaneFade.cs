using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreezePaneFade : MonoBehaviour
{
    [SerializeField] RawImage image;
    private float baseAlpha;
    private bool hit;
 
    // Start is called before the first frame update
    void Start()
    {

        GameEvents.current.onPlayerFreeze += StartFade;
        image = GetComponent<RawImage>();   
        
        Color c = image.color;
        baseAlpha = c.a;
        c.a = 0f;
        image.color = c;

    }

    public void StartFade()
    {
        StartCoroutine(Fade());
    }
    private IEnumerator Fade()
    {
        hit = true;
        Color c = image.color;
        for (float alpha = baseAlpha; alpha >= 0; alpha -= 0.1f)
        {
            c.a = alpha;
            image.color = c;
            yield return new WaitForSeconds(.2f);
        }
        c.a = 0f;
        image.color = c;
        hit = false;
    }
}
