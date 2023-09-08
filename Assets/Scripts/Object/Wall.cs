using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Wall : MonoBehaviour
{
    Player player;
    Rigidbody playerRigid;
    GroundScan groundScan;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        playerRigid = player.GetComponent<Rigidbody>();
        groundScan = player.GetComponent<GroundScan>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //player.wallNomal = collision.contacts[0].normal;



            //Debug.Log("Player");
            ////print(collision.contacts[0].point + "//" + player.transform.position);

            //if (Mathf.Abs(collision.contacts[0].point.y - player.transform.position.y) > 0.1f)
            //{
            //    print("Ãæµ¹");
            //    player.transform.position += new Vector3(WallToPlayerVec.x, 0, WallToPlayerVec.z) * player.speed * Time.deltaTime;
            //    player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //}

        }

        if (collision.collider.CompareTag("Enemy"))
        {
            //print("Enemy is crashed");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //player.wallNomal = Vector3.zero;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            Debug.Log(enemy.enemyType);
        }
    }


}
