using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlorySystem : MonoBehaviour
{
    // MORE BLOOD FOR THE BLOOD GODS
    [SerializeField] private Slider glorySlider;
    [SerializeField] private float glory = 0;
    [SerializeField] private float gloryLossRate = 2f;
    [SerializeField] private float gloryGainMultiplier = 0.05f;
    private float baseGloryLossRate;
    private float baseGloryGainMultiplier;
    private float tempLossRateHolder;
    private bool intermission = false;
    public bool Tutorial = false;

    [SerializeField] private Sprite[] sprites = new Sprite[3];
    [SerializeField] private Image handle;

   // Start is called before the first frame update
   void Start()
    {
        //Akil - attaching to event system
        GameEvents.current.onPlayerHit += PlayerDamagedEvent;
        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onShieldAbsorb += PlayerAbsorbedEvent;
        GameEvents.current.onWaveIntermission += WaveIntermission;
        GameEvents.current.onWaveStart += WaveStart;
        //

        baseGloryLossRate = gloryLossRate;
        baseGloryGainMultiplier = gloryGainMultiplier;

        if (!Tutorial)
            glory = 0;
        else
        {
            glory = -100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        glory -= Time.deltaTime * gloryLossRate;
        
        if (glory < -100)
            glory = -100;

        if (glory < -50)
            handle.sprite = sprites[2];
        else if (glory > 50)
            handle.sprite = sprites[0];
        else
            handle.sprite = sprites[1];

        /*
         old formula: ((100+(2*((glory * 0.5f))))*(0.5f))*0.01f
         simplified: 0.005f*(100f + glory)
         */

        AudienceManager.Instance.setFavor(0.005f * (100f + glory));
        glorySlider.value = glory;
    }

    public void AddGlory(float amt)
    {
        glory += amt * gloryGainMultiplier;

        if (glory > 100)
            glory = 100;
        
    }
    public void MaximumGlory()
    {
        glory = 100;
    }
    public void ChangeGloryGain(float amt)
    {
        gloryGainMultiplier += baseGloryGainMultiplier * amt;

    }

    public void ChangeGloryLoss(float amt)
    {
        if (!intermission)
        {
            gloryLossRate += baseGloryLossRate * amt;
        }
        else
            tempLossRateHolder += baseGloryLossRate * amt;

        if (gloryLossRate < 0)
            gloryLossRate = 0.01f;
    }

    //Akil
    public void WaveIntermission()
    {
        intermission = true;
        tempLossRateHolder = gloryLossRate;
        gloryLossRate = 0;
    }
    public void WaveStart()
    {
        intermission = false;
        gloryLossRate = tempLossRateHolder;
    }
    //
    //Akil
    public void PlayerDamagedEvent(int unused)
    {
        if (!PlayerInfoScript.easyMode)
            glory -= 25;
        else
            glory -= 5;
    }
    public void PlayerAbsorbedEvent()
    {
        if (!PlayerInfoScript.easyMode)
            glory += 25;
        else
            glory += 5;
    }
    //

    //Akil
    public void PlayerDeathEvent()
    {
        if (!PlayerInfoScript.easyMode)
            glory -= 100;
    }
    //

}
