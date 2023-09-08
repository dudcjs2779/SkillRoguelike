using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class OptionKeySetting : MonoBehaviour
{
    public Selectable firstSelected;

    [SerializeField] RectTransform viewportRect;
    [SerializeField] RectTransform contentRect;

    [SerializeField] CustomSlider mouseSlider;
    [SerializeField] TextMeshProUGUI mouseSliderText;

    [SerializeField] CustomSlider gamepadSlider;
    [SerializeField] TextMeshProUGUI gamepadSliderText;

    [SerializeField] ScrollViewItem[] scrollViewItems;
    [SerializeField] RebindingKey[] RebindingKeys;

    OptionMenu optionMenu;

    private void Awake() {
        optionMenu= GetComponentInParent<OptionMenu>();
        scrollViewItems = GetComponentsInChildren<ScrollViewItem>(true);
        RebindingKeys = GetComponentsInChildren<RebindingKey>(true);
        mouseSlider.minStep = 0.1f;
        mouseSlider.maxStep = 0.5f;
        gamepadSlider.minStep = 0.1f;
        gamepadSlider.maxStep = 0.5f;
    }

    void Start()
    {
        mouseSlider.onValueChanged.AddListener(OnChangedMouseSensitivity);
        gamepadSlider.onValueChanged.AddListener(OnChangedGamepadSensitivity);

        mouseSlider.onValueChanged.Invoke(GameManager.Instance.optionSetting.mouseSensitivity / 50f);
        gamepadSlider.onValueChanged.Invoke(GameManager.Instance.optionSetting.gamepadSensitivity / 500f);

        foreach (var item in scrollViewItems)
        {
            item.onSelectEvent.AddListener(HandleEventItemOnSelect);
        }
    }

    public void Init_Keys(){
        foreach (var RebindingKey in RebindingKeys)
        {
            RebindingKey.Init_Key();
        }
    }

    void OnChangedMouseSensitivity(float val){
        val =MathF.Round(val, 1);
        mouseSlider.value = val;
        mouseSliderText.text = val.ToString();
    }

    void OnChangedGamepadSensitivity(float val)
    {
        val = MathF.Round(val, 1);
        gamepadSlider.value = val;
        gamepadSliderText.text = val.ToString();
    }

    public void CheckOptionChanged(){
        if(!Mathf.Approximately(mouseSlider.value, GameManager.Instance.optionSetting.mouseSensitivity / 50f) ||
            !Mathf.Approximately(gamepadSlider.value, GameManager.Instance.optionSetting.gamepadSensitivity / 500f)){
            optionMenu.isOptionChanged = true;
        }
        
    }

    public void ApplyKeySetting(){
        GameManager.Instance.Save_KeysToJson();
        GameManager.Instance.optionSetting.mouseSensitivity = mouseSlider.value * 50f;
        GameManager.Instance.optionSetting.gamepadSensitivity = gamepadSlider.value * 500f;

        PlayerInputControls.Instance.mouseSpeed = GameManager.Instance.optionSetting.mouseSensitivity;
        PlayerInputControls.Instance.rightStick_Speed = GameManager.Instance.optionSetting.gamepadSensitivity;

        GameManager.Instance.Save_OptionSettingToJosn();
    }

    public void CloseKeySetting(){
        GameManager.Instance.Load_KeysFromJson();
        mouseSlider.onValueChanged.Invoke(GameManager.Instance.optionSetting.mouseSensitivity / 50f);
        gamepadSlider.onValueChanged.Invoke(GameManager.Instance.optionSetting.gamepadSensitivity / 500f);
    }

    private void HandleEventItemOnSelect(RectTransform rect){
        Debug.Log(rect.transform.parent.parent.name);
        AutoScroll.MoveViewport(rect, viewportRect, contentRect);
    }
}
