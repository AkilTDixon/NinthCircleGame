using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveIndicatorScript : MonoBehaviour
{
    // Bool that indicates that it is time to show the number of the wave
    public static bool show_wave_num = false;
    public TextMeshProUGUI text_renderer;

    // Var that decreases alpha of the wave indicator
    // :: subtracted from alpha component of the color
    private float alpha_decrease = 0.01f;
    // Var that indicates the time of the decrease of the alpha
    // :: multiplied by the alpha_decrease
    private int ticks_decrease = 0;
    // Var that saves the initial color of the wave indicator
    // :: used to turn back the color to the initial at the start of the wave
    private Color initial_color;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onWaveIntermission += WaveIntermission;
        initial_color = text_renderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (show_wave_num)
        {
            if(ticks_decrease == 0)
            {
                Debug.Log("[WaveIndicatorScript] : set alpha to 1");
                Color temp = initial_color;
                temp.a = 1;
                text_renderer.color = temp;
            }
            ticks_decrease += 1;
            if (SpawnScript.boss_spawned)
                text_renderer.text = "BOSS ENCOUNTER";
            else
                text_renderer.text = "WAVE " + SpawnScript.num_of_waves;
            text_renderer.color = new Color(
                text_renderer.color.r, 
                text_renderer.color.g, 
                text_renderer.color.b, 
                text_renderer.color.a - alpha_decrease
                );
            if (text_renderer.color.a <= 0.01f)
            {               
                Debug.Log("[WaveIndicatorScript] : set alpha to 0");
                show_wave_num = false;
                ticks_decrease = 0;
                Color temp = initial_color;
                temp.a = 0;
                text_renderer.color = temp;
            }
        }
    }
    public void WaveIntermission()
    {
        text_renderer.text = "INTERMISSION";
        Color temp = initial_color;
        temp.a = 1;
        text_renderer.color = temp;
    }

}
