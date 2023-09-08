using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD_Skill : MonoBehaviour
{
    [SerializeField] HUD_SkillSlot[] active_HUD_SkillSlots;
    [SerializeField] HUD_SkillSlot[] passive_HUD_SkillSlots;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        anim.SetBool("isChange", PlayerInputControls.Instance.skillChangeDown);
    }

    private void Start()
    {
        Init_HUDSkill();
    }

    public void Init_HUDSkill()
    {
        // 슬롯창 활성화
        if (GameManager.Instance.playerData.clearLv == 0)
        {
            active_HUD_SkillSlots[4].gameObject.SetActive(false);
            active_HUD_SkillSlots[5].gameObject.SetActive(false);
            active_HUD_SkillSlots[6].gameObject.SetActive(false);
            active_HUD_SkillSlots[7].gameObject.SetActive(false);

            passive_HUD_SkillSlots[4].gameObject.SetActive(false);
            passive_HUD_SkillSlots[5].gameObject.SetActive(false);

        }
        else if (GameManager.Instance.playerData.clearLv == 1)
        {
            active_HUD_SkillSlots[4].gameObject.SetActive(true);
            active_HUD_SkillSlots[5].gameObject.SetActive(true);
            active_HUD_SkillSlots[6].gameObject.SetActive(false);
            active_HUD_SkillSlots[7].gameObject.SetActive(false);

            passive_HUD_SkillSlots[4].gameObject.SetActive(true);
            passive_HUD_SkillSlots[5].gameObject.SetActive(false);
        }
        else if (GameManager.Instance.playerData.clearLv >= 2)
        {
            active_HUD_SkillSlots[4].gameObject.SetActive(true);
            active_HUD_SkillSlots[5].gameObject.SetActive(true);
            active_HUD_SkillSlots[6].gameObject.SetActive(true);
            active_HUD_SkillSlots[7].gameObject.SetActive(true);

            passive_HUD_SkillSlots[4].gameObject.SetActive(true);
            passive_HUD_SkillSlots[5].gameObject.SetActive(true);
        }

        // 슬롯창 채우기
        for (int i = 0; i < active_HUD_SkillSlots.Length; i++)
        {
            ActiveSkill activeSkill = GameManager.Instance.EquipActiveList.Find(x => x.equipOrder == i + 1);

            if(activeSkill == null)
            {
                active_HUD_SkillSlots[i].iconImg.sprite = Resources.Load<Sprite>("Sprite/Skill/Empty");
                active_HUD_SkillSlots[i].skillName.text = "";
                active_HUD_SkillSlots[i].skillLv.text = "";
                continue;
            }

            active_HUD_SkillSlots[i].iconImg.sprite = Resources.Load<Sprite>(activeSkill.iconPath);
            active_HUD_SkillSlots[i].skillName.text = activeSkill.krName;
            active_HUD_SkillSlots[i].skillLv.text = activeSkill.skillLv.ToString();
        }

        for (int i = 0; i < passive_HUD_SkillSlots.Length; i++)
        {
            PassiveSkill passiveSkill = GameManager.Instance.EquipPassiveList.Find(x => x.equipOrder == i + 1);

            if (passiveSkill == null)
            {
                passive_HUD_SkillSlots[i].iconImg.sprite = Resources.Load<Sprite>("Sprite/Skill/Empty");
                passive_HUD_SkillSlots[i].skillName.text = "";
                passive_HUD_SkillSlots[i].skillLv.text = "";
                continue;
            }

            passive_HUD_SkillSlots[i].iconImg.sprite = Resources.Load<Sprite>(passiveSkill.iconPath);
            passive_HUD_SkillSlots[i].skillName.text = passiveSkill.krName;
            passive_HUD_SkillSlots[i].skillLv.text = passiveSkill.skillLv.ToString();
        }

    }

}
