using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objectives
{
    public static string DO_NOT_RELOAD = "Do NOT reload";
    public static string KILL_10_ENEMIES_WITH_A_MELEE = "KILL 10 enemies with a melee";
    public static string DO_NOT_SHOOT = "Do NOT shoot";
}

public class AudienceObjectivesScript : MonoBehaviour
{
    TextMeshPro text_renderer;
    public static string current_objective;
    private string[] objectives = new string[] { Objectives.DO_NOT_RELOAD, Objectives.DO_NOT_SHOOT };
    //private int max_num_objectives = 3; 
    private float last_added = 0f;
    private float addition_interval = 15f;
    private float completion_interval = 12f;
    public static bool objective_completed = false;
    private Color initial_color;
    private float alpha_decrease = 0.00001f;
    private bool objective_fadeaway = false;
    private int ticks_decrease = 0;

    // Start is called before the first frame update
    void Start()
    {
        text_renderer = GetComponent<TextMeshPro>();
        //current_objectives = new string[max_num_objectives];
        current_objective = "";
        initial_color = text_renderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(!objective_fadeaway && Time.time > last_added + addition_interval) 
        {
            text_renderer.color = initial_color;
            current_objective = objectives[(int)Random.Range(0, objectives.Length + 0.1f)];
            last_added = Time.time;
            objective_completed = true;
            text_renderer.text = current_objective;
        }
        if (Time.time > last_added + completion_interval && current_objective != "")
        {
            if (ticks_decrease == 0)
            {
                Debug.Log("[AudienceObjectivesScript]: objective delete");
                Color temp = initial_color;
                if (objective_completed)
                {
                    temp.r = 0;
                    temp.g = 1;
                    temp.b = 0;
                }
                else
                {
                    temp.r = 1;
                    temp.g = 0;
                    temp.b = 0;
                }
                temp.a = 1;
                objective_fadeaway = true;
                text_renderer.color = temp;
            }
            ticks_decrease += 1;
            text_renderer.color = new Color(
                text_renderer.color.r,
                text_renderer.color.g,
                text_renderer.color.b,
                text_renderer.color.a - alpha_decrease * ticks_decrease
                );
            if (text_renderer.color.a <= 0.05f)
            {
                Color temp = initial_color;
                temp.a = 0;
                text_renderer.color = temp;
                text_renderer.text = "";
                current_objective = "";
                objective_fadeaway = false;
                ticks_decrease = 0;
                last_added = Time.time;
            }
        }

        text_renderer.text = current_objective;
    }
}
