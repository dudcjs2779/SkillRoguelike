using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public double playTime;
    public string STR, AGL, VIT, INT, MP, MST;
    public int index, curHealth, curMP, money;
    public int easyOak, easyLich, easyWolf;
    public int normalOak, normalLich, normalWolf;
    public int hardOak, hardLich, hardWolf;
    public int clearLv;
    public bool doTutorial;
    public bool clearEasyEvent;
    public bool clearNormalEvent;
    public bool clearHardEvent;
}