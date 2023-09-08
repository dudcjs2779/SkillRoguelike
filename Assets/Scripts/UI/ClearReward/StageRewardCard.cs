using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StageRewardCard : MonoBehaviour
{
    public enum cardType { MaxHp, MaxStamina, MaxMP, EXP, Bottle };
    public cardType type;
    public Image image;
    public Image bossIcon;
    public TextMeshProUGUI text;

    Button btn;
    Animator anim;

    public bool isBossCard;

    public Player player;
    PlayerState playerState;

    private void Awake()
    {
        playerState = GameObject.Find("Canvas0").GetComponentInChildren<PlayerState>();
        anim = GetComponent<Animator>();
        btn = GetComponent<Button>();
    }

    private void Start() {
        btn.onClick.AddListener(ApplyCard);
    }

    private void OnEnable()
    {

    }

    public void ApplyCard()
    {
        switch (type)
        {
            case cardType.MaxHp:
                player.maxHealth += 8;
                player.curHealth += 8;
                playerState.RefreshBarWidth();
                break;

            case cardType.MaxStamina:
                player.maxStamina += 8;
                player.curStamina += 8;
                playerState.RefreshBarWidth();
                break;

            case cardType.MaxMP:
                player.maxMP += 8;
                player.curMP += 8;
                playerState.RefreshBarWidth();
                break;

            case cardType.EXP:
                player.curEXP += 300;
                break;

            case cardType.Bottle:
                player.bottleCount++;
                Canvas0.Instance.playerState.bottleCountText.text = player.bottleCount.ToString();
                break;
        }

        if (isBossCard) GameManager.Instance.goBossStage = true;
        Canvas5.Instance.CloseReward();
    }


}
