using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DropTalisman : MonoBehaviour
{
    [SerializeField] List<GameObject> TalismanList;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(((int)System.DateTime.Now.Ticks));
        TalismanList = PlayerInfoScript.Instance.TalismanDropItems;
    }
    /*

Talisman    -   ID

Shield      -   0
Heal        -   1
Charge      -   2          
Grapple     -   3
 */
    void Drop()
    {
        /*
         Unlock a random talisman
         There are 4 talismans implemented
         Choose a random integer between 0 and 4
         Check if the player has it unlocked already
         If so, roll for another talisman
        */

        bool[] unlocked = PlayerInfoScript.Instance.talismanUnlocked;
        int count = 0;
        for (int i = 0; i < unlocked.Length; i++)
            if (unlocked[i])
                count++;

        if (count == unlocked.Length)
            return;

        //Random.Range is NOT maximally inclusive when using integers 
        int ranIndex = Random.Range(0, 4);

        while (unlocked[ranIndex])
        {
            ranIndex = Random.Range(0, 4);
        }
        NavMeshHit nhit = new NavMeshHit();
        int areaMask = NavMesh.GetAreaFromName("Walkable");
        NavMesh.SamplePosition(transform.position, out nhit, 50f, -1);
        GameObject drop = Instantiate(TalismanList[ranIndex], nhit.position, TalismanList[ranIndex].transform.rotation);
        //PlayerInfoScript.Instance.talismanUnlocked[ranIndex] = true;
    }
}

