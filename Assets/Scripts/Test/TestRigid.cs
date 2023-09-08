using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRigid : MonoBehaviour
{
    Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

   
    void Update()
    {
        if (Input.GetButtonDown("TestKey"))
        {

            //rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);

            //Debug.Log("CubeTest");
        }

       

    }

    void JumpMove()
    {
        startPos = transform.position;
        endPos = startPos + new Vector3(-3, 0, 0);
        StartCoroutine("BulletMove");
    }

    private Vector3 startPos, endPos;
    //땅에 닫기까지 걸리는 시간
    protected float timer;
    protected float timeToFloor;


    protected static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public float maxH = 0;
    protected IEnumerator BulletMove()
    {
        timer = 0;
        while (transform.position.y >= startPos.y)
        {
            timer += Time.deltaTime;
            Vector3 tempPos = Parabola(startPos, endPos, 30, timer);
            transform.position = tempPos;
            yield return new WaitForEndOfFrame();

            if (Mathf.Abs(transform.position.y - 30.5f) < 0.05f)
            {
                Debug.Log("최고높이");
            }
        }
        

        if (transform.position.y < startPos.y)
        {
            transform.position = endPos;
        }
    }


}
