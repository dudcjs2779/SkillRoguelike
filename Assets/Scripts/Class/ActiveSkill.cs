using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ActiveSkill
{
    public string name, krName, type, explain;
    public int index;
    public string[] statRank, skillLvEx;
    public float effectByStat;
    public int staminaPoint, mpPoint, cost;
    public float[] damage, stgDamage, stgPower, elementalVal, duration;
    public bool isMagicAttack, isBuff, isAiming;
    public string iconPath;

    public int skillLv, equipOrder;
    public bool isBuy, isEquip;

    public ActiveSkill()
    {
        
    }

    public ActiveSkill(ActiveSkill activeSkill){
        name = activeSkill.name;
        krName = activeSkill.krName;
        type = activeSkill.type;
        explain = activeSkill.explain;
        index = activeSkill.index;
        statRank = activeSkill.statRank;
        skillLvEx = activeSkill.skillLvEx;
        effectByStat = activeSkill.effectByStat;
        staminaPoint = activeSkill.staminaPoint;
        mpPoint = activeSkill.mpPoint;
        cost = activeSkill.cost;
        damage = activeSkill.damage;
        stgDamage = activeSkill.stgDamage;
        stgPower = activeSkill.stgPower;
        elementalVal = activeSkill.elementalVal;
        duration = activeSkill.duration;
        isMagicAttack = activeSkill.isMagicAttack;
        isBuff = activeSkill.isBuff;
        isAiming = activeSkill.isAiming;
        iconPath = activeSkill.iconPath;
        skillLv = activeSkill.skillLv;
        equipOrder = activeSkill.equipOrder;
        isBuy = activeSkill.isBuy;
        isEquip = activeSkill.isEquip;
    }
}