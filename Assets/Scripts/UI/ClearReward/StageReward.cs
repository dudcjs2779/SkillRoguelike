using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageReward : MonoBehaviour
{
    enum CardType {HP, Stamina, MP, EXP, Bottle};

    StageRewardCard[] clearRewardCards;
    Player player;

    private void Awake()
    {
        clearRewardCards = GetComponentsInChildren<StageRewardCard>(true);
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void OnEnable()
    {
        SettingCards();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(clearRewardCards[0].gameObject);
    }

    void SettingCards()
    {
        List<System.Enum> cardList = new List<System.Enum>(){CardType .HP, CardType.Stamina, CardType .MP, CardType .EXP, CardType.Bottle};
        foreach (var item in cardList)
        {
            Debug.Log(item.ToString());
        }
        int index = 0;
        while(cardList.Count > 2){
            int ranPick = Random.Range(0, 100);

            if (cardList.Contains(CardType.HP) && ranPick < 27)
            {
                clearRewardCards[index].type = StageRewardCard.cardType.MaxHp;
                clearRewardCards[index].image.sprite = Resources.Load<Sprite>("Sprite/RewardCard/Heart_Red");
                clearRewardCards[index].text.text = "최대 HP";
                clearRewardCards[index].player = player;
                cardList.Remove(CardType.HP);
                index++;
            }
            else if (cardList.Contains(CardType.Stamina) && ranPick < 54)
            {
                clearRewardCards[index].type = StageRewardCard.cardType.MaxStamina;
                clearRewardCards[index].image.sprite = Resources.Load<Sprite>("Sprite/RewardCard/Heart_Yellow");
                clearRewardCards[index].text.text = "최대 스테미너";
                clearRewardCards[index].player = player;
                cardList.Remove(CardType.Stamina);
                index++;
            }
            else if (cardList.Contains(CardType.MP) && ranPick < 81)
            {
                clearRewardCards[index].type = StageRewardCard.cardType.MaxMP;
                clearRewardCards[index].image.sprite = Resources.Load<Sprite>("Sprite/RewardCard/Heart_Blue");
                clearRewardCards[index].text.text = "최대 MP";
                clearRewardCards[index].player = player;
                cardList.Remove(CardType.MP);
                index++;
            }
            else if (cardList.Contains(CardType.EXP) && ranPick < 95)
            {
                clearRewardCards[index].type = StageRewardCard.cardType.EXP;
                clearRewardCards[index].image.sprite = Resources.Load<Sprite>("Sprite/RewardCard/EXP");
                clearRewardCards[index].text.text = "경험치";
                clearRewardCards[index].player = player;
                cardList.Remove(CardType.EXP);
                index++;
            }
            else if (cardList.Contains(CardType.Bottle) && ranPick < 100)
            {
                clearRewardCards[index].type = StageRewardCard.cardType.Bottle;
                clearRewardCards[index].image.sprite = Resources.Load<Sprite>("Sprite/RewardCard/Bottle");
                clearRewardCards[index].text.text = "물병 +1";
                clearRewardCards[index].player = player;
                cardList.Remove(CardType.Bottle);
                index++;
            }
        }

        // 보스 카드 초기화
        foreach (var clearRewardCard in clearRewardCards)
        {
            clearRewardCard.isBossCard = false;
            clearRewardCard.bossIcon.gameObject.SetActive(false);
        }

        // 보스 카드 섞기
        if (GameManager.Instance.stageLevel == 12)
        {
            foreach (var clearRewardCard in clearRewardCards)
            {
                clearRewardCard.isBossCard = true;
                clearRewardCard.bossIcon.gameObject.SetActive(true);
            }
        }
        else if (GameManager.Instance.stageLevel > 7)
        {
            int ranNum = Random.Range(0, 3);

            clearRewardCards[ranNum].isBossCard = true;
            clearRewardCards[ranNum].bossIcon.gameObject.SetActive(true);
        }
    }

}
