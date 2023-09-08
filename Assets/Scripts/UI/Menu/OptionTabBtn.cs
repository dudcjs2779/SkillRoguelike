using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class OptionTabBtn : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI text;
    OptionMenu optionMenu;

    private void Awake() {
        text = GetComponentInChildren<TextMeshProUGUI>();
        optionMenu = GetComponentInParent<OptionMenu>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() {
        // Color color;
        // ColorUtility.TryParseHtmlString("#A8A8A8", out color);
        // text.DOColor(color, 0.1f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("OnSelected");
        // text.DOColor(Color.white, 0.1f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("OnDeselected");

        //Debug.Log("Deselect");
        // Color color;
        // ColorUtility.TryParseHtmlString("#A8A8A8", out color);
        // text.DOColor(color, 0.1f);
    }
}
