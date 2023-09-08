using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI dodgeCountText;
    [SerializeField] private TextMeshProUGUI createDateText;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI moneyText;

    private Button saveSlotButton;

    public bool hasData {get; private set;} = false;

    private void Awake() {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data){
        if(data == null){
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else{
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            DateTime createDate = DateTime.FromBinary(data.createDate);
            createDateText.text = string.Format("생성 날짜: {0}", createDate.ToString("yy.MM.dd HH:mm:ss"));

            TimeSpan timeSpan = TimeSpan.FromSeconds(data.playerData.playTime);
            int totalHours = timeSpan.Days * 24 + timeSpan.Hours;
            playTimeText.text = string.Format("플레이 타임: {0}:{1:D2}:{2:D2}", totalHours, timeSpan.Minutes, timeSpan.Seconds);

            moneyText.text = string.Format("소지금: {0}", data.playerData.money);
        }
    }

    public string GetProfileId(){
        return this.profileId;
    }

    public void SetInteractable(bool interactable){
        saveSlotButton.interactable = interactable;
    }

    public bool IsEmptySlot(){
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        GameData profileData = null;
        profilesGameData.TryGetValue(profileId, out profileData);

        if(profileData == null){
            return true;
        }
        else{
            return false;
        }
    }
}
