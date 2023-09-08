using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SkillWindow : MonoBehaviour
{
    public List<EquipSkillSlot> equipSkillSlotList_Active;
    public List<EquipSkillSlot> equipSkillSlotList_Passive;
    [SerializeField] Button[] tabBtns;
    int tabIndex = 0;

    public GameManager.SkillType tabType;

    Player player;
    PlayerSkill playerSkill;
    PlayerStateManager playerStateManager;
    StatRank statRank;
    StatVal statVal;
    HUD_Skill hud_Skill;

    public SkillSlot[] skillSlots;
    public Button[] skillSlotBtns;

    [SerializeField] RectTransform skillStatRect;

    [SerializeField] GameObject skillInfoAct;
    [SerializeField] GameObject skillInfoPass;
    [SerializeField] GameObject equipMenu;

    int oriIndex;

    [Header("Navigation")]
    [SerializeField] Button equipActiveA;
    [SerializeField] Button equipActiveB;
    [SerializeField] Button equipPassive;
    [SerializeField] GameObject skillSet;
    [SerializeField] GameObject skillSetFirst;
    [SerializeField] GameObject skillSetActiveSlotFirst;
    [SerializeField] GameObject skillSetPassiveSlotFirst;
    [SerializeField] GameObject equipMenuFirst;


    public SkillSlot clicked_SkillSlot;
    public GameObject clicked_Equip;
    public EquipSkillSlot clicked_EquipSkillSlot;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        playerSkill = GameObject.Find("Player").GetComponent<PlayerSkill>();
        playerStateManager = player.GetComponent<PlayerStateManager>();
        statVal = transform.root.GetComponentInChildren<StatVal>();
        statRank = transform.root.GetComponentInChildren<StatRank>();
        hud_Skill = GameObject.Find("Canvas0").GetComponentInChildren<HUD_Skill>();
        skillSlots = GetComponentsInChildren<SkillSlot>(true);
        skillSlotBtns = new Button[skillSlots.Length];
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlotBtns[i] = skillSlots[i].GetComponent<Button>();
        }
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        tabIndex = 0;
        StartCoroutine(Init_EndOfFrame());
        Init_ActiveSkillSlots();
        EquipSet_Init();
    }

    void Update()
    {
        TabChange();
    }

    IEnumerator Init_EndOfFrame(){
        yield return new WaitForEndOfFrame();
        tabBtns[0].animator.SetTrigger("Selected");
    }

    public void Init_ActiveSkillSlots()
    {
        tabType = GameManager.SkillType.Active;

        List<ActiveSkill> MyActiveList = GameManager.Instance.MyActiveSkillList.FindAll(x=> x.isBuy);
        //List<ActiveSkill> MyActiveList = GameManager.Instance.MyActiveSkillList;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (MyActiveList.Count <= i)
            {
                skillSlots[i].gameObject.SetActive(false);
                continue;
            }

            skillSlots[i].gameObject.SetActive(true);
            skillSlots[i].nameText.text = MyActiveList[i].krName;
            skillSlots[i].iconImg.sprite = Resources.Load<Sprite>(MyActiveList[i].iconPath);
            skillSlots[i].tabType = tabType;

            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;
            customNav.selectOnUp = i >= 4 ? skillSlotBtns[i - 4] : null;
            customNav.selectOnDown = i + 4 <= skillSlots.Length ? skillSlotBtns[i + 4] : null;
            customNav.selectOnLeft = i >= 1 ? skillSlotBtns[i - 1] : null;
            customNav.selectOnRight = i + 1 <= skillSlots.Length ? skillSlotBtns[i + 1] : null;

            skillSlotBtns[i].navigation = customNav;
        }
        skillSlotBtns[0].Select();
    }

    public void Init_PassiveSkillSlots()
    {
        tabType = GameManager.SkillType.Passive;

        List<PassiveSkill> MyPassiveList = GameManager.Instance.MyPassiveSkillList.FindAll(x=> x.isBuy);
        //List<PassiveSkill> MyPassiveList = GameManager.Instance.MyPassiveSkillList;

        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (MyPassiveList.Count <= i)
            {
                skillSlots[i].gameObject.SetActive(false);
                continue;
            }

            skillSlots[i].gameObject.SetActive(true);
            skillSlots[i].nameText.text = MyPassiveList[i].krName;
            skillSlots[i].iconImg.sprite = Resources.Load<Sprite>(MyPassiveList[i].iconPath);
            skillSlots[i].tabType = tabType;

            Navigation customNav = new Navigation();
            customNav.mode = Navigation.Mode.Explicit;
            customNav.selectOnUp = i >= 4 ? skillSlotBtns[i - 4] : null;
            customNav.selectOnDown = i + 4 <= skillSlots.Length ? skillSlotBtns[i + 4] : null;
            customNav.selectOnLeft = i >= 1 ? skillSlotBtns[i - 1] : null;
            customNav.selectOnRight = i + 1 <= skillSlots.Length ? skillSlotBtns[i + 1] : null;

            skillSlotBtns[i].navigation = customNav;
        }
        skillSlotBtns[0].Select();
    }

    public void EquipSet_Init()
    {
        // 슬롯창 활성화
        if(GameManager.Instance.playerData.clearLv == 0)
        {
            equipActiveB.gameObject.SetActive(false);

            equipSkillSlotList_Passive[4].gameObject.SetActive(false);
            equipSkillSlotList_Passive[5].gameObject.SetActive(false);
        }
        else if(GameManager.Instance.playerData.clearLv == 1)
        {
            equipSkillSlotList_Active[4].gameObject.SetActive(true);
            equipSkillSlotList_Active[5].gameObject.SetActive(true);
            equipSkillSlotList_Active[6].gameObject.SetActive(false);
            equipSkillSlotList_Active[7].gameObject.SetActive(false);
            equipActiveB.gameObject.SetActive(true);

            equipSkillSlotList_Passive[4].gameObject.SetActive(true);
            equipSkillSlotList_Passive[5].gameObject.SetActive(false);
        }
        else if (GameManager.Instance.playerData.clearLv > 1)
        {
            equipActiveB.gameObject.SetActive(true);
            equipSkillSlotList_Active[4].gameObject.SetActive(true);
            equipSkillSlotList_Active[5].gameObject.SetActive(true);
            equipSkillSlotList_Active[6].gameObject.SetActive(true);
            equipSkillSlotList_Active[7].gameObject.SetActive(true);

            equipSkillSlotList_Passive[4].gameObject.SetActive(true);
            equipSkillSlotList_Passive[5].gameObject.SetActive(true);
        }

        Init_EquipNavgation();

        // 슬롯창 채우기
        for (int i = 0; i < equipSkillSlotList_Active.Count; i++)
        {
            ActiveSkill activeSkill = GameManager.Instance.EquipActiveList.Find(x => x.equipOrder == i + 1);

            if (activeSkill == null)
            {
                equipSkillSlotList_Active[i].nameText.text = "";
                equipSkillSlotList_Active[i].iconImg.sprite = Resources.Load<Sprite>("Sprite/Skill/Empty");
                continue;
            }

            equipSkillSlotList_Active[i].nameText.text = activeSkill.krName;
            equipSkillSlotList_Active[i].iconImg.sprite = Resources.Load<Sprite>(activeSkill.iconPath);
        }

        for (int i = 0; i < equipSkillSlotList_Passive.Count; i++)
        {
            PassiveSkill passiveSkill = GameManager.Instance.EquipPassiveList.Find(x => x.equipOrder == i + 1);

            if (passiveSkill == null)
            {
                equipSkillSlotList_Passive[i].nameText.text = "";
                equipSkillSlotList_Passive[i].iconImg.sprite = Resources.Load<Sprite>("Sprite/Skill/Empty");
                continue;
            }

            equipSkillSlotList_Passive[i].nameText.text = passiveSkill.krName;
            equipSkillSlotList_Passive[i].iconImg.sprite = Resources.Load<Sprite>(passiveSkill.iconPath);

        }

        GameManager.Instance.RefreshUI(equipActiveB.GetComponent<LayoutGroup>());
    }

    void TabChange()
    {
        if (Canvas5.Instance.UISequenceList.Count > 0 && Canvas5.Instance.UISequenceList[Canvas5.Instance.UISequenceList.Count - 1] != Canvas5.UIType.StatSkill_SkillSet) return;
        if (PlayerInputControls.Instance.doChangeTabLeft)
        {
            PlayerInputControls.Instance.doChangeTabLeft = false;

            if (tabIndex <= 0) return;
            tabBtns[tabIndex].animator.SetTrigger("Normal");

            tabIndex--;
            tabBtns[tabIndex].onClick.Invoke();
            tabBtns[tabIndex].animator.SetTrigger("Selected");
        }
        else if (PlayerInputControls.Instance.doChangeTabRight)
        {
            PlayerInputControls.Instance.doChangeTabRight = false;

            if (tabIndex >= 1) return;
            tabBtns[tabIndex].animator.SetTrigger("Normal");

            tabIndex++;
            tabBtns[tabIndex].onClick.Invoke();
            tabBtns[tabIndex].animator.SetTrigger("Selected");
        }
    }

    void Init_EquipNavgation(){
        if(GameManager.Instance.playerData.clearLv < 1){
            Navigation newNavi = new Navigation();
            newNavi = equipActiveA.navigation;
            newNavi.selectOnDown = equipPassive;
            equipActiveA.navigation = newNavi;

            newNavi = new Navigation();
            newNavi = equipPassive.navigation;
            newNavi.selectOnUp = equipActiveA;
            equipPassive.navigation = newNavi;
        }else{
            Navigation newNavi = new Navigation();
            newNavi = equipActiveA.navigation;
            newNavi.selectOnDown = equipActiveB;
            equipActiveA.navigation = newNavi;

            newNavi = new Navigation();
            newNavi = equipPassive.navigation;
            newNavi.selectOnUp = equipActiveB;
            equipPassive.navigation = newNavi;
        }
    }

    void EquipListApply()
    {
        // 스킬 슬롯 초기화
        foreach (var item in GameManager.Instance.EquipActiveList)
        {
            item.isEquip = false;
            item.equipOrder = 0;
        }
        foreach (var item in GameManager.Instance.EquipPassiveList)
        {
            item.isEquip = false;
            item.equipOrder = 0;
        }

        GameManager.Instance.EquipActiveList.Clear();
        GameManager.Instance.EquipPassiveList.Clear();

        // 스킬 적용
        for (int i = 0; i < equipSkillSlotList_Active.Count; i++)
        {
            var curSkill = GameManager.Instance.MyActiveSkillList.Find(x => x.krName == equipSkillSlotList_Active[i].nameText.text);
            if (curSkill != null)
            {
                curSkill.isEquip = true;
                curSkill.equipOrder = i + 1;
                GameManager.Instance.EquipActiveList.Add(curSkill);
            }
        }

        for (int i = 0; i < equipSkillSlotList_Passive.Count; i++)
        {
            var curSkill = GameManager.Instance.MyPassiveSkillList.Find(x => x.krName == equipSkillSlotList_Passive[i].nameText.text);
            if (curSkill != null)
            {
                curSkill.isEquip = true;
                curSkill.equipOrder = i + 1;
                GameManager.Instance.EquipPassiveList.Add(curSkill);
            }
        }
    }

    // 스킬/스탯창 완료 버튼
    public void BtnApply()
    {
        print("BtnApply");
        EquipListApply();
        statRank.ApplyStatRank();
        statVal.StatValLoad();
        GameManager.Instance.Init_playerState.Invoke();
        DataPersistenceManager.Instance.SaveGame();
        //GameManager.Instance.Save_PlayerDataToJosn();
        //GameManager.Instance.Save_SkillDataToJosn();
        hud_Skill.Init_HUDSkill();
    }

    public void BtnClose()
    {
        statVal.Cancle_InStatVals();
    }

    // 스킬 설명창 띄우기
    public void ActiveSkillInfo(ActiveSkill skill, RectTransform slotRect)
    {
        skillInfoAct.GetComponent<SkillInfo>().InputInfo_Active(skill, slotRect);
    }

    public void PassiveSkillInfo(PassiveSkill skill, RectTransform slotRect)
    {
        skillInfoPass.GetComponent<SkillInfo>().InputInfo_Passvie(skill, slotRect);
    }

    // 스킬 설명창 닫기
    public void SkillInfoClose()
    {
        skillInfoAct.SetActive(false);
        skillInfoPass.SetActive(false);
    }


    // ====== 네비게이션 동작 ======
    public void OnClick_SkillSet(){
        Debug.Log("OnClickSkillSet");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(skillSetFirst);

        if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_SkillSet)){
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_SkillSet);
            Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_SkillSet);
        }
    }

    public void Escape_OnClick_SkillSet(){
        Debug.Log("EscapeOnClickSkillSet");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(skillSet);

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_SkillSet);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_SkillSet);
    }

    public void OnClick_SkillSetSlot()
    {
        Debug.Log("OnClickSkillSetSlot");

        clicked_SkillSlot = EventSystem.current.currentSelectedGameObject.GetComponent<SkillSlot>();

        if(clicked_SkillSlot.tabType == GameManager.SkillType.Active){
            if(equipSkillSlotList_Active.Find(x => x.nameText.text == clicked_SkillSlot.nameText.text) != null){
                clicked_SkillSlot = null;
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Error01);
                return;
            }

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(skillSetActiveSlotFirst);

            if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_SkillSetSlot)){
                Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_SkillSetSlot);
                Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_SkillSetSlot);
            }

            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.ButtonClick01);
        }
        else{
            if (equipSkillSlotList_Passive.Find(x => x.nameText.text == clicked_SkillSlot.nameText.text) != null)
            {
                clicked_SkillSlot = null;
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Error01);
                return;
            }

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(skillSetPassiveSlotFirst);
            if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_SkillSetSlot)){
                Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_SkillSetSlot);
                Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_SkillSetSlot);
            }

            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.ButtonClick01);
        }
        
    }

    public void Escape_OnClick_SkillSetSlot()
    {
        Debug.Log("EscapeOnClickSkillSetSlot");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_SkillSlot.gameObject);

        clicked_SkillSlot = null;

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_SkillSetSlot);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_SkillSetSlot);
    }

    public void OnClcik_Equip(){
        Debug.Log("OnClcik_Equip");
        GameObject gObj = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<EquipSkillSlot>().gameObject;
        clicked_Equip = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gObj);

        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_Equip)){
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_Equip);
            Canvas5.Instance.EscapeActionList.Add(Escape_OnClcik_Equip);

        }
    }

    public void Escape_OnClcik_Equip(){
        Debug.Log("Escape_OnClcik_Equip");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_Equip);

        clicked_Equip = null;
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_Equip);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClcik_Equip);
    }

    public void OnClick_EquipSlot()
    {
        Debug.Log("OnClick_EquipSlot");
        EquipSkillSlot equipSkillSlot = EventSystem.current.currentSelectedGameObject.GetComponent<EquipSkillSlot>();

        //키보드 조작
        if(PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse){
            clicked_EquipSkillSlot = equipSkillSlot;

            equipMenu.SetActive(true);
            equipMenu.transform.position = clicked_EquipSkillSlot.transform.position;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(equipMenuFirst);

            if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_EquipSlot)){
                Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_EquipSlot);
                Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_EquipSlot);
            }
        }
        //게임패드 조작
        else if(PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.GamePad){
            // 스킬 장착 슬롯 직접 클릭(팝업 메뉴)
            if (Canvas5.Instance.UISequenceList[Canvas5.Instance.UISequenceList.Count - 1] == Canvas5.UIType.StatSkill_Equip)
            {
                clicked_EquipSkillSlot = equipSkillSlot;

                equipMenu.SetActive(true);
                equipMenu.transform.position = clicked_EquipSkillSlot.transform.position;

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(equipMenuFirst);

                if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill_EquipSlot)){
                    Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.StatSkill_EquipSlot);
                    Canvas5.Instance.EscapeActionList.Add(Escape_OnClick_EquipSlot);
                }

                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.ButtonClick01);
            }
            // 스킬셋에서 스킬 장착을 위한 클릭
            else if (Canvas5.Instance.UISequenceList[Canvas5.Instance.UISequenceList.Count - 1] == Canvas5.UIType.StatSkill_SkillSetSlot)
            {
                if (equipSkillSlot.isTypeMached(clicked_SkillSlot))
                {
                    equipSkillSlot.EquipSkillToSlot(clicked_SkillSlot);

                    Escape_OnClick_SkillSetSlot();
                    SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SlotEquip01);
                }
                else
                {
                    //실패 사운드
                    SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Error01);
                }
            }
        }
    }

    public void Escape_OnClick_EquipSlot()
    {
        Debug.Log("Escape_OnClick_EquipSlot");
        equipMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_EquipSkillSlot.gameObject);

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_EquipSlot);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_EquipSlot);
    }

    public void OnClick_EquipMenu_Unequip(){
        clicked_EquipSkillSlot.Unequip();

        equipMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_EquipSkillSlot.gameObject);

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_EquipSlot);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_EquipSlot);
        SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SlotUnequip01);
    }

    public void OnClick_EquipMenu_Reset()
    {
        EquipSkillSlot[] slots = clicked_EquipSkillSlot.transform.parent.GetComponentsInChildren<EquipSkillSlot>();
        foreach (var slot in slots)
        {
            slot.Unequip();
        }
        equipMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_EquipSkillSlot.gameObject);

        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.StatSkill_EquipSlot);
        Canvas5.Instance.EscapeActionList.Remove(Escape_OnClick_EquipSlot);
        SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SlotUnequip01);
    }

}
