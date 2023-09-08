using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Ink.Runtime;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour, IDataPersistence
{
    private static DialogueManager instance;
    public static DialogueManager Instance
    {
        get
        {
            // if (instance == null)
            // {
            //     var obj = FindObjectOfType<DialogueManager>();
            //     if (obj != null)
            //     {
            //         instance = obj;
            //     }
            //     else
            //     {
            //         var newObj = new GameObject().AddComponent<DialogueManager>();
            //         instance = newObj;
            //     }
            // }
            return instance;
        }
    }

    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Globals Ink File")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialouge UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private Button[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying {get; private set;}
    public bool dialogueIsPausing { get; private set; }
    [SerializeField] private bool canContinueToNextLine = false;

    private Coroutine displayLineCoroutine;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUR_TAG = "layout";

    public DialogueVariables dialogueVariables;
    private InkExternalFunctions inkExternalFunctions;

    public UnityAction ExitDialogueAction;

    private void Awake() {

        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Destroyed DialogueManager instance: " + this.GetInstanceID());
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
        inkExternalFunctions = new InkExternalFunctions();
    }

    private void Start() {

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(Button choice in choices){
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update() {
        if(Keyboard.current.numpad2Key.wasPressedThisFrame){
            // foreach (var item in dialogueVariables.variables)
            // {
            //     Debug.Log("Key: " + item.Key.ToString() + " value: " + item.Value.ToString());
            // }
        }

        if(!dialogueIsPlaying){
            return;
        }
    
        if(canContinueToNextLine && currentStory.currentChoices.Count == 0 && PlayerInputControls.Instance.GetDialogueSubmit()){
            //Debug.Log("ContinueStory");
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, Animator emoteAnimator = null){
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        PlayerInputControls.Instance.ChangeMapDialogue();

        dialogueVariables.StartListening(currentStory);
        inkExternalFunctions.Bind(currentStory, emoteAnimator);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueVariables.StopListening(currentStory);
        inkExternalFunctions.Unbind(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        PlayerInputControls.Instance.ChangeMapPlayer();
        ExitDialogueAction?.Invoke();
    }

    public void PauseDialogueMode()
    {
        dialogueIsPausing = true;
    }

    public void ResumeDialogueMode()
    {
        dialogueIsPausing = false;
        dialoguePanel.SetActive(true);
        PlayerInputControls.Instance.ChangeMapDialogue();
        ContinueStory();
    }

    private void ContinueStory(){
        if (currentStory.canContinue)
        {
            if (dialogueIsPausing)
            {
                dialoguePanel.SetActive(false);
                PlayerInputControls.Instance.ChangeMapPlayer();
                return;
            }

            if(displayLineCoroutine != null){
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = currentStory.Continue();

            if(nextLine.Equals("") && !currentStory.canContinue){
                ExitDialogueMode();
            }
            else{
                HandleTags(currentStory.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line){
        line = line.Replace("\\n", "\n");
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        foreach (char letter in line.ToCharArray())
        {
            if (PlayerInputControls.Instance.GetDialogueSubmit())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            if(letter == '<' || isAddingRichTextTag){
                isAddingRichTextTag = true;

                if(letter == '>'){
                    isAddingRichTextTag = false;
                    dialogueText.maxVisibleCharacters++;
                }
            }
            else{
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        
        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach(Button choiceButton in choices){
            choiceButton.gameObject.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach(string tag in currentTags){
            string[] splirtTag = tag.Split(':');
            if(splirtTag.Length != 2){
                Debug.LogWarning("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splirtTag[0].Trim();
            string tagValue = splirtTag[1].Trim();

            switch (tagKey){
                case SPEAKER_TAG:
                    break;

                case PORTRAIT_TAG:
                    break;

                case LAYOUR_TAG:
                    break;

                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;

            }
        }
    }

    private void DisplayChoices(){
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length){
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " 
                + currentChoices.Count);
        }

        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            choices[i].gameObject.SetActive(true);
            choices[i].onClick.RemoveAllListeners();
            choices[i].onClick.AddListener(() => { MakeChoice(index); });

            if(currentChoices.Count >= i + 1){
                choicesText[i].text = currentChoices[i].text;
            }else{
                choices[i].gameObject.SetActive(false);
            }
        }

        choices[0].Select();
    }

    public void MakeChoice(int choiceIndex){
        if(canContinueToNextLine){
            currentStory.ChooseChoiceIndex(choiceIndex);
            PlayerInputControls.Instance.RegisterSubmitPressed();
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName){
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if(variableValue == null){
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

    public void SetVariableState(string variableName, object variableValue){
        dialogueVariables.SetVariableState(variableName, variableValue);
    }

    public bool IsDialogOpened(){
        return dialoguePanel.activeSelf;
    }

    public void LoadData(GameData data)
    {
        //Debug.Log(this.gameObject.name + ": " + this.GetInstanceID());
        //Debug.Log("DialogueManager LoadData");
        dialogueVariables.LoadFromJson(data.stroy);
    }

    public void SaveData(GameData data)
    {
        //Debug.Log("DialogueManager SaveData");
        data.stroy = dialogueVariables.StroyToJson();
        //Debug.Log(data.stroy);
        //dialogueVariables.SaveVariables();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        DataPersistenceManager.Instance.RemoveDestroyedIDataPersistence(this);
    }
}
