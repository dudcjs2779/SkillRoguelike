using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniTimeTest : MonoBehaviour
{
    float timer;
    bool timerOn;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer();

        //if (Input.GetKeyDown(KeyCode.F10))
        //{
        //    TimerOff();
        //}
    }

    void Timer()
    {
        if (timerOn)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
    }

    void PrintTime()
    {
        print("Time Check : " + timer);
    }

    void TimerOn()
    {
        print("Timer ON");
        timerOn = true;
    }

    void TimerOff()
    {
        print("Timer OFF : " + timer);
        timerOn = false;
    }


}
