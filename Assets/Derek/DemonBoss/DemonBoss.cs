using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemonBoss : MonoBehaviour
{

    private enum AttackType
    {
        SWORD, ROAR, WHIP
    }

    [Header("Stats")]
    [SerializeField] float HP = 6000f;
    [SerializeField] float maxHP = 6000f;
    [SerializeField] float defense = 1f;
    [SerializeField] private GlorySystem gs;

    [Header("Movement")]
    [SerializeField] private float angleOfAttack = 15f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Attack")]
    [SerializeField] private float minDelayBetweenAttacks = 2f;

    [SerializeField] private AttackType nextRangedAttack = AttackType.ROAR;


    [Header("Whip")] public GameObject whipTip;
    [SerializeField] private int whipDamage = 40;
    [SerializeField] private float whipExplosionRadius = 8f;
    [SerializeField] private float whipExplosionPower = 8f;
    [SerializeField] private float whipExplosionLift = 1f;
    [SerializeField] private ParticleSystemRenderer whipParticle;
    [SerializeField] private float rangeWhip = 19f;
    [SerializeField] private float rangeWhipMin = 14f;

    [Header("Roar")]
    [SerializeField] private ParticleSystemRenderer particleRoar;
    [SerializeField] private Transform roarPosition;
    [SerializeField] private float rangeRoar = 24f;

    [Header("Sword")]
    [SerializeField] private float rangeSword = 6f;
    [SerializeField] private int swordDamage = 70;
    [SerializeField] private GameObject swordSphere;
    public bool inSwordStrikeRange = false; //updatd by sphere collider



    [Header("Other")]
    [SerializeField] private float delayBetweenStaggerAnimation = 3f;

    [Header("Spawns")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private bool spawnNextPause;
    [SerializeField] private ParticleSystemRenderer spawnParticleEffect;
    [SerializeField] private float spawnNextHPPercentage = 0.9f;
    [SerializeField] private float spawnNextHPPercentageStep = 0.40f;

    [Header("Entrance")]
    [SerializeField] private Transform parentDemonBossObject;
    [SerializeField] private Transform seatPosition;
    [SerializeField] private SkinnedMeshRenderer renderToHideWhileSeated1;
    [SerializeField] private SkinnedMeshRenderer renderToHideWhileSeated2;
    public bool startEntranceSequenceNextUpdate = false;
    [SerializeField] private float entranceSpeed = 2.5f;
    [SerializeField] private float heightToRiseTo = 24f;
    [SerializeField] private float forwardDistanceToMoveTo = 10;
    [SerializeField] private float descendTo = 0.11f;
    [SerializeField] private PlayerCamera playerCameraScript;

    [Header("Death")] public GameObject DeathCamera;
    [SerializeField] private GameObject ExitToMainMenuButton;
    [SerializeField] private GameObject winText;





    private bool dead = false;
    private NavMeshAgent agent;
    private Transform player;
    private bool isAttacking = false;
    private Animator animator;
    private bool onAttckCoolDown = false;
    private bool swordCausesDamage = false;
    private Slider slider;
    private bool TakenDamage = false;
    private bool canPlayStaggerAnimation = true;
    private bool isSeated = true;
    private bool isDoingEntrance = false;
    private bool reachedHeight = false;
    private bool reachedForwardPosition = false;
    private bool reachedFloor = false;
    private bool entranceComplete = false;






    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = PlayerInfoScript.Instance.player;
        animator = GetComponent<Animator>();
        slider = PlayerInfoScript.Instance.bossElementsHolder.GetComponentInChildren<Slider>();


        isSeated = true;
        if (renderToHideWhileSeated1 != null)
            renderToHideWhileSeated1.enabled = false;
        if (renderToHideWhileSeated2 != null)
            renderToHideWhileSeated2.enabled = false;
        parentDemonBossObject.position = seatPosition.position;
        playerCameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerCamera>();
        Activate();
        
    }



    // Update is called once per frame
    void FixedUpdate()
    {

        if (!entranceComplete)
        {
            checkAndHandleEntranceComplete();
            return;
        }


        if (Input.GetKeyDown(KeyCode.K)) { //todo remove this
            HP = 0;
        }

        bool isFacingPlayerForAttack = false;
        bool closeEnough = false;

        if (HP <= 0) {
            if (!dead) {
                dead = true;
                StartDeathSequence();
            }
            else
                return;
        }
        if (!dead)
        {
            isFacingPlayerForAttack = FacePlayer();
            if (!isFacingPlayerForAttack)
                moveTowardsPlayer();
            else
            {
                if (!isAttacking && !PlayerInfoScript.Instance.Dead)
                {
                    if (inSwordStrikeRange)
                        startSwordAttack();
                    else
                    {

                        SpawnCheck();
                        // Range attack
                        float distanceToPlayer = (player.position - transform.position).magnitude;
                        // Debug.Log("Boss Distance = " + distanceToPlayer);
                        if (distanceToPlayer <= rangeWhip && distanceToPlayer >= rangeWhipMin)
                        {
                            if (isFacingPlayerForAttack && !onAttckCoolDown)
                            {
                                startWhipAttack();
                            }
                            else
                            {
                                animator.SetBool("isWalking", false);
                            }
                        }
                        else if (distanceToPlayer <= rangeRoar)
                            startRoarAttack();
                        else
                            moveTowardsPlayer();
                    }
                }
            }
        }
    }

    private void checkAndHandleEntranceComplete()
    {
        // return value:  false = entrance not complete
        if (entranceComplete)
            return;
        if (startEntranceSequenceNextUpdate)
        {
            startEntranceSequenceNextUpdate = false;
            StartEntrance();
        }

        if (isDoingEntrance)
        {

            if (!reachedHeight)
            {
                reachedHeight = parentDemonBossObject.position.y >= heightToRiseTo;
                parentDemonBossObject.position += new Vector3(0, entranceSpeed, 0) * Time.deltaTime;
            }
            if (!reachedForwardPosition && reachedHeight)
            {
                reachedForwardPosition = parentDemonBossObject.position.x >= forwardDistanceToMoveTo;
                parentDemonBossObject.position += new Vector3(entranceSpeed, 0, 0) * Time.deltaTime;
            }
            if (!reachedFloor && reachedForwardPosition)
            {
                reachedFloor = parentDemonBossObject.position.y <= descendTo;
                parentDemonBossObject.position -= new Vector3(0, entranceSpeed, 0) * Time.deltaTime;
            }

            if (reachedFloor)
            {
                animator.SetBool("isSummoningWeapons", true);
                if (renderToHideWhileSeated1 != null)
                    renderToHideWhileSeated1.enabled = true;
                if (renderToHideWhileSeated2 != null)
                    renderToHideWhileSeated2.enabled = true;
            }


        }
    }


    private void SpawnCheck()
    {
        if (spawnNextPause)
            SpawnAdds();
        if (HP / maxHP <= spawnNextHPPercentage)
        {
            spawnNextPause = true;
            spawnNextHPPercentage -= spawnNextHPPercentageStep;
        }
    }

    private void SpawnAdds()
    {
        spawnNextPause = false;
        if (spawnPrefab != null)
        {
            foreach (Transform t in spawnPoints)
            {
                GameObject s = Instantiate(spawnPrefab, t.position, t.rotation);
                Instantiate(spawnParticleEffect, t.position, Quaternion.LookRotation(Vector3.up));
            }
        }

    }


    private bool FacePlayer() //returns if within attack angle

    {
        Vector3 vecB2P = player.position - transform.position;
        vecB2P.y = 0;
        Quaternion rotation = Quaternion.LookRotation(vecB2P);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed);
        return Vector3.Angle(vecB2P, transform.forward) <= angleOfAttack;
    }

    private void moveTowardsPlayer()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
        }
    }

    private void startWhipAttack()
    {
        isAttacking = true;
        agent.enabled = false;
        animator.SetBool("isWalking", false);
        animator.Play("WhipAttack");
    }
    private void startSwordAttack()
    {
        isAttacking = true;
        agent.enabled = false;
        animator.SetBool("isWalking", false);
        animator.Play("SwordAttack1");
    }
    private void startRoarAttack()
    {
        isAttacking = true;
        agent.enabled = false;
        animator.SetBool("isWalking", false);
        animator.Play("Roar");
    }


    private void moveToSwordRange()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
            agent.stoppingDistance = rangeSword;
        }
    }



    public void Activate()
    {
        GameEvents.current.demonBossCreate(transform);
    }


    public void WhipStart()
    {
        GameEvents.current.demonWhipStart(this.gameObject);
    }

    public void SwordAttack()
    {
        swordCausesDamage = true;
        GameEvents.current.demonBossSwordAttack(transform);
    }

    public void WhipCrack()
    {
        Debug.Log("WhipCrack EVENT called");
        GameEvents.current.demonBossWhipCrack(this.gameObject);
        if (whipParticle)
            Instantiate(whipParticle, whipTip.transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(whipTip.transform.position, whipExplosionRadius);
        bool hitPlayer = false;
        foreach (Collider hit in colliders)
        {
            if (hit.attachedRigidbody)
            {
                hit.attachedRigidbody.AddExplosionForce(whipExplosionPower, whipTip.transform.position,
                    whipExplosionRadius, whipExplosionLift, ForceMode.Impulse);
                if (hit.CompareTag("Player"))
                    hitPlayer = true;
            }
        }
        if (hitPlayer)
            PlayerInfoScript.Instance.TakeDamage(whipDamage);

    }

    public void Roar()
    {
        ParticleSystemRenderer part = Instantiate(particleRoar, roarPosition.position,
            Quaternion.LookRotation(roarPosition.forward));
        GameEvents.current.demonBossRoar(this.gameObject);
    }

    public void Footstep()
    {
        GameEvents.current.demonBossFootstep(this.gameObject);
    }

    public void WingFlap()
    {
        GameEvents.current.demonBossWingFlap(this.gameObject);
    }

    public void EndAttack()
    {
        isAttacking = false;
        swordCausesDamage = false;
        agent.enabled = true;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
        onAttckCoolDown = true;
        StartCoroutine(AllowNextAttack());
    }


    IEnumerator AllowNextAttack()
    {
        yield return new WaitForSeconds(minDelayBetweenAttacks);
        onAttckCoolDown = false;
    }

    public void swordStrikingPlayer()
    {
        // called by animation's collider (DemonBossSword)
        if (swordCausesDamage)
            PlayerInfoScript.Instance.TakeDamage(swordDamage);
    }

    public void TakeDamage(float value, int weaponID)
    {
        if (!entranceComplete)
            return;
        GameEvents.current.hitEnemy();
        HP -= (value / defense);
        if (gs != null)
            gs.AddGlory(value);

        slider.value = HP;
        TakenDamage = true;
        if (!isAttacking && canPlayStaggerAnimation)
        {
            animator.Play("GetHit1");
            canPlayStaggerAnimation = false;
            StartCoroutine(ResetStaggerAnimation());
        }
        Debug.Log($"Demon Boss Took {value} damage");

        IEnumerator ResetStaggerAnimation()
        {
            yield return new WaitForSeconds(delayBetweenStaggerAnimation);
            canPlayStaggerAnimation = true;
        }

    }

    public void Death()
    {
        

        StartCoroutine(delay());
        //todo call End Game screen
    }
    private IEnumerator delay()
    {
        yield return new WaitForSeconds(20f);  // this is the final boss
        Destroy(gameObject);
    }

    public void StartEntrance()
    {
        PlayerInfoScript.Instance.finalBossIntro = true;
        isDoingEntrance = true;
        playerCameraScript.lockOnTo = parentDemonBossObject;
        animator.SetBool("isFlying", true);
        GameEvents.current.demonBossintroSequence();
    }

    public void EntranceComplete()
    { // called by animation event
        entranceComplete = true;
        playerCameraScript.lockOnTo = null;
        slider.enabled = true;
        PlayerInfoScript.Instance.finalBossIntro = false;
        PlayerInfoScript.Instance.bossElementsHolder.SetActive(true);
        PlayerInfoScript.Instance.bossNameText.text = "LORD TOLOMEA";
        slider.maxValue = maxHP;
        slider.value = maxHP;
    }

    public void StartDeathSequence() {
        agent.enabled = false;
        GameObject.Find("PlayerCapsule").GetComponent<CapsuleCollider>().enabled = false;
        PlayerInfoScript.Instance.finalBossKilled = true;
        GameObject.Find("Main Camera").SetActive(false);
        DeathCamera.SetActive(true);
        GameEvents.current.demonBossDeath(this.gameObject);
        animator.Play("Death");
        
        //Destroy all other enemies
        DestrObj[] enemies = GameObject.FindObjectsOfType<DestrObj>();
        foreach (DestrObj e in enemies) {
            e.DestroyObj();
        }
        // Destroy any existing shots
        EnemyShotScript[] shots = GameObject.FindObjectsOfType<EnemyShotScript>();
        foreach (EnemyShotScript s in shots)
            Destroy(s.GameObject());
        //Hide UI
        PlayerInfoScript.Instance.hideUIElementsForIntroOrOutro();
        PlayerInfoScript.Instance.bossElementsHolder.SetActive(false);
        PlayerInfoScript.Instance.bossNameText.text = "";
        slider.enabled = false;
        
        ExitToMainMenuButton.SetActive(false);
        winText.SetActive(false);

        StartCoroutine(ShowExitButtonAfterPause());
        IEnumerator ShowExitButtonAfterPause() {
            yield return new WaitForSeconds(3);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ExitToMainMenuButton.SetActive(true);
            winText.SetActive(true);
        }
        
        
    }

    public void ExitToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}