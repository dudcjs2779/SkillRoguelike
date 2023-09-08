using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionAudio : MonoBehaviour
{
    public Selectable firstSelected;

    [SerializeField] CustomSlider masterVolume;
    [SerializeField] TextMeshProUGUI masterVolumeVal;

    [SerializeField] CustomSlider musicVolume;
    [SerializeField] TextMeshProUGUI musicVolumeVal;

    [SerializeField] CustomSlider gameVolume;
    [SerializeField] TextMeshProUGUI gameVolumeVal;

    [SerializeField] CustomSlider UIVolume;
    [SerializeField] TextMeshProUGUI UIVolumeVal;

    OptionMenu optionMenu;

    

    private void Awake()
    {
        optionMenu = GetComponentInParent<OptionMenu>();
    }

    private void Start()
    {
        masterVolume.minStep = 0.01f;
        masterVolume.maxStep = 0.05f;
        musicVolume.minStep = 0.01f;
        musicVolume.maxStep = 0.05f;
        gameVolume.minStep = 0.01f;
        gameVolume.maxStep = 0.05f;
        UIVolume.minStep = 0.01f;
        UIVolume.maxStep = 0.05f;
    }

    private void OnEnable() {
        Init_Auido();
    }

    public void Init_Auido()
    {
        float masterVal;
        SoundManager.Instance.mixer.GetFloat("Master", out masterVal);
        masterVal = Mathf.Clamp01(Mathf.Pow(10, masterVal / 20));
        masterVolume.value = masterVal;
        masterVolumeVal.text = ((int)(masterVal * 100)).ToString();
        Debug.Log("Init_Auido: " + masterVal);

        float musicVal;
        SoundManager.Instance.mixer.GetFloat("Music", out musicVal);
        musicVal = Mathf.Clamp01(Mathf.Pow(10, musicVal / 20));
        musicVolume.value = musicVal;
        musicVolumeVal.text = ((int)(musicVal * 100)).ToString();

        float gameVal;
        SoundManager.Instance.mixer.GetFloat("GameSFX", out gameVal);
        gameVal = Mathf.Clamp01(Mathf.Pow(10, gameVal / 20));
        gameVolume.value = gameVal;
        gameVolumeVal.text = ((int)(gameVal * 100)).ToString();

        float uiVal;
        SoundManager.Instance.mixer.GetFloat("UISFX", out uiVal);
        uiVal = Mathf.Clamp01(Mathf.Pow(10, uiVal / 20));
        UIVolume.value = uiVal;
        UIVolumeVal.text = ((int)(uiVal * 100)).ToString();
        
    }

    public void MasterSoundVolum(float val)
    {
        val = Mathf.Clamp(val, 0.0001f, 1f);
        SoundManager.Instance.mixer.SetFloat("Master", Mathf.Log10(val) * 20);
        masterVolumeVal.text = ((int)(val * 100)).ToString();

        Debug.Log("MasterSoundVolum: " + val);
    }

    public void MusicSoundVolum(float val)
    {
        val = Mathf.Clamp(val, 0.0001f, 1f);
        SoundManager.Instance.mixer.SetFloat("Music", Mathf.Log10(val) * 20);
        musicVolumeVal.text = ((int)(val * 100)).ToString();
    }

    public void GameSFXSoundVolum(float val)
    {
        val = Mathf.Clamp(val, 0.0001f, 1f);
        SoundManager.Instance.mixer.SetFloat("GameSFX", Mathf.Log10(val) * 20);
        gameVolumeVal.text = ((int)(val * 100)).ToString();
    }

    public void UISFXSoundVolum(float val)
    {
        val = Mathf.Clamp(val, 0.0001f, 1f);
        SoundManager.Instance.mixer.SetFloat("UISFX", Mathf.Log10(val) * 20);
        UIVolumeVal.text = ((int)(val * 100)).ToString();
    }

    public void ApplyAudio()
    {
        Debug.Log("Apply");
        float masterVal;
        SoundManager.Instance.mixer.GetFloat("Master", out masterVal);
        masterVal = Mathf.Clamp01(Mathf.Pow(10, masterVal / 20));
        GameManager.Instance.optionSetting.masterVolume  = masterVal;
        Debug.Log("ApplyAudio: " + masterVal);

        float musicVal;
        SoundManager.Instance.mixer.GetFloat("Music", out musicVal);
        musicVal = Mathf.Clamp01(Mathf.Pow(10, musicVal / 20));
        GameManager.Instance.optionSetting.musicVolume = musicVal;

        float gameVal;
        SoundManager.Instance.mixer.GetFloat("GameSFX", out gameVal);
        gameVal = Mathf.Clamp01(Mathf.Pow(10, gameVal / 20));
        GameManager.Instance.optionSetting.gameVolume = gameVal;

        float uiVal;
        SoundManager.Instance.mixer.GetFloat("UISFX", out uiVal);
        uiVal = Mathf.Clamp01(Mathf.Pow(10, uiVal / 20));
        GameManager.Instance.optionSetting.UIVolume = uiVal;
    }

    public void CloseAudio()
    {
        SoundManager.Instance.mixer.SetFloat("Master", Mathf.Log10(GameManager.Instance.optionSetting.masterVolume) * 20);
        masterVolume.value = GameManager.Instance.optionSetting.masterVolume;

        SoundManager.Instance.mixer.SetFloat("Music", Mathf.Log10(GameManager.Instance.optionSetting.musicVolume) * 20);
        musicVolume.value = GameManager.Instance.optionSetting.musicVolume;

        SoundManager.Instance.mixer.SetFloat("GameSFX", Mathf.Log10(GameManager.Instance.optionSetting.gameVolume) * 20);
        gameVolume.value = GameManager.Instance.optionSetting.gameVolume;

        SoundManager.Instance.mixer.SetFloat("UISFX", Mathf.Log10(GameManager.Instance.optionSetting.UIVolume) * 20);
        UIVolume.value = GameManager.Instance.optionSetting.UIVolume;
    }

    public void CheckOptionChanged(){

        float masterVal;
        SoundManager.Instance.mixer.GetFloat("Master", out masterVal);
        masterVal =  Mathf.Clamp01(Mathf.Pow(10, masterVal / 20));
        
        float musicVal;
        SoundManager.Instance.mixer.GetFloat("Music", out musicVal);
        musicVal = Mathf.Clamp01(Mathf.Pow(10, musicVal / 20));

        float gameVal;
        SoundManager.Instance.mixer.GetFloat("GameSFX", out gameVal);
        gameVal = Mathf.Clamp01(Mathf.Pow(10, gameVal / 20));

        float uiVal;
        SoundManager.Instance.mixer.GetFloat("UISFX", out uiVal);
        uiVal = Mathf.Clamp01(Mathf.Pow(10, uiVal / 20));

        if(GameManager.Instance.optionSetting.masterVolume != masterVal ||
        GameManager.Instance.optionSetting.musicVolume != musicVal ||
        GameManager.Instance.optionSetting.gameVolume != gameVal ||
        GameManager.Instance.optionSetting.UIVolume != uiVal
        ){
            optionMenu.isOptionChanged = true;
        }

        //Debug.Log("abc");
    }

}
