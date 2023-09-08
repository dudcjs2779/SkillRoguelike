using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmorEquipSlot : MonoBehaviour
{
    public Image itemImage;
    public Image itemImageBG;
    public Image itemEquipedImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI detailText;

    ArmorEquipList armorEquipList;

    // Start is called before the first frame update

    private void Awake()
    {
        armorEquipList = GetComponentInParent<ArmorEquipList>();
    }

    void Start()
    {
        
    }
}
