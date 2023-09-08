using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class SkillTab : MonoBehaviour
{
    [SerializeField] SkillInfo activeSkillInfo;
    [SerializeField] SkillInfo passiveSkillInfo;

    [SerializeField] Selectable firstSelect;
    [SerializeField] Selectable firstSelect_Active;
    [SerializeField] Selectable firstSelect_Passive;

    Selectable selectedSkillType;

    GridLayoutGroup gridLayoutGroup;

    private void Awake() {
    }

    private void Start() {

    }

    public void FirstSelect(){
        firstSelect.Select();
    }

    public void OnClickActive(){
        selectedSkillType = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        firstSelect_Active.Select();

        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatPanelActive);
        Canvas5.Instance.EscapeActionList.Add(EscapeActive);
    }

    void EscapeActive(){
        selectedSkillType.Select();
        selectedSkillType = null;

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatPanelActive);
        Canvas5.Instance.EscapeActionList.Remove(EscapeActive);
    }

    public void OnClickPassive()
    {
        selectedSkillType = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        firstSelect_Passive.Select();

        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatPanelPassive);
        Canvas5.Instance.EscapeActionList.Add(EscapePassive);
    }

    void EscapePassive()
    {
        selectedSkillType.Select();
        selectedSkillType = null;

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatPanelPassive);
        Canvas5.Instance.EscapeActionList.Remove(EscapePassive);
    }

    public void ShowSkillInfo(StatPanelSkillSlot statPanelSkillSlot){
        if(statPanelSkillSlot.skillType == GameManager.SkillType.Active){
            ActiveSkill activeSkill = statPanelSkillSlot.GetActvieSkill();
            activeSkillInfo.InputInfo_Active(activeSkill, statPanelSkillSlot.GetRect());
        }
        else{
            PassiveSkill passiveSkill = statPanelSkillSlot.GetPassiveSkill();
            passiveSkillInfo.InputInfo_Passvie(passiveSkill, statPanelSkillSlot.GetRect());
        }
    }

    public void CloseSkillInfo(){
        activeSkillInfo.gameObject.SetActive(false);
        passiveSkillInfo.gameObject.SetActive(false);
    }
}
