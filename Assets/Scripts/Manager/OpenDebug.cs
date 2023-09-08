using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDebug : MonoBehaviour
{
    string myLog = "*begin log";
    string filename = "";
    bool doShow = false;
    //int kChars = 700;
    int lineCount = 0;

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }
    void Update() 
    { 
        if (Input.GetKeyDown(KeyCode.F5)) { doShow = !doShow; }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        string log;
        switch (type)
        {
            case LogType.Error:
                log = "Error: ";
                break;
            case LogType.Assert:
                log = "Assert: ";
                break;
            case LogType.Warning:
                log = "Warning : ";
                break;
            case LogType.Log:
                log = "Log: ";
                break;
            case LogType.Exception:
                log = "Exception: ";
                break;
            default:
                log = "";
                break;
        }

        log += logString;

        // for onscreen...
        myLog = myLog + "\n" + log;
        lineCount++;

        //if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }
        if(lineCount > 23)
        {
            int index = myLog.IndexOf("\n");
            myLog = myLog.Substring(index + 1);
        }

        // for the file ...
        if (filename == "")
        {
            string d = Path.Combine(Application.dataPath, "YOUR_LOGS");
            System.IO.Directory.CreateDirectory(d);
            //string r = UnityEngine.Random.Range(1000, 9999).ToString();
            string time = System.DateTime.Now.ToString();
            filename = d + "/log-" + time + ".txt";

            //string d = System.Environment.GetFolderPath(
            //    System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
            //System.IO.Directory.CreateDirectory(d);
            //string r = UnityEngine.Random.Range(1000, 9999).ToString();
            //filename = d + "/log-" + r + ".txt";

        }
        try { System.IO.File.AppendAllText(filename, logString + "\n"); }
        catch { }
    }

    void OnGUI()
    {
        if (!doShow) { return; }
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
           new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
        GUI.TextArea(new Rect(20, 20, 540, 370), myLog);

        Rect position = new Rect(5, 5, Screen.width, Screen.height);

        float fps = 1.0f / Time.deltaTime;
        float ms = Time.deltaTime * 1000.0f;
        string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);

        GUIStyle style = new GUIStyle();

        style.fontSize = 12;
        style.normal.textColor = new Color(0, 0, 0);

        GUI.Label(position, text, style);
    }

}
