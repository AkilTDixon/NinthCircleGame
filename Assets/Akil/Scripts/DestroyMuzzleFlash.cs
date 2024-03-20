using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMuzzleFlash : MonoBehaviour
{
    void OnDisable()
    {
        Destroy(gameObject);
    }
}
