using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] GameObject Model;
    [SerializeField] float healAmount = 0.25f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerCapsule")
        {
            PlayerInfoScript.Instance.Heal(Mathf.CeilToInt(healAmount * PlayerInfoScript.Instance.GetMaxHP()));
            Destroy(gameObject);
            
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
    // Update is called once per frame
    void FixedUpdate()
    {
        Model.transform.Rotate(Vector3.up);
    }
}
