using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    [SerializeField] GameObject PF_GuardHit;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Shield") && other.CompareTag("EnemyWeapon"))
        {
            //Debug.Log("shield Hit");
            Instantiate(PF_GuardHit, transform.position, Quaternion.identity);
        }
    }
}
