using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Shop Item Configuration", menuName = "Scriptable Object/Shop Item Configuration")]
public class ShopItemScriptableObject : ScriptableObject
{
    public string name;
    public string desc;
    public int cost;
    public Sprite thumbnail;
    public WeaponList wepList;

    protected void OnEnable()
    {
        if (Camera.main != null)
            wepList = Camera.main.GetComponent<WeaponList>();
    }

    public virtual void Equip(GameObject player)
    {
        Debug.Log("Hey... It looks like this Shop Item does nothing....");
    }

}
