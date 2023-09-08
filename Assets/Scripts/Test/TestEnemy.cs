using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class TestEnemy : MonoBehaviour
{
    public Transform currentTarget;
    NavMeshAgent nav;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {

    }

    void Update()
    {
        //nav.SetDestination(currentTarget.position);

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            nav.updatePosition = !nav.updatePosition;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            nav.updateRotation = !nav.updateRotation;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            nav.isStopped = !nav.isStopped;
        }

        if (Input.GetKey(KeyCode.Keypad4))
        {
            nav.velocity = Vector3.zero;
        }
    }



}

