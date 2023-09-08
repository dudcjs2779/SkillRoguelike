using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long createDate;
    public long lastUpdated;

    public List<ActiveSkill> ActiveSkillList;
    public List<PassiveSkill> PassiveSkillList;
    public List<ShopUnlockData> ShopUnlockList;
    public List<ShopItem> ShopItemList;
    public PlayerData playerData;
    public string stroy;

    public GameData(){

    }


}
