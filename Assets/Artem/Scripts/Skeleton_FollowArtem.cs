using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skeleton_Follow : MonoBehaviour
{
    //Transform that NPC has to follow
    public Transform transformToFollow;
    //NavMesh Agent variable
    NavMeshAgent agent;
    public Animator animator;
    public int hp = 2;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0)
        {
            /*agent.destination = agent.transform.position;*/
            /*agent.speed = 0;*/
            animator.SetBool("Fall1", true);
            IEnumerator coroutine = DestroySkeleton();
            StartCoroutine(coroutine);
            //Destroy(gameObject);
        }
        /*animator.SetBool("Attack1h1", false);*/
    }

    public IEnumerator DestroySkeleton() {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {        //Follow the player
        agent.destination = transformToFollow.position;
        animator.SetFloat("speedh", agent.speed);
        if ((agent.transform.position - transformToFollow.position).magnitude < 1.5f)
        {
            animator.SetBool("Attack1h1", true);
            animator.SetFloat("speedh", 0);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.GetComponent<Collider>().name);
        if (col.GetComponent<Collider>().name == "SM_Wep_Crysta_Halberdl_01(Clone)" || col.GetComponent<Collider>().name == "SM_Wep_Crysta_Halberdl_01")
        {
            animator.SetBool("Hit1", true);
            hp -= 1;
        }
    }
}
