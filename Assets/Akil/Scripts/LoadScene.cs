using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LaunchScene(string name)
    {
        PlayerInfoScript.easyMode = false;

        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void EasyMode(string name)
    {
        PlayerInfoScript.easyMode = true;

        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
}
