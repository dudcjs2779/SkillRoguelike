using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ScrollViewItem : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public RectTransform rowRect;

    public UnityEvent<RectTransform> onSelectEvent;

    private void Awake() {
        rowRect = GetComponentInParent<ScrollViewRow>().GetComponent<RectTransform>();
    }

    private void Start() {
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("OnSelect");
        onSelectEvent?.Invoke(rowRect);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("OnDeselect");
    }
}
