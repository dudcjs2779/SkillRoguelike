using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class OptionDisplay : MonoBehaviour
{
    [Header("Dugging")]
    public GameObject clickedObj;

    [Header("Navigation")]
    public Selectable firstSelected;

    [Header("Selectable")]
    public Button resolutionBtn;
    public Button frameRateBtn;
    public Toggle VSyncToggle;
    public Toggle fullScreenToggle;
    public Button qualityBtn;
    CanvasGroup canvasGroup;
    
    [SerializeField] GameObject dropdownView;

    List<Resolution> resolutionList = new List<Resolution>();
    List<string> resolutionTextList = new List<string>();
    List<string> frameRateList = new List<string>();
    List<string> qualityList = new List<string>();

    public Resolution selectedResolution;

    OptionMenu optionMenu;

    private Action<int> onClick_DropdownItem;

    private void Awake() {
        optionMenu = GetComponentInParent<OptionMenu>();
        canvasGroup= GetComponent<CanvasGroup>();
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        Init_Display();
    }

    public void Init_Display()
    {
        GetResolutions();
        GetFrameRate();

        VSyncToggle.isOn = QualitySettings.vSyncCount != 0;

        fullScreenToggle.isOn = Screen.fullScreen;

        GetQuality();
    }

    public void GetResolutions()
    {
        Resolution[] tempResolutions = Screen.resolutions;
        resolutionList.Clear();
        resolutionTextList.Clear();

        for (int i = 0; i < tempResolutions.Length; i++)
        {
            float ratio = (float)tempResolutions[i].width / tempResolutions[i].height;

            if((ratio == 16f / 9f || ratio == 16f / 10f) && tempResolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                resolutionList.Add(tempResolutions[i]);
            }
        }

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutionList.Count; i++)
        {
            string option = resolutionList[i].width + " x " + resolutionList[i].height;
            resolutionTextList.Add(option);

            if (GameManager.Instance.optionSetting.resolution.width == resolutionList[i].width &&
                GameManager.Instance.optionSetting.resolution.height == resolutionList[i].height)
            {
                currentResolutionIndex = i;
                selectedResolution = resolutionList[i];
                resolutionBtn.GetComponentInChildren<TextMeshProUGUI>().text = resolutionTextList[i];
            }
        }
    }

    void GetFrameRate()
    {
        frameRateList.Clear();
        frameRateList.Add("30");
        frameRateList.Add("60");
        frameRateList.Add("120");
        frameRateList.Add("144");
        frameRateList.Add("240");

        frameRateBtn.GetComponentInChildren<TextMeshProUGUI>().text = Application.targetFrameRate.ToString();
    }

    void GetQuality()
    {
        qualityList.Clear();
        qualityList.Add("Very Low");
        qualityList.Add("Low");
        qualityList.Add("Medium");
        qualityList.Add("High");
        qualityList.Add("Very High");
        qualityList.Add("Ultra");

        qualityBtn.GetComponentInChildren<TextMeshProUGUI>().text = qualityList[QualitySettings.GetQualityLevel()];
    }

    public void SetResolution(int resolutionIndex)
    {
        if(Screen.currentResolution.width == resolutionList[resolutionIndex].width &&
        Screen.currentResolution.height == resolutionList[resolutionIndex].height){
            Escape_DropdownView();
            return;
        }

        Debug.Log("SetResolution");
        selectedResolution = resolutionList[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        resolutionBtn.GetComponentInChildren<TextMeshProUGUI>().text = resolutionTextList[resolutionIndex];
        optionMenu.isOptionChanged = true;
        Escape_DropdownView();
    }

    public void OnClick_Resolution(){
        if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_DropdownView)){
            Debug.Log("OnClick_Resolution");
            InteractableDisplay(false);

            onClick_DropdownItem = SetResolution;
            dropdownView.GetComponent<DropdownView>().Init_DropdownView(resolutionTextList, onClick_DropdownItem);

            dropdownView.SetActive(true);
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu_DropdownView);
            Canvas5.Instance.EscapeActionList.Add(Escape_DropdownView);

            clickedObj = EventSystem.current.currentSelectedGameObject;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(dropdownView.GetComponentInChildren<DropdownItem>().gameObject);
        }
    }

    public void Escape_DropdownView(){
        if (Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_DropdownView)){
            Debug.Log("Escape_OnClick_Resolution");
            dropdownView.SetActive(false);

            Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.OptionMenu_DropdownView);
            Canvas5.Instance.EscapeActionList.Remove(Escape_DropdownView);
            InteractableDisplay(true);


            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(clickedObj);

            clickedObj = null;
        }
    }

    public void SetFrameRate(int frameIndex)
    {
        if(Application.targetFrameRate == int.Parse(frameRateList[frameIndex])){
            Escape_DropdownView();
            return;
        }

        Application.targetFrameRate = int.Parse(frameRateList[frameIndex]);
        frameRateBtn.GetComponentInChildren<TextMeshProUGUI>().text = Application.targetFrameRate.ToString();

        optionMenu.isOptionChanged = true;

        Escape_DropdownView();
    }

    public void OnClick_FrameRate()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_DropdownView))
        {
            Debug.Log("OnClick_FrameRate");
            InteractableDisplay(false);

            onClick_DropdownItem = SetFrameRate;
            dropdownView.GetComponent<DropdownView>().Init_DropdownView(frameRateList, onClick_DropdownItem);

            dropdownView.SetActive(true);
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu_DropdownView);
            Canvas5.Instance.EscapeActionList.Add(Escape_DropdownView);

            clickedObj = EventSystem.current.currentSelectedGameObject;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(dropdownView.GetComponentInChildren<DropdownItem>().gameObject);

            dropdownView.SetActive(true);
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        if(Screen.fullScreen == isFullScreen) return;
            
        Screen.fullScreen = isFullScreen;
        optionMenu.isOptionChanged = true;
    }

    public void SetVSync(bool isVSync)
    {
        bool isNowVsync = QualitySettings.vSyncCount > 0 ? true : false;
        if(isNowVsync == isVSync) return;

        if (isVSync)
        {
            QualitySettings.vSyncCount = 1;
            Debug.Log("isVSyncOn");
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Debug.Log("isVSyncOff");
        }

        optionMenu.isOptionChanged = true;
    }

    public void SetQuality(int qualityIndex)
    {
        if(QualitySettings.GetQualityLevel() == qualityIndex){
            Escape_DropdownView();
            return;
        } 

        QualitySettings.SetQualityLevel(qualityIndex);
        qualityBtn.GetComponentInChildren<TextMeshProUGUI>().text = qualityList[qualityIndex];

        optionMenu.isOptionChanged = true;

        Escape_DropdownView();
    }

    public void OnClick_Quality()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu_DropdownView))
        {
            Debug.Log("OnClick_FrameRate");
            InteractableDisplay(false);

            onClick_DropdownItem = SetQuality;
            dropdownView.GetComponent<DropdownView>().Init_DropdownView(qualityList, onClick_DropdownItem);

            dropdownView.SetActive(true);
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu_DropdownView);
            Canvas5.Instance.EscapeActionList.Add(Escape_DropdownView);

            clickedObj = EventSystem.current.currentSelectedGameObject;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(dropdownView.GetComponentInChildren<DropdownItem>().gameObject);

            dropdownView.SetActive(true);
        }
    }

    public void ApplyDisplay()
    {
        GameManager.Instance.optionSetting.resolution = selectedResolution;
        GameManager.Instance.optionSetting.frameRate = Application.targetFrameRate;
        GameManager.Instance.optionSetting.vSyncCount = QualitySettings.vSyncCount;
        GameManager.Instance.optionSetting.isFullScreen = Screen.fullScreen;
        GameManager.Instance.optionSetting.qualityIndex = QualitySettings.GetQualityLevel();
    }

    public void CloseDisplay()
    {
        Debug.Log("Close");
        Resolution resolution = resolutionList.Find(x =>
        x.width == GameManager.Instance.optionSetting.resolution.width
        && x.height == GameManager.Instance.optionSetting.resolution.height);

        Debug.Log(resolution);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        Application.targetFrameRate = GameManager.Instance.optionSetting.frameRate;

        Screen.fullScreen = GameManager.Instance.optionSetting.isFullScreen;

        QualitySettings.SetQualityLevel(GameManager.Instance.optionSetting.qualityIndex);

        QualitySettings.vSyncCount = GameManager.Instance.optionSetting.vSyncCount;
    }

    void InteractableDisplay(bool interactable){
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }
}
