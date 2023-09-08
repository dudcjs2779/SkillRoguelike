using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopUnlockData
{
    public enum StageLv { 디폴트, 쉬움, 보통, 어려움 };
    public enum ItemType { 액티브, 패시브, 아이템 };

    public string name, krName, unlockHint;
    public int index;
    public StageLv clearLv;
    public ItemType itemType;
    public int Oak, Lich, Wolf;
    
    public bool isUnlock;

    public ShopUnlockData(ShopUnlockData shopUnlockData)
    {
        name = shopUnlockData.name;
        krName = shopUnlockData.krName;
        unlockHint = shopUnlockData.unlockHint;
        index = shopUnlockData.index;
        clearLv = shopUnlockData.clearLv;
        itemType = shopUnlockData.itemType;
        Oak = shopUnlockData.Oak;
        Lich = shopUnlockData.Lich;
        Wolf = shopUnlockData.Wolf;
        isUnlock = shopUnlockData.isUnlock;
    }

    public ShopUnlockData()
    {
        // 필드 초기화 또는 아무 작업 없이 빈 상태로 생성
    }

}
