using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Instrumentation;
using System.Runtime.Remoting.Messaging;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateManager : MonoBehaviour
{
    private static PlayerStateManager instance;
    public static PlayerStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<PlayerStateManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<PlayerStateManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    public bool recoverStamina;
    public float recoverST_Delay;
    float prevStamina;

    SkinnedMeshRenderer meshRenderer;

    Player player;
    PlayerSkill playerSkill;
    PlayerEffect playerEffect;
    PlayerState playerState;
    Weapon weapon;

    [Header("BASIC")]
    public int basicVal;
    public float basicMoveSpeed;
    public float mpRecoverPoint;

    public float basicHealAmount;
    public int basicBottleCount;

    [Header("STAT")]
    public float stat_Health;
    public float stat_Stamina;
    public float stat_MeleeDam, stat_MeleeStgDam, stat_MeleeStgPower;
    public float stat_Mp;
    public float stat_MagicDam, stat_MagicStgDam, stat_MagicElement, stat_MpRecover, stat_BuffDuration;

    [Header("ADDITIONAL VAL")]
    public float extraDamage;
    public float extraStgDam, extraStgPower, extraFireVal, extraIceVal, extraLightingVal;
    public float barrierVal, damageUp;

    public float meleeDamVal, meleeStgDamVal, meleeStgPowerVal, defanceVal,
        magicDamVal, magicStgDamVal, magicStgPowerVal, elementalVal, mpRecoverVal,
        guardEnhanceVal, finishHealing, dodgePfVal, weakenVal, moveSpeedVal;
    public float healAmountVal;

    [Header("SHOP VAL")]
    public float shopHealAmount;
    public int shopHealCount;

    public bool isPowerShield;
    public bool isParry;
    public bool isPerfectParry;
    public bool isSuperArmor;

    private void Awake()
    {
        player = transform.GetComponent<Player>();
        playerSkill = transform.GetComponent<PlayerSkill>();
        playerEffect = GetComponent<PlayerEffect>();
        playerState = GameObject.Find("Canvas0").GetComponentInChildren<PlayerState>();
        weapon = GetComponentInChildren<Weapon>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        basicVal = 100;
        basicMoveSpeed = 5.5f;
        extraDamage = 0;
        barrierVal = 1;
        Init_ShopStat();
        Init_PlayerStat();
        Init_EquipArmorStat();
        Init_SkillEffectByStat();

        GameManager.Instance.Init_playerState += () =>
        {
            foreach (var skill in GameManager.Instance.EquipActiveList)
            {
                skill.skillLv = 0;
            }
            foreach (var skill in GameManager.Instance.EquipPassiveList)
            {
                skill.skillLv = 0;
            }
        };

        GameManager.Instance.Init_playerState += Init_ShopStat;
        GameManager.Instance.Init_playerState += Init_PlayerStat;
        GameManager.Instance.Init_playerState += Init_EquipArmorStat;
        GameManager.Instance.Init_playerState += Init_SkillEffectByStat;
        GameManager.Instance.Init_playerState += Canvas0.Instance.hUD_Skill.Init_HUDSkill;
    }

    void Update()
    {
        RecoverStamina();

    }

    void RecoverStamina()
    {
        if (!recoverStamina && (!playerSkill.skilling || playerSkill.aiming) && !player.isAttacking && !player.isCharging && !player.isSA && !player.isDodge)
        {
            recoverST_Delay += Time.deltaTime;
        }

        if (player.maxStamina > player.curStamina && recoverStamina && !player.isCharging && !player.isAttacking && !player.isSA && (!playerSkill.skilling || playerSkill.aiming))
        {
            player.curStamina += Time.deltaTime * player.maxStamina * 0.4f;

            if (player.curStamina > player.maxStamina)
                player.curStamina = player.maxStamina;
        }

        if (prevStamina != player.curStamina && prevStamina >= player.curStamina)
        {
            recoverStamina = false;
            recoverST_Delay = 0f;
            //print("recoverStamina : " + recoverStamina);
        }
        else if (recoverST_Delay >= 0.1f)
        {
            recoverStamina = true;
        }

        prevStamina = player.curStamina;
    }

    //�÷��̾� �⺻ ���� ����
    public void Init_PlayerStat()
    {
        //print(basicVal);
        //print(GameManager.Instance.statValData.STR_health);
        //print(upgradeSkill.healthUpVal);
        GameManager.Instance.statValData.StatUpdate(GameManager.Instance.playerData);

        stat_Health = 100 + GameManager.Instance.statValData.GetHealth();
        stat_Stamina = 100 + GameManager.Instance.statValData.AGL_stamina;
        stat_MeleeDam = 1 + GameManager.Instance.statValData.GetMeleeDam();
        stat_MeleeStgDam = 1 + GameManager.Instance.statValData.VIT_meleeStgDam;
        stat_MeleeStgPower = 1 + GameManager.Instance.statValData.VIT_meleeStgPower;
        stat_Mp = 100 + GameManager.Instance.statValData.GetMp();
        stat_MagicDam = 1 + GameManager.Instance.statValData.MP_magicDam;
        stat_MagicStgDam = 1 + GameManager.Instance.statValData.MST_magicStgDam;
        stat_MagicElement = 1 + GameManager.Instance.statValData.MST_element;
        stat_MpRecover = 1 + GameManager.Instance.statValData.INT_MpRecover;
        stat_BuffDuration = 1 + GameManager.Instance.statValData.INT_effectDuration;

        player.moveSpeed = basicMoveSpeed;

        player.maxHealth = stat_Health;
        player.maxStamina = stat_Stamina;
        player.maxMP = stat_Mp;
        player.curHealth = player.maxHealth;
        player.curStamina = player.maxStamina;
        player.curMP = player.maxMP;

        meleeDamVal = stat_MeleeDam;
        meleeStgDamVal = stat_MeleeStgDam;
        meleeStgPowerVal = stat_MeleeStgPower;
        magicDamVal = stat_MagicDam;
        magicStgDamVal = stat_MagicStgDam;
        magicStgPowerVal = 1;
        elementalVal = stat_MagicElement;
        mpRecoverVal = stat_MpRecover;
        defanceVal = 1;
        guardEnhanceVal = 1;
        dodgePfVal = 1;
        weakenVal = 1;
        moveSpeedVal = 1;
        healAmountVal = 1;
        playerState.RefreshBarWidth();
    }

    // �������� ������ ȿ��
    public void Init_ShopStat()
    {
        List<ShopItem> buyItemList = GameManager.Instance.MyShopItemList.FindAll(x => x.isBuy);
        int additionalCount = 0;
        float additionalHeal = 0;

        for (int i = 0; i < buyItemList.Count; i++)
        {
            switch (buyItemList[i].name)
            {
                case "Bottle Count Up1":
                case "Bottle Count Up2":
                case "Bottle Count Up3":
                    additionalCount++;
                    break;
                case "Bottle Volume Up1":
                case "Bottle Volume Up2":
                case "Bottle Volume Up3":
                    additionalHeal += 15;
                    break;

            }
        }
        shopHealCount = additionalCount;
        shopHealAmount = additionalHeal;

        player.bottleCount = basicBottleCount + shopHealCount;
        player.healAmount = basicHealAmount + shopHealAmount;
        Canvas0.Instance.playerState.bottleCountText.text = player.bottleCount.ToString();
    }

    // �Ƹ� ���� ȿ��
    public void Init_EquipArmorStat()
    {
        ShopItem EquipArmor = GameManager.Instance.MyShopItemList.Find(x => x.isEquip);

        if (EquipArmor == null)
        {
            return;
        }

        switch (EquipArmor.name)
        {
            case "Leather Armor":
                player.maxHealth += 10;
                player.curHealth += 10;
                player.maxStamina += 20;
                meleeDamVal += 0.15f;
                meleeStgDamVal += 0.05f;
                moveSpeedVal += 0.08f;
                player.moveSpeed = basicMoveSpeed * moveSpeedVal;
                defanceVal -= 0.05f;
                break;

            case "Iron Armor":
                player.maxHealth += 20;
                player.curHealth += 20;
                player.maxStamina += 10;
                meleeDamVal += 0.05f;
                meleeStgDamVal += 0.15f;
                meleeStgPowerVal += 0.1f;
                defanceVal -= 0.1f;
                break;

            case "Red Robe":
                player.maxMP += 20;
                player.curMP += 20;
                magicDamVal += 0.15f;
                magicStgPowerVal += 0.1f;
                elementalVal += 0.15f;
                break;
        }

        playerState.RefreshBarWidth();
    }

    // ��ų ȿ��
    public void Init_SkillEffectByStat()
    {
        foreach (var skill in GameManager.Instance.EquipActiveList)
        {
            string[] skillStat = skill.statRank;
            string[] playerStat = new string[6];
            playerStat[0] = GameManager.Instance.playerData.STR;
            playerStat[1] = GameManager.Instance.playerData.AGL;
            playerStat[2] = GameManager.Instance.playerData.VIT;
            playerStat[3] = GameManager.Instance.playerData.INT;
            playerStat[4] = GameManager.Instance.playerData.MP;
            playerStat[5] = GameManager.Instance.playerData.MST;

            float effectVal = 1;
            for (int i = 0; i < skillStat.Length; i++)
            {
                float curStatEffectVal = 0;
                switch (skillStat[i])
                {
                    case "A":
                        curStatEffectVal += 0.12f;
                        break;
                    case "B":
                        curStatEffectVal += 0.08f;
                        break;
                    case "C":
                        curStatEffectVal += 0.05f;
                        break;
                    case "D":
                        curStatEffectVal += 0.02f;
                        break;
                    case "E":
                        break;
                }

                switch (playerStat[i])
                {
                    case "A":
                        curStatEffectVal = curStatEffectVal * 1;
                        break;
                    case "B":
                        curStatEffectVal = curStatEffectVal * 0.75f;
                        break;
                    case "C":
                        curStatEffectVal = curStatEffectVal * 0.5f;
                        break;
                    case "D":
                        curStatEffectVal = curStatEffectVal * 0.25f;
                        break;
                    case "E":
                        curStatEffectVal = curStatEffectVal * 0;
                        break;
                }

                effectVal += curStatEffectVal;
            }

            effectVal = Mathf.Round(effectVal * 10000) / 10000;
            skill.effectByStat = effectVal;
        }
    }

    public void GainMpByAttack(string attackName, bool isStg)
    {
        float point = mpRecoverPoint;

        if (attackName == "A1" || attackName == "A2" || attackName == "A3" || attackName == "SA_Attack")
        {
            if (attackName == "SA_Attack")
                point = point * 2f;

            if (isStg)
                point = point * 1.3f;

            point = point * (mpRecoverVal);

            player.curMP += point;

            if (player.curMP > player.maxMP)
                player.curMP = player.maxMP;

            Debug.Log("GainMpByAttack : " + point);
        }
    }

    public void GainMpByStg()
    {
        float point = player.maxMP * 0.25f;

        point = point * (mpRecoverVal);

        player.curMP += point;

        if (player.curMP > player.maxMP)
            player.curMP = player.maxMP;

        //Debug.Log("GainMpByStg : " + point);

    }

    public void BuffOn(Buff buff, float val = 0)
    {
        float[] vals = new float[3];
        AudioSource audioSource;

        switch (buff.buffName)
        {
            // ��Ƽ�� ��ų
            case "Fire Sword":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = weapon.weaponeDamage * buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;
                vals[1] = buff.actSkill.elementalVal[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;

                extraDamage = extraDamage + vals[0];
                extraFireVal = extraFireVal + vals[1];

                buff.applyVals = new float[2];
                buff.applyVals[0] = vals[0];
                buff.applyVals[1] = vals[1];

                audioSource = Array.Find(weapon.swordCol.GetComponents<AudioSource>(),
                x => x.gameObject.name == SoundManager.GameSFXType.FireBurning01.ToString());
                if (audioSource != null) Destroy(audioSource.gameObject);

                SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning01, weapon.swordCol.gameObject, 1f, true);
                break;

            case "Ice Sword":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = weapon.weaponeDamage * buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;
                vals[1] = weapon.weaponeStaggerVal * buff.actSkill.stgDamage[buff.actSkill.skillLv - 1];
                vals[2] = buff.actSkill.elementalVal[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;

                extraDamage = extraDamage + vals[0];
                extraStgDam = extraStgDam + vals[1];
                extraIceVal = extraIceVal + vals[2];

                buff.applyVals = new float[3];
                buff.applyVals[0] = vals[0];
                buff.applyVals[1] = vals[1];
                buff.applyVals[2] = vals[2];

                audioSource = Array.Find(weapon.swordCol.GetComponents<AudioSource>(),
                x => x.gameObject.name == SoundManager.GameSFXType.IceAmbient01.ToString());
                if (audioSource != null) Destroy(audioSource.gameObject);

                SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.IceAmbient01, weapon.swordCol.gameObject, 1f, true);
                break;

            case "Lighting Sword":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = weapon.weaponeDamage * buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;
                vals[1] = buff.actSkill.elementalVal[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;

                extraDamage = extraDamage + vals[0];
                extraLightingVal = extraLightingVal + vals[1];

                buff.applyVals = new float[2];
                buff.applyVals[0] = vals[0];
                buff.applyVals[1] = vals[1];
                break;

            case "Heavy Sword":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = weapon.weaponeStaggerVal * buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;
                vals[1] = buff.actSkill.stgPower[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;

                extraStgDam = extraStgDam + vals[0];
                extraStgPower = extraStgPower + vals[1];

                buff.applyVals = new float[2];
                buff.applyVals[0] = vals[0];
                buff.applyVals[1] = vals[1];
                break;

            case "Guarding Barrier":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;
                barrierVal = barrierVal - vals[0];

                if (buff.actSkill.skillLv == 2) isSuperArmor = true;
                buff.applyVals = new float[1];
                buff.applyVals[0] = vals[0];
                break;

            case "Guard Enhance":
                Debug.Log(buff.buffName + " || Buff ON");

                vals[0] = buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;
                guardEnhanceVal = guardEnhanceVal * (1 + vals[0]);

                buff.applyVals = new float[1];
                buff.applyVals[0] = vals[0];
                break;

            case "Magic Recover":
                Debug.Log(buff.buffName + " || Buff ON");
                StartCoroutine(MagicRecover(buff));
                break;

            // �нú� ��ų
            case "Perfect Dodge":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = buff.passSkill.effectVal[buff.passSkill.skillLv - 1];
                meleeDamVal = meleeDamVal * (1 + vals[0]);

                buff.applyVals = new float[1];
                buff.applyVals[0] = vals[0];
                break;

            case "Chance of Victory":
                Debug.Log(buff.buffName + " || Buff ON");
                vals[0] = buff.passSkill.effectVal[buff.passSkill.skillLv - 1];
                meleeDamVal = meleeDamVal * (1 + vals[0]);

                buff.applyVals = new float[1];
                buff.applyVals[0] = vals[0];
                break;

            case "Complete Down":
                Debug.Log(buff.buffName + " || Buff ON");
                Debug.Log(val);

                vals[0] = buff.passSkill.effectVal[buff.passSkill.skillLv - 1];
                val = Mathf.Clamp(val, 40, 60);
                val = GameManager.Instance.Remap(val, 40, 60, 0.4f, 1);
                vals[0] = vals[0] * val;
                meleeDamVal = meleeDamVal * (1 + vals[0]);

                buff.applyVals = new float[1];
                buff.applyVals[0] = vals[0];
                break;

            default:
                break;
        }
    }

    public void BuffOff(Buff buff)
    {
        AudioSource audioSource;
        switch (buff.buffName)
        {
            case "Fire Sword":
                Debug.Log(buff.buffName + " || Buff Off");

                extraDamage = extraDamage - buff.applyVals[0];
                extraFireVal = extraFireVal - buff.applyVals[1];

                buff.buffEffects[0].GetComponent<ParticleSystem>().Stop();

                audioSource = Array.Find(weapon.swordCol.GetComponentsInChildren<AudioSource>(),
                x => x.gameObject.name == SoundManager.GameSFXType.FireBurning01.ToString());
                if (audioSource != null) Destroy(audioSource.gameObject);
                break;

            case "Ice Sword":
                Debug.Log(buff.buffName + " || Buff Off");

                extraDamage = extraDamage - buff.applyVals[0];
                extraStgDam = extraStgDam - buff.applyVals[1];
                extraIceVal = extraIceVal - buff.applyVals[2];

                buff.buffEffects[0].GetComponent<ParticleSystem>().Stop();

                audioSource = Array.Find(weapon.swordCol.GetComponentsInChildren<AudioSource>(),
                x => x.gameObject.name == SoundManager.GameSFXType.IceAmbient01.ToString());
                if (audioSource != null) Destroy(audioSource.gameObject);
                break;

            case "Lighting Sword":
                Debug.Log(buff.buffName + " || Buff Off");

                extraDamage = extraDamage - buff.applyVals[0];
                extraLightingVal = extraLightingVal - buff.applyVals[1];

                buff.buffEffects[0].GetComponent<ParticleSystem>().Stop();
                break;

            case "Heavy Sword":
                Debug.Log(buff.buffName + " || Buff Off");

                extraStgDam = extraStgDam - buff.applyVals[0];
                extraStgPower = extraStgPower - buff.applyVals[1];

                buff.buffEffects[0].GetComponent<ParticleSystem>().Stop();
                break;

            case "Guarding Barrier":
                Debug.Log(buff.buffName + " || Buff Off");

                isSuperArmor = false;
                barrierVal = barrierVal + buff.applyVals[0];

                if (!buff.donPlayEffect)
                {
                    Instantiate(buff.buffEffects[1], buff.buffEffects[0].transform.parent);
                }
                Destroy(buff.buffEffects[0]);
                break;

            case "Guard Enhance":
                Debug.Log(buff.buffName + " || Buff Off");

                guardEnhanceVal = guardEnhanceVal - buff.applyVals[0];
                buff.buffEffects[0].GetComponent<ParticleSystem>().Stop();
                break;

            case "Magic Recover":
                Debug.Log(buff.buffName + " || Buff Off");

                buff.buffEffects[0].GetComponent<ParticleSystem>().Stop();
                break;

            case "Perfect Dodge":
                Debug.Log(buff.buffName + " || Buff Off");

                meleeDamVal = meleeDamVal / (1 + buff.applyVals[0]);
                break;

            case "Chance of Victory":
                Debug.Log(buff.buffName + " || Buff Off");

                meleeDamVal = meleeDamVal / (1 + buff.applyVals[0]);
                break;

            case "Complete Down":
                Debug.Log(buff.buffName + " || Buff Off");

                meleeDamVal = meleeDamVal / (1 + buff.applyVals[0]);
                break;

            default:
                break;
        }
    }

    IEnumerator MagicRecover(Buff buff)
    {
        float speed = buff.actSkill.damage[buff.actSkill.skillLv - 1] * buff.actSkill.effectByStat;

        while (GameManager.Instance.Buffs_Playing.Contains(buff) && buff.currentTime > 0)
        {
            if (player.curMP < player.maxMP)
            {
                player.curMP = player.curMP + Time.deltaTime * speed;
            }
            yield return null;
        }
    }

    void SkinEffect()
    {
        player.anim.SetTrigger("SkinShine");
        GameObject skinEffect = Instantiate(playerEffect.skinEffect, transform.position, transform.rotation);
        skinEffect.GetComponent<MeshEffect>().skinnedMeshRenderer = meshRenderer;
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicImpact03, transform.position, 0);
    }

    public void PassiveApply(PassiveSkill skill, float var = 0)
    {
        float val;

        if (skill.passiveType == PassiveSkill.PassiveType.Always)
        {
            switch (skill.name)
            {
                case "Health Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    player.maxHealth = player.maxHealth + val;
                    player.curHealth = player.curHealth + val;
                    playerState.RefreshBarWidth();
                    break;

                case "Stamina Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    player.maxStamina = player.maxStamina + val;
                    player.curStamina = player.curStamina + val;
                    playerState.RefreshBarWidth();
                    break;

                case "Finish Healing":
                    Debug.Log(skill.name + " Skill up");
                    finishHealing = skill.effectVal[skill.skillLv - 1];
                    break;

                case "Stg Damage Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    meleeStgDamVal = meleeStgDamVal + val;
                    break;

                case "Magic Damage Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    magicDamVal = magicDamVal + val;
                    break;

                case "Speed Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    moveSpeedVal = moveSpeedVal + val;
                    player.moveSpeed = basicMoveSpeed * moveSpeedVal;
                    break;

                case "Drain MP":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    mpRecoverVal = mpRecoverVal + val;
                    break;

                case "Defence Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    defanceVal = defanceVal - val;
                    break;

                case "Melee Damage Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    meleeDamVal = meleeDamVal + val;
                    break;

                case "Stg Level Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    meleeStgPowerVal = meleeStgPowerVal + val;
                    break;

                case "Elemental Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    elementalVal = elementalVal + val;
                    break;

                case "Dodge Up":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    dodgePfVal = dodgePfVal + val;
                    break;

                case "Weaken":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    weakenVal = weakenVal + val;
                    break;

                case "Big Bottle":
                    Debug.Log(skill.name + " Skill up");
                    val = skill.effectVal[skill.skillLv - 1];

                    if (skill.skillLv > 1)
                        val = val - skill.effectVal[skill.skillLv - 2];

                    healAmountVal = healAmountVal + val;
                    break;

                default:
                    break;
            }
        }
        else if (skill.passiveType == PassiveSkill.PassiveType.Instant)
        {
            switch (skill.name)
            {
                case "Dodge Recover":
                    if (skill.isCooldown) return;
                    Debug.Log(skill.name + " Activate");

                    val = skill.effectVal[skill.skillLv - 1];
                    player.curMP += val;

                    if (player.maxMP < player.curMP) player.curMP = player.maxMP;
                    skill.isCooldown = true;
                    StartCoroutine(Cooldown(skill, skill.cooldown[0]));

                    SkinEffect();
                    break;

                case "Posture Mending":
                    Debug.Log(skill.name + " Activate");

                    val = skill.effectVal[skill.skillLv - 1];
                    player.curStamina += val;

                    if (player.maxStamina < player.curStamina) player.curStamina = player.maxStamina;

                    SkinEffect();
                    break;

                case "MP Return":
                    Debug.Log(skill.name + " Activate");

                    val = skill.effectVal[skill.skillLv - 1];
                    player.curMP += val;

                    if (player.maxMP < player.curMP) player.curMP = player.maxMP;

                    SkinEffect();
                    break;

                case "Elemental Drain":
                    Debug.Log(skill.name + " Activate");

                    val = skill.effectVal[skill.skillLv - 1];
                    player.curMP += val;

                    if (player.maxMP < player.curMP) player.curMP = player.maxMP;

                    SkinEffect();
                    break;
            }
        }
        else
        {
            string path;
            Sprite icon;

            switch (skill.name)
            {
                case "Perfect Dodge":
                    Debug.Log(skill.name + " Activate");
                    path = skill.iconPath;
                    icon = Resources.Load<Sprite>(path);
                    weapon.buffManager.CreateBuff(skill.name, "Passive", 10, null, icon);

                    SkinEffect();
                    break;


                case "Chance of Victory":
                    Debug.Log(skill.name + " Activate");
                    path = skill.iconPath;
                    icon = Resources.Load<Sprite>(path);
                    weapon.buffManager.CreateBuff(skill.name, "Passive", 6, null, icon, var);

                    SkinEffect();
                    break;

                case "Complete Down":
                    if (var < 40) return;

                    Debug.Log(skill.name + " Activate");
                    path = skill.iconPath;
                    icon = Resources.Load<Sprite>(path);
                    weapon.buffManager.CreateBuff(skill.name, "Passive", 6, null, icon, var);

                    SkinEffect();
                    break;
            }


        }


    }

    public void FinishHealing()
    {
        //Debug.Log("finish");
        player.curHealth += finishHealing;
        if (player.curHealth > player.maxHealth)
            player.curHealth = player.maxHealth;
    }

    IEnumerator Cooldown(PassiveSkill skill, float cloodownTime)
    {
        yield return new WaitForSeconds(cloodownTime);
        skill.isCooldown = false;
    }

}
