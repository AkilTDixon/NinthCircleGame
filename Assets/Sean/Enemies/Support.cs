using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Support : MonoBehaviour
{
    public enum SupportType
    {
        AOE, Chains
    }

    public SupportType supportType = SupportType.AOE;

    [Header("AOE")]
    [SerializeField] private Transform AOE;
    bool shrinkAOE;

    [Header("CHAINS")]
    [SerializeField] private Shopkeeper shopkeeper;
    [SerializeField] private GameObject tetherPrefab;
    private GameObject tetherInstance;
    private Hovl_Tether tetherScript;

    [Header("ATTACK")]
    public float visibilityRange = 30.0f;
    [SerializeField] private float shotDuration = 5.0f;
    [SerializeField] private float shotSpeed = 5.0f;
    [SerializeField] private float angleOfView = 60f;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float damage = 40f;
    [SerializeField] GameObject shotPrefab;
    [SerializeField] private bool applyKnockBack = true;
    [SerializeField] private float knockBackForce = 3f;
    [SerializeField] AOECircle circle;
    [SerializeField] float DefenseBuff;
    [SerializeField] float AttackBuff;
    [SerializeField] float MovementSpeedBuff;
    [SerializeField] float AttackSpeedBuff;
    [SerializeField] int HealthRegenAmount;
    [SerializeField] float HealthRegenTick;

    [SerializeField] Skeleton_Follow2 moveScript;
    private Transform player;
    bool isDead = false;

    void Start()
    {
        if (circle != null)
        {
            circle.defenseBuff = DefenseBuff;
            circle.attackBuff = AttackBuff;
            circle.movementSpeedBuff = MovementSpeedBuff;
            circle.attackSpeedBuff = AttackSpeedBuff;
            circle.healthRegenTick = HealthRegenTick;
            circle.healthRegenAmt = HealthRegenAmount;
        }//allies = GameObject.FindGameObjectsWithTag("Enemy");
        player = PlayerInfoScript.Instance.player;

        if(supportType == SupportType.Chains)
        {
            shopkeeper = GameObject.Find("ShopKeeper").GetComponent<Shopkeeper>();
            Invoke("AddChains", 2f);    // 2 second delay before chains go up
        }    
    }

    public void AddChains()
    {
        shopkeeper.ModifyLocks(1);
        //Enable lazer
        tetherInstance = Instantiate(tetherPrefab, transform.position + new Vector3(0,1,0), transform.rotation);
        tetherInstance.transform.parent = transform;
        tetherScript = tetherInstance.GetComponent<Hovl_Tether>();  
        tetherScript.target = shopkeeper.transform;
    }

    public void Shoot()
    {
        if (!canAttack)
            return;

        Vector3 vecToPlayer = (player.position - transform.position);
        GameObject shot = Instantiate(shotPrefab,
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
            Quaternion.LookRotation(vecToPlayer.normalized, Vector3.up));

        ParticleCollisionInstance particleScript = shot.GetComponent<ParticleCollisionInstance>();
        particleScript.DamageDone = moveScript.DamageAmount;
        if (applyKnockBack)
            particleScript.forceToApply = knockBackForce;

        EnemyShotScript script = shot.GetComponent<EnemyShotScript>();
        script.speed = shotSpeed;

        Destroy(shot, shotDuration);
    }

    private void Update()
    {    
        if(supportType == SupportType.AOE)
        {
            // Shrink AOE
            if (isDead && AOE.localScale != Vector3.zero)
            {
                AOE.localScale -= Vector3.one * Time.deltaTime * 2;

                if (AOE.localScale.x < 0)
                    AOE.localScale = Vector3.zero;
            }
        }  
        else if (supportType == SupportType.Chains)
        {
            //Disable lazer prefab
            if (isDead)
            {
                if (tetherScript) tetherScript.DisablePrepare();
                Destroy(tetherInstance, 1);
            }
        }
    }

    public void OnDeath()
    {
        isDead = true;

        if (supportType == SupportType.Chains)
            shopkeeper.ModifyLocks(-1);
    }
}
