using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatVal : MonoBehaviour
{
    [SerializeField] StatRank statRank;

    public StatValSlot[] statValSlots;
    string[] statVals;

    public float STR_meleePower, STR_health;
    public float AGL_meleePower, AGL_stamina;
    public float VIT_meleeStgDam, VIT_meleeStgPower, VIT_health;
    public float INT_effectDuration, INT_element, INT_MP;
    public float MP_magicPower, MP_MP;
    public float MST_MpRecover, MST_magicStgDam, MST_MP;

    private void Awake()
    {
        statValSlots = GetComponentsInChildren<StatValSlot>();
        statVals = new string[statValSlots.Length];
    }

    void Start()
    {
        StatValLoad();
    }

    void Update()
    {
        
    }

    public void StatValLoad()
    {
        StatValData statValData = GameManager.Instance.statValData;

        statValSlots[0].valueText.text = statValData.GetHealth().ToString();  //HP
        statValSlots[1].valueText.text = statValData.GetStamina().ToString();    //스테미너
        statValSlots[2].valueText.text = statValData.GetMeleeDam().ToString();  //물리공격력
        statValSlots[3].valueText.text = statValData.GetMeleeStgDam().ToString();   //물리 경직치
        statValSlots[4].valueText.text = statValData.GetMeleeStgPower().ToString(); //물리 경직 성능
        statValSlots[5].valueText.text = statValData.GetMp().ToString();  //MP
        statValSlots[6].valueText.text = statValData.GetMagicPower().ToString(); //마법 공격력
        statValSlots[7].valueText.text = statValData.GetMagicStgDam().ToString(); //마법 경직치
        statValSlots[8].valueText.text = statValData.GetElement().ToString();    //속성치
        statValSlots[9].valueText.text = statValData.GetMpRecover().ToString();   //MP 회복
        statValSlots[10].valueText.text = statValData.GetEffectDuration().ToString();   //버프 지속

        // UpdateStatVal("힘", GameManager.Instance.playerData.STR);
        // UpdateStatVal("민첩", GameManager.Instance.playerData.AGL);
        // UpdateStatVal("활력", GameManager.Instance.playerData.VIT);
        // UpdateStatVal("지력", GameManager.Instance.playerData.INT);
        // UpdateStatVal("마력", GameManager.Instance.playerData.MP);
        // UpdateStatVal("정신력", GameManager.Instance.playerData.MST);

        // //Debug.Log("a");
        // for (int i = 0; i < statValSlots.Length; i++)
        // {
        //     statValSlots[i].valueText.text = statValSlots[i].nextValueText.text;
        // }
    }

    public void Cancle_InStatVals()
    {
        for (int i = 0; i < statValSlots.Length; i++)
        {
            statValSlots[i].nextValueText.text = statValSlots[i].valueText.text;
        }
    }

    // 스텟 랭크에 따른 능력치 업데이트
    public void UpdateStatVal(string statType, string rank)
    {
        StatValData statValData = GameManager.Instance.statValData;
        statValData.StatUpdate(GameManager.Instance.playerData);

        statValSlots[0].nextValueText.text = statValData.GetHealth().ToString();  //HP
        statValSlots[1].nextValueText.text = statValData.GetStamina().ToString();    //스테미너
        statValSlots[2].nextValueText.text = statValData.GetMeleeDam().ToString();  //물리공격력
        statValSlots[3].nextValueText.text = statValData.GetMeleeStgDam().ToString();   //물리 경직치
        statValSlots[4].nextValueText.text = statValData.GetMeleeStgPower().ToString(); //물리 경직 성능
        statValSlots[5].nextValueText.text = statValData.GetMp().ToString();  //MP
        statValSlots[6].nextValueText.text = statValData.GetMagicPower().ToString(); //마법 공격력
        statValSlots[7].nextValueText.text = statValData.GetMagicStgDam().ToString(); //마법 경직치
        statValSlots[8].nextValueText.text = statValData.GetElement().ToString();    //속성치
        statValSlots[9].nextValueText.text = statValData.GetMpRecover().ToString();   //MP 회복
        statValSlots[10].nextValueText.text = statValData.GetEffectDuration().ToString();   //버프 지속
    }
}
