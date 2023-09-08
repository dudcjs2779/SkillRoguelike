using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushWallHit : MonoBehaviour
{
    [SerializeField] BossGolem bossGolemScr;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            bossGolemScr.rush_HitWall = true;
        }
    }
}
