using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveSkill
{
    public enum PassiveType { Always, Buff, Instant };

    public string name, krName, type, explain;
    public int index;
    public PassiveType passiveType;
    public string[] skillLvEx;
    public float[] effectVal;
    public float[] cooldown;
    public int cost;
    public string iconPath;

    public int skillLv, equipOrder;
    public bool isBuy, isEquip, isCooldown;

    public PassiveSkill(PassiveSkill passiveSkill)
    {
        name = passiveSkill.name;
        krName = passiveSkill.krName;
        type = passiveSkill.type;
        explain = passiveSkill.explain;
        index = passiveSkill.index;
        passiveType = passiveSkill.passiveType;
        skillLvEx = passiveSkill.skillLvEx;
        effectVal = passiveSkill.effectVal;
        cooldown = passiveSkill.cooldown;
        cost = passiveSkill.cost;
        iconPath = passiveSkill.iconPath;
        skillLv = passiveSkill.skillLv;
        equipOrder = passiveSkill.equipOrder;
        isBuy = passiveSkill.isBuy;
        isEquip = passiveSkill.isEquip;
        isCooldown = passiveSkill.isCooldown;
    }

    public PassiveSkill()
    {
        
    }

}
