using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTurret : MonoBehaviour
{
    public GameObject projectile;
    public float FreezeDuration = 1.5f;
    public float FreezeAmount = 0.5f;
    public float TimeBetweenShots = 0.25f;
    public int ProjectileDamage = 1;
    public float cooldown = 2f;
    private float lastShot = 0f;
    private bool shooting = false;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerInfoScript.Instance.player;
        lastShot = Time.time;
    }

    void Update()
    {
        if (Time.time > lastShot + cooldown && !shooting)
        {
            StartCoroutine(spawnProj());
        }
    }

    private IEnumerator spawnProj()
    {
        shooting = true;
        for (int i = 0; i < 5; i++)
        {
            GameEvents.current.kaeneOrbShoot(this.gameObject);
            Vector3 vecToPlayer = (player.position - transform.position);
            GameObject shot = Instantiate(projectile,
                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                Quaternion.LookRotation(vecToPlayer.normalized, Vector3.up));

            ParticleCollisionInstance particleScript = shot.GetComponent<ParticleCollisionInstance>();

            particleScript.DamageDone = ProjectileDamage;
            Buff frostBuff = shot.AddComponent<Buff>();
            frostBuff.buffType = Buff.BuffType.FREEZE;
            frostBuff.duration = FreezeDuration;
            frostBuff.value = FreezeAmount;
            frostBuff.isHelpful = false;
            frostBuff.text = "Freeze!";

            yield return new WaitForSeconds(TimeBetweenShots);
        }
        lastShot = Time.time;
        shooting = false;
        
    }
}
