using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DropdownItem : MonoBehaviour, IPointerClickHandler, ISelectHandler
{
    public TextMeshProUGUI optionText;
    RectTransform rect;

    DropdownView dropdownView;

    public Action<int> onClick_Item_Callback;
    public UnityEvent<RectTransform> onSelectEvent;

    private void Awake() {
        dropdownView = GetComponentInParent<DropdownView>();
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {

    }

    public void Init_Item(int index, string text, Action<int> action){
        optionText.text = text;
        onClick_Item_Callback = action;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => {onClick_Item_Callback?.Invoke(index);});
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("DropdownItem select");
        onSelectEvent?.Invoke(rect);
    }
}
