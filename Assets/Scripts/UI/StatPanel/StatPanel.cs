using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Text;

public class StatPanel : MonoBehaviour
{
    RectTransform rect;
    [SerializeField] RectTransform statTabRect;
    [SerializeField] RectTransform skillTabRect;

    public TextMeshProUGUI HPText;
    public TextMeshProUGUI StanminaText;
    public TextMeshProUGUI MPText;

    public TextMeshProUGUI STRText;
    public TextMeshProUGUI AGLText;
    public TextMeshProUGUI VITText;
    public TextMeshProUGUI INTText;
    public TextMeshProUGUI MagicPowerText;
    public TextMeshProUGUI MSTText;

    public TextMeshProUGUI MeleeDamText;
    public TextMeshProUGUI MeleeStgDamText;
    public TextMeshProUGUI MeleeStgPowerText;
    public TextMeshProUGUI MagicDamText;
    public TextMeshProUGUI MagicStgDamText;
    public TextMeshProUGUI MagicStgPowerText;
    public TextMeshProUGUI MagicElementText;
    public TextMeshProUGUI MpRecoverText;
    public TextMeshProUGUI BuffDurationText;

    [SerializeField] Image armorImage;
    [SerializeField] TextMeshProUGUI armorName;
    [SerializeField] TextMeshProUGUI armorExplain;

    [SerializeField] List<StatPanelSkillSlot> activeSlotList;
    [SerializeField] List<StatPanelSkillSlot> passiveSlotList;
    [SerializeField] Button[] tabBtns;

    int tabIndex;

    Player player;
    PlayerStateManager playerStateManager;
    [SerializeField] SkillTab skillTab;

    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerStateManager = player.GetComponent<PlayerStateManager>();

