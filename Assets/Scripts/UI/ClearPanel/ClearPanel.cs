using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClearPanel : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI clearTimeText;
    public TextMeshProUGUI clearStageText;
    public TextMeshProUGUI gainGoldText;
    public TextMeshProUGUI oakText;
    public TextMeshProUGUI lichText;
    public TextMeshProUGUI wolfText;
    public ClearPanelSkillSlot[] skillSlots;
    [SerializeField] Button btnConfirm;

    Animator anim;

    private void Awake()
    {
        skillSlots = GetComponentsInChildren<ClearPanelSkillSlot>(true);
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetTrigger("SmoothScale");
        Init_ClearPanel();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(btnConfirm.gameObject);

        if(!GameManager.Instance.isGameOver){
            titleText.text = "클리어!";
        }
        else{
            titleText.text = "게임 오버";
        }
    }

    void Init_ClearPanel()
    {
        playerLevelText.text = string.Format("레벨: {0}", GameManager.Instance.playerLv.ToString());
        difficultyText.text = string.Format("난이도: {0}", GameManager.Instance.difficultyType.ToString());

        TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.dungeonPlayTime);
        string timeText = string.Format("시간:  {0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        clearTimeText.text = timeText;

        clearStageText.text = string.Format("돌파 스테이지: {0}", GameManager.Instance.stageCount.ToString());
        gainGoldText.text = string.Format("획득한 골드: {0}", GameManager.Instance.gainGold.ToString());

        oakText.text = string.Format("오크: {0}", GameManager.Instance.oakKillCount.ToString());
        lichText.text = string.Format("리치: {0}", GameManager.Instance.lichKillCount.ToString());
        wolfText.text = string.Format("늑대: {0}", GameManager.Instance.wolfKillCount.ToString());

        ActiveSkill[] activeSkills = GameManager.Instance.EquipActiveList.ToArray();
        PassiveSkill[] passiveSkills = GameManager.Instance.EquipPassiveList.ToArray();

        List<SkillSlotStruct> skills = new List<SkillSlotStruct>();

        for (int i = 0; i < activeSkills.Length; i++)
        {
            skills.Add(new SkillSlotStruct(activeSkills[i].krName, activeSkills[i].iconPath, activeSkills[i].skillLv));
        }

        for (int i = 0; i < passiveSkills.Length; i++)
        {
            skills.Add(new SkillSlotStruct(passiveSkills[i].krName, passiveSkills[i].iconPath, passiveSkills[i].skillLv));
        }

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if(skills.Count <= i)
            {
                skillSlots[i].gameObject.SetActive(false);
                continue;
            }

            skillSlots[i].gameObject.SetActive(true);
            skillSlots[i].skillNameText.text = skills[i].skillname;
            skillSlots[i].skillImage.sprite = Resources.Load<Sprite>(skills[i].skillPath);
            skillSlots[i].skillLevelText.text = skills[i].skillLevel.ToString();
        }
    }

    
}

public struct SkillSlotStruct
{
    public string skillname;
    public string skillPath;
    public int skillLevel;

    public SkillSlotStruct(string _skillname, string _skillPath, int _skillLevel)
    {
        skillname = _skillname;
        skillPath = _skillPath;
        skillLevel = _skillLevel;
    }
}
