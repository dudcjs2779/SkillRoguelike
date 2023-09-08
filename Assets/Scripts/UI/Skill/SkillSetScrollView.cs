using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSetScrollView : MonoBehaviour
{
    [SerializeField] RectTransform viewportRect;
    [SerializeField] RectTransform contentRect;

    [SerializeField] SkillSlot[] skillSlots;

    private void Awake() {
        skillSlots = GetComponentsInChildren<SkillSlot>(true);
    }

    void Start()
    {
        foreach (var item in skillSlots)
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
