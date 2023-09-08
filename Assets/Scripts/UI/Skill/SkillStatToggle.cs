using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class SkillStatToggle : MonoBehaviour
{
    [SerializeField] RectTransform toggleSwtich;
    [SerializeField] Transform skillSet, statWindow, skillWindow;
    bool isStatTab;

    public Color statColor, skillColor;


    public Ease ease;


    void Start()
    {
        isStatTab = true;
        //skillSet.SetParent(statWindow);
        //skillSet.SetSiblingIndex(2);
        //skillSet.GetComponent<RectTransform>().anchoredPosition = new Vector3(700, 20, 0);
        //skillSet.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 700);
    }

    void Update()
    {
        
    }

    public void btnSkillStatToggle()
    {
        isStatTab = !isStatTab;

        if (!isStatTab)
        {
            toggleSwtich.DOAnchorPos(new Vector2(40, toggleSwtich.anchoredPosition.y), 0.3f).SetEase(Ease.InOutBack);
            toggleSwtich.GetComponent<RawImage>().DOColor(new Color(0, 1, 1), 0.3f);
            skillWindow.parent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBack);
            //skillSet.SetParent(skillWindow);
            //skillSet.GetComponent<RectTransform>().anchoredPosition = new Vector3(150, -30, 0);
            //skillSet.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 800);

        }
        else if (isStatTab)
        {
            toggleSwtich.DOAnchorPos(new Vector2(-40, toggleSwtich.anchoredPosition.y), 0.3f).SetEase(Ease.InOutBack);
            toggleSwtich.GetComponent<RawImage>().DOColor(new Color(99 / 255, 255 / 255, 85 / 255), 0.3f);
            skillWindow.parent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-2000, 0), 0.3f).SetEase(Ease.OutBack);
            //skillSet.SetParent(statWindow);
            //skillSet.SetSiblingIndex(2);
            //skillSet.GetComponent<RectTransform>().anchoredPosition = new Vector3(700, 20, 0);
            //skillSet.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 700);

        }
    }
}
