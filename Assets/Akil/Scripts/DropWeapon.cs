using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapon : MonoBehaviour
{
    [SerializeField] GameObject RPGDrop;
    [SerializeField] GameObject ShotgunDrop;
    public enum Weapons
    {
        RPG,
        Shotgun,
    }
    public Weapons weapon = Weapons.Shotgun;
    public void Drop()
    {
        switch (weapon)
        {
            case Weapons.RPG: Instantiate(RPGDrop, transform.position, transform.rotation);
                break;
            case Weapons.Shotgun: Instantiate(ShotgunDrop, transform.position, transform.rotation);
                break;
        }
    }
}
