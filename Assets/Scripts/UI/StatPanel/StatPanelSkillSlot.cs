using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatPanelSkillSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillLevelText;
    public Image skillImage;
    public GameManager.SkillType skillType;

    RectTransform slotRect;
    SkillTab skillTab;

    private void Awake() {
        slotRect = GetComponent<RectTransform>();
        skillTab = GetComponentInParent<SkillTab>();
    }

    public RectTransform GetRect(){
        return slotRect;
    }

    public ActiveSkill GetActvieSkill(){
        return GameManager.Instance.EquipActiveList.Find(x => x.krName == skillNameText.text);
    }

    public PassiveSkill GetPassiveSkill(){
        return GameManager.Instance.EquipPassiveList.Find(x => x.krName == skillNameText.text);
    }

    public void OnSelect(BaseEventData eventData)
    {
        skillTab.ShowSkillInfo(this);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        skillTab.CloseSkillInfo();
    }
}
