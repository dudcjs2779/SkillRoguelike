using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] Button btnHard;
    [SerializeField] Button btnNoraml;
    [SerializeField] Button btnEasy;

    private void OnEnable() {
        Init_Diffculty();
    }

    void Init_Diffculty()
    {
        if (GameManager.Instance.playerData.clearLv == 0)
        {
            btnHard.interactable = false; 
            btnNoraml.interactable = false;
            btnEasy.interactable = true;
        }
        else if(GameManager.Instance.playerData.clearLv == 1)
        {
            btnHard.interactable = false;
            btnNoraml.interactable = true;
            btnEasy.interactable = true;
        }
        else 
        {
            btnHard.interactable = true;
            btnNoraml.interactable = true;
            btnEasy.interactable = true;
        }

        Navigation();
    }

    void Navigation(){
        Button[] interactableBtns = Array.FindAll(buttons, x => x.interactable);

        for (int i = 0; i < interactableBtns.Length; i++)
        {
            Navigation newNavi = new Navigation();
            newNavi.mode = UnityEngine.UI.Navigation.Mode.Explicit;
            newNavi.selectOnDown = i < interactableBtns.Length - 1 ? interactableBtns[i + 1] : null;
            newNavi.selectOnUp = i > 0 ? interactableBtns[i - 1] : null;

            interactableBtns[i].navigation = newNavi;
        }
    }

    public void BtnHard()
    {
        GameManager.Instance.StartDungeon(GameManager.DifficultyType.Hard);
    }

    public void BtnNoraml()
    {
        GameManager.Instance.StartDungeon(GameManager.DifficultyType.Normal);
    }

    public void BtnEasy()
    {
        GameManager.Instance.StartDungeon(GameManager.DifficultyType.Easy);
    }
}
