using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : WeaponScript
{

    [Header("Shotgun Script Variables")]
    [SerializeField] GameObject MuzzleEffect;
    [SerializeField] Transform MuzzleFlashPosition;
    [SerializeField] int NumberOfPellets = 5;
    [SerializeField] float SpreadWindow = 0.65f;
    [SerializeField] float ChargeCooldown = 3f;
    [SerializeField] float ChargeIncrease = 2f;

    private GameObject instance;
    private float startHold = 0f;
    private float multiplier = 1f;
    private float StartCooldown = 0f;
    private float ColorChangeTime = 0f;


    private Material mat;
    private List<ParticleSystem.MinMaxCurve> baseLifeTimes;
    // Start is called before the first frame update
    void Start()
    {
        
        mat = GetComponent<MeshRenderer>().material;
        mat.EnableKeyword("_EMISSION");
        //mat.emi
        Random.InitState(((int)System.DateTime.Now.Ticks));
        baseLifeTimes = new List<ParticleSystem.MinMaxCurve>();
        ParticleSystem[] sys = MuzzleEffect.GetComponentsInChildren<ParticleSystem>();
        //ParticleSystem.MainModule main = MuzzleEffect.GetComponent<ParticleSystem>().main;
        for (int i = 0; i < sys.Length; i++)
        {
            baseLifeTimes.Add(sys[i].main.startLifetime);
        }

        WeapScriptStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (!Dead)
        {
            offset = (1.0f / CurrentFireRate);

            if (Input.GetKey(PlayerInfoScript.Instance.shootKey) && Time.time > lastShot + offset)
            {

                if (!HasAmmo())
                    return;

                if (anim != null)
                    anim.SetTrigger("playerShot");

                lastShot = Time.time;
                instance = Instantiate(MuzzleEffect, MuzzleFlashPosition.position, MuzzleFlashPosition.rotation);
                instance.transform.SetParent(transform);
                Shoot();

            }
            else if (Input.GetKeyDown(PlayerInfoScript.Instance.reloadKey) && (!anim.GetCurrentAnimatorStateInfo(0).IsName("ShotgunReload") && !anim.GetCurrentAnimatorStateInfo(0).IsName("ShotgunReloadTransition")))
            {
                if (GetAmmoCapCount() > 0 && GetAmmoCount() < AmmoClipSize)
                { 
                    
                    anim.SetTrigger("playerReload"); 
                }


            }

            if (ChargedShot)
            {
                if (Input.GetKey(PlayerInfoScript.Instance.secondaryFireKey) && Time.time > StartCooldown + ChargeCooldown)
                {
                    if (!HasAmmo())
                        return;

                    if (startHold == 0f)
                        startHold = Time.time;

                    if (Time.time > startHold + offset)
                    {
                        multiplier = ChargeIncrease;
                        mat.SetColor("_EmissionColor", Color.red);
                    }
                    else if (Time.time > ColorChangeTime + 0.085f)
                    {
                        ColorChangeTime = Time.time;
                        Color newColor;
                        newColor = mat.GetColor("_EmissionColor");
                        newColor.r += 0.05f;
                        newColor.g += 0.05f;
                        newColor.b += 0.05f;

                        mat.SetColor("_EmissionColor", newColor);
                    }
                    Debug.Log("RightButton");
                }
                if (Input.GetKeyUp(PlayerInfoScript.Instance.secondaryFireKey) && startHold != 0f)
                {

                    if (!HasAmmo())
                        return;

                    startHold = 0f;

                    if (anim != null)
                        anim.SetTrigger("playerShot");

                    lastShot = Time.time;
                    instance = Instantiate(MuzzleEffect, MuzzleFlashPosition.position, MuzzleFlashPosition.rotation);
                    instance.transform.SetParent(transform);
                    Shoot();
                    multiplier = 1f;
                    StartCooldown = Time.time;
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }
    }

    public override void Reload()
    {

        if (Ammo < AmmoClipSize && AmmoCap > 0)
        {
            GameEvents.current.reload("shotgun");
            Ammo++;
            AmmoCap--;
            PlayerInfoScript.Instance.SetAmmoInfo(2, GetAmmoCount(), GetAmmoCapCount());
            if (Ammo == AmmoClipSize || AmmoCap == 0)
            {
                anim.SetTrigger("ammoFull");
            }
        }
        else
        { 
            anim.SetTrigger("ammoFull");
        }
        /*anim.SetTrigger("playerReload");
        if (AmmoCap != 0)
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
    /*
    The idea is multiple rays shooting out from the same starting position, but each with random directions within the confines of a window or plane 
     */
    protected override void Shoot()
    {

        GameEvents.current.shot("shotgun");

        ChangeAmmo(ammoReduction);
        PlayerInfoScript.Instance.SetAmmoInfo(2, GetAmmoCount(), GetAmmoCapCount());
        Vector3 point = MainCam.transform.position + (5f * (MainCam.transform.forward));
        Vector3 leftConfine = point + (SpreadWindow * (-MainCam.transform.right));
        Vector3 rightConfine = point + (SpreadWindow * (MainCam.transform.right));
        Vector3 upConfine = point + (SpreadWindow * (MainCam.transform.up));
        Vector3 downConfine = point + (SpreadWindow * (-MainCam.transform.up));

        float maxX, maxY, maxZ, minX, minY, minZ;

        minX = Mathf.Min(leftConfine.x, rightConfine.x, upConfine.x,downConfine.x);
        maxX = Mathf.Max(leftConfine.x, rightConfine.x, upConfine.x, downConfine.x);

        minY = Mathf.Min(leftConfine.y, rightConfine.y, upConfine.y, downConfine.y);
        maxY = Mathf.Max(leftConfine.y, rightConfine.y, upConfine.y, downConfine.y);

        minZ = Mathf.Min(leftConfine.z, rightConfine.z, upConfine.z, downConfine.z);
        maxZ = Mathf.Max(leftConfine.z, rightConfine.z, upConfine.z, downConfine.z);

        
        float ranX, ranY, ranZ;

        List<Vector3> ranPoint = new List<Vector3>();
        for (int i = 0; i < NumberOfPellets; i++)
        {
            ranX = Random.Range(minX, maxX);
            ranY = Random.Range(minY, maxY);
            ranZ = Random.Range(minZ, maxZ);

            ranPoint.Add(new Vector3(ranX,ranY,ranZ));
        }


        Ray ray;
        RaycastHit hit;

        for (int i = 0; i < NumberOfPellets; i++)
        {
            ray = new Ray(MainCam.transform.position, ranPoint[i] - MainCam.transform.position);
            if (PenetratingAmmo)
            {
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, hitLayers);

                for (int j = 0; j < hits.Length; j++)
                    HitSuccess(hits[j]);                
                
            }
            else
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
                {
                    HitSuccess(hit);
                }
                else
                    Debug.Log("Miss");
            }
        }


        /*        Debug.DrawLine(MainCam.transform.position, point, Color.red, 5f);
                Debug.DrawLine(MainCam.transform.position, rightConfine, Color.red, 5f);
                Debug.DrawLine(MainCam.transform.position, leftConfine, Color.red, 5f);
                Debug.DrawLine(MainCam.transform.position, upConfine, Color.red, 5f);
                Debug.DrawLine(MainCam.transform.position, downConfine, Color.red, 5f);*/

        //for (int i = 0; i < NumberOfPellets; i++)
            //Debug.DrawLine(MainCam.transform.position, ranPoint[i], Color.green, 5f);

    }


    void HitSuccess(RaycastHit hit)
    {
        Debug.Log("Hit");
        Vector3 look = Vector3.Cross(-hit.normal, MainCam.transform.right);
        if (look.y < 0)
            look = -look;
        float dmg = CurrentDamage;

        if (PlayerInfoScript.easyMode)
            dmg *= 1.5f;
        Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.point + look, hit.normal));
        hitp = hit.point;

        if (hit.collider.tag == "Enemy" || hit.collider.tag == "Boss") {

            if (hit.collider.GetComponent<Skeleton_Follow2>() != null)
                hit.collider.GetComponent<Skeleton_Follow2>().TakeDamage(dmg * multiplier, 2);
            else if (hit.collider.GetComponent<KaeneScript>() != null)
                hit.collider.GetComponent<KaeneScript>().TakeDamage(dmg * multiplier, 2);
            else if (hit.collider.GetComponent<EndenorScript>() != null)
                hit.collider.GetComponent<EndenorScript>().TakeDamage(dmg * multiplier, 2);
            else if (hit.collider.GetComponent<DemonBoss>() != null)
                hit.collider.GetComponent<DemonBoss>().TakeDamage(dmg * multiplier, 2);

        }
    }
    protected override void UpdateMuzzleFlash(GameObject effect)
    {
        if (MuzzleEffect != null)
        {
            ParticleSystem[] sys = effect.GetComponentsInChildren<ParticleSystem>();
            ParticleSystem.MainModule main;
            //main.startLifetime = (baseLifeTimes[i].constant / CurrentFireRate);
            for (int i = 0; i < sys.Length; i++)
            {
                main = sys[i].main;
                main.startLifetime = (baseLifeTimes[i].constant / ((1f * FireRateModifier)));
            }


        }
    }
}
