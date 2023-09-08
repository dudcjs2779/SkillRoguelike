using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Keypad0))
        {
            ps.Stop();
        }

        if (ps.IsAlive(true))
        {
            Debug.Log("alive");
        }
        else if(!ps.IsAlive(true))
        {
            Debug.Log("dead");
        }
    }
}
