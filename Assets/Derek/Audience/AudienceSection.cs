using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor;
using UnityEngine;

public class AudienceSection : MonoBehaviour {

    public float favor = 0f;
    // [SerializeField] private Animator animator;
    // [SerializeField] private AudienceManager audienceManager;
    [Header("Randomize Fan")] 

    [SerializeField] private bool randomColor = true;
    
    [Header("Seat Positions")]
    [SerializeField] private float minX = 0.5f;
    [SerializeField] private float maxX = 4.4f;
    [SerializeField] private int seats = 15;
    [SerializeField] private float seat1Y = 0.463f;
    [SerializeField] private float seat1Z = -0.434f;
    [SerializeField] private float stepY = 0.30f;
    [SerializeField] private float stepZ = -0.6f;
    [SerializeField] private Transform giftOrigin;
    

    
    // Start is called before the first frame update
    void Start() {
        // Initialize every fan, as well as build a list of them for future use
        Transform fansFolder = transform.Find("Fans");
        foreach (Transform fan in fansFolder) {
            AudienceManager.Instance.registerFan(fan);
            float randX = Random.Range(minX, maxX);
            fan.localPosition = new Vector3(randX, fan.localPosition.y, fan.localPosition.z);
        }
        // register gift spawn point
        AudienceManager.Instance.audienceGiftSpawnPoints.Add(giftOrigin);
    }

    
    
    
    // Update is called once per frame

    
    
    
    
}
