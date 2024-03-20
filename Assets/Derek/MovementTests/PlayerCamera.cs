using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    // https://www.youtube.com/watch?v=f473C43s8nE&t=10s 

    public float mouseSensitivityX, mouseSensitivityY;
    public Transform playerOrientation;
    public Transform lockOnTo;  // used for DemonBoss script 

    private float xRotation;
    private float yRotation;

    private bool Dead = false;

    void Start()
    {

        //Akil - attaching to event system 
        GameEvents.current.onSkipItemInfo += PlayerReviveEvent;
        GameEvents.current.onItemPickup += PlayerDeathEvent;
        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;
        // 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // set starting rotation (needed for intro script)
        xRotation = transform.rotation.eulerAngles.x;
        yRotation = transform.rotation.eulerAngles.y;

    }

    void Update()
    {

        if (!Dead && !PauseMenu.isPaused && !PlayerInfoScript.Instance.finalBossKilled)
        {

            if (lockOnTo != null)
            {
                Quaternion rotateToLockOn = Quaternion.LookRotation(lockOnTo.position - transform.position);
                xRotation = rotateToLockOn.eulerAngles.x;
                yRotation = rotateToLockOn.eulerAngles.y;
                // transform.rotation = rotateToLockOn; 
                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);

            }
            else
            {
                // Free Mouse input 
                float mouseX = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * mouseSensitivityX;
                float mouseY = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * mouseSensitivityY;
                yRotation += mouseX;
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                // Rotate camera 
                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
            }



            if (Input.GetKeyDown(KeyCode.L))
            {
                GameObject light = GameObject.Find("Directional Light");
                light.GetComponent<Light>().enabled = !light.GetComponent<Light>().enabled;
            }
        }
    }
    //Akil 
    public void PlayerDeathEvent()
    {
        Dead = true;
    }
    public void PlayerReviveEvent()
    {
        Dead = false;
    }
    // 

}
