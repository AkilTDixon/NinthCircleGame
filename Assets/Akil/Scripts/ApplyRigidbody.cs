using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ApplyRigidbody : MonoBehaviour
{
    private Animator animator;
    private bool DeathAnimStart = false;
    private Rigidbody rb;
    private BoxCollider bc;
    private Skeleton_Follow2 skelScript;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == 9 && rb != null)
        {
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            //Die when hitting the ground
            if (!DeathAnimStart && skelScript.GetCurrentHealth() <= 0)
            {
                DeathAnimStart = true;
                Destroy(rb);
                GetComponent<MeshCollider>().enabled = false;
                GetComponent<NavMeshAgent>().enabled = true;
                animator.Play("Fall1");
                //animator.SetBool("Fall1", true);

            }
            else if (skelScript.GetCurrentHealth() > 0)
            {
                Destroy(rb);
                GetComponent<MeshCollider>().enabled = true;
                GetComponent<NavMeshAgent>().enabled = true;
                if (skelScript.rootMotion)
                    animator.applyRootMotion = true;
                //animator.SetFloat("attackSpeed", skelScript.attackSpeed);

                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                if (agent.speed >= 6f)
                    animator.SetFloat("animSpeed", agent.speed / 6f);
                else
                    animator.SetFloat("animSpeed", 1f);
                animator.SetFloat("attackSpeed", skelScript.attackSpeed);

                //animator.SetFloat("speedh", 0);
                //animator.SetBool("Attack1h1", false);
                enabled = false;
            }
        }
       
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        skelScript = GetComponent<Skeleton_Follow2>();
        GetComponent<NavMeshAgent>().enabled = false;
        animator = skelScript.animator;
        if (animator.applyRootMotion)
            animator.applyRootMotion = false;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        animator.Rebind();
        animator.SetFloat("speedh", 0);
        animator.SetBool("Attack1h1", false);

        if (skelScript.ApplyExplosiveForce)
            rb.AddExplosionForce(skelScript.ExplosivePower * 3f, skelScript.ExplosionPosition, skelScript.ExplosiveRadius, 1f, ForceMode.Impulse);
        else
            rb.AddForce(skelScript.forceDirection,ForceMode.Impulse);
    }


}
