using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyStateBar : MonoBehaviour
{
    [SerializeField] GameObject enemyHpBar;
    [SerializeField] GameObject enemySTGBar;
    public Slider bossHealthBar;
    public Slider bossSTGBar;

    private void Awake()
    {
        
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void SpawnEnemyBar(Enemy enemy)
    {
        GameObject instantEnemyHpBar = Instantiate(enemyHpBar, transform.position, Quaternion.identity, transform);
        instantEnemyHpBar.GetComponent<EnemyBar>().enemy = enemy;

        GameObject instantEnemySTGBar = Instantiate(enemySTGBar, transform.position, Quaternion.identity, transform);
        instantEnemySTGBar.GetComponent<EnemyBar>().enemy = enemy;
    }

    //보스 체력 UI
    public void BossBar(BossGolem bossGolem)
    {
        if (!bossHealthBar.gameObject.activeSelf)
            bossHealthBar.gameObject.SetActive(true);
        else if (bossHealthBar.gameObject.activeSelf)
            bossHealthBar.value = Mathf.Lerp(bossHealthBar.value, bossGolem.curHP / bossGolem.maxHP, Time.deltaTime * 10f);

        if (!bossSTGBar.gameObject.activeSelf)
            bossSTGBar.gameObject.SetActive(true);
        else if (bossSTGBar.gameObject.activeSelf)
            bossSTGBar.value = Mathf.Lerp(bossSTGBar.value, bossGolem.curSHP / bossGolem.maxSHP, Time.deltaTime * 10f);
    }
}
