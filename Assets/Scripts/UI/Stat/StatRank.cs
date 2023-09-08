using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatRank : MonoBehaviour
{
    public StatRankSlot[] statRankSlots;
    public TextMeshProUGUI statPointText;

    public int statPoint;

    private void Awake()
    {
        statRankSlots = GetComponentsInChildren<StatRankSlot>();
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        Init_PlayerStat();
    }

    public void Init_PlayerStat()
    {
        statRankSlots[0].statTypeText.text = "Èû";
        statRankSlots[0].rankText.text = GameManager.Instance.playerData.STR;
        statRankSlots[1].statTypeText.text = "¹ÎÃ¸";
        statRankSlots[1].rankText.text = GameManager.Instance.playerData.AGL;
        statRankSlots[2].statTypeText.text = "È°·Â";
        statRankSlots[2].rankText.text = GameManager.Instance.playerData.VIT;
        statRankSlots[3].statTypeText.text = "Áö·Â";
        statRankSlots[3].rankText.text = GameManager.Instance.playerData.INT;
        statRankSlots[4].statTypeText.text = "¸¶·Â";
        statRankSlots[4].rankText.text = GameManager.Instance.playerData.MP;
        statRankSlots[5].statTypeText.text = "Á¤½Å·Â";
        statRankSlots[5].rankText.text = GameManager.Instance.playerData.MST;

        switch (GameManager.Instance.playerData.clearLv)
        {
            case 0:
                statPoint = 16;
                break;

            case 1:
                statPoint = 24;
                break;

            case 2:
            case 3:
                statPoint = 32;
                break;
        }

        //½½·Ô È°¼ºÈ­
        for (int i = 0; i < statRankSlots.Length; i++)
        {
            switch (statRankSlots[i].rankText.text) 
            {
                case "A":
                    statPoint -= 10;
                    break;
                case "B":
                    statPoint -= 6;
                    break;
                case "C":
                    statPoint -= 3;
                    break;
                case "D":
                    statPoint -= 1;
                    break;
                case "E":
                    break;
            }
        }

        statPointText.text = statPoint.ToString();
    }

    public void ApplyStatRank()
    {
        GameManager.Instance.playerData.STR = statRankSlots[0].rankText.text;
        GameManager.Instance.playerData.AGL = statRankSlots[1].rankText.text;
        GameManager.Instance.playerData.VIT = statRankSlots[2].rankText.text;
        GameManager.Instance.playerData.INT = statRankSlots[3].rankText.text;
        GameManager.Instance.playerData.MP = statRankSlots[4].rankText.text;
        GameManager.Instance.playerData.MST = statRankSlots[5].rankText.text;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Keypad1))
        //{
        //    print(GameManager.Instance.playerData.STR);
        //    print(GameManager.Instance.playerData.AGL);
        //    print(GameManager.Instance.playerData.VIT);
        //    print(GameManager.Instance.playerData.INT);
        //    print(GameManager.Instance.playerData.MP);
        //    print(GameManager.Instance.playerData.MST);
        //}
    }

}
