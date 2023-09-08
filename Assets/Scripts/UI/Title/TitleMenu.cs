using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class TitleMenu : Menu
{
    [Header("TitleMenu")]

    [Header("Button")]
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Button exitGameBtn;
    [SerializeField] private List<Button> titleBtnList = new List<Button>();

    [Header("Script")]
    [SerializeField] private SaveSlotMenu saveSlotMenu;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void OnEnable() {
        
    }

    private void Start() {
        DisableBtnsDependingOnData();
    }

    public void DisableBtnsDependingOnData(){
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueBtn.interactable = false;

            firstSelected = titleBtnList.Find(x => x.interactable);
        }

        List<Button> interactableBtns = titleBtnList.FindAll(x => x.interactable);
        for (int i = 0; i < interactableBtns.Count; i++)
        {
            Navigation newNavi = new Navigation();
            newNavi.mode = Navigation.Mode.Explicit;
            newNavi.selectOnUp = i > 0 ? interactableBtns[i - 1] : null;
            newNavi.selectOnDown = i < interactableBtns.Count - 1 ? interactableBtns[i + 1] : null;

            interactableBtns[i].navigation = newNavi;
        }

        firstSelected.Select();
    }

    public void OnContinueClick()
    {
        InteractableMenu(false);
        SceneManager.sceneLoaded += GameManager.Instance.OnSceneLoaded_TitleToGame;
        DataPersistenceManager.Instance.SaveGame();

        GameManager.Instance.SceneChange("StartMap", () =>
        {
            DataPersistenceManager.Instance.DestoyObjectsForGame();
        },
        () => {
            GameManager.Instance.PlayerStartPosition();
        });
    }

    public void OnStartGameClick()
    {
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.SaveSlotMenu);
        Canvas5.Instance.EscapeActionList.Add(EscapeSaveSlotMenu);

        saveSlotMenu.ActivateMenu();
        saveSlotMenu.firstSelected.Select();
        saveSlotMenu.SaveSlotNavigation();
        InteractableMenu(false);
    }

    void EscapeSaveSlotMenu()
    {
        saveSlotMenu.DeactivateMenu();
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.SaveSlotMenu);
        Canvas5.Instance.EscapeActionList.Remove(EscapeSaveSlotMenu);
        startGameBtn.Select();
        InteractableMenu(true);
    }

    public override void OnClick_ExitGame()
    {
        InteractableMenu(false);

        confirmationPopupMenu.SelectCancle();
        confirmationPopupMenu.ActivateMenu(
            "게임을 종료할까요?",
            () =>
            {
                Application.Quit();
            },
            () =>
            {
                confirmationPopupMenu.Escape_ConfirmMenu();
                InteractableMenu(true);
                exitGameBtn.Select();
            }
        );
    }
}
