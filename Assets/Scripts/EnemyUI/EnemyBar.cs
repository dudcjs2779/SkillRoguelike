using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBar : MonoBehaviour
{
    enum enemyBarType { HP, STG };
    [SerializeField] enemyBarType barType;
    public Enemy enemy;
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (barType)
        {
            case enemyBarType.HP:
                slider.value = enemy.curHP / enemy.maxHP;
                break;
            case enemyBarType.STG:
                slider.value = enemy.curSHP / enemy.maxSHP;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            switch (barType)
            {
                case enemyBarType.HP:
                    transform.position = Camera.main.WorldToScreenPoint(enemy.T_HP_Bar.position);
                    slider.value = Mathf.Lerp(slider.value, enemy.curHP / enemy.maxHP, Time.deltaTime * 10f);
                    break;

                case enemyBarType.STG:
                    transform.position = Camera.main.WorldToScreenPoint(enemy.T_HP_Bar.position) + Vector3.down * 9;
                    slider.value = Mathf.Lerp(slider.value, enemy.curSHP / enemy.maxSHP, Time.deltaTime * 10f);
                    break;
            }

            if (enemy.isDead)
            {
                Destroy(gameObject, 1);
            }
        }
        catch
        {
            Destroy(gameObject);
        }

    }
}
