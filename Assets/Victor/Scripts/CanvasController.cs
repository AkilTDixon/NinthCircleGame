using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    GameObject currentCanvas;
    GameObject nextCanvas;

    GameObject canvases;
    GameObject creditsCanvas;
    GameObject mainMenuCanvas;
    GameObject modeSelectCanvas;
    

    private void Awake() {
        canvases = GameObject.Find("Canvases");
        mainMenuCanvas = canvases.transform.Find("MainMenuCanvas").gameObject;
        creditsCanvas = canvases.transform.Find("CreditsCanvas").gameObject;
        modeSelectCanvas = canvases.transform.Find("ModeSelectCanvas").gameObject;
    }
    void Start()
    {
        currentCanvas = GameObject.Find("MainMenuCanvas");
        nextCanvas = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextCanvas != null) {
            Debug.Log("next canvas set");
            if (currentCanvas.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Offline")) {
                currentCanvas.SetActive(false);
                currentCanvas = nextCanvas;
                nextCanvas = null;

                //the king is dead, long live the king!
                currentCanvas.SetActive(currentCanvas);
                currentCanvas.GetComponent<Animator>().SetTrigger("entering");
            }  
        }
    }
    public void onGoToCreditsClick() {
        currentCanvas.GetComponent<Animator>().SetTrigger("exiting");
        nextCanvas = creditsCanvas;
    }

    public void onGoToMainMenuClick() {
        currentCanvas.GetComponent<Animator>().SetTrigger("exiting");
        nextCanvas = mainMenuCanvas;
    }
    public void onGoToModeSelectClick()
    {
        currentCanvas.GetComponent<Animator>().SetTrigger("exiting");
        nextCanvas = modeSelectCanvas;
    }
}
