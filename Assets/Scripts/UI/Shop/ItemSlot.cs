using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public bool isUnlock;
    public bool isBuy;

    public RectTransform slotRect;
    Image itemSlotBG;
    public Image itemImage;
    public Image itemImageBG;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI explainText;
    public TextMeshProUGUI costText;
    public Button btn;

    public GameManager.ShopItemType itemType;

    public ActiveSkill actSkill;
    public PassiveSkill passSkill;
    public ShopItem curItem;
    RectTransform itemSlotRect;
    ShopItemList shopItemList;

    public UnityEvent<RectTransform> onSelectEvent;

    private void Awake()
    {
        itemSlotRect = transform.GetChild(0).GetComponent<RectTransform>();
        itemSlotBG = GetComponent<Image>();
        shopItemList = transform.GetComponentInParent<ShopItemList>();
        slotRect = GetComponent<RectTransform>();
    }

    public void EnableItem(bool enable)
    {
        if (enable)
        {
            //btn.interactable = true;
            nameText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 1f);
            explainText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 1f);
            costText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 1f);
            itemImage.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 1f);
            //nameText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 1f);
        }
        else if (!enable)
        {
            //btn.interactable = false;
            nameText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
            explainText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
            costText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
            itemImage.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
            //nameText.color = new Vector4(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
        }


    }

    void GetTabType(GameManager.ShopItemType type)
    {
        itemType = type;
        print(itemType.ToString());
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectEvent.Invoke(slotRect);

        if (!isUnlock)
            return;

        switch (shopItemList.shopItemType)
        {
            case GameManager.ShopItemType.Active:
                shopItemList.itemDetailType[0].SetActive(true);
                shopItemList.itemDetailType[1].SetActive(false);
                shopItemList.itemDetailType[2].SetActive(false);

                shopItemList.itemDetailType[0].GetComponent<UI_ItemDetail>().FillItemData(actSkill, null, curItem, itemImage.sprite, nameText.text, shopItemList.shopItemType);
                break;

            case GameManager.ShopItemType.Passive:
                shopItemList.itemDetailType[0].SetActive(false);
                shopItemList.itemDetailType[1].SetActive(true);
                shopItemList.itemDetailType[2].SetActive(false);

                shopItemList.itemDetailType[1].GetComponent<UI_ItemDetail>().FillItemData(null, passSkill, curItem, itemImage.sprite, nameText.text, shopItemList.shopItemType);
                break;

            case GameManager.ShopItemType.Item:
                shopItemList.itemDetailType[0].SetActive(false);
                shopItemList.itemDetailType[1].SetActive(false);
                shopItemList.itemDetailType[2].SetActive(true);

                shopItemList.itemDetailType[2].GetComponent<UI_ItemDetail>().FillItemData(null, null, curItem, itemImage.sprite, nameText.text, shopItemList.shopItemType);
                break;

            default:
                shopItemList.itemDetailType[0].SetActive(false);
                shopItemList.itemDetailType[1].SetActive(false);
                shopItemList.itemDetailType[2].SetActive(false);
                break;
        }
    }
}

