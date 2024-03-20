using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public FMODUnity.EventReference rangedAttack;
    public FMODUnity.EventReference meleeAttack;
    public FMODUnity.EventReference endSong;
    public FMODUnity.EventReference demonBossDeath;
    public FMODUnity.EventReference demonBossWhipCrackAttack;
    public FMODUnity.EventReference demonBossWhipStartAttack;
    public FMODUnity.EventReference demonBossRoar;
    public FMODUnity.EventReference demonBossFootstep;
    public FMODUnity.EventReference kaeneDeath;
    public FMODUnity.EventReference kaeneIceSpawn;
    public FMODUnity.EventReference kaeneAoeAttack;
    public FMODUnity.EventReference kaeneOrbAttack;
    public FMODUnity.EventReference kaeneMainAttackHit;
    public FMODUnity.EventReference kaeneMainAttack;
    public FMODUnity.EventReference tankFootstep;
    public FMODUnity.EventReference meleeFootstep;
    public FMODUnity.EventReference rangedFootstep;
    public FMODUnity.EventReference supportFootstep;
    public FMODUnity.EventReference tankRoar;
    public FMODUnity.EventReference playerDash;
    public FMODUnity.EventReference audiences;
    public FMODUnity.EventReference playerFootstep;
    public FMODUnity.EventReference endenorHit;
    public FMODUnity.EventReference endenorDeath;
    public FMODUnity.EventReference endenorCast;
    public FMODUnity.EventReference endenorWind;
    public FMODUnity.EventReference endenorStun;
    public FMODUnity.EventReference endenorDodge;
    public FMODUnity.EventReference endenorMeleeSwing;
    public FMODUnity.EventReference violentArticStormAmbienceLoop;
    public FMODUnity.EventReference calmArticStormAmbienceLoop;
    public FMODUnity.EventReference playerMeleeSwing;
    public FMODUnity.EventReference rifleShot;
    public FMODUnity.EventReference rifleReload;
    public FMODUnity.EventReference shotgunShot;
    public FMODUnity.EventReference shotgunReload;
    public FMODUnity.EventReference rpgLaunch;
    public FMODUnity.EventReference hitMarker;
    public FMODUnity.EventReference shopKeeperDialog;

    public FMODUnity.StudioEventEmitter shopKeeperStudioEventEmitter;

    public Stack<FMOD.Studio.EventInstance> endenorWindStack;   

    public FMOD.Studio.EventInstance calmArticStormAmbienceLoopInstance;

    public FMOD.Studio.EventInstance audiencesInstance;
    public Stack<FMOD.Studio.EventInstance> kaeneMainAttackStack;

    public FMOD.Studio.EventInstance endSongInstance;

    // Start is called before the first frame update
    void Start()
    {
        endenorWindStack = new Stack<FMOD.Studio.EventInstance>();

        GameEvents.current.onRifleShot += onRifleShot;
        GameEvents.current.onShot += onShot;
        GameEvents.current.onRocketLaunch += onRocketLaunch;
        GameEvents.current.onRocketHit += onRocketHit;
        GameEvents.current.onMeleeSwing += onMeleeSwing;
        GameEvents.current.onReload += onReload;
        GameEvents.current.onHitEnemy += onHitEnemy;
        GameEvents.current.onShopMenuOpened += onShopMenuOpened;
        GameEvents.current.onShopMenuClosed += onShopMenuClosed;
        GameEvents.current.onPlayerDeath += onPlayerDeath;
        GameEvents.current.onPlayerRevive += onPlayerRevive;
        GameEvents.current.onEndenorSwing += onEndenorSwing;
        GameEvents.current.onEndenorDodge += onEndenorDodge;
        GameEvents.current.onEndenorStun += onEndenorStun;
        GameEvents.current.onEndenorWindStart += onEndenorWindStart;
        GameEvents.current.onEndenorWindEnd += onEndenorWindEnd;
        GameEvents.current.onEndenorCast += onEndenorCast;
        GameEvents.current.onEndenorHit += onEndenorHit;
        GameEvents.current.onPlayerFootstep += onPlayerFootstep;
        GameEvents.current.onNewAudienceFavor += onNewAudienceFavor;
        GameEvents.current.onPlayerDash += onPlayerDash;
        GameEvents.current.onTankRoar += onTankRoar;
        GameEvents.current.onEnemyFootstep += onEnemyFootstep;
        GameEvents.current.onEndenorDeath += onEndenorDeath;
        GameEvents.current.onKaeneMainAttackHit += onKaeneMainAttackHit;
        GameEvents.current.onKaeneOrbShoot += onKaeneOrbShoot;
        GameEvents.current.onKaeneAOE += onKaeneAOE;
        GameEvents.current.onKaeneIceSpawn += onKaeneIceSpawn;
        GameEvents.current.onKaeneDeath += onKaeneDeath;
        GameEvents.current.onDemonBossFootstep += onDemonBossFootstep;
        GameEvents.current.onDemonBossRoar += onDemonBossRoar;
        GameEvents.current.onDemonBossStartWhip += onDemonBossStartWhip;
        GameEvents.current.onDemonBossStartWhip += onDemonBossWhipCrack;
        GameEvents.current.onDemonBossDeath += onDemonBossDeath;
        GameEvents.current.onDemonBossintroSequence += onDemonBossintroSequence;
        GameEvents.current.onEnemyAttack += onEnemyAttack;
        
        shopKeeperStudioEventEmitter = GameObject.Find("ShopKeeper").GetComponent<FMODUnity.StudioEventEmitter>();

        calmArticStormAmbienceLoopInstance = FMODUnity.RuntimeManager.CreateInstance(calmArticStormAmbienceLoop);
        calmArticStormAmbienceLoopInstance.start();
        calmArticStormAmbienceLoopInstance.release();

        audiencesInstance = FMODUnity.RuntimeManager.CreateInstance(audiences);
        audiencesInstance.start();
        audiencesInstance.setParameterByNameWithLabel("Favor", "sit");

        endSongInstance = FMODUnity.RuntimeManager.CreateInstance(endSong);

        endenorWindStack = new Stack<FMOD.Studio.EventInstance>();
    }
    
    void OnDestroy() {
        calmArticStormAmbienceLoopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        
        audiencesInstance.release();
        audiencesInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        while (endenorWindStack.Count != 0){
            FMOD.Studio.EventInstance windInstance = endenorWindStack.Pop();
            windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        endSongInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //FMODUnity.RuntimeManager.GetBus("Master").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private void onRifleShot() {
        FMODUnity.RuntimeManager.PlayOneShot(rifleShot);
    }
    private void onRocketLaunch()
    {
        FMODUnity.RuntimeManager.PlayOneShot(rpgLaunch);
    }
    private void onRocketHit()
    {
        //TO BE IMPLEMENTED
    }
    private void onMeleeSwing()
    {
        FMODUnity.RuntimeManager.PlayOneShot(playerMeleeSwing);
    }
    private void onReload(string weaponType)
    {
        switch (weaponType) {
            case "rifle":
                FMODUnity.RuntimeManager.PlayOneShot(rifleReload);
                break;
            case "shotgun":
                FMODUnity.RuntimeManager.PlayOneShot(shotgunReload);
                break;
        }
        //TO BE IMPLEMENTED
    }

    private void onShot(string weaponType){
        switch (weaponType) {
            case "shotgun":
                FMODUnity.RuntimeManager.PlayOneShot(shotgunShot);
                break;
        }
    }

    private void onItemBuy() {
        Debug.Log("Item bought");
    }
    private void onHitEnemy() {
        FMODUnity.RuntimeManager.PlayOneShot(hitMarker);
    }

    private void onShopMenuOpened() {
        FMODUnity.RuntimeManager.PlayOneShot(shopKeeperDialog);

        shopKeeperStudioEventEmitter.Stop();
    }

    private void onShopMenuClosed() {
        shopKeeperStudioEventEmitter.Play();
    }
    
    private void onPlayerDeath() {
        FMODUnity.RuntimeManager.MuteAllEvents(true);
    }

    private void onPlayerRevive() {
        FMODUnity.RuntimeManager.MuteAllEvents(false);
    }

    private void onEndenorSwing(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(endenorMeleeSwing, gameObject);
    }

    private void onEndenorDodge(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(endenorDodge, gameObject);
    }

    private void onEndenorStun(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(endenorStun, gameObject);
    }

    private void onEndenorWindStart(GameObject gameObject) {
        FMOD.Studio.EventInstance endenorWindInstance = FMODUnity.RuntimeManager.CreateInstance(endenorWind);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(endenorWindInstance, gameObject.transform);
        endenorWindInstance.start();
        endenorWindInstance.release();
        endenorWindStack.Push(endenorWindInstance);
    }
    private void onEndenorWindEnd() {
        while (endenorWindStack.Count != 0){
            FMOD.Studio.EventInstance windInstance = endenorWindStack.Pop();
            windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }

    private void onEndenorCast(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(endenorCast, gameObject);
    }

    private void onEndenorDeath(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(endenorDeath, gameObject);

        //Make sure his wind doesn't keep on playing
        while (endenorWindStack.Count != 0){
            FMOD.Studio.EventInstance windInstance = endenorWindStack.Pop();
            windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void onEndenorHit(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(endenorHit, gameObject);
    }

    private void onPlayerFootstep() {
        FMODUnity.RuntimeManager.PlayOneShot(playerFootstep);
    }

    private void onNewAudienceFavor(string audienceState) {
        switch (audienceState){
            case "all_cheer":
                audiencesInstance.setParameterByNameWithLabel("Favor", "all_cheer");
                break;
            case "cheer":
                audiencesInstance.setParameterByNameWithLabel("Favor", "cheer");
                break;
            case "clap":
                audiencesInstance.setParameterByNameWithLabel("Favor", "clap");
                break;
            case "stand":
                audiencesInstance.setParameterByNameWithLabel("Favor", "stand");
                break;
            case "sit":
                audiencesInstance.setParameterByNameWithLabel("Favor", "sit");
                break;
        }
        
    }

    private void onPlayerDash() {
        FMODUnity.RuntimeManager.PlayOneShot(playerDash);
    }

    private void onTankRoar(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(tankRoar, gameObject);
    }

    private void onEnemyFootstep(GameObject gameObject, string enemyType) {
        switch(enemyType) {
            case "tank":
                FMODUnity.RuntimeManager.PlayOneShotAttached(tankFootstep, gameObject);
                break;
            case "melee":
                FMODUnity.RuntimeManager.PlayOneShotAttached(meleeFootstep, gameObject);
                break;
            case "support":
                FMODUnity.RuntimeManager.PlayOneShotAttached(supportFootstep, gameObject);
                break;
            case "ranged":
                FMODUnity.RuntimeManager.PlayOneShotAttached(rangedFootstep, gameObject);
                break;
        }
    }

    private void onKaeneMainAttackHit(Vector3 location) {
        FMODUnity.RuntimeManager.PlayOneShot(kaeneMainAttackHit, location);
    }

    private void onKaeneOrbShoot(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(kaeneOrbAttack, gameObject);
    }

    private void onKaeneAOE(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(kaeneAoeAttack, gameObject);
    }

    private void onKaeneIceSpawn(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(kaeneIceSpawn, gameObject);
    }

    private void onKaeneDeath(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(kaeneDeath, gameObject);
    }

    private void onDemonBossFootstep(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(demonBossFootstep, gameObject);
    }

    private void onDemonBossRoar(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(demonBossRoar, gameObject);
    }

    private void onDemonBossStartWhip(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(demonBossWhipStartAttack, gameObject);
    }

    private void onDemonBossWhipCrack(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(demonBossWhipCrackAttack, gameObject);
    }

    private void onDemonBossDeath(GameObject gameObject) {
        FMODUnity.RuntimeManager.PlayOneShotAttached(demonBossDeath, gameObject);
    }

    private void onDemonBossintroSequence() {
        endSongInstance.start();
        endSongInstance.release();
    }

    private void onEnemyAttack(GameObject gameObject, string type) {
        switch(type) {
            case "melee":
                FMODUnity.RuntimeManager.PlayOneShotAttached(meleeAttack, gameObject);
                break;
            case "ranged":
                FMODUnity.RuntimeManager.PlayOneShotAttached(rangedAttack, gameObject);
                break;
        }
    }
}
