using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    public ActiveSkill actSkill;
    public PassiveSkill passSkill;

    public string buffName;
    public string type;
    public float duration;
    public float currentTime;
    public float[] applyVals;
    public float val;

    public bool donPlayEffect;

    public Image icon;
    public GameObject[] buffEffects;

    Animator anim;
    PlayerStateManager playerStateManager;

    WaitForSeconds seconds = new WaitForSeconds(0.1f);

    private void Awake()
    {
        icon = GetComponent<Image>();
        anim = GetComponent<Animator>();
        playerStateManager = GameObject.Find("Player").GetComponent<PlayerStateManager>();
    }

    public void Init(string buffName, string type, float du, GameObject[] effects, float var = 0)
    {
        this.buffName = buffName;
        this.type = type;
        duration = du;
        buffEffects = effects;
        currentTime = duration;
        this.val = var;

        if (type == "Active")
        {
            actSkill = GameManager.Instance.EquipActiveList.Find(x => x.name == buffName);
        }
        else
        {
            passSkill = GameManager.Instance.EquipPassiveList.Find(x => x.name == buffName);
        }

        Execute();
    }

    public void Execute()
    {
        //버프가 중복되면 해제 후 다시 활성화
        if (GameManager.Instance.Buffs_Playing.Find(x => x.buffName == buffName) != null)
        {
            Buff buff = GameManager.Instance.Buffs_Playing.Find(x => x.buffName == buffName);
            GameManager.Instance.Buffs_Playing.Remove(buff);
            playerStateManager.BuffOff(buff);
            Destroy(buff.gameObject);
        }

        GameManager.Instance.Buffs_Playing.Add(this);
        playerStateManager.BuffOn(this, val);
        StartCoroutine(Activation());
    }

    IEnumerator Activation()
    {
        float time = 0;
        while (currentTime > 0)
        {
            time = time + Time.deltaTime;
            currentTime -= 0.1f;
            if(currentTime / duration < 0.5)
            {
                anim.Play("BuffFlicker");
                anim.speed = 1 - currentTime / duration * 2;
                //print(anim.speed);
            }
            yield return seconds;
        }
        //icon.fillAmount = 0;
        currentTime = 0;
        DeActivation();
    }

    public void DeActivation()
    {
        GameManager.Instance.Buffs_Playing.Remove(this);
        playerStateManager.BuffOff(this);
        Destroy(gameObject);
    }
}
