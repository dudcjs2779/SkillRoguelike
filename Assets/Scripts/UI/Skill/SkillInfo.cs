using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillInfo : MonoBehaviour
{
    RectTransform canvas5Rect;
    [SerializeField] RectTransform basicRect, statRankRect, skillLvRect;
    [SerializeField] Image skillIcon;
    [SerializeField] TextMeshProUGUI skillName, skillExplain;
    [SerializeField] TextMeshProUGUI[] basicValTitle, basicVal, statRankVal, skillLvVal;
    GridLayoutGroup Grid_Basic;

    RectTransform thisRect;
    public Vector2 standPos;

    private void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        if (basicRect != null) Grid_Basic = basicRect.GetComponentInChildren<GridLayoutGroup>();
    }

    void Start()
    {
        if (basicRect != null) standPos = basicRect.anchoredPosition;
    }

    void Update()
    {
        RectPosition();
    }

    void RectPosition(){

        // if(GameManager.Instance.controlType == GameManager.ControlType.KeyboardMouse){
        //     Vector2 mousePos = Input.mousePosition;
        //     if (mousePos.y < thisRect.rect.size.y){
        //         thisRect.pivot = new Vector2(0, 0);
        //     }
        //     else{
        //         thisRect.pivot = new Vector2(0, 1);
        //     }

        //     transform.position = Input.mousePosition;
        // }
    }

    void InfoPosition(RectTransform rect){
        Vector2 pos = rect.transform.position;

        if (pos.y < thisRect.rect.size.y)
        {
            pos.x += rect.rect.size.x / 2;
            pos.y -= rect.rect.size.y / 2;

            thisRect.pivot = new Vector2(0, 0);
        }
        else
        {
            pos += rect.rect.size / 2;
            thisRect.pivot = new Vector2(0, 1);
        }

        thisRect.transform.position = pos;
    }

    public void InputInfo_Active(ActiveSkill skill, RectTransform slotRect)
    {
        gameObject.SetActive(true);
        InfoPosition(slotRect);

        skillIcon.sprite = Resources.Load<Sprite>(skill.iconPath);
        skillName.text = skill.krName;
        skillExplain.text = skill.explain;

        if (skill.staminaPoint != 0)
        {
            basicVal[0].transform.parent.gameObject.SetActive(true);
            basicVal[0].text = skill.staminaPoint.ToString();
        }
        else basicVal[0].transform.parent.gameObject.SetActive(false);

        if (skill.mpPoint != 0)
        {
            basicVal[1].transform.parent.gameObject.SetActive(true);
            basicVal[1].text = skill.mpPoint.ToString();
        }
        else basicVal[1].transform.parent.gameObject.SetActive(false);

        if (skill.damage.Length > 0)
        {
            basicVal[2].transform.parent.gameObject.SetActive(true);

            if (skill.isBuff){
                basicValTitle[2].text = "효력";
                basicVal[2].text = string.Format("{0}%", skill.damage[0] * 100);
            }
            else{
                basicValTitle[2].text = "데미지";
                basicVal[2].text = (skill.damage[0] * 20).ToString();
            }

        }
        else basicVal[2].transform.parent.gameObject.SetActive(false);

        if (skill.stgDamage.Length > 0)
        {
            basicVal[3].transform.parent.gameObject.SetActive(true);
            basicVal[3].text = (skill.stgDamage[0] * 30).ToString();
        }
        else basicVal[3].transform.parent.gameObject.SetActive(false);

        if (skill.stgPower.Length > 0)
        {
            basicVal[4].transform.parent.gameObject.SetActive(true);
            basicVal[4].text = skill.stgPower[0].ToString();
        }
        else basicVal[4].transform.parent.gameObject.SetActive(false);

        if (skill.elementalVal.Length > 0)
        {
            basicVal[5].transform.parent.gameObject.SetActive(true);
            basicVal[5].text = skill.elementalVal[0].ToString();
        }
        else basicVal[5].transform.parent.gameObject.SetActive(false);

        if (skill.duration.Length > 0)
        {
            basicVal[6].transform.parent.gameObject.SetActive(true);
            basicVal[6].text = skill.duration[0].ToString();
        }
        else basicVal[6].transform.parent.gameObject.SetActive(false);

        if (statRankVal.Length > 1)
        {
            string[] ranks = skill.statRank;

            for (int i = 0; i < ranks.Length; i++)
            {
                statRankVal[i].text = ranks[i];
            }
        }

        skillLvVal[0].text = skill.skillLvEx[0];
    }

    public void InputInfo_Passvie(PassiveSkill skill, RectTransform slotRect)
    {
        gameObject.SetActive(true);
        InfoPosition(slotRect);

        print(skill.name);
        skillIcon.sprite = Resources.Load<Sprite>(skill.iconPath);
        skillName.text = skill.krName;
        skillExplain.text = skill.explain;

        skillLvVal[0].text = skill.skillLvEx[0];
        skillLvVal[1].text = skill.skillLvEx[1];
        skillLvVal[2].text = skill.skillLvEx[2];
    }
}
