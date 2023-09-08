using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTab : MonoBehaviour
{
    public GameObject selectTabFirst;

    public GameObject diffcultyFirst;
    public GameObject diffcultyClose;

    public GameObject statTabFirst;
    public GameObject statTabClose;

    public GameObject skillTabFirst;
    public GameObject skillTabClose;

    public GameObject armorEquipFirst;
    public GameObject armorEquipClose;

    public GameObject shopFirst;
    public GameObject shopClose;


    private void Awake() {
    }

    private void Start() {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.SelectTab))
            gameObject.SetActive(false);
    }
}
