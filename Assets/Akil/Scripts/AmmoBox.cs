using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public enum AmmoType
    {
        Rifle,
        RPG,
        Shotgun
    };

    [SerializeField] GameObject Model;
    public AmmoType TypeOfAmmo = AmmoType.Rifle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            
            switch (TypeOfAmmo)
            {
                case AmmoType.Rifle:
                    if (PlayerInfoScript.Instance.GetRifleAmmoCount() < PlayerInfoScript.Instance.GetRifleClipSize() || PlayerInfoScript.Instance.GetRifleAmmoCap() < PlayerInfoScript.Instance.GetRifleBaseCap())
                    {
                        //PlayerInfoScript.Instance.SetAmmoInfo(0, PlayerInfoScript.Instance.GetRifleClipSize(), PlayerInfoScript.Instance.GetRifleBaseCap());
                        PlayerInfoScript.Instance.MaximizeWeaponAmmo(0, PlayerInfoScript.Instance.GetRifleClipSize(), PlayerInfoScript.Instance.GetRifleBaseCap());
                        Destroy(gameObject);
                    }
                    break;
                case AmmoType.RPG:
                    if (PlayerInfoScript.Instance.GetRPGAmmoCount() < PlayerInfoScript.Instance.GetRPGClipSize() || PlayerInfoScript.Instance.GetRPGAmmoCap() < PlayerInfoScript.Instance.GetRPGBaseCap())
                    {
                        //PlayerInfoScript.Instance.SetAmmoInfo(1, PlayerInfoScript.Instance.GetRPGClipSize(), PlayerInfoScript.Instance.GetRPGBaseCap());
                        PlayerInfoScript.Instance.MaximizeWeaponAmmo(1, PlayerInfoScript.Instance.GetRPGClipSize(), PlayerInfoScript.Instance.GetRPGBaseCap());
                        Destroy(gameObject);
                    }
                    break;
                case AmmoType.Shotgun:
                    if (PlayerInfoScript.Instance.GetShotgunAmmoCount() < PlayerInfoScript.Instance.GetShotgunClipSize() || PlayerInfoScript.Instance.GetShotgunAmmoCap() < PlayerInfoScript.Instance.GetShotgunBaseCap())
                    {
                        PlayerInfoScript.Instance.MaximizeWeaponAmmo(2, PlayerInfoScript.Instance.GetShotgunClipSize(), PlayerInfoScript.Instance.GetShotgunBaseCap());
                        Destroy(gameObject);
                    }
                    break;

                   
            }
        }
    }
    void Start()
    {
        StartCoroutine(DeathCount());
    }
    private IEnumerator DeathCount()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        Model.transform.Rotate(Vector3.up);
    }
}
