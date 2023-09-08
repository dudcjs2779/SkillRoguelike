using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float time;
    bool onTimer;

    // Update is called once per frame
    void Update()
    {
        if (onTimer)
            timer();
    }

    void timerOn()
    {
        onTimer = true;
        time = 0;
    }

    void timer()
    {
        time = time + Time.deltaTime;
    }

    float timerOff()
    {
        onTimer = false;

        return time;
    }
}
