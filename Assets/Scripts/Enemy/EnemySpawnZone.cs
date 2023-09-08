using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;

public class EnemySpawnZone : MonoBehaviour
{
    public GameObject[] spawnEnemyType;
    public GameManager.EnemyActType actType;
    public ProbePoint[] probePoints;
    public bool isNonStopProbe;

    private void Awake() {
        probePoints = GetComponentsInChildren<ProbePoint>(true);
    }

    void Start()
    {
        foreach (var probePoint in probePoints)
        {
            Physics.Raycast(probePoint.transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit probePoint_Hit, 1, LayerMask.GetMask("Ground"));
            probePoint.transform.position = probePoint_Hit.point;
            //print(gameObject.name + "-" + probePoint.name + " : " + probePoint_Hit.point);
        }
    }

    void Update()
    {

    }

}
