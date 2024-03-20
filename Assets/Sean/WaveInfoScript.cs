using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveInfoScript : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyNumText;

    // Update is called once per frame
    void Update()
    {
        waveText.text = "WAVE " + SpawnScript.num_of_waves.ToString();
        enemyNumText.text = SpawnScript.num_enemies.ToString() + " ENEMIES LEFT";
    }
}
