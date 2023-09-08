using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuyCheck : MonoBehaviour
{
    public TextMeshProUGUI buyCheckText;
    [SerializeField] GameObject btn_Yes, btn_No;
    [SerializeField] RectTransform[] rects;
    Animator anim;

    ShopItemList shopItemList;

    public ActiveSkill actSkill;
    public PassiveSkill passSkill;
    public ShopItem curItem;

    public GameManager.ShopItemType type;
    public int cost;

    void Awake()
    {
        shopItemList = transform.parent.GetComponentInChildren<ShopItemList>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // buyCheckText.text = cost + " 골드를 내고 아이템을 구매할까요?";
        // btn_Yes.SetActive(true);
        // btn_No.SetActive(true);
    }

    public void Init_BuyCheck(ItemSlot itemSlot){
        type = itemSlot.itemType;
        cost = int.Parse(itemSlot.costText.text);
        actSkill = itemSlot.actSkill;
        passSkill = itemSlot.passSkill;
        curItem = itemSlot.curItem;

        buyCheckText.text = string.Format("{0} 골드를 내고 \"{1}\"을 구매할까요?", cost, itemSlot.nameText.text);
        btn_Yes.SetActive(true);
        btn_No.SetActive(true);
    }

    public void Btn_Yes()
    {
        print("Yes");

        if (type == GameManager.ShopItemType.Active)
        {
            print("SkillBuy");
            actSkill.isBuy = true;
            GameManager.Instance.playerData.money = GameManager.Instance.playerData.money - actSkill.cost;
        }
        else if (type == GameManager.ShopItemType.Passive)
        {
            print("SkillBuy");
            passSkill.isBuy = true;
            GameManager.Instance.playerData.money = GameManager.Instance.playerData.money - passSkill.cost;
        }
        else if(type == GameManager.ShopItemType.Item)
        {
            print("ItemBuy");
            curItem.isBuy = true;
            GameManager.Instance.playerData.money = GameManager.Instance.playerData.money - curItem.cost;
        }

        shopItemList.Init_ItemList();
        shopItemList.MyMoney();
        //GameManager.Instance.Save_ItemDataToJosn();
        //GameManager.Instance.Save_PlayerDataToJosn();
        //GameManager.Instance.Save_SkillDataToJosn();
        PlayerStateManager.Instance.Init_ShopStat();

        SoundManager.Instance.PlayUISound(SoundManager.UISFXType.ButtonClick01);
    }
}
