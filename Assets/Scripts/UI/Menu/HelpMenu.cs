using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour
{
    public Selectable firstSelected;

    [SerializeField] TextMeshProUGUI keyboardText;
    [SerializeField] TextMeshProUGUI gamepadText;
    [SerializeField] TextMeshProUGUI staggerText;
    [SerializeField] TextMeshProUGUI elementalText;
    [SerializeField] TextMeshProUGUI mpRecoverText;
    [SerializeField] TextMeshProUGUI statText;

    TextMeshProUGUI nowText;

    [SerializeField] Scrollbar scroll;
    public GameObject clicked_Tab;

    [SerializeField] CanvasGroup tabBtns_CanvasGroup;

    private void Awake() {
    }

    private void OnEnable() {
        Init_Help();
    }

    void Init_Help(){
        nowText = keyboardText;
        keyboardText.gameObject.SetActive(true);
        gamepadText.gameObject.SetActive(false);
        staggerText.gameObject.SetActive(false);
        elementalText.gameObject.SetActive(false);
        mpRecoverText.gameObject.SetActive(false);
        statText.gameObject.SetActive(false);
    }

    public void OpenKeyboard(){
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help_Tab);

        tabBtns_CanvasGroup.interactable = false;
        tabBtns_CanvasGroup.blocksRaycasts = false;

        nowText.gameObject.SetActive(false);
        keyboardText.gameObject.SetActive(true);
        nowText = keyboardText;
        scroll.value = 1;

        clicked_Tab = EventSystem.current.currentSelectedGameObject;
        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", true);
        clicked_Tab.GetComponent<Button>().animator.SetTrigger("Selected");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scroll.gameObject);

        Canvas5.Instance.EscapeActionList.Add(CloseHelpTab);
    }

    public void OpenGamepad()
    {
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help_Tab);

        tabBtns_CanvasGroup.interactable = false;
        tabBtns_CanvasGroup.blocksRaycasts = false;

        nowText.gameObject.SetActive(false);
        gamepadText.gameObject.SetActive(true);
        nowText = gamepadText;
        scroll.value = 1;

        clicked_Tab = EventSystem.current.currentSelectedGameObject;
        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", true);
        clicked_Tab.GetComponent<Button>().animator.SetTrigger("Selected");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scroll.gameObject);

        Canvas5.Instance.EscapeActionList.Add(CloseHelpTab);
    }

    public void OpenStagger()
    {
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help_Tab);

        tabBtns_CanvasGroup.interactable = false;
        tabBtns_CanvasGroup.blocksRaycasts = false;

        nowText.gameObject.SetActive(false);
        staggerText.gameObject.SetActive(true);
        nowText = staggerText;
        scroll.value = 1;

        clicked_Tab = EventSystem.current.currentSelectedGameObject;
        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", true);
        clicked_Tab.GetComponent<Button>().animator.SetTrigger("Selected");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scroll.gameObject);

        Canvas5.Instance.EscapeActionList.Add(CloseHelpTab);
    }

    public void OpenElemental()
    {
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help_Tab);

        tabBtns_CanvasGroup.interactable = false;
        tabBtns_CanvasGroup.blocksRaycasts = false;

        nowText.gameObject.SetActive(false);
        elementalText.gameObject.SetActive(true);
        nowText = elementalText;
        scroll.value = 1;

        clicked_Tab = EventSystem.current.currentSelectedGameObject;
        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", true);
        clicked_Tab.GetComponent<Button>().animator.SetTrigger("Selected");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scroll.gameObject);

        Canvas5.Instance.EscapeActionList.Add(CloseHelpTab);
    }

    public void OpenMpRecover()
    {
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help_Tab);

        tabBtns_CanvasGroup.interactable = false;
        tabBtns_CanvasGroup.blocksRaycasts = false;

        nowText.gameObject.SetActive(false);
        mpRecoverText.gameObject.SetActive(true);
        nowText = mpRecoverText;
        scroll.value = 1;

        clicked_Tab = EventSystem.current.currentSelectedGameObject;
        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", true);
        clicked_Tab.GetComponent<Button>().animator.SetTrigger("Selected");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scroll.gameObject);

        Canvas5.Instance.EscapeActionList.Add(CloseHelpTab);
    }

    public void OpenStat()
    {
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help_Tab);

        tabBtns_CanvasGroup.interactable = false;
        tabBtns_CanvasGroup.blocksRaycasts = false;

        nowText.gameObject.SetActive(false);
        statText.gameObject.SetActive(true);
        nowText = statText;
        scroll.value = 1;

        clicked_Tab = EventSystem.current.currentSelectedGameObject;
        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", true);
        clicked_Tab.GetComponent<Button>().animator.SetTrigger("Selected");

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(scroll.gameObject);

        Canvas5.Instance.EscapeActionList.Add(CloseHelpTab);
    }

    public void CloseHelpTab(){
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.Help_Tab);
        Canvas5.Instance.EscapeActionList.Remove(CloseHelpTab);

        clicked_Tab.GetComponent<Button>().animator.SetBool("Lock", false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clicked_Tab);

        clicked_Tab = null;

        tabBtns_CanvasGroup.interactable = true;
        tabBtns_CanvasGroup.blocksRaycasts = true;
    }
}
