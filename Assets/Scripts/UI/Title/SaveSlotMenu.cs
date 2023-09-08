using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SaveSlotMenu : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private SaveSlot clickedSaveSlot;

    [Header("Navigation")]
    public Selectable firstSelected;

    [Header("Component")]
    [SerializeField] Button[] saveSlotBtns;
    [SerializeField] RectTransform PopupMenu;

    [Header("Script")]
    [SerializeField] private TitleMenu titleMenu;
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private SaveSlot[] saveSlots;


    private void Awake(){
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(){
        clickedSaveSlot = EventSystem.current.currentSelectedGameObject.GetComponent<SaveSlot>();

        if(clickedSaveSlot.hasData){
            if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.SaveSlot_PopupListMenu))
            {
                Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.SaveSlot_PopupListMenu);
                Canvas5.Instance.EscapeActionList.Add(Escape_PopupMenu);
                PopupMenu.gameObject.SetActive(true);
            }

            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            {
                PopupMenu.transform.position = Mouse.current.position.ReadValue();
            }
            else
            {
                PopupMenu.transform.position = clickedSaveSlot.transform.position;
            }

            PopupMenu.GetComponentInChildren<Button>().Select();
        }
        else{
            StartConfirmCheck();
        }
    }

    public void OnClick_PopupMenuLoad(){
        StartConfirmCheck();
    }

    public void OnClick_PopupMenuClear()
    {
        SaveSlot saveSlot = clickedSaveSlot;
        confirmationPopupMenu.SelectConfirm();
        confirmationPopupMenu.ActivateMenu(
            string.Format("슬롯{0} 데이터를 정말로 삭제할까요?", int.Parse(saveSlot.GetProfileId()) + 1),
            () => {
                DataPersistenceManager.Instance.DeleteProfileData(clickedSaveSlot.GetProfileId());
                titleMenu.DisableBtnsDependingOnData();
                confirmationPopupMenu.Escape_ConfirmMenu();
                ActivateMenu();
                clickedSaveSlot.GetComponent<Button>().Select();
                clickedSaveSlot = null;
            },
            () =>{
                confirmationPopupMenu.Escape_ConfirmMenu();
                ActivateMenu();

                clickedSaveSlot.GetComponent<Button>().Select();
                clickedSaveSlot = null;
            });

        Escape_PopupMenuOnlyDisable();
    }

    void StartConfirmCheck(){
        DisableMenuButtons();

        // 슬롯에 데이터가 있는경우
        if(clickedSaveSlot.hasData){
            SaveSlot saveSlot = clickedSaveSlot;
            confirmationPopupMenu.SelectConfirm();
            confirmationPopupMenu.ActivateMenu(
                string.Format("슬롯{0} 데이터를 불러올까요?", int.Parse(saveSlot.GetProfileId()) + 1),
                () => {
                    confirmationPopupMenu.Escape_ConfirmMenu();
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    //DataPersistenceManager.Instance.NewGame();
                    Canvas5.Instance.UISequenceList.Clear();
                    Canvas5.Instance.EscapeActionList.Clear();

                    SaveGameAndLoadScene();
                },

                () =>{
                    confirmationPopupMenu.Escape_ConfirmMenu();
                    clickedSaveSlot.GetComponent<Button>().Select();
                    clickedSaveSlot = null;

                    this.ActivateMenu();
                }

            );
            Escape_PopupMenuOnlyDisable();
        }
        // 슬롯에 데이터가 없는 경우
        else{
            SaveSlot saveSlot = clickedSaveSlot;
            confirmationPopupMenu.SelectConfirm();
            confirmationPopupMenu.ActivateMenu(
                string.Format("슬롯{0} 데이터로 게임을 새로 시작할까요?", int.Parse(saveSlot.GetProfileId()) + 1),
                () =>
                {
                    confirmationPopupMenu.Escape_ConfirmMenu();
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    Canvas5.Instance.UISequenceList.Clear();
                    Canvas5.Instance.EscapeActionList.Clear();

                    SaveGameAndLoadScene();
                },

                () =>
                {
                    Debug.Log(clickedSaveSlot.name);
                    confirmationPopupMenu.Escape_ConfirmMenu();
                    clickedSaveSlot.GetComponent<Button>().Select();
                    clickedSaveSlot = null;

                    this.ActivateMenu();
                }

            );
            Escape_PopupMenuOnlyDisable();
        }
    }

    private void SaveGameAndLoadScene(){
        SceneManager.sceneLoaded += GameManager.Instance.OnSceneLoaded_TitleToGame;
        DataPersistenceManager.Instance.SaveGame();

        GameManager.Instance.SceneChange("StartMap", () => 
        {
            DataPersistenceManager.Instance.DestoyObjectsForGame();
        },
        () => {
            GameManager.Instance.PlayerStartPosition();
        }
        );
    }

    void Escape_PopupMenu(){
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.SaveSlot_PopupListMenu);
        Canvas5.Instance.EscapeActionList.Remove(Escape_PopupMenu);
        PopupMenu.gameObject.SetActive(false);

        clickedSaveSlot.GetComponent<Button>().Select();
        clickedSaveSlot = null;
    }

    void Escape_PopupMenuOnlyDisable(){
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.SaveSlot_PopupListMenu);
        Canvas5.Instance.EscapeActionList.Remove(Escape_PopupMenu);
        PopupMenu.gameObject.SetActive(false);
    }

    void Escape_ConfirmMenu(){
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.ConfirmMenu);
        Canvas5.Instance.EscapeActionList.Remove(Escape_ConfirmMenu);
    }

    public void ActivateMenu(){
        this.gameObject.SetActive(true);

        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        foreach(SaveSlot saveSlot in saveSlots){
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            saveSlot.SetInteractable(true);
        }
    }

    public void DeactivateMenu(){
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
    }

    public void SaveSlotNavigation(){
        for (int i = 0; i < saveSlotBtns.Length; i++)
        {
            Navigation newNavi = new Navigation();
            newNavi.mode = Navigation.Mode.Explicit;

            newNavi.selectOnUp = i > 0 ? saveSlotBtns[i - 1] : null;
            newNavi.selectOnDown = i < saveSlotBtns.Length - 1 ? saveSlotBtns[i + 1] : null;

            saveSlotBtns[i].navigation = newNavi;
        }
    }
}
