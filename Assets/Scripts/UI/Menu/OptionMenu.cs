using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionMenu : MonoBehaviour
{
    [Header("Navigation")]
    public Selectable firstSelected;

    [Header("Button")]
    [SerializeField] Button displayBtn;
    [SerializeField] Button audioBtn;
    [SerializeField] Button keySettingBtn;
    [SerializeField] Button resetBtn;

    [Header("CanvasGroup")]
    [SerializeField] CanvasGroup optionTab_CanvasGroup;
    [SerializeField] CanvasGroup display_CanvasGroup;
    [SerializeField] CanvasGroup audio_CanvasGroup;
    [SerializeField] CanvasGroup keySetting_CanvasGroup;
    CanvasGroup canvasGroup;

    [Header("Script")]
    [SerializeField] OptionDisplay optionDisplay;
    [SerializeField] OptionAudio optionAudio;
    [SerializeField] OptionKeySetting optionKeySetting;

    Button clicked_OptionBtn;

    public bool isOptionChanged;

    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        isOptionChanged = false;
        display_CanvasGroup.interactable = false;
        display_CanvasGroup.blocksRaycasts = false;
        optionDisplay.gameObject.SetActive(true);
        optionAudio.gameObject.SetActive(false);
        optionKeySetting.gameObject.SetActive(false);
    }

    private void Start() {
        if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu)){
            gameObject.SetActive(false);
        }

        displayBtn.onClick.AddListener(OnClick_Display);
        audioBtn.onClick.AddListener(OnClick_Audio);
        keySettingBtn.onClick.AddListener(OnClick_KeySetting);
        resetBtn.onClick.AddListener(OnClick_OptionReset);
    }

    public void OnClick_Display()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_Display))
        {
            Debug.Log("OpenDisplay");
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu_Display);
            Canvas5.Instance.EscapeActionList.Add(Escape_Display);

            optionDisplay.gameObject.SetActive(true);
            optionAudio.gameObject.SetActive(false);
            optionKeySetting.gameObject.SetActive(false);

            InteractableDisplay(true);

            clicked_OptionBtn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            clicked_OptionBtn.animator.SetBool("Lock", true);
            clicked_OptionBtn.animator.SetTrigger("Selected");

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionDisplay.firstSelected.gameObject);
        }
    }

    public void Escape_Display()
    {
        if (Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_Display))
        {
            Debug.Log("CloseDisplay");
            Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.OptionMenu_Display);
            Canvas5.Instance.EscapeActionList.Remove(Escape_Display);
            InteractableDisplay(false);

            clicked_OptionBtn.animator.SetBool("Lock", false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(clicked_OptionBtn.gameObject);
            clicked_OptionBtn = null;
        }
    }

    public void OnClick_Audio()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_Audio))
        {
            Debug.Log("OpenAudio");
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu_Audio);
            Canvas5.Instance.EscapeActionList.Add(Escape_Audio);

            optionDisplay.gameObject.SetActive(false);
            optionAudio.gameObject.SetActive(true);
            optionKeySetting.gameObject.SetActive(false);

            InteractableAudio(true);

            clicked_OptionBtn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            clicked_OptionBtn.animator.SetBool("Lock", true);
            clicked_OptionBtn.animator.SetTrigger("Selected");

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionAudio.firstSelected.gameObject);
            
        }
    }

    public void Escape_Audio()
    {
        if (Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_Audio))
        {
            Debug.Log("CloseAudio");
            Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.OptionMenu_Audio);
            Canvas5.Instance.EscapeActionList.Remove(Escape_Audio);
            InteractableAudio(false);

            clicked_OptionBtn.animator.SetBool("Lock", false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(clicked_OptionBtn.gameObject);
            clicked_OptionBtn = null;

            optionAudio.CheckOptionChanged();
        }
    }

    public void OnClick_KeySetting()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_KeySetting))
        {
            Debug.Log("OpenAudio");
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu_KeySetting);
            Canvas5.Instance.EscapeActionList.Add(Escape_KeySetting);

            optionDisplay.gameObject.SetActive(false);
            optionAudio.gameObject.SetActive(false);
            optionKeySetting.gameObject.SetActive(true);

            InteractableKeySetting(true);

            clicked_OptionBtn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            clicked_OptionBtn.animator.SetBool("Lock", true);
            clicked_OptionBtn.animator.SetTrigger("Selected");

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionKeySetting.firstSelected.gameObject);
        }
    }

    public void Escape_KeySetting()
    {
        if (Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_KeySetting))
        {
            Debug.Log("CloseAudio");
            Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.OptionMenu_KeySetting);
            Canvas5.Instance.EscapeActionList.Remove(Escape_KeySetting);

            InteractableKeySetting(false);

            clicked_OptionBtn.animator.SetBool("Lock", false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(clicked_OptionBtn.gameObject);
            clicked_OptionBtn = null;

            optionKeySetting.CheckOptionChanged();
        }
    }

    public void OptionApplyYes(){
        optionDisplay.ApplyDisplay();
        optionAudio.ApplyAudio();
        optionKeySetting.ApplyKeySetting();
        GameManager.Instance.Save_OptionSettingToJosn();
    }

    public void OptionApplyNo()
    {
        optionDisplay.CloseDisplay();
        optionAudio.CloseAudio();
        optionKeySetting.CloseKeySetting();
        GameManager.Instance.Save_OptionSettingToJosn();
    }

    public void OnClick_OptionReset()
    {
        InteractableOptionMenu(false);
        clicked_OptionBtn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Debug.Log(clicked_OptionBtn.name);
        clicked_OptionBtn.animator.SetBool("Lock", true);

        confirmationPopupMenu.SelectCancle();
        confirmationPopupMenu.ActivateMenu(
            "정말로 초기화할까요?",
            () =>
            {
                ResetYes();
                confirmationPopupMenu.Escape_ConfirmMenu();

                clicked_OptionBtn.animator.SetBool("Lock", false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(clicked_OptionBtn.gameObject);
                clicked_OptionBtn = null;

                InteractableOptionMenu(true);
            },
            () =>
            {
                confirmationPopupMenu.Escape_ConfirmMenu();
                Debug.Log("Cancle");

                clicked_OptionBtn.animator.SetBool("Lock", false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(clicked_OptionBtn.gameObject);
                clicked_OptionBtn = null;

                InteractableOptionMenu(true);
            }
        );
    }

    void ResetYes()
    {
        string optionPath = Path.Combine(Application.streamingAssetsPath, "JSON/OptionSetting.json"); ;
        if(File.Exists(optionPath)){
            File.Delete(optionPath);
            GameManager.Instance.Load_OptionSettingFromJson();
        }
        
        string keyPath = Path.Combine(Application.streamingAssetsPath, "JSON/RebindKeys.json");
        if(File.Exists(keyPath)){
            File.WriteAllText(keyPath, "");
            GameManager.Instance.Load_KeysFromJson();
        }

        optionDisplay.Init_Display();
        optionAudio.Init_Auido();
        optionKeySetting.Init_Keys();
    }

    public void InteractableDisplay(bool interactable)
    {
        optionTab_CanvasGroup.interactable = !interactable;
        optionTab_CanvasGroup.blocksRaycasts = !interactable;

        display_CanvasGroup.interactable = interactable;
        display_CanvasGroup.blocksRaycasts = interactable;
    }

    public void InteractableAudio(bool interactable)
    {
        optionTab_CanvasGroup.interactable = !interactable;
        optionTab_CanvasGroup.blocksRaycasts = !interactable;

        audio_CanvasGroup.interactable = interactable;
        audio_CanvasGroup.blocksRaycasts = interactable;
    }

    public void InteractableKeySetting(bool interactable)
    {
        optionTab_CanvasGroup.interactable = !interactable;
        optionTab_CanvasGroup.blocksRaycasts = !interactable;

        keySetting_CanvasGroup.interactable = interactable;
        keySetting_CanvasGroup.blocksRaycasts = interactable;
    }

    public void InteractableOptionMenu(bool interactable){
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }
}
