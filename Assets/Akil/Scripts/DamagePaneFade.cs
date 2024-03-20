using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePaneFade : MonoBehaviour
{
    [SerializeField] RawImage image;
    private Color baseColor;
    private float baseAlpha;
    private bool hit;
 
    // Start is called before the first frame update
    void Start()
    {

        GameEvents.current.onPlayerHit += StartFade;
        image = GetComponent<RawImage>();
        
        Color c = image.color;
        baseAlpha = c.a;
        c.a = 0f;
        image.color = c;
        baseColor = image.color;

    }

    public void StartFade(int unused)
    {
        if(!hit)
            StartCoroutine(Fade());
    }
    private IEnumerator Fade()
    {
        hit = true;
        Color c = image.color;
        if (PlayerInfoScript.Instance.GetShieldHP() > 0)
        {
            c.r = 0;
            c.g = 0;
            c.b = 1;
        }
        for (float alpha = baseAlpha; alpha >= 0; alpha -= 0.1f)
        {
            c.a = alpha;
            image.color = c;
            yield return new WaitForSeconds(.1f);
        }
        image.color = baseColor;
        hit = false;
    }
}
