using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeScript : WeaponScript
{
    [Header("Melee Script Variables")]
    public float knockbackForceXZ = 5f;
    public float knockbackForceY = 5f;
    public GameObject BossScythe;
    public RuntimeAnimatorController BossController;

    public MeshFilter BossMesh;
    public MeshRenderer BossRenderer;
    public Transform BossTransform;
    // Start is called before the first frame update
    void Start()
    {
        WeapScriptStart();
        anim.keepAnimatorControllerStateOnDisable = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        float dmg = CurrentDamage;

        if (PlayerInfoScript.easyMode)
            dmg *= 1.5f;
        if (other.gameObject.GetComponent<Skeleton_Follow2>() != null)
        {
            Skeleton_Follow2 sf = other.gameObject.GetComponent<Skeleton_Follow2>();



            if (Knockback)
            {
                Vector3 dir = PlayerInfoScript.Instance.mainCam.transform.forward;
                dir.y += 1;
                dir.x *= knockbackForceXZ;
                dir.y *= knockbackForceY;
                dir.z *= knockbackForceXZ;
                sf.TakeDamageWithForce(dmg, dir); 
            }
            else
                sf.TakeDamage(dmg, 3);

            if (AmmoReplenish && sf.GetCurrentHealth() <= 0)
                PlayerInfoScript.Instance.ReplenishWeaponAmmo();
        }
        else if (other.gameObject.GetComponent<EndenorScript>() != null)
        {
            other.gameObject.GetComponent<EndenorScript>().TakeDamage(dmg, 3);
        }
        else if (other.gameObject.GetComponent<KaeneScript>() != null)
        {
            other.gameObject.GetComponent<KaeneScript>().TakeDamage(dmg, 3);
        }
        else if (other.gameObject.GetComponent<DemonBoss>() != null) {
            other.gameObject.GetComponent<DemonBoss>().TakeDamage(dmg, 4);
        }
    }
    void OnEnable()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (!Dead)
        {
            offset = 1f / CurrentFireRate;

            if (Input.GetKey(PlayerInfoScript.Instance.shootKey) && (Time.time > lastShot + offset || lastShot == 0f))
            {
                Shoot();
            }
        }
    }
    public void TryAttack()
    {
        if (Time.time > lastShot + offset || lastShot == 0f)
        {
            if (anim == null)
                Start();

            Shoot();
        }
    }
    protected override void Shoot()
    {
        GameEvents.current.meleeSwing();
        if (anim != null)
            anim.SetTrigger("playerShot");


        lastShot = Time.time;
    }
    public void EndenorDead()
    {
        anim.enabled = false;
        anim.Rebind();

        anim.runtimeAnimatorController = BossController;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        MeshFilter mf = GetComponent<MeshFilter>();

        Material[] newM = BossRenderer.materials;
        GetComponent<MeshRenderer>().materials = newM;

        mf.sharedMesh = BossMesh.sharedMesh;
        //mr.sharedMaterial = BossRenderer.sharedMaterial;
        transform.localPosition = new Vector3(-0.01f, -1.68f, 0.94f);
        transform.localRotation = BossTransform.rotation;
        transform.localScale = new Vector3(1, 1, 1);
        GetComponent<BoxCollider>().center = new Vector3(-0.001366883f, -0.3165867f, 0.2155853f);
        GetComponent<BoxCollider>().size = new Vector3(0.7815948f, 1.72151f, 4.232242f);
        anim.enabled = true;
        //StartCoroutine(SetTransform());

    }
    public IEnumerator SetTransform()
    {
        yield return new WaitForSeconds(1.5f);
        transform.localPosition = new Vector3(-0.01f, -1.68f, 0.94f);
        transform.localRotation = BossTransform.rotation;
        transform.localScale = new Vector3(1, 1, 1);
        GetComponent<BoxCollider>().center = new Vector3(-0.001366883f, -0.3165867f, 0.2155853f);
        GetComponent<BoxCollider>().size = new Vector3(0.7815948f, 1.72151f, 4.232242f);
        anim.enabled = true;

    }
    public void ActivateCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
    public void DeactivateCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
}
