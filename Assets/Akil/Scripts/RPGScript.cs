using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGScript : WeaponScript
{
    [Header("RPG Script Variables")]
    public GameObject RocketModel;

    void Start()
    {
        WeapScriptStart();

    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (!Dead)
        {
            if (HasAmmo())
            {
                offset = (1.0f / CurrentFireRate);

                if (Time.time > lastShot + offset)
                    RocketModel.SetActive(true);
                if (Input.GetKey(PlayerInfoScript.Instance.shootKey) && Time.time > lastShot + offset)
                {
                    lastShot = Time.time;
                    RocketModel.SetActive(false);
                    Shoot();
                }

            }
            else if (RocketModel.activeSelf)
                RocketModel.SetActive(false);
        }
    }


    protected override void Shoot()
    {
        if (!HasAmmo())
            return;

        ChangeAmmo(ammoReduction);
        GameEvents.current.rocketLaunch();
        PlayerInfoScript.Instance.SetAmmoInfo(1, GetAmmoCount(), GetAmmoCapCount());
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        Vector3 position = ProjectileSpawnPoint.transform.position;

        Vector3 dir = camTransform.transform.forward;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && !hits[i].collider.isTrigger)
            {
                dir = (hits[i].point - position).normalized;
                break;
            }
        }


        GameObject center = Instantiate(BulletObject, position, Quaternion.FromToRotation(bulFacing, dir));

        //Debug.DrawRay(position, dir * 100f, Color.green, 10f);
        center.GetComponent<Rigidbody>().velocity = dir * velocityFactor;
        center.GetComponent<BulletCollision>().SetDamage(CurrentDamage);
        if (Multishot)
            center.GetComponent<BulletCollision>().SetMultishot(bulFacing, velocityFactor);
        
    }
    protected override void UpdateMuzzleFlash(GameObject effect)
    {
        //Unused
    }
}
