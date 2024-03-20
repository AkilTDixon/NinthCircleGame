using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fan : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        SkinnedMeshRenderer skin = GetComponentInChildren<SkinnedMeshRenderer>();
        string alternateSkin = "Character_Goblin_Male";
        switch (Random.Range(0, 4)) {
            case 0:
                // no change
                break;
            case 1:
                alternateSkin = "Character_Goblin_Female";
                break;
            case 2:
                alternateSkin = "Character_Goblin_Warrior_Male";
                break;
            case 3:
                alternateSkin = "Character_Goblin_Warrior_Female";
                break;
            default:
                alternateSkin = "Character_Goblin_Male";
                break;
        }
        //make female
        transform.Find("Character_Goblin_Male").gameObject.SetActive(false);
        transform.Find(alternateSkin).gameObject.SetActive(true);
        
        
        skin.material.color = Random.ColorHSV();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
