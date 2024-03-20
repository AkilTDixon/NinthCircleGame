using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [Header("Weapon Script Variables")]
    [SerializeField] protected GameObject BulletObject;
    [SerializeField] protected float BaseDamage = 10;
    [SerializeField] protected float BaseFireRate = 1f;
    [SerializeField] protected float velocityFactor = 20f;
    [SerializeField] protected GameObject ImpactEffect;
    [SerializeField] protected GameObject ExplosiveAmmoImpact;
    [SerializeField] protected GameObject PlayerObject;
    [SerializeField] protected int Ammo = 4;
    [SerializeField] protected int AmmoCap = 4;
    [SerializeField] protected int AmmoClipSize = 4;


    [Header("Weapon Upgrade Booleans")]
    [Tooltip("Multishot applies to the Rifle and RPG")]
    public bool Multishot = false;
    [Tooltip("Explosive Ammo applies to the Rifle")]
    public bool ExplosiveAmmo = false;
    [Tooltip("Charged Shot applies to the Shotgun")]
    public bool ChargedShot = false;
    [Tooltip("Penetrating Ammo applies to the Shotgun")]
    public bool PenetratingAmmo = false;
    [Tooltip("Knockback applies to the Melee weapon")]
    public bool Knockback = false;
    [Tooltip("AmmoReplenish applies to the Melee weapon")]
    public bool AmmoReplenish = false;

    public int ammoReduction = -1;
    public GameObject ProjectileSpawnPoint;

    [Header("Weapon Damage and Fire Rate")]
    public float CurrentFireRate = 0;
    public float CurrentDamage = 0;


    protected float FireRateModifier = 1f;
    protected float DamageModifier = 1f;
    protected Transform camTransform;
    protected float bulDistance = 1.5f;
    protected float lastShot = 0f;
    protected float offset;
    protected Vector3 bulFacing;
    protected List<float> FireRateModifiers = new List<float>();
    protected List<float> DamageModifiers = new List<float>();
    protected Camera MainCam;
    public LayerMask hitLayers;
    protected Animator anim;
    protected Vector3 hitp;

    protected bool Dead = false;

    // Start is called before the first frame update
    protected void WeapScriptStart()
    {
        GameEvents.current.onSkipItemInfo += PlayerReviveEvent;
        GameEvents.current.onItemPickup += PlayerDeathEvent;
        GameEvents.current.onPlayerDeath += PlayerDeathEvent;
        GameEvents.current.onPlayerRevive += PlayerReviveEvent;

        hitLayers = LayerMask.GetMask("Enemy") | LayerMask.GetMask("whatIsGround");
        MainCam = Camera.main.GetComponent<Camera>();

        if (CurrentDamage == 0)
            CurrentDamage = BaseDamage;
        if (CurrentFireRate == 0)
            CurrentFireRate = BaseFireRate;

        if (GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();

        //IncreaseDamage(0.5f); //damage increased by 50%
        //IncreaseFireRate(0.5f); //fire rate increased by 50%

        camTransform = Camera.main.transform;
        if (BulletObject != null)
            bulFacing = BulletObject.GetComponent<BulletInfo>().facingDirection;
       
    }

    protected virtual void Shoot()
    {
        Debug.Log("Shooting");
    }
    public void ChangeAmmo(int value)
    {
        Ammo += value;
        if (Ammo < 0)
            Ammo = 0;
    }
    public void SetAmmo(int ammo, int ammoCap, int clipSize)
    {
        Ammo = ammo;
        AmmoCap = ammoCap;
        AmmoClipSize = clipSize;
    }
    public int GetAmmoCount()
    {
        return Ammo;
    }
    public int GetAmmoCapCount()
    {
        return AmmoCap;
    }
    public int GetAmmoClipSize()
    {
        return AmmoClipSize;
    }
    public bool HasAmmo()
    {
        return Ammo > 0;
    }


    public void IncreaseFireRate(List<float> Modifiers)
    {
        //Multiplicative
        for (int i = 0; i < Modifiers.Count; i++)
        { 
            CurrentFireRate *= (1.0f + Modifiers[i]);
            FireRateModifiers.Add(Modifiers[i]);
        }
    }
    public virtual void Reload()
    {
        /*if (AmmoCap != 0)
        {
            GameEvents.current.reload();
            int AmmoDiff = AmmoClipSize - Ammo;
            AmmoCap -= AmmoDiff;

            if (AmmoCap < 0)
            {
                Ammo = AmmoClipSize + AmmoCap;
                AmmoCap = 0;
            }
            else
                Ammo = AmmoClipSize;
        }*/
        
    }
    public void IncreaseFireRate(float changeRate)
    {

        //If changeRate is 0.5, then the current FireRate is increased by 50%

        //Additive
        FireRateModifiers.Add(changeRate);
        FireRateModifier += changeRate;
        CurrentFireRate = BaseFireRate * FireRateModifier;

        if (anim != null)
            anim.speed = 1f * FireRateModifier;
        


        //Multiplicative
        //FireRateModifiers.Add(changeRate);
        //CurrentFireRate *= (1.0f + changeRate);

        Debug.Log(CurrentFireRate);
    }
    public void IncreaseDamage(List<float> Modifiers)
    {
        //Multiplicative
        for (int i = 0; i < Modifiers.Count; i++)
        {
            CurrentDamage *= (1.0f + Modifiers[i]);
            DamageModifiers.Add(Modifiers[i]);
        }
    }
    public void IncreaseDamage(float changeDamage)
    {
        //If changeDamage is 0.5, then the current Damage is increased by 50%
        DamageModifiers.Add(changeDamage);

        DamageModifier += changeDamage;
        CurrentDamage = BaseDamage * DamageModifier;



        //Multiplicative
        //DamageModifiers.Add(changeDamage);
        //CurrentDamage *= (1.0f + changeDamage);


        Debug.Log(CurrentDamage);
    }
    protected virtual void UpdateMuzzleFlash(GameObject effect)
    {
        //Debug.Log("Change MuzzleFlash Speed")
    }
    public void PlayerDeathEvent()
    {
        Dead = true;
        if (anim != null)
            anim.Rebind();
    }
    public void PlayerReviveEvent()
    {
        Dead = false;
    }
}