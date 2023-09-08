using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShootBullet : MonoBehaviour
{
    public GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
        }
    }

    
}
