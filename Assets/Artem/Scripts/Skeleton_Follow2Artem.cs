using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_Follow2Artem : MonoBehaviour
{

    public enum EnemyType
    {
        Melee,
        Ranged,
        Tank,
        Support
    };

    //Transform that NPC has to follow
    public Transform transformToFollow;
    [SerializeField] Material DamageTaken;
    [SerializeField] MeshCollider ThisMeshCollider;
    [SerializeField] SkinnedMeshRenderer ChildMeshRenderer;
    [SerializeField] int DamageAmount = 5;
    private bool TakenDamage = false;
    private Material originalMaterial;
    //private GameObject PlayerObject;
    //NavMesh Agent variable
    NavMeshAgent agent;
    public Animator animator;
    public int hp = 2;
    private bool Dead = false;
    private float damageTime = 0f;
    private bool ApplyForce = false;
    [HideInInspector] public float ExplosivePower;
    [HideInInspector] public float ExplosiveRadius;
    [HideInInspector] public Vector3 ExplosionPosition;

 
    [SerializeField] private GlorySystem gs;

    public EnemyType EnemyBehavior = EnemyType.Melee;
    private float visibilityRange = 0f;

    private bool PlayerDead = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;

        originalMaterial = ChildMeshRenderer.material;
        //PlayerObject = GameObject.Find("Player");
        transformToFollow = GameObject.Find("PlayerCapsule").transform;
        gs = GameObject.Find("GlorySystem").GetComponent<GlorySystem>();

        if (EnemyBehavior == EnemyType.Ranged) 
            visibilityRange = GetComponent<RangedDPS>().visibilityRange;

    }

    // Update is called once per frame
    void Update()
    {
        if (TakenDamage && damageTime == 0f)
        {
            damageTime = Time.time;
            ChildMeshRenderer.material = DamageTaken;
        }

        if (Time.time > damageTime + 0.15f && TakenDamage)
        {
            TakenDamage = false;
            damageTime = 0f;
            ChildMeshRenderer.material = originalMaterial;
        }

        if(hp <= 0 && !Dead)
        {

            /*agent.destination = agent.transform.position;*/
            /*agent.speed = 0;*/
            Dead = true;
            // Update the number of dead enemies in the SpawnScript
            SpawnScriptArtem.num_enemies_dead += 1;
            //ThisMeshCollider.enabled = false;
            animator.SetFloat("speedh", 0);
            
            if (ApplyForce)
                GetComponent<ApplyRigidbody>().enabled = true;           
            else
            {
                ThisMeshCollider.enabled = false;
                animator.SetBool("Fall1", true);
                IEnumerator coroutine = DestroySkeleton();
                StartCoroutine(coroutine);
            }

            //IEnumerator coroutine = DestroySkeleton();
            //StartCoroutine(coroutine);

            //Destroy(gameObject);
        }
        /*animator.SetBool("Attack1h1", false);*/
    }

    public IEnumerator DestroySkeleton() {
        GetComponent<EnemyExperience>().TransferXP();
        yield return new WaitForSeconds(2);
        GameEvents.current.enemyDeath();
        GameEvents.current.onPlayerDeath -= PlayerDeathEvent;
        GameEvents.current.onPlayerRevive -= PlayerReviveEvent;
        Destroy(gameObject);
    }
    public void CheckInflictDamage()
    {
        if ((agent.transform.position - transformToFollow.position).magnitude < 5f && !PlayerDead)
            GameEvents.current.playerHit(DamageAmount);
        
    }
    private void FixedUpdate()
    {
        
         
        if (!Dead && !PlayerDead)
        {
            if (agent.isStopped)
                agent.isStopped = false;
            if (EnemyBehavior == EnemyType.Melee)
            {
                if ((agent.transform.position - transformToFollow.position).magnitude < 2f)
                {
                    animator.SetBool("Attack1h1", true);
                    animator.SetFloat("speedh", 0);
                }
                else
                {
                    agent.SetDestination(transformToFollow.position);
                    animator.SetBool("Attack1h1", false);
                    animator.SetFloat("speedh", agent.speed);
                }
            }
            else if (EnemyBehavior == EnemyType.Ranged)
            {
                if ((agent.transform.position - transformToFollow.position).magnitude < visibilityRange)
                {
                    agent.isStopped = true;
                    animator.SetBool("Attack1h1", true);
                    animator.SetFloat("speedh", 0);
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(transformToFollow.position);
                    animator.SetBool("Attack1h1", false);
                    animator.SetFloat("speedh", agent.speed);
                }
            }
        }
        else
            agent.isStopped = true;
    }
    


    public void TakeDamageWithForce(float value, float power, Vector3 position, float radius)
    {
        GameEvents.current.hitEnemy();
        hp -= (int)value;
        if (gs != null)
            gs.AddGlory(value);
        ApplyForce = true;
        ExplosionPosition = position;
        ExplosivePower = power;
        ExplosiveRadius = radius;
        TakenDamage = true;
    }

    public void TakeDamage(float value)
    {
        GameEvents.current.hitEnemy();
        hp -= (int)value;
        if(gs != null)
            gs.AddGlory(value);
        TakenDamage = true;
    }

    public void PlayerDeathEvent()
    {
        PlayerDead = true;
    }
    public void PlayerReviveEvent()
    {        
        StartCoroutine(ResumeDelay());
    }
    private IEnumerator ResumeDelay()
    {
        for (int i = 0; i < 1; i++)
            yield return new WaitForSeconds(0.4f);

        PlayerDead = false;

    }

}
