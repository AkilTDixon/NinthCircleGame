using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExperience : MonoBehaviour
{
    [SerializeField] float AmmoDropChance = 0.05f;
    [SerializeField] float RPGAmmoDropChance = 0.05f;
    [SerializeField] float ShotgunAmmoDropChance = 0.35f;
    [SerializeField] int ExperienceAmountOnKill = 0;
    [SerializeField] GameObject RifleAmmoPrefab;
    [SerializeField] GameObject RPGAmmoPrefab;
    [SerializeField] GameObject ShotgunAmmoPrefab;
    [SerializeField] float HealthDropChance = 0.005f;
    [SerializeField] GameObject HealthPrefab;

    private PlayerInfoScript playerInfo;
    private float baseDropChance;
    private float baseRPGDropChance;
    private float baseShotgunDropChance;
    void Start()
    {
        Random.InitState(((int)System.DateTime.Now.Ticks));
        playerInfo = GameObject.Find("Player").GetComponent<PlayerInfoScript>();

        baseDropChance = AmmoDropChance;
        baseRPGDropChance = RPGAmmoDropChance;
        baseShotgunDropChance = ShotgunAmmoDropChance;

        
    }

    public void TransferXP()
    {
        float ran = Random.value;

        //if RPG isn't unlocked
        if (!PlayerInfoScript.Instance.weaponUnlocked[1])
            RPGAmmoDropChance = 0;
        else
            RPGAmmoDropChance = baseRPGDropChance;

        //if Shotgun isn't unlocked
        if (!PlayerInfoScript.Instance.weaponUnlocked[2])
            ShotgunAmmoDropChance = 0;
        else
            ShotgunAmmoDropChance = baseShotgunDropChance;

        if (ran >= (1f- AmmoDropChance))
        {
            ran = Random.value;
            if (ran > (1f - RPGAmmoDropChance))
                Instantiate(RPGAmmoPrefab, transform.position, transform.rotation);
            else if (ran > (1f - ShotgunAmmoDropChance))
                Instantiate(ShotgunAmmoPrefab, transform.position, transform.rotation);
            else
                Instantiate(RifleAmmoPrefab, transform.position, transform.rotation);
        }
        playerInfo.ChangeExp(ExperienceAmountOnKill);
    }
}
