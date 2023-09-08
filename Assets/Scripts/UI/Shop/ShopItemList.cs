using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Linq;

public class ShopItemList : MonoBehaviour
{
    [SerializeField] RectTransform shopRect;
    CanvasGroup shopCanvasGroup;

    public GameManager.ShopItemType shopItemType;
    [SerializeField] TextMeshProUGUI myMoneyText;

    [SerializeField] GameObject itemSlot;
    [SerializeField] GameObject buyCheckWindow;

    public ItemSlot[] itemSlots;
    public GameObject[] itemDetailType;

    [SerializeField] Button[] tabBtns;
    [SerializeField] Image[] tabBtn_Images;
    public int tabIndex = 0;

    [SerializeField] GameObject buyCheck;
    [SerializeField] GameObject buyCheckFirst;
    [SerializeField] GameObject noMoeny;
    [SerializeField] GameObject noMoenyFirst;

    ItemSlot clickedItem;

    private void Awake()
    {
        itemSlots = GetComponentsInChildren<ItemSlot>(true);
        shopCanvasGroup = shopRect.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Shop))
            shopRect.gameObject.SetActive(false);
    }

    void Update()
    {
        TabChange();
    }

    private void OnEnable() {
        tabIndex = 0;
        shopItemType = GameManager.ShopItemType.Active;
        Init_ItemList();
        StartCoroutine(Init_TabBtn());
    }

    IEnumerator Init_TabBtn(){
        yield return new WaitForEndOfFrame();
        tabBtns[1].animator.SetBool("isLocked", false);
        tabBtns[2].animator.SetBool("isLocked", false);
        tabBtns[0].animator.SetBool("isLocked", true);
        tabBtns[0].animator.SetTrigger("Selected");
    }

    public void Init_ItemList()
    {
        MyMoney();

        // 상점 슬롯들 초기화
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].gameObject.SetActive(false);
            itemSlots[i].isUnlock = false;
            itemSlots[i].isBuy = false;
        }

        // 언락 조건 체크
        CheckShopItemUnlock();

        // 슬롯에 데이터 넣기
        if (shopItemType == GameManager.ShopItemType.Active)
        {
            List<ActiveSkill> curShopList_ActSkill = new List<ActiveSkill>();   // 실제 리스트
            List<ShopUnlockData> curUnlockActiveList = new List<ShopUnlockData>();  // 임시 리스트 언락데이터
            List<ActiveSkill> tempActiveList = GameManager.Instance.MyActiveSkillList.FindAll(x => !x.isBuy);   // 임시 리스트 액티브스킬

            // 구매하지 않은 언락데이터
            for (int i = 0; i < tempActiveList.Count; i++)
            {
                ShopUnlockData shopUnlockData = GameManager.Instance.MyShopUnlockList.Find(x => x.name == tempActiveList[i].name);
                curUnlockActiveList.Add(shopUnlockData);
            }

            // 정렬
            curUnlockActiveList = curUnlockActiveList.OrderBy(x => !x.isUnlock).ToList();
            foreach (var item in curUnlockActiveList)
            {
                //Debug.Log(item.name + " " + item.isUnlock);
            }

            // 결과를 curShopList_ActSkill 에 삽입
            for (int i = 0; i < curUnlockActiveList.Count; i++)
            {
                ActiveSkill activeSkill =  tempActiveList.Find(x => x.name == curUnlockActiveList[i].name);
                curShopList_ActSkill.Add(activeSkill);
                //Debug.Log(curUnlockActiveList[i].name);
            }

            //스킬
            for (int i = 0; i < curShopList_ActSkill.Count; i++)
            {
                ShopUnlockData unlockSkill = curUnlockActiveList[i];
                //Debug.Log(i.ToString() + curShopList_ActSkill[i].name + " " + unlockSkill.name + " " + itemSlots[i].name);

                if (GameManager.Instance.playerData.clearLv >= (int)unlockSkill.clearLv) itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].itemImageBG.color = new Vector4(0, 0, 0, 1f);

                string Path = curShopList_ActSkill[i].iconPath;
                itemSlots[i].itemImage.sprite = Resources.Load<Sprite>(Path);

                itemSlots[i].actSkill = curShopList_ActSkill[i];
                itemSlots[i].nameText.text = curShopList_ActSkill[i].krName;
                itemSlots[i].isUnlock = unlockSkill.isUnlock;
                itemSlots[i].isBuy = curShopList_ActSkill[i].isBuy;
                itemSlots[i].itemType = shopItemType;

                // 설명 또는 해금 힌트
                if (unlockSkill.isUnlock)
                {
                    itemSlots[i].explainText.text = curShopList_ActSkill[i].explain;
                }
                else
                {
                    itemSlots[i].explainText.text = UnlockHint(unlockSkill, itemSlots[i]);
                }

                itemSlots[i].costText.text = curShopList_ActSkill[i].cost.ToString();
                itemSlots[i].EnableItem(unlockSkill.isUnlock);
            }

            ItemSlot[] visibleItems = Array.FindAll(itemSlots, x => x.gameObject.activeSelf);
            SetNavigation(visibleItems);
        }
        else if (shopItemType == GameManager.ShopItemType.Passive)
        {
            List<PassiveSkill> curShopList_PassSkill = new List<PassiveSkill>();    // 실제 리스트
            List<ShopUnlockData> curUnlockPassList = new List<ShopUnlockData>();  // 임시 리스트 언락데이터
            List<PassiveSkill> tempPassiveList = GameManager.Instance.MyPassiveSkillList.FindAll(x => !x.isBuy);   // 임시 리스트 액티브스킬

            // 구매하지 않은 언락데이터
            for (int i = 0; i < tempPassiveList.Count; i++){
                ShopUnlockData shopUnlockData = GameManager.Instance.MyShopUnlockList.Find(x => x.name == tempPassiveList[i].name);
                curUnlockPassList.Add(shopUnlockData);
            }

            // 정렬
            curUnlockPassList = curUnlockPassList.OrderBy(x => !x.isUnlock).ToList();

            // 결과를 curShopList_ActSkill 에 삽입
            for (int i = 0; i < curUnlockPassList.Count; i++){
                PassiveSkill passiveSkill = tempPassiveList.Find(x => x.name == curUnlockPassList[i].name);
                curShopList_PassSkill.Add(passiveSkill);
            }

            //스킬
            for (int i = 0; i < curShopList_PassSkill.Count; i++)
            {
                //Debug.Log(curShopList_PassSkill[i].name);
                ShopUnlockData unlockSkill = curUnlockPassList[i];

                if (GameManager.Instance.playerData.clearLv >= (int)unlockSkill.clearLv) itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].itemImageBG.color = new Vector4(0, 0, 0, 1f);

                string Path = curShopList_PassSkill[i].iconPath;
                itemSlots[i].itemImage.sprite = Resources.Load<Sprite>(Path);

                itemSlots[i].passSkill = curShopList_PassSkill[i];
                itemSlots[i].nameText.text = curShopList_PassSkill[i].krName;
                itemSlots[i].isUnlock = unlockSkill.isUnlock;
                itemSlots[i].isBuy = curShopList_PassSkill[i].isBuy;
                itemSlots[i].itemType = shopItemType;

                // 설명 또는 해금 힌트
                if (unlockSkill.isUnlock) itemSlots[i].explainText.text = curShopList_PassSkill[i].explain;
                else
                {
                    itemSlots[i].explainText.text = UnlockHint(unlockSkill, itemSlots[i]);
                }

                itemSlots[i].costText.text = curShopList_PassSkill[i].cost.ToString();

                itemSlots[i].EnableItem(unlockSkill.isUnlock);
            }

            ItemSlot[] visibleItems = Array.FindAll(itemSlots, x => x.gameObject.activeSelf);
            SetNavigation(visibleItems);
        }
        else if (shopItemType == GameManager.ShopItemType.Item)
        {
            //아이템
            List<ShopItem> CurShopList_Item = GameManager.Instance.MyShopItemList.FindAll(x => !x.isBuy);
            //List<ShopItem> CurShopList_Item = GameManager.Instance.MyShopItemList;

            for (int i = 0; i < CurShopList_Item.Count; i++)
            {
                ShopUnlockData unlockItem = GameManager.Instance.MyShopUnlockList.Find(x => x.name == CurShopList_Item[i].name);
                Debug.Log(CurShopList_Item[i].name);

                if (GameManager.Instance.playerData.clearLv >= (int)unlockItem.clearLv)
                {
                    itemSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    itemSlots[i].gameObject.SetActive(false);
                    continue;
                }

                itemSlots[i].itemImageBG.color = new Vector4(0.6941177f, 0.7744685f, 0.8196079f, 1f);

                string Path = CurShopList_Item[i].iconPath;
                itemSlots[i].itemImage.sprite = Resources.Load<Sprite>(Path);
                itemSlots[i].curItem = CurShopList_Item[i];
                itemSlots[i].nameText.text = CurShopList_Item[i].krName;
                itemSlots[i].explainText.text = !CurShopList_Item[i].isBuy ? CurShopList_Item[i].explain : "구매 완료";
                itemSlots[i].costText.text = CurShopList_Item[i].cost.ToString();
                itemSlots[i].isUnlock = unlockItem.isUnlock;
                itemSlots[i].isBuy = CurShopList_Item[i].isBuy;
                itemSlots[i].itemType = shopItemType;
                itemSlots[i].EnableItem(unlockItem.isUnlock && !CurShopList_Item[i].isBuy);
            }

            ItemSlot[] visibleItems = Array.FindAll(itemSlots, x => x.gameObject.activeSelf);
            SetNavigation(visibleItems);
        }

        ItemSlot itemSlot = Array.Find(itemSlots, x => x.isUnlock);
        if (itemSlot != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(itemSlot.gameObject);
        }
    }

    string UnlockHint(ShopUnlockData unlockSkill, ItemSlot itemSlot)
    {
        string unlockHint;
        if (unlockSkill.Oak == 0 && unlockSkill.Lich == 0 && unlockSkill.Wolf == 0)
        {
            unlockHint = string.Format("{0} 난이도 클리어", unlockSkill.clearLv);
            if (unlockSkill.clearLv == 0) itemSlot.gameObject.SetActive(false);
        }
        else
        {
            string oak = "", lich = "", wolf = "";
            int oakCount = 0, lichCount = 0, wolfCount = 0;
            switch (unlockSkill.clearLv)
            {
                case ShopUnlockData.StageLv.디폴트:
                    oakCount = GameManager.Instance.playerData.easyOak;
                    lichCount = GameManager.Instance.playerData.easyLich;
                    wolfCount = GameManager.Instance.playerData.easyWolf;
                    break;

                case ShopUnlockData.StageLv.쉬움:
                    oakCount = GameManager.Instance.playerData.normalOak;
                    lichCount = GameManager.Instance.playerData.normalLich;
                    wolfCount = GameManager.Instance.playerData.normalWolf;
                    break;

                case ShopUnlockData.StageLv.보통:
                    oakCount = GameManager.Instance.playerData.hardOak;
                    lichCount = GameManager.Instance.playerData.hardLich;
                    wolfCount = GameManager.Instance.playerData.hardWolf;
                    break;
            }

            if (unlockSkill.Oak != 0)
                oak = string.Format("오크 {0}/{1}마리 ", oakCount, unlockSkill.Oak);
            if (unlockSkill.Lich != 0)
                lich = string.Format("리치 {0}/{1}마리 ", lichCount, unlockSkill.Lich);
            if (unlockSkill.Wolf != 0)
                wolf = string.Format("늑대 {0}/{1}마리 ", wolfCount, unlockSkill.Wolf);

            unlockHint = string.Format("{0} 난이도에서 {1} {2} {3} 토벌", unlockSkill.clearLv + 1, oak, lich, wolf);
        }

        return unlockHint;
    }

    void SetNavigation(ItemSlot[] itemSlots){
        for (int i = 0; i < itemSlots.Length; i++)
        {
            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;
            customNav.selectOnUp = i > 0 ? itemSlots[i - 1].btn : null;
            customNav.selectOnDown = i < itemSlots.Length - 1 ? itemSlots[i + 1].btn : null;

            itemSlots[i].btn.navigation = customNav;
        }
    }

    void CheckShopItemUnlock()
    {
        //List<ShopItem> shopItemList = GameManager.Instance.ShopItemList.FindAll(x => !x.isUnlock);
        List<ShopUnlockData> lockSkillList = GameManager.Instance.MyShopUnlockList.FindAll(x => !x.isUnlock);
        Debug.Log(lockSkillList.Count);

        foreach (ShopUnlockData lockSkill in lockSkillList)
        {
            Debug.Log(lockSkill.name + " " + lockSkill.isUnlock);
            if(GameManager.Instance.playerData.clearLv < (int)lockSkill.clearLv){
                lockSkill.isUnlock = false;
                continue;
            }

            if (lockSkill.clearLv == ShopUnlockData.StageLv.디폴트)
            {
                if (GameManager.Instance.playerData.easyOak >= lockSkill.Oak
                    && GameManager.Instance.playerData.easyLich >= lockSkill.Lich
                    && GameManager.Instance.playerData.easyWolf >= lockSkill.Wolf)
                    lockSkill.isUnlock = true;
            }
            else if (lockSkill.clearLv == ShopUnlockData.StageLv.쉬움)
            {
                if (GameManager.Instance.playerData.normalOak >= lockSkill.Oak
                    && GameManager.Instance.playerData.normalLich >= lockSkill.Lich
                    && GameManager.Instance.playerData.normalWolf >= lockSkill.Wolf)
                    lockSkill.isUnlock = true;
            }
            else if (lockSkill.clearLv == ShopUnlockData.StageLv.보통)
            {
                if (GameManager.Instance.playerData.hardOak >= lockSkill.Oak
                    && GameManager.Instance.playerData.hardLich >= lockSkill.Lich
                    && GameManager.Instance.playerData.hardWolf >= lockSkill.Wolf)
                    lockSkill.isUnlock = true;
            }
        }
    }

    public void BtnActive()
    {
        if(shopItemType == GameManager.ShopItemType.Active) return;
        Debug.Log("BtnActive");

        tabBtns[1].animator.SetBool("isLocked", false);
        tabBtns[1].animator.SetTrigger("Normal");
        tabBtns[2].animator.SetBool("isLocked", false);
        tabBtns[2].animator.SetTrigger("Normal");
        tabBtns[0].animator.SetBool("isLocked" , true);
        tabBtns[0].animator.SetTrigger("Selected");

        shopItemType = GameManager.ShopItemType.Active;
        Init_ItemList();

    }

    public void BtnPassive()
    {
        if (shopItemType == GameManager.ShopItemType.Passive) return;

        tabBtns[0].animator.SetTrigger("Normal");
        tabBtns[0].animator.SetBool("isLocked", false);
        tabBtns[2].animator.SetTrigger("Normal");
        tabBtns[2].animator.SetBool("isLocked", false);

        tabBtns[1].animator.SetBool("isLocked", true);
        tabBtns[1].animator.SetTrigger("Selected");
        
        shopItemType = GameManager.ShopItemType.Passive;
        Init_ItemList();

    }

    public void BtnItem()
    {
        if (shopItemType == GameManager.ShopItemType.Item) return;

        tabBtns[0].animator.SetBool("isLocked", false);
        tabBtns[0].animator.SetTrigger("Normal");
        tabBtns[1].animator.SetBool("isLocked", false);
        tabBtns[1].animator.SetTrigger("Normal");

        tabBtns[2].animator.SetBool("isLocked", true);
        tabBtns[2].animator.SetTrigger("Selected");
        
        shopItemType = GameManager.ShopItemType.Item;
        Init_ItemList();

    }

    void TabChange()
    {
        if(Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Shop_ItemSlot)) return;
        if (PlayerInputControls.Instance.doChangeTabLeft)
        {
            PlayerInputControls.Instance.doChangeTabLeft = false;

            if (tabIndex <= 0) return;
            tabBtns[tabIndex].animator.SetTrigger("Normal");

            tabIndex--;
            tabBtns[tabIndex].onClick.Invoke();
            //tabBtns[tabIndex].animator.SetTrigger("Selected");
        }
        else if(PlayerInputControls.Instance.doChangeTabRight){
            PlayerInputControls.Instance.doChangeTabRight = false;

            if (tabIndex >= 2) return;
            tabBtns[tabIndex].animator.SetTrigger("Normal");

            tabIndex++;
            tabBtns[tabIndex].onClick.Invoke();
            //tabBtns[tabIndex].animator.SetTrigger("Selected");

        }
    }

    public void MyMoney()
    {
        myMoneyText.text = GameManager.Instance.playerData.money.ToString();
    }

    public void OnClick_ShopSlot(){
        clickedItem = EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>();

        if (!clickedItem.isUnlock){
            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Error01);
            clickedItem = null;
            return;
        }


        int cost = int.Parse(clickedItem.costText.text);

        if(GameManager.Instance.playerData.money >= cost){
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Shop_ItemSlot);
            Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_ShopSlot);
            shopCanvasGroup.interactable = false;

            buyCheck.SetActive(true);
            buyCheck.GetComponent<BuyCheck>().Init_BuyCheck(clickedItem);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buyCheckFirst);

            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.OpenSmall01);
        }
        else{
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Shop_ItemSlot);
            Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_ShopSlot);
            shopCanvasGroup.interactable = false;

            noMoeny.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(noMoenyFirst);

            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Error01);
        }
    }

    public void Escape_OnClick_ShopSlot(){
        buyCheck.SetActive(false);
        noMoeny.SetActive(false);
        shopCanvasGroup.interactable = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clickedItem.gameObject);
        tabBtns[tabIndex].animator.SetTrigger("Selected");

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.Shop_ItemSlot);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_ShopSlot);
    }
}
