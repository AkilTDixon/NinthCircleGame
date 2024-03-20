using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    public float maxOpening = 2.81f;
    public float maxClosing = -0.5450001f;
    public float speed = 5f;
    private bool someonesAtDoor;
    private bool opening;

    private void Start()
    {
        someonesAtDoor = false;
        opening = false;
    }


    private void Update()
    {
        if (someonesAtDoor)
        {
            if (door.transform.position.y < maxOpening)
            {   
                door.transform.Translate(0f,speed*Time.deltaTime,0f);
            }
        }
        else
        {
            if (door.transform.position.y > maxClosing)
            {                
                door.transform.Translate(0f,-speed*Time.deltaTime,0f);
            }
        }

    }

    public void Open()
    {
        GameEvents.current.gateOpen();
        someonesAtDoor = true;
    }
    
    public void Close()
    {
        GameEvents.current.gateClose();
        someonesAtDoor = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Close();
        }
    }
}
