using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceGift : MonoBehaviour {

    private float deathCount = 0;

    [SerializeField] private GameObject toRotate;
    // Start is called before the first frame update    
    void Start()
    {
        GameEvents.current.audienceGiftCreated();
        deathCount = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (toRotate != null)
            toRotate.transform.Rotate(Vector3.up);
        if (Time.time > deathCount + 25f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // transfer buffs to player
            Buff[] buffs = GetComponents<Buff>();
            foreach (Buff b in buffs) {
                Buff newBuff = PlayerInfoScript.Instance.PlayerObject.AddComponent<Buff>();
                newBuff.copyFrom(b);
                newBuff.Activate();
                GameEvents.current.audienceGiftPickup();
                Destroy(newBuff, b.duration);
            }
            Destroy(gameObject);

        }
    }
}
