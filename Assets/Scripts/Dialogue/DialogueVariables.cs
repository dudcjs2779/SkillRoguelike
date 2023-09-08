using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables {get; private set;}

    private Story globalVariablesStory;

    private const string saveVariablesKey = "INK_VARIABLES";

    // 전역 변수가 저장된 globals.ink 파일로부터 변수들을 가저와 variables에 저장
    public DialogueVariables(TextAsset loadGlobalsJSON){
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        //LOAD DATA
        // if(PlayerPrefs.HasKey(saveVariablesKey)){
        //     string jsonState = PlayerPrefs.GetString(saveVariablesKey);
        //     globalVariablesStory.state.LoadJson(jsonState);
        // }

        // variables = new Dictionary<string, Ink.Runtime.Object>();
        // foreach (string name in globalVariablesStory.variablesState)
        // {
        //     Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
        //     variables.Add(name, value);
        //     Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        // }
    }

    public void SaveVariables(){
        if (globalVariablesStory != null){
            VariablesToStory(globalVariablesStory);
            PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
        }
    }

    public string StroyToJson(){
        string jsonData = "";
        if (globalVariablesStory != null)
        {
            VariablesToStory(globalVariablesStory);
            //PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
            jsonData = globalVariablesStory.state.ToJson();
            //Debug.Log(jsonData);
        }

        return jsonData;
    }

    public void LoadFromJson(string jsonData){
        //Debug.Log(jsonData);
        if(jsonData != ""){
            globalVariablesStory.state.LoadJson(jsonData);
        }

        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            //Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public void StartListening(Story story){
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story){
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value){
        if(variables.ContainsKey(name)){
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    // globals 파일에서 가져온 변수들 variables을 해당 스토리의 전역 변수로 선언
    private void VariablesToStory(Story story){
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables){
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

    public void SetVariableState(string variableName, object variableValue){
        if(variables.ContainsKey(variableName)){
            Ink.Runtime.Object inkObject = null;
            variables.TryGetValue(variableName, out inkObject);

            if(inkObject.GetType() == typeof(StringValue)){
                ((Ink.Runtime.StringValue)inkObject).value = variableValue.ToString();
                globalVariablesStory.variablesState[variableName] = ((Ink.Runtime.StringValue)inkObject).value;
            }
            else if(inkObject.GetType() == typeof(IntValue)){
                ((Ink.Runtime.IntValue)inkObject).value = (int)variableValue;
                globalVariablesStory.variablesState[variableName] = ((Ink.Runtime.IntValue)inkObject).value;
            }
        }
    }

    public void DebugSetVariable(){
        //Debug.Log(globalVariablesStory.variablesState[variableName]);

        foreach (var item in globalVariablesStory.variablesState)
        {
            Debug.Log(item + ": " + globalVariablesStory.variablesState.GetVariableWithName(item));
        }

        foreach (var item in variables)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }

    }
}
