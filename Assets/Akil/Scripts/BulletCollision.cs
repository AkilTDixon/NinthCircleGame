using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField] GameObject HitEffectPrefab;
    [SerializeField] int Life = 5;
    public bool Explosive = false;
    public float ExplosiveRadius = 0;
    public float ExplosivePower = 0;
    private int deathCounter = 0;
    private float CurrentDamage = 0;
    private string[] colException = {"PlayerCapsule", "Player"};
    private bool Multishot = false;
    private float splitTime = 0;
    private Vector3 bulFacing;
    private float velocityFactor = 0;

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.name == gameObject.name)
            return;

        bool viableHit = false;
        for (int i = 0; i < colException.Length; i++)
            if (collision.gameObject.name != colException[i])
                viableHit = true;

        if (viableHit)
        {
            GameObject hitEffect = Instantiate(HitEffectPrefab, transform.position, HitEffectPrefab.transform.rotation);
            GameEvents.current.rocketHit();
            if (Explosive)
            {
                Collider[] col = Physics.OverlapSphere(hitEffect.transform.position, ExplosiveRadius);
                foreach (Collider hit in col)
                {
                    if (hit.name != gameObject.name)
                    {
                        float dmg = CurrentDamage;

                        if (PlayerInfoScript.easyMode)
                            dmg *= 1.5f;

                        if (hit.GetComponent<Skeleton_Follow2>() != null)
                            hit.GetComponent<Skeleton_Follow2>().TakeDamageWithExplosiveForce(dmg, ExplosivePower, hitEffect.transform.position, ExplosiveRadius);
                        else if (hit.GetComponent<EndenorScript>() != null)
                            hit.GetComponent<EndenorScript>().TakeDamage(dmg, 1);
                        else if (hit.GetComponent<DemonBoss>() != null)
                            hit.GetComponent<DemonBoss>().TakeDamage(dmg, 3);
                    }
                }

            }
            


            Destroy(gameObject);
        }
    }
    public void SetDamage(float value)
    {
        CurrentDamage = value;
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (Multishot)
        {
            if (Time.time > splitTime + 0.15f)
            {

                
                float offset = 1;
                Vector3 f = transform.forward;
                Vector3 u = transform.up;
                Vector3 r = transform.right;

                Vector3[] bulCoord = {transform.position + (-f * offset), transform.position + (-u * offset), transform.position + (u * offset), transform.position + (f * offset) };
                for (int i = 0; i < 4; i++)
                {
                    Vector3 dir = ((bulCoord[i] + (r * 6f)) - transform.position).normalized;
                    GameObject newBul = Instantiate(gameObject, transform.position, Quaternion.FromToRotation(bulFacing, dir));
                    
                    newBul.GetComponent<Rigidbody>().velocity = dir * velocityFactor;
                    newBul.GetComponent<BulletCollision>().SetDamage(CurrentDamage);
                }



                Destroy(gameObject);
            }
        }
        deathCounter++;
        if (deathCounter > Life * 100)
            Destroy(gameObject);

    }
    public void SetMultishot(Vector3 facing, float velocity)
    {
        Multishot = true;
        velocityFactor = velocity;
        bulFacing = facing;
        splitTime = Time.time;
    }
}
