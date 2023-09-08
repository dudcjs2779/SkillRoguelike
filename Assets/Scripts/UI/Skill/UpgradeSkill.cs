using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class UpgradeSkill : MonoBehaviour
{
    UpgradeSlot[] upgradeSlots;
    Button[] upgradeSlotBtns;
    [SerializeField] Button[] tabBtns;
    private int tabIndex = 0;

    Animator anim;

    Canvas5 central;
    PlayerSkill playerSkill;
    Player player;
    PlayerStateManager playerStateManager;

    public string tabType;

    // 페시브 스킬 변수들
    public int healthUpVal, staminaUpVal, finishHealVal, mpUpVal;
    public float heavyAttackVal, magicDamageUpVal, speedUpVal, drainMpVal, defenceUpVal, meleeDamageUpVal;

    private void Awake()
    {
        central = GameObject.Find("Canvas5").GetComponent<Canvas5>();
        playerSkill = GameObject.Find("Player").GetComponent<PlayerSkill>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerStateManager = player.GetComponent<PlayerStateManager>();
        upgradeSlots = GetComponentsInChildren<UpgradeSlot>(true);
        upgradeSlotBtns = new Button[upgradeSlots.Length];
        for (int i = 0; i < upgradeSlotBtns.Length; i++)
        {
            upgradeSlotBtns[i] = upgradeSlots[i].GetComponent<Button>();
        }

        anim = GetComponentInParent<Animator>();
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        tabIndex = 0;
        tabType = "Active";
        StartCoroutine(Init_UpgradeSlot());
        anim.SetTrigger("SmoothScale");
        tabBtns[0].animator.SetTrigger("Selected");
    }

    private void OnDisable()
    {
        
    }

    void Update()
    {
        TabChange();
    }

    public IEnumerator Init_UpgradeSlot()
    {
        if (tabType == "Active")
        {
            List<ActiveSkill> equipActiveList = GameManager.Instance.EquipActiveList.FindAll(x => x.skillLv < 2);
            for (int i = 0; i < upgradeSlots.Length; i++)
            {
                if(i > equipActiveList.Count - 1){
                    upgradeSlots[i].gameObject.SetActive(false);
                    continue;
                }

                upgradeSlots[i].gameObject.SetActive(true);
                upgradeSlots[i].activeSkill = equipActiveList[i];
                upgradeSlots[i].type = tabType;
                upgradeSlots[i].nameText.text = equipActiveList[i].krName;
                upgradeSlots[i].upgradeText1.text = equipActiveList[i].skillLv == 0 ? equipActiveList[i].explain : equipActiveList[i].skillLvEx[0];
                upgradeSlots[i].upgradeText2.text = "";
                upgradeSlots[i].iconImage.sprite = Resources.Load<Sprite>(equipActiveList[i].iconPath);

                Navigation newNavi = new Navigation();
                newNavi.mode = Navigation.Mode.Explicit;
                newNavi.selectOnLeft = i > 0 ? upgradeSlotBtns[i - 1] : null;
                newNavi.selectOnRight = i < upgradeSlotBtns.Length - 1 ? upgradeSlotBtns[i + 1] : null;
                newNavi.selectOnUp = i > 3 ? upgradeSlotBtns[i - 4] : null;
                newNavi.selectOnDown = i < 4 ? upgradeSlotBtns[i + 4] : null;

                upgradeSlotBtns[i].navigation = newNavi;

                
            }
            upgradeSlotBtns[0].Select();
        }
        else
        {
            List<PassiveSkill> equipPassiveList = GameManager.Instance.EquipPassiveList.FindAll(x => x.skillLv < 3);
            for (int i = 0; i < upgradeSlots.Length; i++)
            {
                if (i > equipPassiveList.Count - 1){
                    upgradeSlots[i].gameObject.SetActive(false);
                    continue;
                }

                upgradeSlots[i].gameObject.SetActive(true);
                upgradeSlots[i].passiveSkill = equipPassiveList[i];
                upgradeSlots[i].type = tabType;
                upgradeSlots[i].nameText.text = equipPassiveList[i].krName;
                upgradeSlots[i].iconImage.sprite = Resources.Load<Sprite>(equipPassiveList[i].iconPath);

                if (equipPassiveList[i].skillLv == 0)
                {
                    upgradeSlots[i].upgradeText1.text = equipPassiveList[i].skillLvEx[equipPassiveList[i].skillLv];
                }
                else
                {
                    upgradeSlots[i].upgradeText1.text = "현재 레벨: " + equipPassiveList[i].skillLvEx[equipPassiveList[i].skillLv - 1];
                    upgradeSlots[i].upgradeText2.text = "다음 레벨: " + equipPassiveList[i].skillLvEx[equipPassiveList[i].skillLv];
                }

                Navigation newNavi = new Navigation();
                newNavi.mode = Navigation.Mode.Explicit;
                newNavi.selectOnLeft = i > 0 ? upgradeSlotBtns[i - 1] : null;
                newNavi.selectOnRight = i < upgradeSlotBtns.Length - 1 ? upgradeSlotBtns[i + 1] : null;
                newNavi.selectOnUp = i > 3 ? upgradeSlotBtns[i - 4] : null;
                newNavi.selectOnDown = i < 4 ? upgradeSlotBtns[i + 4] : null;

                upgradeSlotBtns[i].navigation = newNavi;
            }
            upgradeSlotBtns[0].Select();
        }

        yield return null;

        foreach (UpgradeSlot upgradeSlot in upgradeSlots)
        {
            upgradeSlot.GetSlotPos();
        }
    }

    public void ResetSlotPos()
    {
        for (int i = 0; i < upgradeSlots.Length; i++)
        {
            upgradeSlots[i].slotRect.anchoredPosition = upgradeSlots[i].oriPos;
        }
    }

    void TabChange()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.SkillUpgrade)) return;
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

    public void BtnActive()
    {
        tabType = "Active";
        StartCoroutine(Init_UpgradeSlot());
    }

    public void BtnPassvie()
    {
        tabType = "Passive";
        StartCoroutine(Init_UpgradeSlot());
    }
}
