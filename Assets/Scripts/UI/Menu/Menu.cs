using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Selectable firstSelected;

    public GameObject menu;
    protected CanvasGroup canvasGroup;

    [SerializeField] Button resumeBtn;
    [SerializeField] Button optionBtn;
    [SerializeField] Button helpBtn;
    [SerializeField] Button giveUpBtn;
    [SerializeField] Button toTitleBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] List<Button> menuBtns = new List<Button>();

    [SerializeField] OptionMenu optionMenu;
    [SerializeField] HelpMenu helpMenu;

    Button clickedBtn;

    [SerializeField] protected ConfirmationPopupMenu confirmationPopupMenu;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Menu) && menu != null)
        {
            menu.gameObject.SetActive(false);
        }
    }

    protected virtual void OnEnable() {
        MenuNavigation();
    }

    protected void InteractableMenu(bool interactable){
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    public void OnCLick_Option()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu))
        {
            Debug.Log("OpenOption");
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.OptionMenu);
            Canvas5.Instance.EscapeActionList.Add(Escape_Option);
            optionMenu.gameObject.SetActive(true);

            InteractableMenu(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionMenu.firstSelected.gameObject);
        }
    }

    public void Escape_Option()
    {
        // 옵션 변경사항이 있을경우
        if(optionMenu.isOptionChanged){
            confirmationPopupMenu.SelectConfirm();
            confirmationPopupMenu.ActivateMenu(
                "변경 내용을 저장할까요?",
                () =>
                {
                    optionMenu.OptionApplyYes();
                    confirmationPopupMenu.Escape_ConfirmMenu();
                    CloseOption();
                },
                () =>
                {
                    optionMenu.OptionApplyNo();
                    confirmationPopupMenu.Escape_ConfirmMenu();
                    CloseOption();
                }
            );
        }
        // 옵션 변경사항이 없을 경우
        else{
            CloseOption();
        }
    }

    void CloseOption(){
        if (Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.OptionMenu))
        {
            Debug.Log("CloseOption");
            Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.OptionMenu);
            Canvas5.Instance.EscapeActionList.Remove(Escape_Option);

            optionMenu.gameObject.SetActive(false);
            InteractableMenu(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionBtn.gameObject);
        }
    }

    public void OnClick_Help()
    {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Help))
        {
            Debug.Log("OpenHelp");
            Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.Help);
            Canvas5.Instance.EscapeActionList.Add(Escape_Help);
            
            helpMenu.gameObject.SetActive(true);
            InteractableMenu(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(helpMenu.firstSelected.gameObject);
        }
    }

    public void Escape_Help()
    {
        if (Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Help))
        {
            Debug.Log("CloseHelp");
            Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.Help);
            Canvas5.Instance.EscapeActionList.Remove(Escape_Help);

            helpMenu.gameObject.SetActive(false);
            InteractableMenu(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(helpBtn.gameObject);
        }
    }

    public void OnClick_ToTitle()
    {
        InteractableMenu(false);
        clickedBtn = toTitleBtn;

        confirmationPopupMenu.SelectCancle();
        confirmationPopupMenu.ActivateMenu(
            "타이틀로 돌아갈까요?",
            () =>
            {
                GameManager.Instance.Init_playerState = null;
                DataPersistenceManager.Instance.SaveGame();
                SceneManager.sceneLoaded += DataPersistenceManager.Instance.OnSceneLoaded_LoadData;

                GameManager.Instance.isTitle = true;
                PlayerInputControls.Instance.ToTitle();
                GameManager.Instance.ResumeGame();

                GameManager.Instance.SceneChange("TitleScene", () => {
                    DataPersistenceManager.Instance.DestoyObjectsForTitle();
                },
                () => {});
                
                //SceneManager.LoadSceneAsync("TitleScene");
            },
            () =>
            {
                confirmationPopupMenu.Escape_ConfirmMenu();
                InteractableMenu(true);
                EventSystem.current.SetSelectedGameObject(null);
                clickedBtn.Select();
                clickedBtn = null;
            }
        );
    }

    public virtual void OnClick_ExitGame(){
        InteractableMenu(false);
        clickedBtn = exitBtn;

        confirmationPopupMenu.SelectCancle();
        confirmationPopupMenu.ActivateMenu(
            "게임을 종료할까요?",
            () => {
                DataPersistenceManager.Instance.SaveGame();
                Application.Quit();
            },
            () => {
                confirmationPopupMenu.Escape_ConfirmMenu();
                InteractableMenu(true);
                EventSystem.current.SetSelectedGameObject(null);
                clickedBtn.Select();
                clickedBtn = null;
            }
        );
    }

    public void OnClick_GiveUp()
    {
        InteractableMenu(false);
        clickedBtn = giveUpBtn;

        confirmationPopupMenu.SelectCancle();
        confirmationPopupMenu.ActivateMenu(
            "정말로 포기할까요?",
            () =>
            {
                confirmationPopupMenu.Escape_ConfirmMenu();
                clickedBtn = null;
                Canvas5.Instance.Escape();
                InteractableMenu(true);
                GameManager.Instance.GameOver();
            },
            () =>
            {
                confirmationPopupMenu.Escape_ConfirmMenu();
                InteractableMenu(true);
                //EventSystem.current.SetSelectedGameObject(null);
                clickedBtn.Select();
                clickedBtn = null;
            }
        );
    }

    void MenuNavigation(){
        if(GameManager.Instance.stageLevel > 0){
            giveUpBtn.gameObject.SetActive(true);
            toTitleBtn.gameObject.SetActive(false);
            exitBtn.gameObject.SetActive(false);
        }
        else{
            giveUpBtn.gameObject.SetActive(false);
            toTitleBtn.gameObject.SetActive(true);
            exitBtn.gameObject.SetActive(true);
        }

        List<Button> interactableBtns = menuBtns.FindAll(x => x.gameObject.activeSelf);
        
        for (int i = 0; i < interactableBtns.Count; i++)
        {
            Navigation newNavi = new Navigation();
            newNavi.mode = Navigation.Mode.Explicit;
            newNavi.selectOnUp = i > 0 ? interactableBtns[i - 1] : null;
            newNavi.selectOnDown = i < interactableBtns.Count - 1 ? interactableBtns[i + 1] : null;

            interactableBtns[i].navigation = newNavi;
        }
    }
}
