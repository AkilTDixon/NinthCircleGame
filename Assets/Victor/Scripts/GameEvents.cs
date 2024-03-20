using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {
    public static GameEvents current  { get; set; }
    public static bool first_time_shop_open = true;

    // Start is called before the first frame update
    private void Awake() {
        if (current != null && current != this) {
            Destroy(this);
            Debug.Log("Destroying extra instance of GameEvents (This is a Singleton!)");
            return;
        }
        current = this;
    }

    public event Action onGateOpen;
    public void gateOpen()
    {
        if (onGateOpen != null)
            onGateOpen();
    }
    public event Action onGateClose;
    public void gateClose()
    {
        if (onGateClose != null)
            onGateClose();
    }
    public event Action<GameObject> onKaeneAOE;
    public void kaeneAOE(GameObject gameObject)
    {
        if (onKaeneAOE != null)
            onKaeneAOE(gameObject);
    }
    public event Action<GameObject> onKaeneOrbShoot;
    public void kaeneOrbShoot(GameObject gameObject)
    {
        if (onKaeneOrbShoot != null)
            onKaeneOrbShoot(gameObject);
    }
    public event Action<GameObject> onKaeneIceSpawn;
    public void kaeneIceSpawn(GameObject gameObject)
    {
        if (onKaeneIceSpawn != null)
            onKaeneIceSpawn(gameObject);
    }
    public event Action<GameObject> onKaeneMainAttack;
    public void kaeneMainAttack(GameObject gameObject)
    {
        if (onKaeneMainAttack != null)
            onKaeneMainAttack(gameObject);
    }

    public event Action<Vector3> onKaeneMainAttackHit;
    public void kaeneMainAttackHit(Vector3 location)
    {
        if (onKaeneMainAttackHit != null)
            onKaeneMainAttackHit(location);
    }
    public event Action onKaeneChannelEnd;
    public void kaeneChannelEnd()
    {
        if (onKaeneChannelEnd != null)
            onKaeneChannelEnd();
    }
    public event Action onKaeneChannelStart;
    public void kaeneChannelStart()
    {
        if (onKaeneChannelStart != null)
            onKaeneChannelStart();
    }
    public event Action<GameObject> onKaeneDeath;
    public void kaeneDeath(GameObject gameObject)
    {
        if (onKaeneDeath != null)
            onKaeneDeath(gameObject);
    }
    public event Action<GameObject> onEndenorDeath;
    public void endenorDeath(GameObject gameObject)
    {
        if (onEndenorDeath != null)
            onEndenorDeath(gameObject);
    }
    public event Action<GameObject> onEndenorSwing;
    public void endenorSwing(GameObject gameObject)
    {
        if (onEndenorSwing != null)
            onEndenorSwing(gameObject);
    }
    public event Action<GameObject> onEndenorCast;
    public void endenorCast(GameObject gameObject)
    {
        if (onEndenorCast != null)
            onEndenorCast(gameObject);
    }
    public event Action<GameObject> onEndenorWindStart;
    public void endenorWindStart(GameObject gameObject)
    {
        if (onEndenorWindStart != null)
            onEndenorWindStart(gameObject);
    }
    public event Action onEndenorWindEnd;
    public void endenorWindEnd()
    {
        if (onEndenorWindEnd != null)
            onEndenorWindEnd();
    }
    public event Action<GameObject> onEndenorDodge;
    public void endenorDodge(GameObject gameObject)
    {
        if (onEndenorDodge != null)
            onEndenorDodge(gameObject);
    }
    public event Action<GameObject> onEndenorStun;
    public void endenorStun(GameObject gameObject)
    {
        if (onEndenorStun != null)
            onEndenorStun(gameObject);
    }
    public event Action<GameObject> onEndenorHit;
    public void endenorHit(GameObject gameObject)
    {
        if (onEndenorHit != null)
            onEndenorHit(gameObject);
    }
    public event Action<string> onShot;
    public void shot(string weaponType) {
        if (onShot != null){
            onShot(weaponType);
        }
    }
    public event Action onRifleShot;
    public void rifleShot() {
        if (onRifleShot != null) {
            onRifleShot();
        }
    }
    public event Action onShotgunShot;
    public void shotgunShot()
    {
        if (onShotgunShot != null)
        {
            onShotgunShot();
        }
    }
    public event Action onRocketLaunch;
    public void rocketLaunch()
    {
        if (onRocketLaunch != null)
        {
            onRocketLaunch();
        }
    }
    public event Action onRocketHit;
    public void rocketHit()
    {
        if (onRocketHit != null)
        {
            onRocketHit();
        }
    }
    public event Action onMeleeSwing;
    public void meleeSwing()
    {
        if (onMeleeSwing != null)
            onMeleeSwing();
    }
    public event Action<string> onReload;
    public void reload(string weaponType)
    {
        if (onReload != null)
            onReload(weaponType);
    }


    public event Action onEnemyDeath;
    public void enemyDeath() {
        if (onEnemyDeath != null) {
            onEnemyDeath();
        }
    }
    public event Action<int> onPlayerHit;
    public void playerHit(int dmg)
    {
        if (onPlayerHit != null && !PlayerInfoScript.Instance.Dead)
        {
            onPlayerHit(dmg);
        }
    }

    public event Action onPlayerFreeze;
    public void playerFreeze() {
        if (onPlayerFreeze != null)
            onPlayerFreeze();
    }
    
    public event Action onPlayerKnockBack;
    public void playerKnockBack() {
        if (onPlayerKnockBack != null)
            onPlayerKnockBack();
    }
    
    public event Action onAudienceGiftCreated;
    public void audienceGiftCreated() {
        if (onAudienceGiftCreated != null) {
            onAudienceGiftCreated();
        }
            
    }
    
    public event Action onAudienceGiftPickup;
    public void audienceGiftPickup() {
        if (onAudienceGiftPickup != null)
            onAudienceGiftPickup();
    }

    public event Action onHitEnemy;
    public void hitEnemy() {
        if (onHitEnemy != null) {
            onHitEnemy();
        }
    }

    public event Action onPlayerDeath;
    public void playerDeath()
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath();
        }
    }

    public event Action onPlayerWin;
    public void playerWin()
    {
        if (onPlayerWin != null)
        {
            onPlayerWin();
        }
    }

    public event Action onPlayerRevive;
    public void playerRevive()
    {
        if (onPlayerRevive != null)
        {
            onPlayerRevive();
        }
    }

    public event Action<int> onWeaponUnlock;
    public void weaponUnlock(int id)
    {
        if (onWeaponUnlock != null)
        {
            onWeaponUnlock(id);
        }
    }

    public event Action onWeaponSwap;
    public void weaponSwapped(int id)
    {
        if (onWeaponSwap != null)
        {
            onWeaponSwap();
        }
    }
    public event Action onShieldAbsorb;
    public void shieldAbsorb()
    {
        if (onShieldAbsorb != null)
        {
            onShieldAbsorb();
        }
    }
    public event Action onShieldBreak;
    public void shieldBroken()
    {
        if (onShieldBreak != null)
        {
            onShieldBreak();
        }
    }
    public event Action<int> onChargeActivated;
    public void chargeStart(int damage)
    {
        if (onChargeActivated != null)
        {
            onChargeActivated(damage);
        }
    }
    public event Action onChargeDeactivated;
    public void chargeEnd()
    {
        if (onChargeDeactivated != null)
        {
            onChargeDeactivated();
        }
    }

    public event Action onShopMenuOpened;

    public void shopMenuOpened() {
        if (onShopMenuOpened != null) {
            onShopMenuOpened();
            if (first_time_shop_open)
            {
                Debug.Log("paused");
                var pause_menu = GameObject.Find("PlayerContainer").transform.Find("Player").Find("PlayerCanvas").Find("PauseMenu").GetComponent<PauseMenu>();
                var shopkeeper_text_renderer = GameObject.Find("ShopKeeper").transform.Find("ShopCanvas").Find("PressLabel").GetComponent<TMPro.TextMeshProUGUI>();
                shopkeeper_text_renderer.text = "Take a good look! Next time game won't be paused...";
                shopkeeper_text_renderer.color = new Color(1, 0.4f, 0);
                shopkeeper_text_renderer.fontSize = 32;
                pause_menu.Pause();
                pause_menu.menu.SetActive(false);
            }
        }
    }

    public event Action<GameObject> onDemonBossFootstep;
    public void demonBossFootstep(GameObject gameObject) {
        if (onDemonBossFootstep != null)
            onDemonBossFootstep(gameObject);
    }
    public event Action<GameObject> onDemonBossRoar;
    public void demonBossRoar(GameObject gameObject) {
        if (onDemonBossRoar != null)
            onDemonBossRoar(gameObject);
    }
    public event Action<GameObject> onDemonBossStartWhip;
    public void demonWhipStart(GameObject gameObject) {
        if (onDemonBossStartWhip != null)
            onDemonBossStartWhip(gameObject);
    }
    public event Action<GameObject> onDemonBossWhipCrack;
    public void demonBossWhipCrack(GameObject gameObject) {
        if (onDemonBossWhipCrack != null)
            onDemonBossWhipCrack(gameObject);
    }
    
    public event Action onShopMenuClosed;

    public void shopMenuClosed() {
        if (onShopMenuClosed != null) {
            if (first_time_shop_open)
            {
                Debug.Log("unpaused");
                var pause_menu = GameObject.Find("PlayerContainer").transform.Find("Player").Find("PlayerCanvas").Find("PauseMenu").GetComponent<PauseMenu>();
                var shopkeeper_text_renderer = GameObject.Find("ShopKeeper").transform.Find("ShopCanvas").Find("PressLabel").GetComponent<TMPro.TextMeshProUGUI>();
                shopkeeper_text_renderer.text = "Press corresponding number to buy";
                shopkeeper_text_renderer.color = new Color(1, 1, 1);
                shopkeeper_text_renderer.fontSize = 36;
                pause_menu.Unpause();
                first_time_shop_open = false;
            }
            onShopMenuClosed();
        }
    }
    
    public event Action<Transform> onDemonBossCreate;
    public void demonBossCreate(Transform t) {
        if (onDemonBossCreate != null) {
            onDemonBossCreate(t);
        }
    }

    public event Action<Transform> onDemonBossSwordSwing;
    public void demonBossSwordAttack(Transform t) {
        if (onDemonBossSwordSwing != null) {
            onDemonBossSwordSwing(t);
        }
    }
    public event Action<GameObject> onDemonBossDeath;
    public void demonBossDeath(GameObject gameObject) {
        if (onDemonBossDeath != null) {
            onDemonBossDeath(gameObject);
        }
    }

    public event Action<GameObject> onDemonBossWingFlap;
    public void demonBossWingFlap(GameObject gameObject) {
        if (onDemonBossWingFlap != null) {
            onDemonBossWingFlap(gameObject);
        }
    }

    public event Action onDemonBossintroSequence;
    public void demonBossintroSequence() {
        if (onDemonBossintroSequence != null) {
            onDemonBossintroSequence();
        }
    }

    public event Action onItemPickup;
    public void itemPickedUp()
    {
        if (onItemPickup != null)
        {
            onItemPickup();
        }
    }
    public event Action onSkipItemInfo;
    public void infoSkipped()
    {
        if (onSkipItemInfo != null)
        {
            onSkipItemInfo();
        }
    }

    public event Action onWaveIntermission;
    public void waveWait()
    {
        if (onWaveIntermission != null)
            onWaveIntermission();
    }
    public event Action onWaveStart;
    public void waveStart()
    {
        if (onWaveStart != null)
            onWaveStart();
    }
    public event Action onPlayerFootstep;

    public void playerFootstep() {
        if (onPlayerFootstep != null){
            onPlayerFootstep();
        }
    }

    public event Action<string> onNewAudienceFavor;

    public void newAudienceFavor(string audienceState){
        if (onNewAudienceFavor != null) {
            onNewAudienceFavor(audienceState);
        }
    }

    public event Action onPlayerDash;
    public void PlayerDash() {
        if (onPlayerDash != null) {
            onPlayerDash();
        }
    }

    public event Action<GameObject> onTankRoar;
    public void tankRoar(GameObject gameObject) {
        if (onTankRoar != null) {
            onTankRoar(gameObject);
        }
    }

    public event Action<GameObject, string> onEnemyFootstep;
    public void enemyFootstep(GameObject gameObject, string type) {
        if (onEnemyFootstep != null) {
            onEnemyFootstep(gameObject, type);
        }
    }

    public event Action<GameObject, string> onEnemyAttack;

    public void enemyAttack(GameObject gameObject, string type) {
        if (onEnemyAttack != null) {
            onEnemyAttack(gameObject, type);
        }
    }
}
