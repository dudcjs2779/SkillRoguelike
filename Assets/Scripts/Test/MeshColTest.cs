using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Test"))
        {
            Debug.Log("Test_Enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Test"))
        {
            Debug.Log("Test_Exit");
        }
    }
}
