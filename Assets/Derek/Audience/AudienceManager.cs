using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudienceManager : MonoBehaviour
{
    private enum AudienceState {
            all_cheer,
            cheer,
            clap,
            stand,
            sit
    }

    private AudienceState currentAudienceState;
    //Singleton Pattern
    public static AudienceManager Instance { get; set; }
    private List<Transform> fans = new List<Transform>();
    private List<Animator> fanAnimators = new List<Animator>();

    [Header("Glory Favor")]
    [SerializeField] private float favor = 0;

    [SerializeField] private float minDeltaFavorChangeToUpdateAnimations = 0.1f;
    [SerializeField] private float allCheerThreshold = 0.9f;
    [SerializeField] private float cheerThreshold = 0.85f;
    [SerializeField] private float clapThreshold = 0.75f;
    [SerializeField] private float standThreshold = 0.50f;
    [SerializeField] private GameObject giftPrefab;
    [SerializeField] private float healthChance = 0.05f;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private GameObject increaseMaxHPPrefab;
    [SerializeField] private GameObject prefabXPMultiplier;
    [SerializeField] private GameObject[] AmmoPrefabs;
    [SerializeField] float SecondaryGiftChance = 0.05f;
    [SerializeField] float RPGAmmoDropChance = 0.05f;
    [SerializeField] float ShotgunAmmoDropChance = 0.35f;
    [SerializeField] private float increaseMaxHPCHance = 0.05f;
    [SerializeField] private Transform giftSpawnPosition;
    [SerializeField] private Vector3 giftThrowForce = new Vector3(0, 10, 100);
    [SerializeField] private float giftSecondsBetweenChanceToGetGift = 5.0f;
    public List<Transform> audienceGiftSpawnPoints;
    // Start is called before the first frame update
    private float baseDropChance;
    private float baseRPGDropChance;
    private float baseShotgunDropChance;
    private float nextGiftTest = 0;
    
    

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            Debug.Log("Destroying extra instance of AudienceManage (This is a Singleton!)");
            return;
        }
        
        Instance = this;
        audienceGiftSpawnPoints = new List<Transform>();
    }
    void Start() {
        nextGiftTest = Time.time + giftSecondsBetweenChanceToGetGift;

        baseDropChance = SecondaryGiftChance;
        baseRPGDropChance = RPGAmmoDropChance;
        baseShotgunDropChance = ShotgunAmmoDropChance;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Equals)) 
            setFavor(favor + 0.1f);
        if (Input.GetKeyDown(KeyCode.Minus)) 
            setFavor(favor - 0.1f);
        if (Time.time > nextGiftTest) 
            checkIfAudienceGivesGift();
        
    }

    public void setFavor(float newFavor) {
        bool updateAnimations = false;
        AudienceState newAudienceState;

        switch (true) {
            case var value when newFavor >= allCheerThreshold: {
                newAudienceState = AudienceState.all_cheer;
                break;
            }
            case var value when newFavor >= cheerThreshold: {
                newAudienceState = AudienceState.cheer;
                break;
            }
            case var value when newFavor >= clapThreshold: {
                newAudienceState = AudienceState.clap;
                break;
            }
            case var value when newFavor >= standThreshold: {
                newAudienceState = AudienceState.stand;
                break;
            }
            default: {
                newAudienceState = AudienceState.sit;
                break;
            }
        }

        if(newAudienceState != currentAudienceState){
            GameEvents.current.newAudienceFavor(newAudienceState.ToString());
            updateAnimations = true;
            currentAudienceState = newAudienceState;
        }

        favor = newFavor;
        if (updateAnimations) {
            foreach (Animator a in fanAnimators) {
                float roll = favor - (Random.Range(0f,0.4f));
                if (favor >= allCheerThreshold) {
                    if (Random.value > 0.5f)
                        a.Play("Cheer");
                    else
                        a.Play("CheerMirror");
                }
                else if (roll > cheerThreshold) {
                    if (Random.value > 0.5f)
                        a.Play("Cheer");
                    else
                        a.Play("CheerMirror");
                }
                else if (roll > clapThreshold) {
                    a.Play("Clap");
                }
                else if (roll > standThreshold) {
                    a.Play("Stand");
                }
                else {
                    a.Play("Sit");
                }
            }
        }
    }

    public void registerFan(Transform fan) {
        // all fans from all sections should register themselves here.
        fans.Add(fan);
        fanAnimators.Add(fan.GetComponent<Animator>());
    }

    private void checkIfAudienceGivesGift() {
        nextGiftTest = Time.time + giftSecondsBetweenChanceToGetGift;
        if (Random.value < favor)
            createGift();
    }
    
    public static void createGift() {
        Transform origin = AudienceManager.Instance.giftSpawnPosition;
        List<Transform> sections = AudienceManager.Instance.audienceGiftSpawnPoints;
        Vector3 throwForce = Vector3.zero;
        if (sections.Count > 0) {
            origin = sections[Random.Range(0, sections.Count)];
            throwForce = AudienceManager.Instance.giftThrowForce;
        }
        GameObject mainGift;
        GameObject secondGift = null;
        float secondGiftRan = Random.value;
        float healthRan = Random.value;
        float ammoRan = Random.value;
        float increaseHPRan = Random.value;

        //if RPG isn't unlocked
        if (!PlayerInfoScript.Instance.weaponUnlocked[1])
            Instance.RPGAmmoDropChance = 0;
        else
            Instance.RPGAmmoDropChance = Instance.baseRPGDropChance;

        //if Shotgun isn't unlocked
        if (!PlayerInfoScript.Instance.weaponUnlocked[2])
            Instance.ShotgunAmmoDropChance = 0;
        else
            Instance.ShotgunAmmoDropChance = Instance.baseShotgunDropChance;


        if (secondGiftRan >= 1f - AudienceManager.Instance.SecondaryGiftChance)
        {
            if (healthRan >= 1f - AudienceManager.Instance.healthChance)
            {
                secondGift = Instantiate(AudienceManager.Instance.healthPrefab, origin.position, Quaternion.LookRotation(origin.forward));
            }
            else
            {
                float ran = Random.value;
                if (ran > (1f - Instance.RPGAmmoDropChance))
                    secondGift = Instantiate(AudienceManager.Instance.AmmoPrefabs[2], origin.position, Quaternion.LookRotation(origin.forward));
                else if (ran > (1f - Instance.ShotgunAmmoDropChance))
                    secondGift = Instantiate(AudienceManager.Instance.AmmoPrefabs[1], origin.position, Quaternion.LookRotation(origin.forward));
                else
                    secondGift = Instantiate(AudienceManager.Instance.AmmoPrefabs[0], origin.position, Quaternion.LookRotation(origin.forward));
            }
        }
        if (increaseHPRan >= 1f - Instance.increaseMaxHPCHance) {
            mainGift = Instantiate(AudienceManager.Instance.increaseMaxHPPrefab, origin.position, Quaternion.LookRotation(origin.forward));
            Buff buff = mainGift.AddComponent<Buff>();
            buff.buffType = Buff.BuffType.INCREASE_MAX_HP;
            buff.duration = 300;
            buff.value = 5f;
            buff.text = "+" + ((int)buff.value) + " Max HP";
            buff.isHelpful = true;
            buff.tick = 0;
        }
        else
        {

            mainGift = Instantiate(AudienceManager.Instance.prefabXPMultiplier, origin.position, Quaternion.LookRotation(origin.forward));
            Buff buff = mainGift.AddComponent<Buff>();
            buff.buffType = Buff.BuffType.XP_MULTIPLIER;
            buff.text = "+1% XP";
            buff.duration = 120;
            buff.value = 0.01f;
            buff.isHelpful = true;
            buff.tick = 0;
        }
        //GameObject gift = Instantiate(AudienceManager.Instance.giftPrefab, origin.position, Quaternion.LookRotation(origin.forward));
        if (secondGift != null)
        {
            secondGift.GetComponent<Rigidbody>().AddRelativeForce(throwForce, ForceMode.Impulse);
            //secondGift.layer = LayerMask.NameToLayer("AudienceGift");
        }
        mainGift.GetComponent<Rigidbody>().AddRelativeForce(throwForce, ForceMode.Impulse);
        //mainGift.layer = LayerMask.NameToLayer("AudienceGift");
        
    }
}
