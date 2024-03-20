using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScriptArtem : MonoBehaviour
{
    public float SpawnInterval = 5f;
    [SerializeField] GameObject EnemyPrefab;
    public Transform[] ChildObjects;
    private int numOfChildren;
    private float lastSpawned;

    // Number of waves
    // :: increases by 1 when SpawnNumber reaches 0
    public static int num_of_waves = 1;
    // Delta of the increase of the number of enemies
    // :: added to SpawnNumber after each wave
    private static int enemies_increase = 5;
    // Base number of enemies
    private static int base_num_enemies = 5;
    // Number of dead enemies
    public static int num_enemies_dead = 0;
    // Var that indicates that all enemies of the wave were cleared
    private bool wave_cleared = false;
    // Var that indicates after what num of waves boss is spawned
    private int boss_spawn_constant = 3;
    // Var that indicates that the boss is spawned
    private bool boss_spawned = false;

    private int SpawnNumber = base_num_enemies + enemies_increase * num_of_waves;
   

    public int EntitiesPerInterval = 5;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[SpawnScript] : spawn number at the beginning is " + SpawnNumber);
        ChildObjects = GetComponentsInChildren<Transform>();
        numOfChildren = ChildObjects.Length;
        lastSpawned = 0f;

        // At the start, depict that it is wave 1
        num_of_waves = 1;
        WaveIndicatorScript.show_wave_num = true;
    }

    // Update is called once per frame
    void Update()
    {
        wave_cleared = false;
        if ((Time.time > lastSpawned + SpawnInterval || lastSpawned == 0f) && SpawnNumber > 0 && !wave_cleared && !boss_spawned)
        {
            for (int i = 0; i < EntitiesPerInterval; i++)
            {

                Transform spawnPoint = ChildObjects[Random.Range(1, numOfChildren - 1)];

                GameObject obj = Instantiate(EnemyPrefab, spawnPoint.position, EnemyPrefab.transform.rotation);

                SpawnNumber--;
                lastSpawned = Time.time;
            }
        }
        if (SpawnNumber == 0 && num_enemies_dead == base_num_enemies + enemies_increase * num_of_waves)
        {
            if (num_of_waves % boss_spawn_constant == 0)
                boss_spawned = true;
            num_enemies_dead = 0;
            num_of_waves += 1;
            SpawnNumber = base_num_enemies + enemies_increase * num_of_waves;
            WaveIndicatorScript.show_wave_num = true;
            StartCoroutine(IntroduceWave());
        }
    }

    IEnumerator IntroduceWave()
    {
        yield return new WaitForSeconds(5);
        wave_cleared = true;
    }
}
