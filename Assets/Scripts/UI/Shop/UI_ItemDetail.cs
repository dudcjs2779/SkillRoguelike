using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemDetail : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI nameText, explainText, costText, quantityText;
    [SerializeField] TextMeshProUGUI[] basicValTitle, basicVal, statRankVal, skillLvVal;

    public void FillItemData(ActiveSkill actSkill, PassiveSkill passSkill, ShopItem item, Sprite iconImage, string name, GameManager.ShopItemType type)
    {
        itemImage.sprite = iconImage;

        switch (type)
        {
            case GameManager.ShopItemType.Active:
                nameText.text = actSkill.krName;
                explainText.text = actSkill.explain;
                costText.text = actSkill.cost.ToString();

                string[] stats = actSkill.statRank;

                if (actSkill.staminaPoint != 0)
                {
                    basicVal[0].transform.parent.gameObject.SetActive(true);
                    basicVal[0].text = actSkill.staminaPoint.ToString();
                }
                else basicVal[0].transform.parent.gameObject.SetActive(false);

                if (actSkill.mpPoint != 0)
                {
                    basicVal[1].transform.parent.gameObject.SetActive(true);
                    basicVal[1].text = actSkill.mpPoint.ToString();
                }
                else basicVal[1].transform.parent.gameObject.SetActive(false);

                if (actSkill.damage.Length != 0)
                {
                    if (actSkill.isBuff)
                    {
                        basicValTitle[2].text = "효력";
                        basicVal[2].text = string.Format("{0}%", actSkill.damage[0] * 100);
                    }
                    else
                    {
                        basicValTitle[2].text = "데미지";
                        basicVal[2].text = (actSkill.damage[0] * 20).ToString();
                    }
                }
                else basicVal[2].transform.parent.gameObject.SetActive(false);

                if (actSkill.stgDamage.Length != 0)
                {
                    basicVal[3].transform.parent.gameObject.SetActive(true);
                    basicVal[3].text = (actSkill.stgDamage[0] * 30).ToString();
                }
                else basicVal[3].transform.parent.gameObject.SetActive(false);

                if (actSkill.stgPower.Length != 0)
                {
                    basicVal[4].transform.parent.gameObject.SetActive(true);
                    basicVal[4].text = actSkill.stgPower[0].ToString();
                }
                else basicVal[4].transform.parent.gameObject.SetActive(false);

                if (actSkill.elementalVal.Length != 0)
                {
                    basicVal[5].transform.parent.gameObject.SetActive(true);
                    basicVal[5].text = actSkill.elementalVal[0].ToString();
                }
                else basicVal[5].transform.parent.gameObject.SetActive(false);

                if (actSkill.duration.Length != 0)
                {
                    basicVal[6].transform.parent.gameObject.SetActive(true);
                    basicVal[6].text = actSkill.duration[0].ToString();
                }
                else basicVal[6].transform.parent.gameObject.SetActive(false);


                for (int i = 0; i < stats.Length; i++)
                {
                    statRankVal[i].text = stats[i];
                }

                skillLvVal[0].text = actSkill.skillLvEx[0];
                break;

            case GameManager.ShopItemType.Passive:
                nameText.text = passSkill.krName;
                explainText.text = passSkill.explain;
                costText.text = passSkill.cost.ToString();

                skillLvVal[0].text = passSkill.skillLvEx[0];
                skillLvVal[1].text = passSkill.skillLvEx[1];
                skillLvVal[2].text = passSkill.skillLvEx[2];
                break;

            case GameManager.ShopItemType.Item:
                nameText.text = item.krName;
                explainText.text = item.explain;
                costText.text = item.cost.ToString();
                break;

            default:
                break;
        }

        GameManager.Instance.StartCoroutine(GameManager.Instance.RefreshUI(this.GetComponent<ContentSizeFitter>()));
    }
}
