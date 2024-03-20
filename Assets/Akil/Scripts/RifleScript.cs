using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleScript : WeaponScript
{
    [Header("Rifle Script Variables")]
    [SerializeField] GameObject MuzzleEffect;
    [SerializeField] Transform MuzzleFlashPosition;
    [SerializeField] Transform MuzzleFlashPosition2;
    [SerializeField] Mesh Multimesh;
    private MeshFilter currentMesh;
    //private WeaponScript WeaponComponent;
    private GameObject instance;
    private List<ParticleSystem.MinMaxCurve> baseLifeTimes;
    // Start is called before the first frame update
    void Start()
    {
        WeapScriptStart();
        baseLifeTimes = new List<ParticleSystem.MinMaxCurve>();
        ParticleSystem[] sys = MuzzleEffect.GetComponentsInChildren<ParticleSystem>();
        //ParticleSystem.MainModule main = MuzzleEffect.GetComponent<ParticleSystem>().main;
        for (int i = 0; i < sys.Length; i++)
        {
            baseLifeTimes.Add(sys[i].main.startLifetime);
        }

        currentMesh = GetComponent<MeshFilter>();
        //WeaponComponent = GetComponent<WeaponScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (PauseMenu.isPaused)
            return;

        if (!Dead)
        {

            if (Multishot && currentMesh.mesh != Multimesh)
                currentMesh.mesh = Multimesh;


            offset = (1.0f / CurrentFireRate);



            if (Input.GetKey(PlayerInfoScript.Instance.shootKey) && Time.time > lastShot + offset && !anim.GetCurrentAnimatorStateInfo(0).IsName("RifleReload"))
            {
                if (!HasAmmo())
                    return;

                // If the current objective given by the audience is not to shoot, the objective is failed
                if (AudienceObjectivesScript.current_objective == Objectives.DO_NOT_SHOOT)
                    AudienceObjectivesScript.objective_completed = false;

                GameEvents.current.rifleShot();



                if (anim != null)
                    anim.SetTrigger("playerShot");

                lastShot = Time.time;
                instance = Instantiate(MuzzleEffect, MuzzleFlashPosition.position, MuzzleFlashPosition.rotation);
                UpdateMuzzleFlash(instance);
                instance.transform.SetParent(transform);
                Shoot();

                if (Multishot)
                {
                    instance = Instantiate(MuzzleEffect, MuzzleFlashPosition2.position, MuzzleFlashPosition2.rotation);
                    UpdateMuzzleFlash(instance);
                    instance.transform.SetParent(transform);
                }

            }
            else if (Input.GetKeyDown(PlayerInfoScript.Instance.reloadKey) && !anim.GetCurrentAnimatorStateInfo(0).IsName("RifleReload"))
            {
                if (GetAmmoCapCount() > 0 && GetAmmoCount() < AmmoClipSize)
                {
                    // If the current objective given by the audience is not to reload, the objective is failed
                    if (AudienceObjectivesScript.current_objective == Objectives.DO_NOT_RELOAD)
                        AudienceObjectivesScript.objective_completed = false;
                    anim.SetTrigger("playerReload"); 
                }
                
            }
        }
    }

    public override void Reload()
    {
        if (AmmoCap != 0)
        {
            GameEvents.current.reload("rifle");
            int AmmoDiff = AmmoClipSize - Ammo;
            AmmoCap -= AmmoDiff;

            if (AmmoCap < 0)
            {
                Ammo = AmmoClipSize + AmmoCap;
                AmmoCap = 0;
            }
            else
                Ammo = AmmoClipSize;
            PlayerInfoScript.Instance.SetAmmoInfo(0, GetAmmoCount(), GetAmmoCapCount());
        }
    }
    protected override void Shoot()
    {

        ChangeAmmo(ammoReduction);
        PlayerInfoScript.Instance.SetAmmoInfo(0, GetAmmoCount(), GetAmmoCapCount());
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);
        RaycastHit hit;
        if (ExplosiveAmmo && ImpactEffect != ExplosiveAmmoImpact)
            ImpactEffect = ExplosiveAmmoImpact;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
        {

            float dmg = CurrentDamage;

            if (PlayerInfoScript.easyMode)
                dmg *= 1.5f;
            // Debug.Log("Hit");
            Vector3 look = Vector3.Cross(-hit.normal, MainCam.transform.right);
            if (look.y < 0)
                look = -look;
            Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.point + look, hit.normal));
            //Debug.DrawLine(MainCam.transform.position, MainCam.transform.position + (MainCam.transform.forward*5f), Color.red, 5f);
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Boss")
            {
                GameEvents.current.hitEnemy();
                
                hitp = hit.point;


                if (hit.collider.GetComponent<Skeleton_Follow2>() != null)
                {
                    hit.collider.GetComponent<Skeleton_Follow2>().TakeDamage(dmg, 0);
                    if (ExplosiveAmmo)
                        ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);

                    if (Multishot)
                    {
                        hit.collider.GetComponent<Skeleton_Follow2>().TakeDamage(dmg, 0);
                        if (ExplosiveAmmo)
                            ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);  
                    }
                }
                else if (hit.collider.GetComponent<KaeneScript>() != null)
                {
                    hit.collider.GetComponent<KaeneScript>().TakeDamage(dmg, 0);
                    if (ExplosiveAmmo)
                        ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);

                    if (Multishot)
                    {
                        hit.collider.GetComponent<KaeneScript>().TakeDamage(dmg, 0);
                        if (ExplosiveAmmo)
                            ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);
                    }
                }
                else if (hit.collider.GetComponent<EndenorScript>() != null)
                {
                    hit.collider.GetComponent<EndenorScript>().TakeDamage(dmg, 0);
                    if (ExplosiveAmmo)
                        ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);

                    if (Multishot)
                    {
                        hit.collider.GetComponent<EndenorScript>().TakeDamage(dmg, 0);
                        if (ExplosiveAmmo)
                            ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);
                    }
                }
                else if (hit.collider.GetComponent<DemonBoss>() != null)
                {
                    hit.collider.GetComponent<DemonBoss>().TakeDamage(dmg, 0);
                    if (ExplosiveAmmo)
                        ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);

                    if (Multishot)
                    {
                        hit.collider.GetComponent<DemonBoss>().TakeDamage(dmg, 0);
                        if (ExplosiveAmmo)
                            ExplosiveRounds(hit.point, 10, 1.5f, hit.collider.gameObject);
                    }
                }
            }


        }
        // else
            // Debug.Log("Miss");
    }

    void ExplosiveRounds(Vector3 hitPoint, int ExplosivePower, float ExplosiveRadius, GameObject Enemy)
    {
        float dmg = CurrentDamage;

        if (PlayerInfoScript.easyMode)
            dmg *= 1.5f;
        Collider[] col = Physics.OverlapSphere(hitPoint, ExplosiveRadius);
        foreach (Collider hit in col)
        {
            if (hit.name != ImpactEffect.name && hit.gameObject != Enemy)
            {
                Rigidbody r = hit.GetComponent<Rigidbody>();
                if (r != null)
                    r.AddExplosionForce(ExplosivePower, hitPoint, ExplosiveRadius, 0.01f, ForceMode.Impulse);

                if (hit.GetComponent<Skeleton_Follow2>() != null)
                    hit.GetComponent<Skeleton_Follow2>().TakeDamage(dmg, 0);
            }
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
                main.startLifetime = (baseLifeTimes[i].constant / (1f * FireRateModifier));
            }


        }
    }
}
