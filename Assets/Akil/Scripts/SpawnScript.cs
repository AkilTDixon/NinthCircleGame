using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SpawnScript : MonoBehaviour
{

    public static SpawnScript Instance { get; private set; }

    [System.Serializable]
    struct WaveProfile
    {
        public int MeleeNumber;
        public int RangedNumber;
        public int SupportNumber;
        public int TankNumber;
        public WaveProfile(int melee, int ranged, int support, int tank)
        {
            MeleeNumber = melee;
            RangedNumber = ranged;
            TankNumber = tank;
            SupportNumber = support;
        }
    };
    [System.Serializable]
    struct PrefabsPerLevel
    {
        public GameObject MeleePrefab;
        public GameObject RangedPrefab;
        public GameObject[] SupportPrefab;
        public GameObject TankPrefab;


        public PrefabsPerLevel(GameObject melee, GameObject ranged, GameObject[] support, GameObject tank)
        {
            MeleePrefab = melee;
            RangedPrefab = ranged;
            SupportPrefab = support;
            TankPrefab = tank;
        }
    };

    /*
     Waves 1 - 3 : Gate 1
     Waves 4 - 6 : Gate 2
     Waves 7 - 8 : Gate 3
     Wave  9     : Gate 4
     
     */
    public float SpawnInterval = 5f;

    [Header("Wave Profiles and Prefabs")]
    public int Level = 1;
    [SerializeField] WaveProfile[] Level1WaveProfiles;
    [SerializeField] WaveProfile[] Level2WaveProfiles;
    [SerializeField] WaveProfile[] Level3WaveProfiles;
    [SerializeField] PrefabsPerLevel[] LevelSpecificPrefabs;
    [SerializeField] List<GameObject> Minibosses;
    [SerializeField] List<GameObject> Bosses;
    public GameObject[] Gates;
    public Slider slider;
    public GameObject waveIndicatorPanel;
    


    [Header("Misc")]   
    public List<Transform> SpawnPoints;
    public TextMeshProUGUI levelIndicator;
    public TextMeshProUGUI waveInfoLevelText;
    public List<GameObject> arrowPointers;
    private List<WaveProfile[]> WaveProfilesPerLevel;
    private float lastSpawned;
    private int gateIndex = 0;

    //[SerializeField] int Level = 1;
    // Number of waves
    // :: increases by 1 when SpawnNumber reaches 0
    public static int num_of_waves = 9;

    // Number of enemies to kill
    public static int num_enemies = 0;
    // Var that indicates that all enemies of the wave were cleared
    private bool wave_cleared = false;
    // Var that indicates after what num of waves boss is spawned
    private int boss_spawn_constant = 9;
    private int miniboss_spawn_constant = 5;
    // Var that indicates that the boss is spawned
    public static bool boss_spawned = false;
    private bool miniboss_spawned = false;
    private int SpawnNumber;
    private List<int> enemyTypeArray = new List<int>();
    public int EntitiesPerInterval = 5;


    private int MeleeCount = 0, RangedCount = 0, SupportCount = 0, TankCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        
    }


        // Start is called before the first frame update
     void Start()
    {


        /*        WaveProfiles = new WaveProfile[9];
                WaveProfiles[0] = new WaveProfile(1f,0,0,0);
                WaveProfiles[1] = new WaveProfile(0.8f, 0.2f, 0, 0);
                WaveProfiles[2] = new WaveProfile(0.7f, 0.2f, 0.1f, 0);
                WaveProfiles[3] = new WaveProfile(0.6f, 0.3f, 0.1f, 0);
                WaveProfiles[4] = new WaveProfile(0.6f, 0.3f, 0.1f, 0);
                WaveProfiles[5] = new WaveProfile(0.6f, 0.2f, 0.2f, 0);
                WaveProfiles[6] = new WaveProfile(0.6f, 0.2f, 0.1f, 0.1f);
                WaveProfiles[7] = new WaveProfile(0.6f, 0.2f, 0.1f, 0.1f);
                WaveProfiles[8] = new WaveProfile(0.6f, 0.2f, 0.1f, 0.1f);*/
        WaveProfilesPerLevel = new List<WaveProfile[]>();
        WaveProfilesPerLevel.Add(Level1WaveProfiles);
        WaveProfilesPerLevel.Add(Level2WaveProfiles);
        WaveProfilesPerLevel.Add(Level3WaveProfiles);
        Debug.Log("[SpawnScript] : spawn number at the beginning is " + SpawnNumber);

        Transform[] points = Gates[gateIndex].GetComponentsInChildren<Transform>();
        for (int i  = 1; i < points.Length; i++)
            SpawnPoints.Add(points[i]);

        //numOfChildren = ChildObjects.Length;
        lastSpawned = 0f;

        // At the start, depict that it is wave 1
        num_of_waves = 1;


        //SpawnNumber = base_num_enemies + enemies_increase * num_of_waves;

        MeleeCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].MeleeNumber;
        RangedCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].RangedNumber;
        TankCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].TankNumber;
        SupportCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].SupportNumber;
        arrowPointers[gateIndex].SetActive(true);
        SpawnNumber = MeleeCount + RangedCount + SupportCount + TankCount;
        num_enemies = SpawnNumber;
        waveInfoLevelText.text = "Level " + Level;
        slider.maxValue = num_enemies; 
        levelIndicator.enabled = false;
        WaveIndicatorScript.show_wave_num = true;

    }

    // Update is called once per frame
    void Update()
    {

        slider.value = num_enemies;
        if ((Time.time > lastSpawned + SpawnInterval || lastSpawned == 0f) && SpawnNumber > 0 && !wave_cleared && !boss_spawned && !PlayerInfoScript.Instance.Dead)
        {

            if (lastSpawned == 0 && num_enemies != SpawnNumber)
                num_enemies = SpawnNumber;

            if (MeleeCount > 0)
                enemyTypeArray.Add(0);
            if (RangedCount > 0)
                enemyTypeArray.Add(1);
            if (SupportCount > 0)
                enemyTypeArray.Add(2);
            if (TankCount > 0)
                enemyTypeArray.Add(3);


            for (int i = 0; i < EntitiesPerInterval && i < num_enemies && SpawnNumber > 0; i++)
            {



                if (MeleeCount == 0)
                    enemyTypeArray.Remove(0);
                if (RangedCount == 0)
                    enemyTypeArray.Remove(1);
                if (SupportCount == 0)
                    enemyTypeArray.Remove(2);
                if (TankCount == 0)
                    enemyTypeArray.Remove(3);

                
                

                Transform spawnPoint = SpawnPoints[Random.Range(1, SpawnPoints.Count)];
                int ran = Random.Range(0, enemyTypeArray.Count);

                if (Level != 3)
                {
                    if (miniboss_spawn_constant == num_of_waves && !miniboss_spawned)
                    {
                        Instantiate(Minibosses[Level - 1], spawnPoint.position, Minibosses[0].transform.rotation);
                        num_enemies++;
                        miniboss_spawned = true;
                    }
                }
                switch (enemyTypeArray[ran])
                {
                    //Spawn Melee
                    case 0:
                        if (MeleeCount != 0)
                        {
                            Instantiate(LevelSpecificPrefabs[Level-1].MeleePrefab, spawnPoint.position, LevelSpecificPrefabs[Level - 1].MeleePrefab.transform.rotation);
                            MeleeCount--;
                        }
                        break;
                    //Spawn Ranged
                    case 1:
                        if (RangedCount != 0)
                        {
                            Instantiate(LevelSpecificPrefabs[Level - 1].RangedPrefab, spawnPoint.position, LevelSpecificPrefabs[Level - 1].RangedPrefab.transform.rotation);
                            RangedCount--;
                        }
                        break;
                    //Spawn Support
                    case 2:
                        if (SupportCount != 0)
                        {
                            int ranSupport = Random.Range(0, LevelSpecificPrefabs[Level - 1].SupportPrefab.Length);
                            Instantiate(LevelSpecificPrefabs[Level - 1].SupportPrefab[ranSupport], spawnPoint.position, LevelSpecificPrefabs[Level - 1].SupportPrefab[ranSupport].transform.rotation);
                            SupportCount--;
                        }
                        break;
                    //Spawn Tank
                    case 3:
                        if (TankCount != 0)
                        {
                            Instantiate(LevelSpecificPrefabs[Level - 1].TankPrefab, spawnPoint.position, LevelSpecificPrefabs[Level - 1].TankPrefab.transform.rotation);
                            TankCount--;
                        }
                        break;
                }


                //GameObject obj = Instantiate(EnemyPrefab, spawnPoint.position, EnemyPrefab.transform.rotation);

                SpawnNumber--;
                
            }
            enemyTypeArray.Clear();
            lastSpawned = Time.time;
        }
        if (num_enemies <= 0 && !boss_spawned)
        {
            if (num_of_waves == 3 || num_of_waves == 6 || num_of_waves == 8)
            {
                gateIndex++;
                Transform[] points = Gates[gateIndex].GetComponentsInChildren<Transform>();
                arrowPointers[gateIndex].SetActive(true);
                for (int i = 1; i < points.Length; i++)
                    SpawnPoints.Add(points[i]);
            }

            if (num_of_waves % boss_spawn_constant == 0)
            {   boss_spawned = true;
                waveIndicatorPanel.SetActive(false);
                for (int i = 0; i < 4; i++)
                    arrowPointers[i].SetActive(false);
            }
            else
            {
                num_of_waves += 1;

                MeleeCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].MeleeNumber;
                RangedCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].RangedNumber;
                TankCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].TankNumber;
                SupportCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].SupportNumber;

                

                SpawnNumber = MeleeCount + RangedCount + SupportCount + TankCount;
                num_enemies = SpawnNumber;
                slider.maxValue = num_enemies;
            }

            


            wave_cleared = true;
            StartCoroutine(IntroduceWave());
        }
    }

    IEnumerator IntroduceWave()
    {

        GameEvents.current.waveWait();
        yield return new WaitForSeconds(10f);
        levelIndicator.enabled = false;
        wave_cleared = false;
        GameEvents.current.waveStart();
        if (boss_spawned)
        {   if (Level != 3)
                Bosses[Level - 1].SetActive(true);
            else
                Bosses[Level - 1].GetComponent<DemonBoss>().startEntranceSequenceNextUpdate = true;
        }
        WaveIndicatorScript.show_wave_num = true;

    }

    private IEnumerator BossWaveEnd()
    {
        yield return new WaitForSeconds(20f);
        waveInfoLevelText.text = "Level " + Level;
        levelIndicator.enabled = true;
        levelIndicator.text = "Level " + Level;
        StartCoroutine(IntroduceWave());
    }
    public void ResetWaves()
    {

        Level++;
        waveIndicatorPanel.SetActive(true);
        if (Level == 3)
            EntitiesPerInterval = 20;
        gateIndex = 0;
        arrowPointers[gateIndex].SetActive(true);
        SpawnPoints.Clear();
        boss_spawned = false;
        miniboss_spawned = false;
        GetComponent<HazardController>().Level++;
        Transform[] points = Gates[gateIndex].GetComponentsInChildren<Transform>();
        for (int i = 1; i < points.Length; i++)
            SpawnPoints.Add(points[i]);


        lastSpawned = 0f;

        // At the start, depict that it is wave 1
        num_of_waves = 1;


        MeleeCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].MeleeNumber;
        RangedCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].RangedNumber;
        TankCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].TankNumber;
        SupportCount = WaveProfilesPerLevel[Level - 1][num_of_waves - 1].SupportNumber;

        SpawnNumber = MeleeCount + RangedCount + SupportCount + TankCount;
        num_enemies = SpawnNumber;

        slider.maxValue = num_enemies;
        

        wave_cleared = true;
        StartCoroutine(BossWaveEnd());
    }

}
