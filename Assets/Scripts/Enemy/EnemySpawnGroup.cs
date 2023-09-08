using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnGroup : MonoBehaviour
{
    public List<EnemySpawnZone> EnemySpawnZoneList;
    public bool isGroupProbe;
    public int groupMemberCount;
    public int arrivedStack;
    public bool isAllArrive;

    private void Awake()
    {
        EnemySpawnZone[] EnemySpawnZones = GetComponentsInChildren<EnemySpawnZone>();
        foreach (var item in EnemySpawnZones)
        {
            EnemySpawnZoneList.Add(item);
        }

    }

    void Start()
    {
        
    }

    void Update()
    {
        if(groupMemberCount == arrivedStack)
        {
            isAllArrive = true;
        }
        else
        {
            isAllArrive = false;
        }
    }

    
}
