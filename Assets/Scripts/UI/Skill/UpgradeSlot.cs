using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class UpgradeSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public bool isPointerEnter;
    public bool isInitDone;
    public Vector2 oriPos;
    public Vector2 upPos;
    public string type;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI upgradeText1;
    public TextMeshProUGUI upgradeText2;
    public Image iconImage;

    RectTransform canvas5Rect;
    public RectTransform slotRect;

    public ActiveSkill activeSkill;
    public PassiveSkill passiveSkill;
    UpgradeSkill upgradeSkill;
    PlayerStateManager playerStateManager;
    Canvas5 canvas5;

    private void Awake()
    {
        upgradeSkill = GetComponentInParent<UpgradeSkill>();
        canvas5 = GameObject.Find("Canvas5").GetComponent<Canvas5>();
        canvas5Rect = canvas5.GetComponent<RectTransform>();
        slotRect = GetComponent<RectTransform>();
        playerStateManager = GameObject.Find("Player").GetComponent<PlayerStateManager>();

    }

    void Start()
    {

    }

    public void GetSlotPos()
    {
        oriPos = slotRect.anchoredPosition;
        upPos = oriPos + Vector2.up * 10;
        isInitDone = true;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Keypad3))
        {
            //oriPos = slotRect.anchoredPosition;
            //print(oriPos);
        }
    }

    public void OnClick(){
        if (!isInitDone) return;

        if (type == "Active")
        {
            activeSkill.skillLv = activeSkill.skillLv + 1;
            print(activeSkill.krName + " " + activeSkill.skillLv);
            Canvas5.Instance.CloseSkillUpgrade();
        }
        else
        {
            passiveSkill.skillLv = passiveSkill.skillLv + 1;
            print(passiveSkill.krName + " " + passiveSkill.skillLv);
            if (passiveSkill.passiveType == PassiveSkill.PassiveType.Always) playerStateManager.PassiveApply(passiveSkill);
            Canvas5.Instance.CloseSkillUpgrade();
        }
    }

    IEnumerator SlotUp()
    {
        Vector2 targetPos;

        float lerp = 0;
        while (lerp < 1)
        {
            if (!isPointerEnter)
                yield break;

            targetPos = Vector2.Lerp(slotRect.anchoredPosition, upPos, lerp);
            slotRect.anchoredPosition = targetPos;
            lerp += Time.unscaledDeltaTime * 3f;
            yield return null;
        }

        slotRect.anchoredPosition = upPos;
    }

    IEnumerator SlotDown()
    {
        Vector2 targetPos;
        float lerp = 0;
        while (lerp < 1)
        {
            if (isPointerEnter)
                yield break;

            targetPos = Vector2.Lerp(slotRect.anchoredPosition, oriPos, lerp);
            slotRect.anchoredPosition = targetPos;
            lerp += Time.unscaledDeltaTime * 3f;
            yield return null;
        }

        slotRect.anchoredPosition = oriPos;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!isInitDone) return;

        isPointerEnter = true;
        StartCoroutine(SlotUp());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!isInitDone) return;

        isPointerEnter = false;
        StartCoroutine(SlotDown());
    }
}
