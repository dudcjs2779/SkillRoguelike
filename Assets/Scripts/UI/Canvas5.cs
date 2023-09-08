using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Canvas5 : MonoBehaviour
{
    private static Canvas5 instance;
    public static Canvas5 Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<Canvas5>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<Canvas5>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }
    public RectTransform MenuRect;

    public RectTransform SelectTabRect;
    public RectTransform StatSkillRect;
    public RectTransform EquipTabRect;
    public RectTransform ShopRect;
    public RectTransform DifficultyRect;
    public RectTransform SkillUpgradeRect;

    public RectTransform RewardRect;
    public RectTransform ClearPanelRect;
    public RectTransform StatPanelRect;

    public RectTransform SkillSetRect;

    Menu menu;
    StatWindow statWindow;
    SkillWindow skillWindow;
    StatRank statRank;
    StatVal statVal;
    ShopItemList shopItemList;
    ArmorEquipList armorEquipList;
    UpgradeSkill upgradeSkill;
    OptionMenu optionMenu;

    Player player;
    PlayerSkill playerSkill;

    [SerializeField]
    public List<UIType> UISequenceList;

    public Action focusItem;
    public Action OpenAction;
    public List<Action> EscapeActionList = new List<Action>();

    public enum UIType
    {
        SelectTab,
        StatSkill,
        EquipTab,
        Shop,
        Difficulty,

        TitleMenu,
        SaveSlotMenu,
        SaveSlot_PopupListMenu,

        Menu,
        OptionMenu,
        OptionMenu_Display,
        OptionMenu_Audio,
        OptionMenu_KeySetting,
        OptionMenu_DropdownView,
        Help,
        Help_Tab,

        StatSkill_StatRank,
        StatSkill_SkillSet,
        StatSkill_SkillSetSlot,
        StatSkill_Equip,
        StatSkill_EquipSlot,
        Shop_ItemSlot,

        SkillUpgrade,
        Reward,
        ClearPanel,

        StatPanel,
        StatPanelActive,
        StatPanelPassive,

        ConfirmMenu,
    }

    private void Awake()
    {
        var objs = FindObjectsOfType<Canvas5>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        menu = GetComponentInChildren<Menu>(true);
        player = GameObject.Find("Player")?.GetComponent<Player>();
        playerSkill = player?.GetComponent<PlayerSkill>();
        statRank = GetComponentInChildren<StatRank>();
        statVal = GetComponentInChildren<StatVal>();
        upgradeSkill = GetComponentInChildren<UpgradeSkill>(true);
        shopItemList = ShopRect != null ? ShopRect.GetComponentInChildren<ShopItemList>() : null;
        skillWindow = StatSkillRect != null ? StatSkillRect.GetComponentInChildren<SkillWindow>() : null;
        statWindow = GetComponentInChildren<StatWindow>();
        armorEquipList = EquipTabRect != null ? EquipTabRect.GetComponentInChildren<ArmorEquipList>() : null;
    }

    void Update()
    {
        GetInput();

        if(PlayerInputControls.Instance.escDown){
            PlayerInputControls.Instance.escDown = false;
            Escape();
        }

        if(Keyboard.current.numpad1Key.wasPressedThisFrame){
            Debug.Log(EscapeActionList.Count);
            foreach (var item in EscapeActionList)
            {
                Debug.Log(item.Method);
            }
        }

        CursorState();
    }

    void CursorState(){
        if (GameManager.Instance.isTitle)
        {
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            {
                GameManager.Instance.ShowCursor();
                GameManager.Instance.UnlockCursor();
            }
            else
            {
                GameManager.Instance.HideCursor();
                GameManager.Instance.LockCursor();
            }
        }
        else
        {
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            {

                if (UISequenceList.Count > 0 || DialogueManager.Instance.dialogueIsPlaying)
                {
                    GameManager.Instance.UnlockCursor();
                    GameManager.Instance.ShowCursor();
                }
                else
                {
                    GameManager.Instance.HideCursor();
                }
            }
            else
            {
                GameManager.Instance.HideCursor();
                GameManager.Instance.LockCursor();
            }
        }
    }

    void GetInput()
    {
        if (PlayerInputControls.Instance.skillWindowDown)
        {
            OpenSkillStat();
        }

        if (PlayerInputControls.Instance.lvUpDown)
        {
            OpenSkillUpgrade();
        }

        if (PlayerInputControls.Instance.shopDown)
        {
            OpenShop();
        }

        if (PlayerInputControls.Instance.selectTabDown)
        {
            OpenSelectTab();
        }

    }

    public void Escape()
    {
        if(UISequenceList.Count > 0)
        {
            //OpeningWindows.Remove(OpeningWindows[OpeningWindows.Count - 1]);
            if (UISequenceList[UISequenceList.Count - 1] == UIType.SelectTab)
            {
                CloseSelectTab();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatSkill)
            {
                CloseSkillStat();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.EquipTab)
            {
                CloseEquipTab();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.Shop)
            {
                CloseShop();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.Difficulty)
            {
                CloseDifficulty();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.TitleMenu)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.SaveSlotMenu)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.SaveSlot_PopupListMenu)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.ConfirmMenu)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.Menu)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.OptionMenu)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.OptionMenu_Display)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.OptionMenu_Audio)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.OptionMenu_KeySetting)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.OptionMenu_DropdownView)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.Help)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.Help_Tab)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatSkill_StatRank)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if(UISequenceList[UISequenceList.Count - 1] == UIType.StatSkill_SkillSet){
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatSkill_SkillSetSlot)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatSkill_Equip)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatSkill_EquipSlot)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.Shop_ItemSlot)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);

            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatPanelActive)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            else if (UISequenceList[UISequenceList.Count - 1] == UIType.StatPanelPassive)
            {
                EscapeActionList[EscapeActionList.Count - 1].Invoke();
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            }
            GameManager.Instance.DefaultCursor();
        }
    }

    #region "퍼즈 메뉴"
    public void OpenMenu()
    {
        if (!UISequenceList.Contains(UIType.Menu))
        {
            Debug.Log("OpenMenu");
            GameManager.Instance.PauseGame();
            UISequenceList.Add(UIType.Menu);
            EscapeActionList.Add(CloseMenu);

            MenuRect.gameObject.SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menu.firstSelected.gameObject);

            ChangeActionMap();
        }
    }

    public void CloseMenu()
    {
        if (UISequenceList.Contains(UIType.Menu))
        {
            Debug.Log("CloseMenu");
            UISequenceList.Remove(UIType.Menu);
            EscapeActionList.Remove(CloseMenu);
            MenuRect.gameObject.SetActive(false);
            ChangeActionMap();
            GameManager.Instance.ResumeGame();
        }
    }
    #endregion

    #region " ==== NPC 메뉴 ==== "
    public void OpenSelectTab()
    {
        if (!UISequenceList.Contains(UIType.SelectTab))
        {
            Debug.Log("OpenSelectTab");
            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.OpenFirst01);
            UISequenceList.Add(UIType.SelectTab);
            SelectTabRect.gameObject.SetActive(true);
            SelectTabRect.anchoredPosition = Vector2.zero;

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.selectTabFirst);

            ChangeActionMap();
        }
    }

    public void CloseSelectTab(){
        if (UISequenceList.Contains(UIType.SelectTab))
        {
            Debug.Log("CloseSelectTab");
            SelectTabRect.gameObject.SetActive(false);
            UISequenceList.Clear();
            ChangeActionMap();
        }
    }

    public void OpenSkillStat()
    {
        if (!UISequenceList.Contains(UIType.StatSkill))
        {
            Debug.Log("OpenSkillStat");
            UISequenceList.Add(UIType.StatSkill);
            StatSkillRect.gameObject.SetActive(true);

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.statTabFirst);

            ChangeActionMap();
        }
    }

    public void CloseSkillStat()
    {
        if (UISequenceList.Contains(UIType.StatSkill))
        {
            Debug.Log("CloseSkillStat");
            skillWindow.BtnApply();

            int index = UISequenceList.FindIndex(x => x == UIType.StatSkill);
            Debug.Log(index);
            UISequenceList.RemoveRange(index, UISequenceList.Count - index);

            StatSkillRect.gameObject.SetActive(false);
            skillWindow.SkillInfoClose();

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.statTabClose);

            ChangeActionMap();
        }
    }

    public void OpenSkillUpgrade()
    {
        if (!UISequenceList.Contains(UIType.SkillUpgrade))
        {
            UISequenceList.Add(UIType.SkillUpgrade);
            if(!GameManager.Instance.isCutScene) ChangeActionMap();
            SkillUpgradeRect.gameObject.SetActive(true);
            Debug.Log("OpenSkillUpgrade");
        }
    }

    public void CloseSkillUpgrade()
    {
        if (UISequenceList.Contains(UIType.SkillUpgrade))
        {
            UISequenceList.Remove(UIType.SkillUpgrade);
            if (!GameManager.Instance.isCutScene) ChangeActionMap();
            SkillUpgradeRect.gameObject.SetActive(false);
            Debug.Log("CloseSkillUpgrade");
        }
    }

    public void OpenShop()
    {
        if (!UISequenceList.Contains(UIType.Shop))
        {
            UISequenceList.Add(UIType.Shop);
            ShopRect.gameObject.SetActive(true);
            //shopItemList.Init_ItemList();
            ChangeActionMap();
        }
    }

    public void CloseShop()
    {
        if (UISequenceList.Contains(UIType.Shop))
        {
            Debug.Log("CloseShop");

            int index = UISequenceList.FindIndex(x => x == UIType.Shop);
            UISequenceList.RemoveRange(index, UISequenceList.Count - index);

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.shopClose);

            UISequenceList.Remove(UIType.Shop);
            ChangeActionMap();
            ShopRect.gameObject.SetActive(false);
        }
    }

    public void OpenEquipTab()
    {
        if (!UISequenceList.Contains(UIType.EquipTab))
        {
            Debug.Log("openEquipTab");

            SelectTabRect.GetComponent<CanvasGroup>().interactable = false;

            UISequenceList.Add(UIType.EquipTab);
            ChangeActionMap();
            EquipTabRect.gameObject.SetActive(true);
        }
    }

    public void CloseEquipTab()
    {
        if (UISequenceList.Contains(UIType.EquipTab))
        {
            Debug.Log("CLoseEquipTab");

            SelectTabRect.GetComponent<CanvasGroup>().interactable = true;

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.armorEquipClose);

            UISequenceList.Remove(UIType.EquipTab);
            ChangeActionMap();
            EquipTabRect.gameObject.SetActive(false);
        }
    }

    public void OpenDifficulty()
    {
        if (!UISequenceList.Contains(UIType.Difficulty))
        {
            UISequenceList.Add(UIType.Difficulty);
            DifficultyRect.gameObject.SetActive(true);
            DifficultyRect.GetComponent<Animator>().SetTrigger("SmoothScale");

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.diffcultyFirst);

            Debug.Log("OpenDifficulty");
        }
    }

    public void CloseDifficulty()
    {
        if (UISequenceList.Contains(UIType.Difficulty))
        {
            UISequenceList.Remove(UIType.Difficulty);
            DifficultyRect.gameObject.SetActive(false);

            SelectTab selectTab = SelectTabRect.GetComponent<SelectTab>();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.diffcultyClose);

            Debug.Log("CloseDifficulty");
        }
    }
    #endregion

    public void OpenReward()
    {
        if (!UISequenceList.Contains(UIType.Reward))
        {
            UISequenceList.Add(UIType.Reward);
            ChangeActionMap();
            RewardRect.gameObject.SetActive(true);
            Debug.Log("OpenRewardRect");
        }
    }

    public void CloseReward()
    {
        if (UISequenceList.Contains(UIType.Reward))
        {
            UISequenceList.Remove(UIType.Reward);
            RewardRect.gameObject.SetActive(false);
            ChangeActionMap();
            Debug.Log("CloseRewardRect");
        }
    }

    public void OpenClearPanel()
    {
        if (!UISequenceList.Contains(UIType.ClearPanel))
        {
            UISequenceList.Add(UIType.ClearPanel);
            ChangeActionMap();
            ClearPanelRect.gameObject.SetActive(true);
            Debug.Log("OpenClearPanel");
        }
    }

    public void CloseClearPanel()
    {
        if (UISequenceList.Contains(UIType.ClearPanel))
        {
            UISequenceList.Remove(UIType.ClearPanel);
            ClearPanelRect.gameObject.SetActive(false);
            ChangeActionMap();
            Debug.Log("CloseClearPanel");
        }
    }

    public void OpenStatPanel()
    {
        if (!UISequenceList.Contains(UIType.StatPanel) && UISequenceList.Count == 0)
        {
            Debug.Log("OpenStatPanel");
            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.OpenFirst01);
            UISequenceList.Add(UIType.StatPanel);
            ChangeActionMap();
            StatPanelRect.gameObject.SetActive(true);
            StatPanelRect.GetComponent<Animator>().SetTrigger("MoveFromLeft");
        }
    }

    public void CloseStatPanel()
    {
        if (UISequenceList.Contains(UIType.StatPanel))
        {
            Debug.Log("CloseStatPanel");
            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.Escape01);
            //UISequenceList.Remove(UIType.StatPanel);
            UISequenceList.Clear();
            EscapeActionList.Clear();
            ChangeActionMap();
            StatPanelRect.GetComponentInChildren<SkillTab>(true).CloseSkillInfo();
            StatPanelRect.GetComponent<Animator>().SetTrigger("MoveToLeft");
            StartCoroutine(CloseDelay(StatPanelRect, 0.5f));
        }
    }

    IEnumerator CloseDelay(RectTransform rect, float delay)
    {
        Animator anim = rect.GetComponent<Animator>();
        yield return new WaitForSecondsRealtime(delay);
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Empty") && !anim.IsInTransition(0)) rect.gameObject.SetActive(false);
    }

    public void ChangeActionMap()
    {
        if(GameManager.Instance.isTitle) return;

        if (Canvas5.Instance.UISequenceList.Count <= 0)
        {
            PlayerInputControls.Instance.ChangeMapPlayer();
        }
        else
        {
            PlayerInputControls.Instance.ChangeMapUI();
        }
    }

    protected virtual void Test(){
        Debug.Log("test");
    }

    bool IsAnimating(Animator animator, int layerIndex)
    {
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime <= 1 && !animator.IsInTransition(layerIndex))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator DisableAfterAnim(GameObject gObj, Animator animator, int layerIndex)
    {

        WaitForSeconds waitTime = new WaitForSeconds(0.5f);

        while (true)
        {
            if (!IsAnimating(animator, 0)) gObj.SetActive(false);
            yield return waitTime;
        }
    }
}
