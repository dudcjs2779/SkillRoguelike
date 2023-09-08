using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TestPlayer : MonoBehaviour
{
    public float hAxisRaw, vAxisRaw, hAxis, vAxis;

    public Vector3 hvVec;
    public Vector3 moveVec;
    public Animator anim;
    public Rigidbody rigid;

    public List<Collider> colliders1;
    public List<Collider> colliders2;

    public Collider cube1;
    public Collider cube2;
    public Collider cube3;
    public Collider cube4;

    public GameObject meshTrail;

    public VisualEffect vfx;

    private void Awake()
    {

    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();

    }

    void Move()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        hvVec = new Vector3(hAxis, 0, vAxis);

        anim.SetFloat("V", hvVec.magnitude);

        moveVec = new Vector3(hAxis, 0, vAxis);
        Vector3 velocity = moveVec * 5f;
        rigid.velocity = velocity;
    }

    public float rotateSpeed;

    void Turn()
    {
        float turnAmount;
        float rot;

        if (hAxis == 0 && vAxis == 0) return;
        turnAmount = Mathf.Atan2(hvVec.x, hvVec.z) * Mathf.Rad2Deg;   //실제 각도

        rot = Mathf.LerpAngle(transform.eulerAngles.y, turnAmount, Time.deltaTime * rotateSpeed);   //보간된 각도 값
        transform.eulerAngles = new Vector3(0, rot, 0);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject instantObj = Instantiate(meshTrail, transform.position, transform.rotation);
            instantObj.GetComponent<MeshEffect>().skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            TestObj[] testObjs = FindObjectsOfType<TestObj>(true);

            foreach (var item in testObjs)
            {
                Debug.Log(item.name + ": " + item.GetInstanceID());
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            colliders1.RemoveAt(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            colliders2 = colliders1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Debug.Log("cols1 : " + colliders1.Count);
            Debug.Log("cols2 : " + colliders2.Count);
        }



    }

    private void FixedUpdate()
    {
        Move();
        Turn();

    }


    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnDisable()
    {
        colliders1.Clear();
    }

}
