using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KaeneScript : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] float HP = 4000f;
    [SerializeField] float maxHP = 4000f;
    [SerializeField] int mainAttackDamage = 25;
    [SerializeField] int ultimateDamage = 75;
    [SerializeField] float defense = 1.5f;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] private float TransitionTimer = 30f;
    [SerializeField] private float ChannelingTimer = 8f;
    [SerializeField] float FreezeDuration = 0.5f;
    [SerializeField] float FreezeAmount = 1.5f;

    [Header("Effect and Projectile Objects")]
    [SerializeField] private GameObject ChannelingAura;
    [SerializeField] private GameObject MainAttack;
    [SerializeField] private GameObject Orb;
    [SerializeField] private GameObject AOE;


    [Header("Minions")]
    [SerializeField] private GameObject MinionPrefab;
    [SerializeField] private int NumberOfMinions = 5;
    [SerializeField] private float MinionSpawnInterval = 10f;
    [SerializeField] private Transform[] MinionSpawnPoints;

    [Header("Misc")]
    [SerializeField] Transform RightHand;
    [SerializeField] Transform OrbPosition1;
    [SerializeField] Transform OrbPosition2;
    [SerializeField] GameObject SnowFX;
    [SerializeField] GameObject SnowSquall;
    [SerializeField] GameObject bossIceClump;
    [SerializeField] GameObject IcePositions;
    [SerializeField] private GlorySystem gs;


    private bool EndChanneling = false;
    private Animator anim;
    private bool transitionStart = false;
    private bool channeling = false;
    private float TimeUntilTransition = 0f;
    private float ChannelingStart = 0f;
    private float lastSpawned = 0f;
    private bool TakenDamage = false;
    private GameObject auraInstance;
    private GameObject orbInstance1;
    private GameObject orbInstance2;
    private GameObject iceInstance;
    private Transform player;
    private Slider slider;
    private bool debuffed;
    private bool dead = false;
    // Start is called before the first frame update
    void Start()
    {

        PlayerInfoScript.Instance.bossElementsHolder.SetActive(true);
        PlayerInfoScript.Instance.bossNameText.text = "LORD KAENE";
        slider = PlayerInfoScript.Instance.bossElementsHolder.GetComponentInChildren<Slider>();
        slider.maxValue = maxHP;
        slider.value = maxHP;
        SnowFX.SetActive(false);
        SnowSquall.SetActive(true);
        gs = GameObject.Find("GlorySystem").GetComponent<GlorySystem>();
        anim = GetComponent<Animator>();
        TimeUntilTransition = Time.time;
        player = GameObject.Find("PlayerCapsule").transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (HP <= 0 && !dead)
        {
            anim.Play("death");
            GameEvents.current.kaeneDeath(this.gameObject);
            dead = true;
        }
        else if (!dead)
        {
            transform.forward = player.position - transform.position;
            if (!transitionStart)
            {
                if (Time.time > TimeUntilTransition + TransitionTimer)
                {
                    transitionStart = true;
                    anim.SetBool("mainAttack", false);
                    anim.Play("SpawnOrb");

                }
                else
                {
                    anim.SetBool("mainAttack", true);
                    anim.SetFloat("attackSpeed", attackSpeed);
                    if (Time.time > lastSpawned + MinionSpawnInterval)
                    {
                        lastSpawned = Time.time;
                        SpawnMinions(MinionPrefab, NumberOfMinions);
                    }
                }
            }
            else
            {
                if (channeling)
                {
                    if (Time.time > ChannelingTimer + ChannelingStart && !EndChanneling)
                    {
                        EndChanneling = true;
                        anim.SetBool("endChannel", true);
                    }
                }
            }
        }
    }
    public void Death()
    {
        
        if (orbInstance1 != null)
            Destroy(orbInstance1);
        if (orbInstance2 != null)
            Destroy (orbInstance2);
        if (auraInstance != null)
            Destroy(auraInstance);
        if (iceInstance != null)
            Destroy(iceInstance);

        GetComponent<EnemyExperience>().TransferXP();
        PlayerInfoScript.Instance.bossElementsHolder.SetActive(false);
        PlayerInfoScript.Instance.bossNameText.text = "";
        SpawnScript.Instance.ResetWaves();
        SnowFX.SetActive(true);
        SnowSquall.SetActive(false);
        StartCoroutine(delay());
    }
    private IEnumerator delay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    public void SpawnOrbs()
    {
        orbInstance1 = Instantiate(Orb, OrbPosition1.position, transform.rotation);
        orbInstance2 = Instantiate(Orb, OrbPosition2.position, transform.rotation);
        Transform[] positions = IcePositions.GetComponentsInChildren<Transform>();
        int ran = Random.Range(1, positions.Length);

        GameEvents.current.kaeneIceSpawn(this.gameObject);
        iceInstance = Instantiate(bossIceClump, positions[ran].position, bossIceClump.transform.rotation);



    }
    public void StartChanneling()
    {
        GameEvents.current.kaeneChannelStart();
        auraInstance = Instantiate(ChannelingAura, transform.position, transform.rotation);
        channeling = true;
        ChannelingStart = Time.time;
    }
    public void CastMainAttack()
    {
        Vector3 vec1 = (player.position - RightHand.position); 

        Quaternion rotation1 = Quaternion.LookRotation(vec1.normalized, Vector3.up);    
        GameObject shot1 = Instantiate(MainAttack, RightHand.position, rotation1);
        ApplyFrost(shot1);

        Vector3 vec2 = shot1.transform.forward + (shot1.transform.right * 0.15f);
        Quaternion rotation2 = Quaternion.LookRotation(vec2.normalized, Vector3.up);
        GameObject shot2 = Instantiate(MainAttack, RightHand.position, rotation2);
        ApplyFrost(shot2);

        Vector3 vec3 = shot1.transform.forward + ((-shot1.transform.right) * 0.15f);
        Quaternion rotation3 = Quaternion.LookRotation(vec3.normalized, Vector3.up);
        GameObject shot3 = Instantiate(MainAttack, RightHand.position, rotation3);
        ApplyFrost(shot3);


        int dmg = Mathf.CeilToInt(PlayerInfoScript.Instance.GetMaxHP() * 0.05f);
        ParticleCollisionInstance particleScript = shot1.GetComponent<ParticleCollisionInstance>();
        particleScript.DamageDone = dmg;

        particleScript = shot2.GetComponent<ParticleCollisionInstance>();
        particleScript.DamageDone = dmg;

        particleScript = shot3.GetComponent<ParticleCollisionInstance>();
        particleScript.DamageDone = dmg;
    }
    void ApplyFrost(GameObject shot)
    {
        Buff frostBuff = shot.AddComponent<Buff>();
        frostBuff.buffType = Buff.BuffType.FREEZE;
        frostBuff.duration = FreezeDuration;
        frostBuff.value = FreezeAmount;
        frostBuff.isHelpful = false;
        frostBuff.text = "Freeze!";
    }
    public void CastAOE()
    {
        GameEvents.current.kaeneAOE(this.gameObject);

        Instantiate(AOE, transform.position, transform.rotation);
        Ray r = new Ray(transform.position + (Vector3.up * 2f), player.position - transform.position);
        RaycastHit hit;
        
        Debug.DrawLine(r.origin, player.position, Color.red, 500f);
        if (Physics.Raycast(r, out hit, Mathf.Infinity, LayerMask.GetMask("Arena", "whatIsGround", "Player")))
        {

            if (hit.collider.tag != "IceClump")
            {
                Vector3 dir = (-Camera.main.transform.forward);
                dir.y = 0.25f;

                GameObject p = PlayerInfoScript.Instance.PlayerObject;
                p.GetComponent<Rigidbody>().AddForce((dir * 100f), ForceMode.Impulse);
                GameEvents.current.playerHit(Mathf.CeilToInt(PlayerInfoScript.Instance.GetMaxHP() * 0.65f));

            }
        }
        else 
        {
            Vector3 dir = (-Camera.main.transform.forward);
            dir.y = 0.25f;

            GameObject p = PlayerInfoScript.Instance.PlayerObject;
            p.GetComponent<Rigidbody>().AddForce((dir * 100f), ForceMode.Impulse);
            GameEvents.current.playerHit(ultimateDamage);
        }
    }
    public void EndAura()
    {
        GameEvents.current.kaeneChannelEnd();
        Destroy(iceInstance);
        Destroy(auraInstance);
        Destroy(orbInstance1);
        Destroy(orbInstance2);
        channeling = false;
        EndChanneling = false;
        transitionStart = false;
        anim.SetBool("endChannel", false);
        TimeUntilTransition = Time.time;

    }

    public void TakeDamage(float value, int weaponID)
    {

        GameEvents.current.hitEnemy();
        HP -= (value / defense);
        if (gs != null)
            gs.AddGlory(value);

        slider.value = HP;
        TakenDamage = true;

    }
    public void SpawnMinions(GameObject enemy, int num)
    {
        for (int i = 0; i < num; i++)
        {
            Transform spawnPoint = MinionSpawnPoints[Random.Range(0, MinionSpawnPoints.Length)];
            Instantiate(enemy, spawnPoint.position, enemy.transform.rotation);
        }

    }
    public void GrappleDebuff()
    {
        StartCoroutine(debuff());
    }
    private IEnumerator debuff()
    {
        debuffed = true;
        float temp = defense;
        defense = 0.65f;
        yield return new WaitForSeconds(5f);
        defense = temp;
        debuffed = false;
    }
}
