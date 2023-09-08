using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public bool pointerOnSlot;
    public GameManager.SkillType tabType;

    Canvas canvas;
    public RectTransform slotRect;
    CanvasGroup canvasGroup;
    public TextMeshProUGUI nameText;
    public Image iconImg;

    GameObject instantIcon;
    RectTransform instantSlotRect;
    CanvasGroup instantCanvasGroup;

    SkillWindow skillWindow;

    public UnityEvent<RectTransform> onSelectEvent;

    private void Awake()
    {
        skillWindow = transform.root.GetComponentInChildren<SkillWindow>();
        canvas = Canvas5.Instance.GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        slotRect = GetComponent<RectTransform>();
    }

    void Start()
    {

    }

    private void Update()
    {
        //SkillInfo();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        instantIcon = Instantiate(gameObject, eventData.position, Quaternion.identity, transform.root);
        instantSlotRect = instantIcon.GetComponent<RectTransform>();
        instantSlotRect.sizeDelta = new Vector2(90, 105);
        instantSlotRect.pivot = new Vector2(0.5f, 0.5f);
        instantCanvasGroup = instantIcon.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.6f;
        instantCanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        instantSlotRect.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        instantCanvasGroup.blocksRaycasts = true;
        Destroy(instantIcon);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(tabType == GameManager.SkillType.Active)
        {
            ActiveSkill activeSkill = GameManager.Instance.MyActiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.ActiveSkillInfo(activeSkill, slotRect);

        }
        else
        {
            Vector2 pos = transform.position;
            pos += slotRect.rect.size / 2;

            PassiveSkill passiveSkill = GameManager.Instance.MyPassiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.PassiveSkillInfo(passiveSkill, slotRect);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("SkillSetExit");
        skillWindow.SkillInfoClose();
    }

    public void OnDrop(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectEvent.Invoke(slotRect);

        if (tabType == GameManager.SkillType.Active)
        {
            ActiveSkill activeSkill = GameManager.Instance.MyActiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.ActiveSkillInfo(activeSkill, slotRect);
        }
        else
        {
            PassiveSkill passiveSkill = GameManager.Instance.MyPassiveSkillList.Find(x => x.krName == nameText.text);
            skillWindow.PassiveSkillInfo(passiveSkill, slotRect);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        skillWindow.SkillInfoClose();
    }

}

