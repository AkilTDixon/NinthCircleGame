using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShopkeeperMovement : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Transform movePointContainer;
    private NavMeshAgent agent;
    private Coroutine walkCoroutine;
    [SerializeField] private float updateRate = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3[] allWalkPoints;
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointSet = false;
    [SerializeField] private float walkPointRange;
    [SerializeField] private Animator anim;

    public bool isMoving = true;

    private void Awake()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        walkPointSet = false;

        anim.SetBool("isMoving", isMoving);
    }

    public void Start()
    {
        // Fetch all move points if they exist
        if (movePointContainer != null)
        {
            int i = 0;
            allWalkPoints = new Vector3[movePointContainer.childCount];
            foreach (Transform point in movePointContainer)
            {
                allWalkPoints[i++] = point.position;
            }
        }
        else
        {
            allWalkPoints[0] = Vector3.zero;
        }

        StartWalking();
    }

    private void Update()
    {
        anim.SetBool("isMoving", isMoving);

        if(!isMoving)
            transform.LookAt(new Vector3(player.transform.position.x, 0, player.transform.position.z));
    }

    public void StartWalking()
    {
        if (walkCoroutine == null)
            walkCoroutine = StartCoroutine(WalkAround());
        else
            Debug.LogWarning("Called StartChasing on Shopkeeper that is already walking around! This is likely a bug in some calling class!");
    }

    private IEnumerator WalkAround()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateRate);

        while (gameObject.activeSelf)
        {
            if (!walkPointSet)
                SearchWalkPoint();

            if (walkPointSet && agent.isActiveAndEnabled)
                agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            // Walked to Point
            if (distanceToWalkPoint.magnitude < 2f)
                walkPointSet = false;

            yield return Wait;
        }
    }

    void SearchWalkPoint()
    {
        Vector3 chosenPoint = allWalkPoints[Random.Range(0,allWalkPoints.Length-1)];
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(chosenPoint.x + randomX, transform.position.y, chosenPoint.z + randomZ);

        // Is this point out of the map?
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    public void PauseMovement()
    {
        isMoving = false;
        agent.isStopped = true;
    }

    public void ContinueMovement()
    {
        isMoving = true;
        agent.isStopped = false;
    }
}
