using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScrollView : MonoBehaviour
{
    [SerializeField] RectTransform viewportRect;
    [SerializeField] RectTransform contentRect;

    [SerializeField] ItemSlot[] itemSlots;


    private void Awake()
    {
        itemSlots = GetComponentsInChildren<ItemSlot>(true);
    }

    void Start()
    {
        foreach (var item in itemSlots)
        {
            item.onSelectEvent.AddListener(HandleEventItemOnSelect);
        }
    }

    void Update()
    {

    }

    private void HandleEventItemOnSelect(RectTransform rect)
    {
        //Debug.Log("HandleEventItemOnSelect");
        AutoScroll.MoveViewport(rect, viewportRect, contentRect);
    }
}
