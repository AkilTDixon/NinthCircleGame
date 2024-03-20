using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    public FMODUnity.EventReference mainMenuSong;
    public FMODUnity.EventReference hoverEnter;
    public FMODUnity.EventReference click;

    public FMOD.Studio.EventInstance mainMenuSongInstance;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuSongInstance = FMODUnity.RuntimeManager.CreateInstance(mainMenuSong);
        mainMenuSongInstance.start();
        mainMenuSongInstance.release();

        MainMenuGameEventsSystem.current.onHoverEnter += onHoverEnter;
        MainMenuGameEventsSystem.current.onClick += onClick;
    }

    void OnDestroy() {
        mainMenuSongInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    void onHoverEnter() {
        FMODUnity.RuntimeManager.PlayOneShot(hoverEnter);
    }

    void onClick() {
        FMODUnity.RuntimeManager.PlayOneShot(click);
    }
}
