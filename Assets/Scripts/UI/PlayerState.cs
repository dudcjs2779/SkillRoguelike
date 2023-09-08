using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider staminaSlider;
    [SerializeField] Slider mpSlider;
    [SerializeField] Slider expSlider;
    public TextMeshProUGUI playerLvText;
    public TextMeshProUGUI bottleCountText;

    [SerializeField] private TextMeshProUGUI init_PlayerLvText;
    [SerializeField] private TextMeshProUGUI init_BottleCountText;

    public RectTransform lockOnImg;
    public Transform lockOnImgLoc;
    public float lockOnImgYOffset;
    Camera gameCamera;

    public bool isObjDetected;

    Player player;
    [SerializeField] EnemyStateBar enemyStateBar;


    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        gameCamera = Camera.main;
        playerLvText = init_PlayerLvText;
        bottleCountText = init_BottleCountText;
    }

    void Start()
    {
        hpSlider.value = player.curHealth / player.maxHealth;
        staminaSlider.value = player.curStamina / player.maxStamina;
        mpSlider.value = player.curMP / player.maxMP;
    }

    void Update()
    {
        HealthBar();
        StaminaBar();
        MpBar();
        EXPBar();
        LockOn_Keep();
    }

    void HealthBar()
    {
        hpSlider.value = Mathf.Lerp(hpSlider.value, player.curHealth / player.maxHealth, Time.deltaTime * 10f);
    }

    void StaminaBar()
    {
        staminaSlider.value = Mathf.Lerp(staminaSlider.value, player.curStamina / player.maxStamina, Time.deltaTime * 15f);
    }

    void MpBar()
    {
        mpSlider.value = Mathf.Lerp(mpSlider.value, player.curMP / player.maxMP, Time.deltaTime * 10f);
    }

    void EXPBar()
    {
        expSlider.value = Mathf.Lerp(expSlider.value, player.curEXP / player.maxEXP, Time.deltaTime * 10f);
    }

    public void RefreshBarWidth()
    {
        RectTransform hpBarRect = hpSlider.GetComponent<RectTransform>();
        hpBarRect.sizeDelta = new Vector2(player.maxHealth * 5, hpBarRect.rect.height);

        RectTransform staminaBarRect = staminaSlider.GetComponent<RectTransform>();
        staminaBarRect.sizeDelta = new Vector2(player.maxStamina * 5, staminaBarRect.rect.height);

        RectTransform mpBarRect = mpSlider.GetComponent<RectTransform>();
        mpBarRect.sizeDelta = new Vector2(player.maxMP * 5, mpBarRect.rect.height);

    }

    public void LockOn(Transform t, float y)
    {
        lockOnImg.gameObject.SetActive(true);
        lockOnImgLoc = t;
        lockOnImgYOffset = y;
    }

    void LockOn_Keep()
    {
        if (player.enemyLocked)
        {
            lockOnImg.position = gameCamera.WorldToScreenPoint(lockOnImgLoc.position + new Vector3(0, lockOnImgYOffset, 0));
            lockOnImg.rotation = Quaternion.Euler(0, lockOnImg.eulerAngles.y + Time.deltaTime * 100, 0);
            lockOnImg.localScale = Vector3.one * (10 / (gameCamera.transform.position - player.currentTarget.position).magnitude);
        }
        else
        {
            if(lockOnImg.gameObject.activeSelf)
                lockOnImg.gameObject.SetActive(false);
        }
    }
}