        statTabRect.gameObject.SetActive(true);
        skillTabRect.gameObject.SetActive(false);
    }

    private void Start()
    {

    }

    private void Update()
    {
        Init_StatTab();
        TabChange();
    }

    private void OnEnable()
    {
        tabIndex = 0;
        Init_StatTab();
        Init_SkillTab();
        StartCoroutine(Init_TabBtn());
    }

    IEnumerator Init_TabBtn()
    {
        yield return new WaitForEndOfFrame();
        BtnStatTab();
    }

    void Init_StatTab()
    {
        if (Time.frameCount % 5 == 0)
        {
            sb.AppendFormat("{0:F0} / {1}", player.curHealth, player.maxHealth);
            HPText.text = sb.ToString();
            sb.Remove(0, sb.Length);

            sb.AppendFormat("{0:F0} / {1}", player.curStamina, player.maxStamina);
            StanminaText.text = sb.ToString();
            sb.Remove(0, sb.Length);

            sb.AppendFormat("{0:F0} / {1}", player.curMP, player.maxMP);
            MPText.text = sb.ToString();
            sb.Remove(0, sb.Length);

            STRText.text = GameManager.Instance.playerData.STR;
            AGLText.text = GameManager.Instance.playerData.AGL;
            VITText.text = GameManager.Instance.playerData.VIT;
            INTText.text = GameManager.Instance.playerData.INT;
            MagicPowerText.text = GameManager.Instance.playerData.MP;
            MSTText.text = GameManager.Instance.playerData.MST;

            MeleeDamText.text = string.Format("{0:P1}", playerStateManager.meleeDamVal);
            MeleeStgDamText.text = string.Format("{0:P1}", playerStateManager.meleeStgDamVal);
            MeleeStgPowerText.text = string.Format("{0:P1}", playerStateManager.meleeStgPowerVal);
            MagicDamText.text = string.Format("{0:P1}", playerStateManager.magicDamVal);
            MagicStgDamText.text = string.Format("{0:P1}", playerStateManager.magicStgDamVal);
            MagicStgPowerText.text = string.Format("{0:P1}", playerStateManager.magicStgPowerVal);
            MagicElementText.text = string.Format("{0:P1}", playerStateManager.elementalVal);
            MpRecoverText.text = string.Format("{0:P1}", playerStateManager.mpRecoverVal);
            BuffDurationText.text = string.Format("{0:P1}", playerStateManager.stat_BuffDuration);
        }
    }

    void Init_SkillTab()
    {
        ShopItem armor = GameManager.Instance.MyShopItemList.Find(x => x.isEquip);
        if (armor != null)
        {
            armorImage.sprite = Resources.Load<Sprite>(armor.iconPath);
            armorName.text = armor.krName;
            armorExplain.text = armor.explainDetail;
        }
        else
        {
            armorImage.sprite = Resources.Load<Sprite>("Sprite/Skill/Empty");
            armorName.text = "";
            armorExplain.text = "";
        }
        

        ActiveSkill[] activeSkills = GameManager.Instance.EquipActiveList.ToArray();

        for (int i = 0; i < activeSlotList.Count; i++)
        {
            if(activeSkills.Length <= i)
            {
                activeSlotList[i].gameObject.SetActive(false);
                continue;
            }

            activeSlotList[i].gameObject.SetActive(true);
            activeSlotList[i].skillNameText.text = activeSkills[i].krName;
            activeSlotList[i].skillLevelText.text = activeSkills[i].skillLv.ToString();
            activeSlotList[i].skillImage.sprite = Resources.Load<Sprite>(activeSkills[i].iconPath);
            activeSlotList[i].skillType = GameManager.SkillType.Active;

        }

        PassiveSkill[] passiveSkills = GameManager.Instance.EquipPassiveList.ToArray();

        for (int i = 0; i < passiveSlotList.Count; i++)
        {
            if (passiveSkills.Length <= i)
            {
                passiveSlotList[i].gameObject.SetActive(false);
                continue;
            }

            passiveSlotList[i].gameObject.SetActive(true);
            passiveSlotList[i].skillNameText.text = passiveSkills[i].krName;
            passiveSlotList[i].skillLevelText.text = passiveSkills[i].skillLv.ToString();
            passiveSlotList[i].skillImage.sprite = Resources.Load<Sprite>(passiveSkills[i].iconPath);
            passiveSlotList[i].skillType = GameManager.SkillType.Passive;
        }
    }

    public void BtnStatTab()
    {
        tabBtns[1].animator.SetBool("isLocked", false);
        tabBtns[1].animator.SetTrigger("Normal");

        tabBtns[0].animator.SetBool("isLocked", true);
        tabBtns[0].animator.SetTrigger("Selected");

        statTabRect.gameObject.SetActive(true);
        skillTabRect.gameObject.SetActive(false);
    }

    public void BtnSkillTab()
    {
        skillTab.FirstSelect();
        tabBtns[0].animator.SetBool("isLocked", false);
        tabBtns[0].animator.SetTrigger("Normal");

        tabBtns[1].animator.SetBool("isLocked", true);
        tabBtns[1].animator.SetTrigger("Selected");
        
        statTabRect.gameObject.SetActive(false);
        skillTabRect.gameObject.SetActive(true);
    }

    void TabChange()
    {
        if (PlayerInputControls.Instance.doChangeTabLeft)
        {
            PlayerInputControls.Instance.doChangeTabLeft = false;

            if (tabIndex <= 0) return;
            tabBtns[tabIndex].animator.SetTrigger("Normal");

            tabIndex--;
            tabBtns[tabIndex].onClick.Invoke();
            //tabBtns[tabIndex].animator.SetTrigger("Selected");
        }
        else if (PlayerInputControls.Instance.doChangeTabRight)
        {
            PlayerInputControls.Instance.doChangeTabRight = false;

            if (tabIndex >= 1) return;
            tabBtns[tabIndex].animator.SetTrigger("Normal");

            tabIndex++;
            tabBtns[tabIndex].onClick.Invoke();
            //tabBtns[tabIndex].animator.SetTrigger("Selected");

        }
    }


}
