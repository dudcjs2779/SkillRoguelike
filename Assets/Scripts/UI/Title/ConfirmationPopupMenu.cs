using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationPopupMenu : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancleButton;

    public void ActivateMenu(string displayText, Action confirmAction, Action cancleAction){
        Canvas5.Instance.UISequenceList.Add(Canvas5.UIType.ConfirmMenu);
        Canvas5.Instance.EscapeActionList.Add(cancleAction);
        confirmButton.onClick.RemoveAllListeners();
        cancleButton.onClick.RemoveAllListeners();

        gameObject.SetActive(true);
        this.displayText.text = displayText;

        confirmButton.onClick.AddListener(() => {
            DeactivateMenu();
            confirmAction();
        });

        cancleButton.onClick.AddListener(() => {
            DeactivateMenu();
            cancleAction();
        });
    }

    public void DeactivateMenu(){
        gameObject.SetActive(false);
    }

    public void SelectConfirm(){
        confirmButton.Select();
    }

    public void SelectCancle()
    {
        cancleButton.Select();
    }

    public void Escape_ConfirmMenu()
    {
        Canvas5.Instance.UISequenceList.Remove(Canvas5.UIType.ConfirmMenu);
        Canvas5.Instance.EscapeActionList.Remove(Canvas5.Instance.EscapeActionList[Canvas5.Instance.EscapeActionList.Count - 1]);
        DeactivateMenu();
    }

}
