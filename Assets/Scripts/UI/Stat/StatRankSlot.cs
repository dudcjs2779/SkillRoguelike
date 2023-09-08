using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StatRankSlot : MonoBehaviour
{
    StatRank statRank;
    StatVal statVal;

    public Button btnDown;
    public Button btnUp;

    public TextMeshProUGUI statTypeText;
    public TextMeshProUGUI rankText;


    private void Awake()
    {
        statRank = GetComponentInParent<StatRank>();
        statVal = transform.parent.parent.parent.GetComponentInChildren<StatVal>();
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void BtnRankUp()
    {
        int requirePoint = 0;

        

        switch (rankText.text.Trim())
        {
            case "E":
                requirePoint = 1;
                if (requirePoint > statRank.statPoint) return;
                rankText.text = "D";
                break;

            case "D":
                requirePoint = 2;
                if (requirePoint > statRank.statPoint) return;
                rankText.text = "C";
                break;

            case "C":
                requirePoint = 3;
                if (requirePoint > statRank.statPoint) return;
                rankText.text = "B";
                break;

            case "B":
                requirePoint = 4;
                if (requirePoint > statRank.statPoint) return;
                rankText.text = "A";
                break;

            case "A":
                rankText.text = "A";
                break;
        }

        switch (statTypeText.text)
        {
            case "힘":
                GameManager.Instance.playerData.STR = rankText.text.Trim();
                break;

            case "민첩":
                GameManager.Instance.playerData.AGL = rankText.text.Trim();
                break;

            case "활력":
                GameManager.Instance.playerData.VIT = rankText.text.Trim();
                break;

            case "지력":
                GameManager.Instance.playerData.INT = rankText.text.Trim();
                break;

            case "마력":
                GameManager.Instance.playerData.MP = rankText.text.Trim();
                break;

            case "정신력":
                GameManager.Instance.playerData.MST = rankText.text.Trim();
                break;
        }
        
        statRank.statPoint -= requirePoint;
        statRank.statPointText.text = statRank.statPoint.ToString();
        statVal.UpdateStatVal(statTypeText.text.Trim(), rankText.text.Trim());
    }

    public void BtnRankDown()
    {
        int requirePoint = 0;

        switch (rankText.text.Trim())
        {
            case "E":
                rankText.text = "E";
                break;

            case "D":
                requirePoint = 1;
                rankText.text = "E";
                break;

            case "C":
                requirePoint = 2;
                rankText.text = "D";
                break;

            case "B":
                requirePoint = 3;
                rankText.text = "C";
                break;

            case "A":
                requirePoint = 4;
                rankText.text = "B";
                break;
        }

        switch (statTypeText.text)
        {
            case "힘":
                GameManager.Instance.playerData.STR = rankText.text.Trim();
                break;

            case "민첩":
                GameManager.Instance.playerData.AGL = rankText.text.Trim();
                break;

            case "활력":
                GameManager.Instance.playerData.VIT = rankText.text.Trim();
                break;

            case "지력":
                GameManager.Instance.playerData.INT = rankText.text.Trim();
                break;

            case "마력":
                GameManager.Instance.playerData.MP = rankText.text.Trim();
                break;

            case "정신력":
                GameManager.Instance.playerData.MST = rankText.text.Trim();
                break;
        }


        statRank.statPoint += requirePoint;
        statRank.statPointText.text = statRank.statPoint.ToString();
        statVal.UpdateStatVal(statTypeText.text.Trim(), rankText.text.Trim());
    }
}
