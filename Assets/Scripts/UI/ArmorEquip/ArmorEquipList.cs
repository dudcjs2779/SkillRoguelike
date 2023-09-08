using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ArmorEquipList : MonoBehaviour
{
    [SerializeField] RectTransform equipTabRect;
    [SerializeField] RectTransform armorEquipMenu;

    ArmorEquipSlot[] armorEquipSlots;
    Button[] armorEquipSlot_Btns;
    PlayerStateManager playerStateManager;
    Player player;

    public ArmorEquipSlot equiped_ArmorEquipSlot;

    private void Awake()
    {
        Debug.Log("ABC");
        armorEquipSlots = GetComponentsInChildren<ArmorEquipSlot>(true);
        playerStateManager = GameObject.Find("Player").GetComponent<PlayerStateManager>();
        player = playerStateManager.GetComponent<Player>();

        armorEquipSlot_Btns = new Button[armorEquipSlots.Length];
        for (int i = 0; i < armorEquipSlots.Length; i++)
        {
            armorEquipSlot_Btns[i] = armorEquipSlots[i].GetComponent<Button>();
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.parent.gameObject.SetActive(false);
        if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.EquipTab))
            equipTabRect.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Init_EquipList();
    }

    void Init_EquipList()
    {
        List<ShopItem> equipItemList = GameManager.Instance.MyShopItemList.FindAll(x => x.isEquipItem && x.isBuy);
        //List<ShopItem> equipItemList = GameManager.Instance.MyShopItemList.FindAll(x => x.isEquipItem);

        int count = equipItemList.Count;
        for (int i = 0; i < armorEquipSlots.Length; i++)
        {
            if(i < equipItemList.Count)
            {
                armorEquipSlots[i].gameObject.SetActive(true);
                armorEquipSlots[i].nameText.text = equipItemList[i].krName;
                armorEquipSlots[i].detailText.text = equipItemList[i].explainDetail;

                Sprite sprite = Resources.Load<Sprite>(equipItemList[i].iconPath);
                armorEquipSlots[i].itemImage.sprite = sprite;

                if (equipItemList[i].isEquip)
                {
                    equiped_ArmorEquipSlot = armorEquipSlots[i];
                    equiped_ArmorEquipSlot.itemEquipedImage.DOFade(0.6f, 0.1f);

                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(armorEquipSlots[i].gameObject);
                }

                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.Explicit;
                customNav.selectOnUp = i != 0 ? armorEquipSlot_Btns[i - 1] : null;
                customNav.selectOnDown = i != equipItemList.Count - 1 ? armorEquipSlot_Btns[i + 1] : null;

                armorEquipSlot_Btns[i].navigation = customNav;
            }
            else
            {
                armorEquipSlots[i].gameObject.SetActive(false);
            }
        }

        if(equiped_ArmorEquipSlot == null){
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(armorEquipSlots[0].gameObject);
        }
    }

    public void OnClick_ArmorEquipSlot(){
        Debug.Log("OnClick_ArmorEquipSlot");

        ArmorEquipSlot clickedItem = EventSystem.current.currentSelectedGameObject.GetComponent<ArmorEquipSlot>();

        // 이미 선택한 아이템이면 장착해제
        if(clickedItem == equiped_ArmorEquipSlot){
            GameManager.Instance.MyShopItemList.Find(x => x.krName == equiped_ArmorEquipSlot.nameText.text).isEquip = false;
            equiped_ArmorEquipSlot.itemEquipedImage.DOFade(0, 0.1f);
            equiped_ArmorEquipSlot = null;

            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SlotUnequip01);
        }
        else{
            if(equiped_ArmorEquipSlot != null) equiped_ArmorEquipSlot.itemEquipedImage.DOFade(0, 0.1f);

            equiped_ArmorEquipSlot = clickedItem;
            equiped_ArmorEquipSlot.itemEquipedImage.DOFade(0.6f, 0.1f);

            ShopItem prevEquipItem = GameManager.Instance.MyShopItemList.Find(x => x.isEquip);

            if (prevEquipItem != null)
            {
                prevEquipItem.isEquip = false;
            }

            ShopItem nowEquipItem = GameManager.Instance.MyShopItemList.Find(x => x.krName == equiped_ArmorEquipSlot.nameText.text);
            nowEquipItem.isEquip = true;

            SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SlotEquip01);
        }

        playerStateManager.Init_PlayerStat();
        playerStateManager.Init_EquipArmorStat();
        //GameManager.Instance.Save_ItemDataToJosn();
    }
}
