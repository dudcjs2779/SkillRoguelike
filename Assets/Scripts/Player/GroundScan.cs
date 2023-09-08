using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScan : MonoBehaviour
{

    public float distance = 1.0f; // distance to raycast downwards (i.e. between transform.position and bottom of object)
    public LayerMask hitMask; // which layers to raycast against
    public float angle;

    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = new Ray(transform.position + transform.forward * 0.5f + Vector3.up * 0.4f, Vector3.down);
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);

        if (Physics.Raycast(ray, out hit, distance, hitMask))
        {
            //Debug.Log("Hit collider " + hit.collider + ", at " + hit.point + ", normal " + hit.normal);
            //Debug.DrawRay(hit.point, hit.normal * 2f, Color.blue);

            angle = Vector3.Angle(hit.normal, Vector3.up);
            //Debug.Log("angle " + angle);

            if (angle > 45)
            {
            }
            else
            {
            }
        }
        else // is not colliding
        {

        }

    }
}
