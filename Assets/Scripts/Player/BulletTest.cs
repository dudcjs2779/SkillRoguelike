using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest : MonoBehaviour
{

    public Transform targetTr;
    Vector3 targetVec;

    [SerializeField] float sight_distance;
    LayerMask enemy_layerMask = 1 << 9;
    Rigidbody rigid;
    public float turn;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //rigid.velocity = transform.forward * 5f;
        rigid.AddForce(transform.forward * 25f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        //print(rigid.velocity.magnitude);


        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            
        }



    }

    private void FixedUpdate()
    {
        Guidance();
        
    }

    void Guidance()
    {
        Collider[] detectArea = Physics.OverlapSphere(transform.position, sight_distance, enemy_layerMask);

        if (detectArea.Length > 0)
        {
            targetVec = detectArea[0].transform.position - transform.position;
            float angle = Vector3.Angle(rigid.velocity, targetVec);
            //print(angle);

            Vector3 crossVec = Vector3.Cross(rigid.velocity, targetVec);

            if(crossVec.y < 0)
            {
                angle = -angle;
            }

            if (Mathf.Abs(angle) < 100f)
            {
                rigid.velocity = transform.forward * 25f;
                var targetRot = Quaternion.LookRotation(new Vector3(targetVec.x, 0, targetVec.z));
                rigid.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRot, turn));
            }
        }
        
    }
}
