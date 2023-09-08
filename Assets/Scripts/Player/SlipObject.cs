using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipObject : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("InvisibleWall"))
        {
            player.wallNormal = -collision.contacts[0].normal;
        }
        else
        {
            if (player.wallNormal != Vector3.zero)
                player.wallNormal = Vector3.zero;
        }

        //collision.collider.gameObject.layer == LayerMask.GetMask("Ground")
        if (collision.collider.CompareTag("Ground"))
        {
            player.isGround = true;
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            //print("Enemy is crashed");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("InvisibleWall"))
        {
            player.wallNormal = Vector3.zero;

        }

        if (collision.collider.CompareTag("Ground"))
        {
            player.isGround = false;
        }
    }
}
