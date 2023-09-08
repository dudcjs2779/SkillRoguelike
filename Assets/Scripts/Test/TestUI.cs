using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        print("Enter");

        
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        print("Exit");
        
    }

}
