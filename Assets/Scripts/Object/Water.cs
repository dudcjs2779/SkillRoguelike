using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] GameObject waterWave;

    float waveDelay;

    private void Update()
    {
        waveDelay += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(waveDelay > 0.3f)
            {
                GameObject instantWave = Instantiate(waterWave,
                new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z),
                                                waterWave.transform.rotation);
                waveDelay = 0f;
                Destroy(instantWave, 3f);
            }
        }
    }

}
