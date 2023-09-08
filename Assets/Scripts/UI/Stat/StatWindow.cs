using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatWindow : MonoBehaviour
{
    StatRank statRank;
    StatRankSlot clicked_StatRankSlot;

    private void Awake() {
        statRank = GetComponentInChildren<StatRank>();
    }

    public void OnClick_StatRankSlot(){
        Debug.Log("OnClickStatRow");
        StatRankSlot statRankSlot = EventSystem.current.currentSelectedGameObject.GetComponent<StatRankSlot>();
        clicked_StatRankSlot = statRankSlot;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(statRankSlot.btnUp.gameObject);
        if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_StatRank)){
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_StatRank);
            Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_StatRankSlot);
        }
    }

    public void Escape_OnClick_StatRankSlot(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_StatRankSlot.gameObject);
        clicked_StatRankSlot = null;

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_StatRank);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_StatRankSlot);
    }

}
