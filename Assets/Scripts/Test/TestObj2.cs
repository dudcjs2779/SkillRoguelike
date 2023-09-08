using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObj2 : MonoBehaviour
{
    public GameObject effect;
    public GameObject effect2;
    Collider thisCol;
    public Collider playerCol;
    public Collider cubeCol;

    private void Awake()
    {
        thisCol = GetComponent<Collider>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Physics.IgnoreCollision(thisCol, playerCol, false);
            Debug.Log("act1");
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Physics.IgnoreCollision(thisCol, cubeCol, false);
            Debug.Log("act2");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log(other.ClosestPointOnBounds(transform.position));
            //Instantiate(effect, other.ClosestPointOnBounds(transform.position), transform.rotation);

            Debug.Log(other.ClosestPoint(transform.position));
            Instantiate(effect2, other.ClosestPoint(transform.position), transform.rotation);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        }

        if (other.CompareTag("PlayerNoHit"))
        {
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Instantiate(effect, collision.contacts[0].point, transform.rotation);
            Physics.IgnoreCollision(thisCol, collision.collider, true);
            Debug.Log("Collision");
        }
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
    }
}
