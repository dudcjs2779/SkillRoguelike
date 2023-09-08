using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushHit : MonoBehaviour
{
    [SerializeField] BossGolem bossGolemScr;
    public Player player;

    Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !player.isIframes && !player.hitColStack.Contains(col))
        {
            bossGolemScr.rush_Hit = true;
        }
    }
    
}
