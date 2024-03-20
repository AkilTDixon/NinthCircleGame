using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class RangedDPS : MonoBehaviour
{
    // Start is called before the first frame update

    public enum RangedDPSType
    {
        FIRE, FROST, MAGE
    }

    public RangedDPSType DPSType = RangedDPSType.FIRE;

    public float visibilityRange = 30.0f;
    [SerializeField] private float shotDuration = 3.0f;
    [SerializeField] private float shotSpeed = 10.0f;
    [SerializeField] private float angleOfView = 60f;
    public float damage = 40f;

    [Header("FIRE")]
    [SerializeField] GameObject shotPrefabFire;
    
    [Header("FROST")]
    [SerializeField] GameObject shotPrefabFrost;
    public bool applyFreeze = true;
    public float freezeSlowdown = 1.0f;
    public float freezeDuration = 3.0f;
    
    [Header("MAGE")]
    [SerializeField] GameObject shotPrefabMage;
    public bool applyKnockBack = true;
    public float knockBackForce = 3f;

    private GameObject shotPrefab;
    [SerializeField] Skeleton_Follow2 moveScript;
    private Transform player;

    void Start()
    {
        player = PlayerInfoScript.Instance.player;
        switch (DPSType)
        {
            case RangedDPSType.FIRE:
                shotPrefab = shotPrefabFire;
                break;
            case RangedDPSType.FROST:
                shotPrefab = shotPrefabFrost;
                break;
            case RangedDPSType.MAGE:
                shotPrefab = shotPrefabMage;
                break;
        }
    }
    public void Shoot()
    {
        Vector3 vecToPlayer = (player.position - transform.position);
        GameObject shot = Instantiate(shotPrefab,
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
            Quaternion.LookRotation(vecToPlayer.normalized, Vector3.up));
        
        ParticleCollisionInstance particleScript = shot.GetComponent<ParticleCollisionInstance>();
        particleScript.DamageDone = moveScript.DamageAmount;
        if (DPSType == RangedDPSType.FROST) {
            Buff frostBuff = shot.AddComponent<Buff>();
            frostBuff.buffType = Buff.BuffType.FREEZE;
            frostBuff.duration = freezeDuration;
            frostBuff.value = freezeSlowdown;
            frostBuff.isHelpful = false;
            frostBuff.text = "Freeze!";
        }

        if (DPSType == RangedDPSType.MAGE) {
            if (applyKnockBack)
                particleScript.forceToApply = knockBackForce;
        }
        
        
        EnemyShotScript script = shot.GetComponent<EnemyShotScript>();
        script.speed = shotSpeed;
        script.duration = shotDuration;
        Destroy(shot, shotDuration);

        
    }

    //Original commented out in case needed/wanted later

    // Update is called once per frame
    /*    void Update() {
            canShoot = ((timeOfLastShot + shotDelay) < Time.time);
            if (canShoot) {
                // Check if we have visibility to the player
                Vector3 vecToPlayer = (player.position - transform.position);
                if (vecToPlayer.magnitude < visibilityRange) {
                    Debug.DrawRay(transform.position, vecToPlayer, Color.red, 10f);
                    timeOfLastShot = Time.time;
                    GameObject shot = Instantiate(shotPrefab,
                        new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
                        Quaternion.LookRotation(vecToPlayer.normalized, Vector3.up));
                    EnemyShotScript script = shot.GetComponent<EnemyShotScript>();
                    script.direction = vecToPlayer.normalized;
                    script.speed = shotSpeed;
                    Destroy(shot,shotDuration);

                }
            }



        }*/
}
