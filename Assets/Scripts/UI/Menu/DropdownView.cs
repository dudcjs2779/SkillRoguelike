using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DropdownView : MonoBehaviour
{
    [SerializeField] GameObject prefab_Item;
    public RectTransform viewportRect;
    public RectTransform contentRect;

    public DropdownItem[] dropdownItems;
    public Button[] itemBtns;


    private void Start() {

    }

    public void Init_DropdownView(List<string> list, Action<int> action)
    {
        dropdownItems = new DropdownItem[list.Count];
        itemBtns = new Button[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            GameObject gObj = Instantiate(prefab_Item, contentRect.transform);
            dropdownItems[i] = gObj.GetComponent<DropdownItem>();
            dropdownItems[i].Init_Item(i, list[i], action);
            itemBtns[i] = dropdownItems[i].GetComponent<Button>();

            dropdownItems[i].onSelectEvent.AddListener(HandleEventItemOnSelect);
        }

        for (int i = 0; i < dropdownItems.Length; i++)
        {
            Navigation newNavi = new Navigation();
            newNavi.mode = Navigation.Mode.Explicit;
            newNavi.selectOnDown = i < dropdownItems.Length - 1 ? itemBtns[i + 1] : null;
            newNavi.selectOnUp = i > 0 ? itemBtns[i - 1] : null;

            itemBtns[i].navigation = newNavi;
        }
    }

    public void HandleEventItemOnSelect(RectTransform rect){
        Debug.Log(rect.GetComponent<DropdownItem>().optionText.text);
        AutoScroll.MoveViewport(rect, viewportRect, contentRect);
    }

    private void OnDisable() {
        foreach (var item in dropdownItems)
        {
            Destroy(item.gameObject);
        }
    }
}
