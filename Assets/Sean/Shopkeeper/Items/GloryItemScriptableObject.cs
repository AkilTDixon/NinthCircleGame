using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Glory Shop Item", menuName = "Scriptable Object/Shop Item/Glory Item")]
public class GloryItemScriptableObject : ShopItemScriptableObject
{
    public float gloryGainChange = 0;
    public float gloryLossChange = 0;

    private GlorySystem gs;

    private void OnEnable()
    {
        GameObject go = GameObject.Find("GlorySystem");
        if(go != null)
            gs = go.GetComponent<GlorySystem>();
    }

    public override void Equip(GameObject player)
    {
        gs = GameObject.Find("GlorySystem").GetComponent<GlorySystem>();

        if (gloryGainChange != 0)
            gs.ChangeGloryGain(gloryGainChange);

        if (gloryLossChange != 0)
            gs.ChangeGloryLoss(gloryLossChange);
    }
}