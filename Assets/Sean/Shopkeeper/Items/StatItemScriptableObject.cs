using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Shop Item", menuName = "Scriptable Object/Shop Item/Stat Item")]
public class StatItemScriptableObject : ShopItemScriptableObject
{
    public int maxHPChange = 0;
    public float attackChange = 0;
    public float defenseChange = 0;
    public float speedChange = 0;
    public float jumpChange = 0;
    public float expChange = 0;
    public float shopCostChange = 0;


    public override void Equip(GameObject player)
    {
        if (wepList == null)
            wepList = Camera.main.GetComponent<WeaponList>();

        PlayerInfoScript pis = player.GetComponent<PlayerInfoScript>();

        if (attackChange != 0)
            wepList.GlobalDamageIncrease(attackChange);

        if (maxHPChange != 0)
            pis.ChangeMaxHP(maxHPChange);

        if (defenseChange != 0)
            pis.ChangeDefense(defenseChange);

        if (speedChange != 0)
            pis.ChangeSpeed(speedChange);

        if (jumpChange != 0)
            pis.ChangeJumpForce(jumpChange);

        if (expChange != 0)
            pis.ChangeExpMultiplier(expChange);

        if (shopCostChange != 0)
            pis.ChangeShopCostMultiplier(shopCostChange);
    }
}
