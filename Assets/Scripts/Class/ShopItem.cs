using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string name, krName, iconPath, explain, explainDetail;
    public int index;
    public int cost;
    public bool isEquipItem;

    public bool isUnlock, isBuy, isEquip;

    public ShopItem(ShopItem shopItem)
    {
        name = shopItem.name;
        krName = shopItem.krName;
        iconPath = shopItem.iconPath;
        explain = shopItem.explain;
        explainDetail = shopItem.explainDetail;
        index = shopItem.index;
        cost = shopItem.cost;
        isEquipItem = shopItem.isEquipItem;
        isUnlock = shopItem.isUnlock;
        isBuy = shopItem.isBuy;
        isEquip = shopItem.isEquip;
    }

    public ShopItem()
    {
        // 필드 초기화 또는 아무 작업 없이 빈 상태로 생성
    }
}
