using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SelectTabBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] Image selectTabImg;
    [SerializeField] TextMeshProUGUI selectTabText;

    private void Start()
    {
        selectTabText.outlineWidth = 0.3f;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Debug.Log("selectTabEnter");
        selectTabImg.DOFade(1f, 1f);
        selectTabText.DOFade(1f, 1f);
        SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SlotSelect01);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Debug.Log("selectTabExit");
        selectTabImg.DOFade(0.1f, 1f);
        selectTabText.DOFade(0.5f, 1f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("selectTabEnter");
        selectTabImg.DOFade(1f, 1f);
        selectTabText.DOFade(1f, 1f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("selectTabExit");
        selectTabImg.DOFade(0.1f, 1f);
        selectTabText.DOFade(0.5f, 1f);
    }
}
