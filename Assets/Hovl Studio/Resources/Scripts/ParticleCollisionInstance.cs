/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ParticleCollisionInstance : MonoBehaviour
{
    public GameObject[] EffectsOnCollision;
    public float DestroyTimeDelay = 5;
    public bool UseWorldSpacePosition;
    public float Offset = 0;
    public Vector3 rotationOffset = new Vector3(0,0,0);
    public bool useOnlyRotationOffset = true;
    public bool UseFirePointRotation;
    public bool DestoyMainEffect = false;
    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    //private ParticleSystem ps;
    public int DamageDone = 0;
    public float forceToApply = 0f;
    public bool DoesAreaDamage = false;
    public float DamageRadius = 1f;
    public bool LightningStrike = false;





    void Start()
    {
        part = GetComponent<ParticleSystem>();

    }
    void OnParticleCollision(GameObject other)
    {   
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        bool playerHit = false;
        if (DoesAreaDamage)
        {
            Collider[] col = Physics.OverlapSphere(collisionEvents[0].intersection + collisionEvents[0].normal * Offset, DamageRadius);
            foreach (Collider hit in col)
                if (hit.tag == "Player")
                {
                    playerHit = true;
                    break;
                }
        }

        if (other.tag == "Player" && PlayerInfoScript.Instance.GetHP() > 0 || playerHit) {
            GameEvents.current.playerHit(DamageDone);
            transferBuffsToPlayer();
            if (forceToApply > 0f && numCollisionEvents > 0) {
                Vector3 dir = collisionEvents[0].normal;
          
                if (LightningStrike)
                {
                    PlayerInfoScript.Instance.movementScript.rb.velocity = Vector3.zero;

                    PlayerInfoScript.Instance.movementScript.rb.AddForce(Vector3.up * (forceToApply * 20f));

                }
                else
                    PlayerInfoScript.Instance.movementScript.rb.AddForce(dir * (forceToApply * -1f), ForceMode.Impulse);
                GameEvents.current.playerKnockBack();    
            }
        }
            
        for (int i = 0; i < numCollisionEvents; i++)
        {
            foreach (var effect in EffectsOnCollision)
            {
                var instance = Instantiate(effect, collisionEvents[i].intersection + collisionEvents[i].normal * Offset, new Quaternion()) as GameObject;
                if (!UseWorldSpacePosition) instance.transform.parent = transform;
                if (UseFirePointRotation) { instance.transform.LookAt(transform.position); }
                else if (rotationOffset != Vector3.zero && useOnlyRotationOffset) { instance.transform.rotation = Quaternion.Euler(rotationOffset); }
                else
                {
                    instance.transform.LookAt(collisionEvents[i].intersection + collisionEvents[i].normal);
                    instance.transform.rotation *= Quaternion.Euler(rotationOffset);
                }
                Destroy(instance, DestroyTimeDelay);
            }
        }
        if (DestoyMainEffect == true)
        {
            Destroy(gameObject, DestroyTimeDelay + 0.5f);
        }
    }

    void transferBuffsToPlayer() {
        Buff[] buffs = GetComponents<Buff>();
        foreach (Buff b in buffs) {
            Buff newBuff=PlayerInfoScript.Instance.AddComponent<Buff>();
            newBuff.copyFrom(b);
            newBuff.Activate();
            Destroy(newBuff, b.duration);
        }


    }
    

}
