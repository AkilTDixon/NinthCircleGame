using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoArena : MonoBehaviour {

    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private float radius = 50f;
    [SerializeField] private int height = 2;
    private MeshRenderer renderer;
    private float width;
    
    
    // Start is called before the first frame update
    void Start() {
        renderer = panelPrefab.GetComponent<MeshRenderer>();
        width =renderer.bounds.size.x;
        drawArena();
    }


    void drawArena() {  
        float perim = 2f * Mathf.PI * radius;
        int qty = (int) (perim / width);
        float angle = 360f / qty;
        for (int i = 0; i < qty; i++) {
            Vector3 spawnPoint = new Vector3(radius * Mathf.Cos(angle * i), 0, radius * Mathf.Sin(angle * i));
            GameObject p = Instantiate(panelPrefab, spawnPoint, Quaternion.LookRotation(transform.position - spawnPoint, Vector3.up));
            
        }
    }
    
    
    // Update is called once per frame
    void Update() {

        
    }
}
