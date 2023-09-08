using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipSkillSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI nameText;
    public Image iconImg;
    public GameManager.SkillType tabType;

    RectTransform slotRect;

    SkillWindow skillWindow;

    private void Awake()
    {
        skillWindow = GetComponentInParent<SkillWindow>();
        slotRect= GetComponent<RectTransform>();
    }

    public void Unequip(){
        nameText.text = "";
        iconImg.sprite = Resources.Load<Sprite>("Sprite/Skill/Empty");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        if (eventData.pointerDrag != null && tabType == skillWindow.tabType)
        {
            //eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            SkillSlot skillSlot = eventData.pointerDrag.GetComponent<SkillSlot>();

            if(isTypeMached(skillSlot)){
                EquipSkillToSlot(skillSlot);
            }
        }
    }

    public bool isTypeMached(SkillSlot skillSlot){
        if (tabType == GameManager.SkillType.Active)
        {
            if (skillWindow.equipSkillSlotList_Active.Find(x => x.nameText.text == skillSlot.nameText.text) != null)
                return false;
            else
                return true;

        }
        else
        {
            if (skillWindow.equipSkillSlotList_Passive.Find(x => x.nameText.text == skillSlot.nameText.text) != null)
                return false;
            else
                return true;
        }
    }

    public void EquipSkillToSlot(SkillSlot skillSlot)
    {
        nameText.text = skillSlot.nameText.text;
        iconImg.sprite = skillSlot.iconImg.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nameText.text == "") return;

        if (tabType == GameManager.SkillType.Active)
        {
            ActiveSkill activeSkill = GameManager.Instance.MyActiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.ActiveSkillInfo(activeSkill, slotRect);

        }
        else
        {
            Vector2 pos = transform.position;
            pos += slotRect.rect.size / 2;

            PassiveSkill passiveSkill = GameManager.Instance.MyPassiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.PassiveSkillInfo(passiveSkill, slotRect);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("SkillSetExit");
        skillWindow.SkillInfoClose();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(nameText.text == "") return;

        if (tabType == GameManager.SkillType.Active)
        {
            ActiveSkill activeSkill = GameManager.Instance.MyActiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.ActiveSkillInfo(activeSkill, slotRect);
        }
        else
        {
            PassiveSkill passiveSkill = GameManager.Instance.MyPassiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.PassiveSkillInfo(passiveSkill, slotRect);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        skillWindow.SkillInfoClose();
    }

}
