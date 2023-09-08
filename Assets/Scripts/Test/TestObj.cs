using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObj : MonoBehaviour, IDataPersistence
{
    public Collider playerCol;
    public Collider thisCol;

    public Collider[] colliders;

    private void Awake()
    {
        Debug.Log(GetComponentInChildren<Collider>().name);
        colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            Debug.Log(collider.name);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
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
        }
    }

    private void OnDisable()
    {

    }

    private void OnDestroy()
    {
    }

    public void LoadData(GameData data)
    {
        Debug.Log(this.gameObject.name + ": " + this.GetInstanceID());
    }

    public void SaveData(GameData data)
    {
        Debug.Log(this.gameObject.name + ": " + this.GetInstanceID());
    }
}
