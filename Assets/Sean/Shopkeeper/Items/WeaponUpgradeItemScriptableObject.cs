using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Upgrade Shop Item", menuName = "Scriptable Object/Shop Item/Weapon Upgrade Item")]
public class WeaponUpgradeItemScriptableObject : ShopItemScriptableObject
{
    public float fireRateChange;
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
    public float ammoAmtChange;


    public override void Equip(GameObject player)
    {
        if (wepList == null)
            wepList = Camera.main.GetComponent<WeaponList>();

        GameObject weapon = wepList.GetActiveWeapon();
        WeaponScript ws = weapon.GetComponent<WeaponScript>();

        if (fireRateChange != 0)
            ws.IncreaseFireRate(fireRateChange);
        if (Multishot)
            ws.Multishot = true;
        if (ExplosiveAmmo)
            ws.ExplosiveAmmo = true;
        if (ChargedShot)
            ws.ChargedShot = true;
        if (AmmoReplenish)
            ws.AmmoReplenish = true;
        if (PenetratingAmmo)
            ws.PenetratingAmmo = true;
        if (Knockback)
            ws.Knockback = true;

        if (ammoAmtChange != 0)
        {
            PlayerInfoScript pis = PlayerInfoScript.Instance;

            switch (wepList.GetActiveWeaponId())
            {
                case 0:
                    pis.IncreaseAmmoInfo(0, 6);
                    break;
                case 1:
                    pis.IncreaseAmmoInfo(1, 1);
                    break;
                case 2:
                    pis.IncreaseAmmoInfo(2, 4);
                    break;
                default:
                    Debug.Log("?????");
                    break;
            }
        }
    }
}
