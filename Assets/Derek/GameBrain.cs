using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBrain : MonoBehaviour
{
    //Singleton
    public static GameBrain Instance { get; private set; }

    public Transform player;


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            Debug.Log("Destroying extra instance of GameBrain (This is a Singleton!)");
            return;
        }

        Instance = this; 
        player = GameObject.Find("PlayerContainer/Player").GetComponent<Transform>();


    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
