using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleTalisman : TalismanBase
{

    private float Reach = 5f;
    private float DistancePerTick = 2f;
    private LayerMask hitLayers;
    [SerializeField] GameObject Indicator;
    private GameObject hitObject;
    private Vector3 hitPoint;
    private float pullSpeed = 25f;
    private Skeleton_Follow2.EnemyType eType = Skeleton_Follow2.EnemyType.Melee;

    // Start is called before the first frame update
    protected override void Start()
    {
        hitLayers = LayerMask.GetMask("Enemy") | LayerMask.GetMask("whatIsGround") | LayerMask.GetMask("Arena");
        base.Start();
        switch (rarity)
        {
            case Rarity.Low:
                Reach = 12f;
                DistancePerTick = 2f;
                Cooldown = 20f;          
                break;
            case Rarity.Medium:
                Reach = 24f;
                DistancePerTick = 4f;
                Cooldown = 15f;
                break;
            case Rarity.High:
                Reach = 30f;
                DistancePerTick = 5f;
                Cooldown = 15f;
                break;
            case Rarity.Ultra:
                Reach = 35f;
                DistancePerTick = 7f;
                Cooldown = 10f;
                break;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (hitObject != null)
        {

            float step = pullSpeed * Time.deltaTime;
            if (!hitObject.CompareTag("Boss") && hitObject.layer == LayerMask.NameToLayer("Enemy") && eType != Skeleton_Follow2.EnemyType.Tank)
            {
                
                hitObject.transform.position = Vector3.MoveTowards(hitObject.transform.position, transform.position, step);

                if (Vector3.Distance(hitObject.transform.position, transform.position) < 1f)
                {
                    hitObject.GetComponent<Animator>().enabled = true;
                    hitObject.GetComponent<Animator>().Play("Idle - Walk - Run");
                    
                    hitObject = null;

                }
            }
            else if (hitObject.layer == LayerMask.NameToLayer("whatIsGround") || hitObject.layer == LayerMask.NameToLayer("Arena") || eType == Skeleton_Follow2.EnemyType.Tank)
            {
                PlayerInfoScript.Instance.player.position = Vector3.MoveTowards(PlayerInfoScript.Instance.player.position, hitPoint, step);

                if (Vector3.Distance(hitPoint, PlayerInfoScript.Instance.player.position) < 1f)
                {                    
                    hitObject = null;
                }
            }
        }

    }

    protected override void Activate()
    {
        cooldownStartTime = Time.time;
        StartCoroutine(ExtendRay());
        /*
         Shoot out a ray
         Every second, shoot out another ray that is longer than the last one
         Do this until the maximum distance is reached (Reach), or the ray hits an enemy collider
         If the ray hits an enemy collider, pull that enemy to the player
         If the ray hits a wall or the ground, pull the player to that spot
         */


    }
    private IEnumerator ExtendRay()
    {
  
        Ray r = new Ray(PlayerInfoScript.Instance.mainCam.transform.position, PlayerInfoScript.Instance.mainCam.transform.forward);
        RaycastHit hit = new RaycastHit();
        float tickAmount = 0.05f;
        float newDist = (5f*tickAmount) * DistancePerTick;
        for (float i = newDist; i < Reach; i += newDist)
        {
            
            if (Physics.Raycast(r, out hit, i, hitLayers))
            {
                if (hit.collider.name != "InvisibleWall")
                {
                    //Hit ground or enemy
                    Debug.Log("Grapple Hit");
                    hitObject = hit.collider.gameObject;
                    if (hitObject.GetComponent<Animator>() != null)
                    {   if (hitObject.GetComponent<Skeleton_Follow2>() != null)
                        {   Skeleton_Follow2 sf = hitObject.GetComponent<Skeleton_Follow2>();
                            eType = sf.EnemyBehavior;
                            sf.GrappleDebuff();

                            if (eType != Skeleton_Follow2.EnemyType.Tank)
                                hitObject.GetComponent<Animator>().enabled = false;
                        }
                        else if (hitObject.GetComponent<EndenorScript>() != null)
                            hitObject.GetComponent<EndenorScript>().GrappleDebuff();
                        else if (hitObject.GetComponent<KaeneScript>() != null)
                            hitObject.GetComponent<KaeneScript>().GrappleDebuff();
                    }
                    hitPoint = hit.point;

                    break;
                }
            }
            Instantiate(Indicator, r.GetPoint(i), Indicator.transform.rotation);
            yield return new WaitForSeconds(tickAmount);
        }
        

    }
}
