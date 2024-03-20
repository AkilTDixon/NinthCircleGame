using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EndenorScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float HP = 12000f;
    [SerializeField] float maxHP = 12000f;
    [SerializeField] int Damage = 15;
    [SerializeField] float defense = 1f;
    [SerializeField] float WindPhaseDuration = 15f;
    [SerializeField] float StunDuration = 3f;


    [Header("Effects")]
    [SerializeField] GameObject HandEffect;
    [SerializeField] GameObject AuraEffect;
    [SerializeField] GameObject WindRow1;
    [SerializeField] GameObject WindRow2;

    [Header("Misc")]
    [SerializeField] private GlorySystem gs;
    public Rotator rotateScript;
    public Afterimages afterimages;
    public Animator anim;
    public NavMeshAgent agent;
    public GameObject Player;
    public Transform AnchorPoint;
    public BoxCollider ThisBoxCollider;
    public GameObject meleeWeapon;
    public GameObject middleRow1;
    public GameObject middleRow2;

    private float TransportSpeed = 25f;
    private bool stunned = false;
    private bool channeling = false;
    private bool transport = false;
    private float windPhaseStart = 0f;
    private bool dodging = false;
    private Vector3 dodgePoint;
    private Slider slider;
    private GameObject auraInstance;
    private int StunCounter = 0;
    private int StunThreshold = 1;
    private bool debuffed;
    private bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        
        PlayerInfoScript.Instance.bossElementsHolder.SetActive(true);
        PlayerInfoScript.Instance.bossNameText.text = "PRINCE ENDENOR";
        slider = PlayerInfoScript.Instance.bossElementsHolder.GetComponentInChildren<Slider>();
        slider.maxValue = maxHP;
        slider.value = maxHP;
        gs = GameObject.Find("GlorySystem").GetComponent<GlorySystem>();
        WindRow1.SetActive(true);
        WindRow2.SetActive(true);
        WindRow1.SetActive(false);
        WindRow2.SetActive(false);
        Player = PlayerInfoScript.Instance.PlayerObject;
        anim.SetFloat("forwardSpeed", 0.5f);
        agent.SetDestination(Player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0 && !dead)
        {
            anim.Play("Death");
            if (auraInstance != null)
                Destroy(auraInstance);
            Destroy(WindRow1);
            Destroy(WindRow2);
            GameEvents.current.endenorDeath(this.gameObject);
            dead = true;

        }
        else if (!dead)
        {
            if (!stunned && !channeling && !dodging)
            {
                if (auraInstance != null)
                    Destroy(auraInstance);
                if (!afterimages.enabled)
                    ToggleImages(true);

                //if (agent.transform.position != Player.transform.position)
                    //agent.transform.LookAt(Player.transform.position);
                Movement();
            }
            else if (transport && channeling)
            {
                if (transform.position != AnchorPoint.position)
                {
                    float step = TransportSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, AnchorPoint.position, step);
                }
                else
                {

                    ToggleImages(false);
                    agent.enabled = true;
                    ThisBoxCollider.enabled = true;
                    transport = false;
                    anim.SetFloat("animSpeed", 1);
                    StartCoroutine(WindPhase());
                }
            }
            else if (transport && dodging)
            {
                if (transform.position != dodgePoint)
                {
                    float step = TransportSpeed * 1.5f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, dodgePoint, step);
                }
                else
                {
                    transport = false;
                    dodging = false;
                    agent.enabled = true;
                }
            }
        }
    }
    public void CheckInflictDamage()
    {
        GameEvents.current.endenorSwing(this.gameObject);
        int dmg = Mathf.CeilToInt(PlayerInfoScript.Instance.GetMaxHP() * 0.2f);
        if ((agent.transform.position - Player.transform.position).magnitude < 12f)
            GameEvents.current.playerHit(dmg);
    }
    public void Cast()
    {
        if (!HandEffect.activeSelf)
        {   GameEvents.current.endenorCast(this.gameObject);
            HandEffect.SetActive(true);
        }
        else
        {
            GameEvents.current.endenorWindStart(middleRow1);
            GameEvents.current.endenorWindStart(middleRow2);
            HandEffect.SetActive(false);
            auraInstance = Instantiate(AuraEffect, transform.position, AuraEffect.transform.rotation);
            WindRow1.SetActive(true);
            WindRow2.SetActive(true);
        }
    }
    public void Death()
    {
        if (auraInstance != null)
            Destroy(auraInstance);


        GetComponent<EnemyExperience>().TransferXP();
        meleeWeapon.GetComponent<MeleeScript>().EndenorDead();
        
        ToggleImages(false);
        PlayerInfoScript.Instance.bossElementsHolder.SetActive(false);
        PlayerInfoScript.Instance.bossNameText.text = "";
        SpawnScript.Instance.ResetWaves();

        StartCoroutine(delay());
    }
    private IEnumerator delay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private IEnumerator WindPhase()
    {

        yield return new WaitForSeconds(WindPhaseDuration);
        GameEvents.current.endenorWindEnd();
        if (WindRow1 != null)
        {
            WindRow1.SetActive(false);
            WindRow2.SetActive(false);
        }
        channeling = false;
    }
    void Movement()
    {
        if ((agent.transform.position - Player.transform.position).magnitude < 5f)
        {
            anim.SetFloat("forwardSpeed", 0);
            anim.SetBool("attack", true);

        }
        else
        {
            anim.SetFloat("forwardSpeed", 0.5f);
            anim.SetBool("attack", false);
            agent.SetDestination(Player.transform.position);
        }
    }
    void Dodge()
    {
        GameEvents.current.endenorDodge(this.gameObject);
        transport = true;
        dodging = true;
        agent.enabled = false;
        int radius = 5;
        float ranXOffset = Random.Range(-radius, radius);
        float ranZOffset = Random.Range(-radius, radius);

        Vector3 p = transform.position;
        p.x += ranXOffset;
        p.z += ranZOffset;

        Ray r = new Ray(transform.position, (p - transform.position));
        RaycastHit hit;

        //if (Physics.Raycast(r, out hit,(p - transform.position).magnitude, LayerMask.NameToLayer("Arena")))
        //{
            while (Physics.Raycast(r, out hit, (p - transform.position).magnitude))
            {
                ranXOffset = Random.Range(-radius, radius);
                ranZOffset = Random.Range(-radius, radius);

                p = transform.position;
                p.x += ranXOffset;
                p.z += ranZOffset;

                r = new Ray(transform.position, (p - transform.position));
            }
        //}
        dodgePoint = p;
    }
    public void TakeDamage(float value, int id)
    {
        if (transport)
            return;


        //Melee Weapon
        if ((id == 0 || id == 2) && !stunned && !channeling)
            Dodge();
        else
        {
            if ((id == 3 || id == 1) && !stunned && !channeling)
            {
                GameEvents.current.endenorHit(this.gameObject);
                StunCounter++;
                if (StunCounter == StunThreshold)
                {
                    Stunned();
                    StunCounter = 0;
                    StunThreshold++;
                    if (StunThreshold > 3)
                        StunThreshold = 3;
                }
                else
                    anim.Play("TakeHit");
            }
            float mult = 1f;
            if (stunned)
                mult = 10f;

            GameEvents.current.hitEnemy();
            HP -= (value / defense);
            if (gs != null)
                gs.AddGlory(value*mult);

            slider.value = HP;
        }
    }
    void Stunned()
    {
        GameEvents.current.endenorStun(this.gameObject);
        anim.Rebind();
        ToggleImages(false);
        stunned = true;
        rotateScript.enabled = true;
        StartCoroutine(StunWait());
    }
    void ToggleImages(bool b)
    {
        for (int i = 0; i < afterimages.imageNumber; i++)
            afterimages.shadow[i].SetActive(b);
        afterimages.enabled = b;
    }

    private IEnumerator StunWait()
    {
        yield return new WaitForSeconds(StunDuration);
        if (HP > 0)
        {
            rotateScript.enabled = false;
            ToggleImages(true);
            stunned = false;
            channeling = true;
            anim.Play("Teleport");
        }
    }

    public void TeleportToAnchor()
    {
        GameEvents.current.endenorDodge(this.gameObject);
        ThisBoxCollider.enabled = false;
        transport = true;
        agent.enabled = false;
        anim.SetFloat("animSpeed", 0); 
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
