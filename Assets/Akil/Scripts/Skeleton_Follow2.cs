using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_Follow2 : MonoBehaviour
{

    public enum EnemyType
    {
        Melee,
        Ranged,
        Tank,
        Support
    };

    [Header("NPC Stats")]
    public float hp = 100;
    public int maxHP = 100;
    public int DamageAmount = 5;
    public float defense = 1;
    public float attackSpeed = 1f;
    public EnemyType EnemyBehavior = EnemyType.Melee;

    [Header("MISC")]
    //Transform that NPC has to follow 
    public Transform playerTransform;
    public static List<Transform> allyTransforms = new List<Transform>();
    public Transform transformToFollow;
    [SerializeField] Material DamageTaken;
    [SerializeField] MeshCollider ThisMeshCollider;
    [SerializeField] BoxCollider ThisBoxCollider;
    [SerializeField] SkinnedMeshRenderer ChildMeshRenderer;
    [SerializeField] MeshRenderer ChildRenderer;
    [SerializeField] GameObject ChargingHitBox;
    public bool Miniboss = false;

    private bool TakenDamage = false;
    private Material originalMaterial;

    //NavMesh Agent variable d
    public NavMeshAgent agent;
    public Animator animator;
    private bool Dead = false;
    private float damageTime = 0f;
    [HideInInspector] public bool ApplyForce = false;
    [HideInInspector] public bool ApplyExplosiveForce = false;
    [HideInInspector] public float ExplosivePower;
    [HideInInspector] public float ExplosiveRadius;
    [HideInInspector] public Vector3 ExplosionPosition;
    [HideInInspector] public Vector3 forceDirection;


    [SerializeField] private GlorySystem gs;
    private float reviveTime = 0f;
    private bool playerDied = false;
    private bool revived = false;
    private float waitTime = 0.5f;
    private float visibilityRange = 0f;
    private bool debuffed = false;
    private bool PlayerDead = false;
    [HideInInspector] public bool rootMotion = false;
    private bool DefenseUp = false;
    private bool Stuck = false;
    private bool CheckingPos = false;
    private bool MeleeRange = false;
    private bool Roaring = false;
    private bool TankPointReached = false;
    private Vector3 TankPoint;
    private bool Searching = false;
    private float tankBaseDefense;
    private bool CheckingStuck = false;
    private float elapsedTimeSinceLastFootstep;

    // Start is called before the first frame update 
    void Start()
    {




        agent = GetComponent<NavMeshAgent>();
        if (EnemyBehavior != EnemyType.Support)
            allyTransforms.Add(transform);
/*        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;*/
        
        if (agent.speed >= 6f)
            animator.SetFloat("animSpeed", agent.speed / 6f);
        else
            animator.SetFloat("animSpeed", 1f);
        animator.SetFloat("attackSpeed", attackSpeed);
        if (ChildMeshRenderer != null)
            originalMaterial = ChildMeshRenderer.material;
        else
            originalMaterial = ChildRenderer.material;

        playerTransform = GameObject.Find("PlayerCapsule").transform;
        if (EnemyBehavior == EnemyType.Support)
            FindAllyToFollow();
        else
            transformToFollow = playerTransform;


        //PlayerObject = GameObject.Find("Player"); 
        
        
        gs = GameObject.Find("GlorySystem").GetComponent<GlorySystem>();
        rootMotion = animator.hasRootMotion;
        if (EnemyBehavior == EnemyType.Ranged)
            visibilityRange = GetComponent<RangedDPS>().visibilityRange;
        if (EnemyBehavior == EnemyType.Support)
            visibilityRange = GetComponent<Support>().visibilityRange;


        TankPoint = playerTransform.position;
        if (EnemyBehavior == EnemyType.Tank)
        {   
            tankBaseDefense = defense;
            DefenseUp = true;
            animator.SetFloat("speedh", 0);
            //agent.speed = 45;
            agent.transform.forward = transformToFollow.position - agent.transform.position;
            Roaring = true;
            animator.Play("Roar");

            Stuck = false;
        }
        elapsedTimeSinceLastFootstep = 0.0f;
    }
    void CleanList()
    {
        for (int i = 0; i < allyTransforms.Count; i++)
            if (allyTransforms[i] == null)
                allyTransforms.RemoveAt(i);
    }
    public void FindAllyToFollow()
    {
        //Random.Range(0, allyTransforms.Count)
        if (allyTransforms.Count > 0)
        {
            transformToFollow = allyTransforms[Random.Range(0, allyTransforms.Count)];
            if (transformToFollow == null)
                CleanList();
        }
        else
            transformToFollow = playerTransform;
    }

    private IEnumerator CheckForTransforms()
    {
        Searching = true;
        if (allyTransforms.Count > 0)
        {
            FindAllyToFollow();
        }
        yield return new WaitForSeconds(10f);
        Searching = false;
    }
    // Update is called once per frame 
    void Update()
    {
        if (TakenDamage && damageTime == 0f)
        {
            damageTime = Time.time;
            if (ChildMeshRenderer != null)
                ChildMeshRenderer.material = DamageTaken;
            else
                ChildRenderer.material = DamageTaken;
        }

        if (Time.time > damageTime + 0.15f && TakenDamage)
        {
            TakenDamage = false;
            damageTime = 0f;
            if (ChildMeshRenderer != null)
                ChildMeshRenderer.material = originalMaterial;
            else
                ChildRenderer.material = originalMaterial;

        }

        if (hp <= 0 && !Dead)
        {
            if (ChargingHitBox != null)
                Destroy(ChargingHitBox);
            if (gs != null)
                gs.AddGlory(50f);
            /*agent.destination = agent.transform.position;*/
            /*agent.speed = 0;*/

            if (EnemyBehavior == EnemyType.Support)
                GetComponent<Support>().OnDeath();

            allyTransforms.Remove(transform);
            Dead = true;
            // Update the number of dead enemies in the SpawnScript 
            
            //ThisMeshCollider.enabled = false; 
            animator.SetFloat("speedh", 0);
/*            GameEvents.current.onPlayerDeath -= PlayerDeathEvent;
            GameEvents.current.onPlayerRevive -= PlayerReviveEvent;*/
            GameEvents.current.enemyDeath();
            GetComponent<EnemyExperience>().TransferXP();

            if ((ApplyExplosiveForce || ApplyForce) && GetComponent<ApplyRigidbody>() != null)
                GetComponent<ApplyRigidbody>().enabled = true;
            else
            {
                if (ThisMeshCollider != null)
                    ThisMeshCollider.enabled = false;
                else
                    ThisBoxCollider.enabled = false;
                animator.Play("Fall1");

                //animator.SetBool("Fall1", true);

            }

            //IEnumerator coroutine = DestroySkeleton(); 
            //StartCoroutine(coroutine); 

            //Destroy(gameObject); 
        }
        else if ((ApplyExplosiveForce || ApplyForce) && GetComponent<ApplyRigidbody>() != null)
        {
            GetComponent<ApplyRigidbody>().enabled = true;
            ApplyExplosiveForce = false;
            ApplyForce = false;
        }
        /*animator.SetBool("Attack1h1", false);*/
        
        if (animator.GetFloat("speedh") > 0) {
            elapsedTimeSinceLastFootstep += Time.deltaTime;
                if (isTimeToTakeAfootStep()) {
                    GameEvents.current.enemyFootstep(this.gameObject, EnemyBehavior.ToString().ToLower());
                }
        }
    }
    public void Death()
    {
        if (Miniboss)
            GetComponent<DropWeapon>().Drop();

        SpawnScript.num_enemies -= 1;
        Destroy(gameObject);
    }

    public void CheckInflictDamage()
    {
        if ((agent.transform.position - transformToFollow.position).magnitude < 5f && !PlayerDead)
            GameEvents.current.playerHit(DamageAmount);
    }
    public void GrappleDebuff()
    {
        StartCoroutine(debuff());
    }
    private IEnumerator debuff()
    {
        debuffed = true;
        float temp = defense;
        defense = 0.5f;
        yield return new WaitForSeconds(10f);
        defense = temp;
        debuffed = false;
    }
    private IEnumerator CheckIfStuck()
    {
        CheckingStuck = true;
        Vector3 startPos = agent.transform.position;
        yield return new WaitForSeconds(0.5f);
        if ((startPos - agent.transform.position).magnitude < 2f)
        {
            if (Roaring)
            {
                Roaring = false;
            }
        }
        CheckingStuck = false;
    }
    private void FixedUpdate()
    {
        if (!PlayerInfoScript.Instance.Dead)
        {
            if (playerDied)
            {
                playerDied = false;
                revived = true;
                reviveTime = Time.time;
            }
            if (revived)
            {

                if (Time.time > reviveTime + waitTime)
                    revived = false;

            }
            else if (agent.isOnNavMesh)
            {
                if (!Dead && !PlayerDead)
                {
                    if (agent.isStopped)
                        agent.isStopped = false;

                    if (EnemyBehavior == EnemyType.Support)
                    {
                        if (transformToFollow != null)
                        {
                            if (transformToFollow != playerTransform && transformToFollow.GetComponent<Skeleton_Follow2>().Dead)
                            {
                                FindAllyToFollow();
                            }
                            else if (transformToFollow == playerTransform && !Searching)
                                StartCoroutine(CheckForTransforms());
                        }
                        else if (!Searching)
                            StartCoroutine(CheckForTransforms());
                    }

                    if (EnemyBehavior == EnemyType.Melee)
                    {
                        MeleeMovement();
                    }
                    else if (EnemyBehavior == EnemyType.Ranged)
                    {
                        RangedMovement();
                    }
                    else if (EnemyBehavior == EnemyType.Support)
                    {
                        SupportMovement();
                    }
                    else if (EnemyBehavior == EnemyType.Tank)
                    {
                        TankMovement();
                    }
                }
                else
                    agent.isStopped = true;
            }
        }
        else
        {
            if (!playerDied)
            {   agent.SetDestination(agent.transform.position);
                playerDied = true;
            }
        }
    }
    private void RangedMovement()
    {

        if ((transform.position - playerTransform.position).magnitude < visibilityRange)
        {
            Ray r = new Ray(agent.transform.position, playerTransform.position - agent.transform.position);
            RaycastHit hit;
            Physics.Raycast(r, out hit, visibilityRange, ~LayerMask.GetMask("Shop"));
            if (hit.collider == null || hit.collider.tag == "Player")
            {
                if (agent.isOnNavMesh)
                    agent.isStopped = true;
                animator.SetBool("Attack1h1", true);
                animator.SetFloat("speedh", 0);
                agent.transform.forward = playerTransform.position - agent.transform.position;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(transformToFollow.position);
                animator.SetBool("Attack1h1", false);
                animator.SetFloat("speedh", agent.speed);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(transformToFollow.position);
            animator.SetBool("Attack1h1", false);
            animator.SetFloat("speedh", agent.speed);
        }
    }
    private void TankMovement()
    {
        /* 
                     If player is close to the tank after the 2nd roar check, start attack animation 
                     */
        if ((agent.transform.position - transformToFollow.position).magnitude < 5f)
        {

            DefenseUp = true;
            animator.SetFloat("speedh", 0);

            //agent.speed = 0;
            agent.transform.forward = transformToFollow.position - agent.transform.position;
            if (MeleeRange)
            {
                Roaring = false;
                animator.SetBool("Attack1h1", true);

            }
            else
            {
                animator.SetBool("Attack1h1", false);
                Roaring = true;
                MeleeRange = true;
                animator.Play("Roar");
                GameEvents.current.tankRoar(this.gameObject);
            }

        }
        else if (!TankPointReached && (agent.transform.position - TankPoint).magnitude < 5f)
        {

            TankPointReached = true;
            Roaring = true;
            DefenseUp = true;
            MeleeRange = false;
            animator.SetFloat("speedh", 0);
            //agent.speed = 0;
            agent.transform.forward = transformToFollow.position - agent.transform.position;
            animator.SetBool("Attack1h1", false);
            animator.Play("Roar");
            GameEvents.current.tankRoar(this.gameObject);

        }
        else if (!Roaring && !agent.pathPending)
        {

            Roaring = true;
            DefenseUp = true;
            MeleeRange = false;
            animator.SetFloat("speedh", 0);
            //agent.speed = 0;
            agent.transform.forward = transformToFollow.position - agent.transform.position;
            animator.SetBool("Attack1h1", false);
            animator.Play("Roar");
            GameEvents.current.tankRoar(this.gameObject);
        }
        else if (!CheckingStuck && !DefenseUp)
                StartCoroutine(CheckIfStuck());
    }
    private void MeleeMovement()
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
    private void SupportMovement()
    {   if (transformToFollow != null)
        {
            if (transformToFollow != playerTransform && (transform.position - transformToFollow.position).magnitude > 5f)
            {
                agent.isStopped = false;
                agent.SetDestination(transformToFollow.position);
                animator.SetBool("Attack1h1", false);
                animator.SetFloat("speedh", agent.speed);
            }
            else
            {
                RangedMovement();
            }
        }
    }

    public void ActivateDamageBox()
    {
        if (ChargingHitBox != null)
            if (ChargingHitBox.GetComponent<TankChargeBox>() != null)
                ChargingHitBox.GetComponent<TankChargeBox>().enabled = true;
    }
    public void DeactivateDamageBox()
    {
        if (ChargingHitBox != null)
            if (ChargingHitBox.GetComponent<TankChargeBox>() != null)
                ChargingHitBox.GetComponent<TankChargeBox>().enabled = false;
    }
    public void InitiateCharge()
    {
        bool viablePath = false;
        TankPointReached = false;
        ActivateDamageBox();
        Vector3 dir = (transformToFollow.position - agent.transform.position).normalized;
        TankPoint = transformToFollow.position + (dir * 5f);

        NavMeshPath navPath = new NavMeshPath();
        NavMeshHit nhit = new NavMeshHit();
        
        int areaMask = NavMesh.GetAreaFromName("Not Walkable") | NavMesh.GetAreaFromName("Walkable");
        NavMesh.SamplePosition(TankPoint, out nhit, 5f, areaMask);
        if (nhit.hit)  
            NavMesh.CalculatePath(agent.transform.position, nhit.position, areaMask, navPath);
        else
        {
            TankPoint = transformToFollow.position;
            NavMesh.SamplePosition(TankPoint, out nhit, 5f, areaMask);

            if (nhit.hit)
                NavMesh.CalculatePath(agent.transform.position, nhit.position, areaMask, navPath);
            else
            {
                viablePath = false;
                Roaring = false;
                MeleeRange = false;
                return;
            }
        }
        if (navPath.status == NavMeshPathStatus.PathComplete)
        {
            viablePath = true;
            agent.transform.forward = transformToFollow.position - agent.transform.position;
            //agent.speed = 60;
            agent.SetDestination(nhit.position);
            animator.SetFloat("speedh", agent.speed);
            DefenseUp = false;
        }
        

        if (!viablePath)
        {
            Roaring = false;
            MeleeRange = false;
        }





    }

    public void TakeDamageWithExplosiveForce(float value, float power, Vector3 position, float radius)
    {
        GameEvents.current.hitEnemy();
        hp -= value;
        if (gs != null)
            gs.AddGlory(value);
        ApplyExplosiveForce = true;
        ExplosionPosition = position;
        ExplosivePower = power;
        ExplosiveRadius = radius;
        TakenDamage = true;
    }
    public void TakeDamageWithForce(float value, Vector3 direction)
    {
        GameEvents.current.hitEnemy();
        hp -= (value / defense);
        if (gs != null)
            gs.AddGlory(value);
        ApplyForce = true;
        forceDirection = direction;
        TakenDamage = true;
    }
    public void TakeDamage(float value, int weaponID)
    {
        if (EnemyBehavior == EnemyType.Tank)
        {
            if (!debuffed)
            {
                if (DefenseUp)
                    defense = 5f * tankBaseDefense;
                else
                    defense = tankBaseDefense;

                if (weaponID == 0)
                    defense *= 4f;
                else if (weaponID == 1 | weaponID == 2)
                    value *= 1.5f;
            }
        }
        //if (!DefenseUp) 
        //{ 
        GameEvents.current.hitEnemy();
        if (weaponID == 2)
            hp -= (value / (defense * 0.5f));
        else  
            hp -= (value / defense);

        if (gs != null)
            gs.AddGlory(value);
        TakenDamage = true;
        //} 
    }
    public void Heal(int value)
    {
        if (hp < maxHP)
            hp += value;

        if (hp > maxHP)
            hp = maxHP;
    }
    public float GetCurrentHealth()
    {
        return hp;
    }

    private bool isTimeToTakeAfootStep() {
        if ((elapsedTimeSinceLastFootstep * agent.speed * 0.5) >= 1){
            elapsedTimeSinceLastFootstep = 0;
            return true;
        }
        return false;
    }


}