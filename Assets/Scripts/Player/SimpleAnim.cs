using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        endPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        LerpPos(endPos, speed);
    }


    public Vector3 startPos;
    public Vector3 endPos;
    public float speed;


    public void LerpPos(Vector3 endPos, float speed)
    {
        if (Vector3.Distance(transform.position, endPos) >= 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime * speed);
            //print("LerpPos");
        }
    }
}
