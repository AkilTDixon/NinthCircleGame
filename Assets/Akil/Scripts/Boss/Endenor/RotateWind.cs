using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWind : MonoBehaviour
{
    public GameObject placeIndicator;
    public Transform AnchorPoint;
    public float RotateSpeed = 50f;
    private GameObject indicatorInstance;
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(AnchorPoint.position, Vector3.up, RotateSpeed * Time.deltaTime);
    }
    void OnEnable()
    {
        RotateSpeed += 5f;
        if (indicatorInstance != null)
            Destroy(indicatorInstance);
    }

    void OnDisable()
    {
        indicatorInstance = Instantiate(placeIndicator, transform.position, placeIndicator.transform.rotation);
    }
    void OnDestroy()
    {
        if (indicatorInstance != null)
            Destroy(indicatorInstance);
    }
}
