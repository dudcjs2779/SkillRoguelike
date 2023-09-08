using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BossCollision : MonoBehaviour
{
    [Header("HIT PARA")]
    string attackName;
    float damage;
    float stgDamage;
    float stgPower;
    float fireVal, iceVal, lightingVal;
    ActiveSkill hitSkill;

    public bool isBurning;
    public bool isFrozen;
    public bool isShock;
    float shockStatusVal;

    private SkinnedMeshRenderer meshRenderer;
    private Color originColor;
    CapsuleCollider enemyHitCollider;
    Animator golemAni;

    GameObject ob_Golem;
    [SerializeField] VisualEffect burningEffect;
    [SerializeField] VisualEffect shockEffect;
    [SerializeField] GameObject frozenInEffect;
    [SerializeField] GameObject frozenOutEffect;


    List<Collider> hitColStack = new List<Collider>();
    List<Collider> groupHitColStack = new List<Collider>();

    BossGolem bossGolem;
    BossWeapon bossWeapon;
    Weapon weapon;
    PlayerBullet playerBullet;
    Player player;
    PlayerStateManager playerStateManager;
    Animator playerAnim;
    EffectAnim effectAnim;

    private void Awake()
    {
        ob_Golem = transform.parent.gameObject;
        meshRenderer = ob_Golem.GetComponentInChildren<SkinnedMeshRenderer>();
        golemAni = ob_Golem.GetComponent<Animator>();
        enemyHitCollider = GetComponent<CapsuleCollider>();

        originColor = meshRenderer.material.color;

        bossGolem = ob_Golem.GetComponent<BossGolem>();
        bossWeapon = ob_Golem.GetComponentInChildren<BossWeapon>();

        player = GameObject.Find("Player").GetComponent<Player>();
        playerStateManager = player.GetComponent<PlayerStateManager>();
        playerAnim = player.GetComponent<Animator>();
        effectAnim = bossGolem.GetComponent<EffectAnim>();
    }

    // Start is called before the first frame update
    void Start()
    {
        shockStatusVal = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (hitColStack.Count > 0)
        {
            for (int i = hitColStack.Count - 1; i >= 0; i--)
            {
                if (!hitColStack[i].enabled)
                {
                    //print(hitColStack[i].name + " is out");
                    hitColStack.Remove(hitColStack[i]);
                }
            }
        }
    }

    // 엔터 트리거 판정
    private void OnTriggerEnter(Collider other)
    {
        if (hitColStack.Contains(other) || groupHitColStack.Contains(other))
            return;

        PlayerWeaponCol hitPlayerWeapon;
        
        if (other.CompareTag("Sword"))
        {
            //print("Sword");
            weapon = other.transform.root.GetComponentInChildren<Weapon>();
            hitColStack.Add(other);

            hitPlayerWeapon = weapon.playerWeaponColList.Find(x => x.colList.Find(x => x == other));
            if (hitPlayerWeapon == null)
                return;

            playerStateManager.GainMpByAttack(hitPlayerWeapon.attackName, bossGolem.isStaggering);

            if (weapon.weaponColGroup.Count > 0)
            {
                groupHitColStack = weapon.weaponColGroup;
            }

            //// 실드러쉬 적 끌고가기
            //if (weapon.isStickAttck && !weapon.isStickedEnemy)
            //{
            //    weapon.isStickAttck = false;
            //    weapon.isStickedEnemy = true;
            //    weapon.shieldCol.enabled = false;
            //    StartCoroutine(StickToPlayer());
            //}

            StartCoroutine(OnHitSword(hitPlayerWeapon));

            Instantiate(weapon.hitEffect, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            //print(other.name);

        }
        else if (other.CompareTag("PlayerBullet"))
        {
            PlayerBullet playerBullet = other.transform.root.GetComponent<PlayerBullet>();
            hitPlayerWeapon = playerBullet.playerWeaponColList.Find(x => x.colList.Find(x => x == other));

            if (playerBullet.isNotBullet) hitColStack.Add(other);


            if (playerBullet.bulletColGroup.Count > 0)
            {
                groupHitColStack = playerBullet.bulletColGroup;
            }

            Instantiate(playerBullet.hitEffect, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

            StartCoroutine(OnHitSword(hitPlayerWeapon));
            //print(other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            PlayerBullet playerBullet = other.transform.root.GetComponent<PlayerBullet>();

            // 체인 공격일 경우 여러번 맞을 수 있게 hitColStack에서 제거해주기
            if (playerBullet.isChainBullet) hitColStack.Remove(other);
        }
    }

    
    IEnumerator OnHitSword(PlayerWeaponCol hit)
    {
        //Debug.Log("SwordHit : " + attackName);
        attackName = hit.attackName;
        float damage = hit.damage;
        float stgDamage = hit.stgDamage;
        float stgPower = hit.stgPower;
        float fireVal = hit.fireVal;
        float iceVal = hit.iceVal;
        float lightingVal = hit.lightingVal;

        // 데미지 및 경직 보정치 계산
        if (hitSkill == null || !hitSkill.isMagicAttack)
        {
            damage = damage * playerStateManager.meleeDamVal * playerStateManager.guardEnhanceVal + playerStateManager.extraDamage;
            stgDamage = stgDamage * playerStateManager.meleeStgDamVal * shockStatusVal + playerStateManager.extraStgDam;
            stgPower = stgPower * playerStateManager.meleeStgPowerVal + playerStateManager.extraStgPower;
            fireVal = fireVal * playerStateManager.elementalVal + playerStateManager.extraFireVal;
            iceVal = iceVal * playerStateManager.elementalVal + playerStateManager.extraIceVal;
            lightingVal = lightingVal * playerStateManager.elementalVal + playerStateManager.extraLightingVal;

            Buff guardEnhance = GameManager.Instance.Buffs_Playing.Find(x => x.buffName == "Guard Enhance");
            if (guardEnhance != null)
            {
                guardEnhance.DeActivation();
            }
        }
        else
        {
            damage = damage * playerStateManager.magicDamVal;
            stgDamage = stgDamage * playerStateManager.magicStgDamVal;
            stgPower = stgPower * playerStateManager.magicStgPowerVal;
            fireVal = fireVal * playerStateManager.elementalVal;
            iceVal = iceVal * playerStateManager.elementalVal;
            lightingVal = lightingVal * playerStateManager.elementalVal;

            if (isBurning || isFrozen || isShock)
            {
                damage = damage * playerStateManager.weakenVal;
            }
        }

        // 경직 중 피격시 보너스
        damage = damage * (bossGolem.isStaggering ? 1.3f : 1);
        fireVal = fireVal * (bossGolem.isStaggering ? 1.2f : 1);
        iceVal = iceVal * (bossGolem.isStaggering ? 1.2f : 1);
        lightingVal = lightingVal * (bossGolem.isStaggering ? 1.2f : 1);

        // 데미지 계산
        damage = Mathf.Round(damage * 10) / 10;
        bossGolem.curHP = bossGolem.curHP - damage;

        // 경직 데미지 계산
        stgDamage = Mathf.Round(stgDamage * 10) / 10;
        if (!bossGolem.isStaggering && !bossGolem.canStagger && !isFrozen)
            bossGolem.curSHP = bossGolem.curSHP - stgDamage;

        // 속성 데미지 계산
        fireVal = Mathf.Round(fireVal * 10) / 10;
        iceVal = Mathf.Round(iceVal * 10) / 10;
        lightingVal = Mathf.Round(lightingVal * 10) / 10;
        if (!isBurning) bossGolem.curFireHP -= fireVal;
        if (!isFrozen) bossGolem.curIceHP -= iceVal;
        if (!isShock) bossGolem.curLightingHP -= lightingVal;

        Debug.Log("damage : " + damage);
        Debug.Log("stgVal : " + stgDamage);
        //Debug.Log("stgPower : " + stgPower);
        //Debug.Log("fireVal : " + fireVal);
        //Debug.Log("iceVal : " + iceVal);
        //Debug.Log("lightingVal : " + lightingVal);

        // 사망
        if (bossGolem.curHP <= 0)
        {
            Dead();
            yield break;
        }

        if (!isBurning && bossGolem.curFireHP <= 0)
        {
            StartCoroutine(StatusBurning());

            PassiveSkill elementalDrain = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Elemental Drain");
            if (elementalDrain != null && elementalDrain.skillLv != 0) playerStateManager.PassiveApply(elementalDrain);
        }

        if (!isFrozen && bossGolem.curIceHP <= 0)
        {
            isFrozen = true;
            StartCoroutine(StatusFrozen());

            PassiveSkill elementalDrain = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Elemental Drain");
            if (elementalDrain != null && elementalDrain.skillLv != 0) playerStateManager.PassiveApply(elementalDrain);
        }

        if (!isShock && bossGolem.curLightingHP <= 0)
        {
            StartCoroutine(StatusShock());

            PassiveSkill elementalDrain = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Elemental Drain");
            if (elementalDrain != null && elementalDrain.skillLv != 0) playerStateManager.PassiveApply(elementalDrain);
        }

        // 경직 성능
        if (!isFrozen && (bossGolem.curSHP <= 0 || bossGolem.canStagger))
        {
            float stgAnimSpeed = 20 / stgPower;

            if (stgPower == 0)
            {
                bossGolem.canStagger = true;
                meshRenderer.gameObject.layer = LayerMask.NameToLayer("OutlineObject");
            }
            else if (stgPower < 25)
            {
                if (!bossGolem.canStagger || stgPower == 0)
                {
                    bossGolem.canStagger = true;
                    meshRenderer.gameObject.layer = LayerMask.NameToLayer("OutlineObject");
                }
                else if (bossGolem.canStagger)
                {
                    bossGolem.curSHP = bossGolem.maxSHP;
                    bossGolem.canStagger = false;
                    meshRenderer.gameObject.layer = 0;
                    bossGolem.StartCoroutine(bossGolem.Staggering(stgAnimSpeed));

                    playerStateManager.GainMpByStg();

                    PassiveSkill postureMending = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Posture Mending");
                    if (postureMending != null && postureMending.skillLv != 0) playerStateManager.PassiveApply(postureMending);

                    PassiveSkill chanceOfVictory = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Chance of Victory");
                    if (chanceOfVictory != null && chanceOfVictory.skillLv != 0) playerStateManager.PassiveApply(chanceOfVictory);

                    PassiveSkill completeDown = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Complete Down");
                    if (completeDown != null && completeDown.skillLv != 0) playerStateManager.PassiveApply(completeDown, stgPower);

                    if (hitSkill != null && hitSkill.isMagicAttack)
                    {
                        PassiveSkill mpReturn = GameManager.Instance.EquipPassiveList.Find(x => x.name == "MP Return");
                        if (mpReturn != null && mpReturn.skillLv != 0) playerStateManager.PassiveApply(mpReturn);
                    }

                }
            }
            else
            {
                bossGolem.curSHP = bossGolem.maxSHP + bossGolem.curSHP;
                bossGolem.canStagger = false;
                meshRenderer.gameObject.layer = 0;
                bossGolem.StartCoroutine(bossGolem.Staggering(stgAnimSpeed));

                playerStateManager.GainMpByStg();

                PassiveSkill postureMending = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Posture Mending");
                if (postureMending != null && postureMending.skillLv != 0) playerStateManager.PassiveApply(postureMending);

                PassiveSkill chanceOfVictory = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Chance of Victory");
                if (chanceOfVictory != null && chanceOfVictory.skillLv != 0) playerStateManager.PassiveApply(chanceOfVictory);

                PassiveSkill completeDown = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Complete Down");
                if (completeDown != null && completeDown.skillLv != 0) playerStateManager.PassiveApply(completeDown, stgPower);

                if (hitSkill != null && hitSkill.isMagicAttack)
                {
                    PassiveSkill mpReturn = GameManager.Instance.EquipPassiveList.Find(x => x.name == "MP Return");
                    if (mpReturn != null && mpReturn.skillLv != 0) playerStateManager.PassiveApply(mpReturn);
                }
            }
            //print(stgAnimSpeed);
        }

        if (!bossGolem.canStagger)
            meshRenderer.material.color = Color.red;

        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyHit01_1, transform.position);
        yield return new WaitForSeconds(0.2f);

        if (!bossGolem.canStagger)
            meshRenderer.material.color = originColor;
    }

    IEnumerator StatusBurning()
    {
        float time = 15;
        float frequency = 0.5f;
        isBurning = true;
        burningEffect.gameObject.SetActive(true);
        AudioSource audioSource = SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning04, gameObject);

        while (time > 0)
        {
            bossGolem.curHP -= 1;

            // 사망
            if (bossGolem.curHP <= 0)
            {
                Dead();
            }

            time -= frequency;
            yield return new WaitForSeconds(frequency);
        }
        bossGolem.maxFireHP = bossGolem.maxFireHP * 1.2f;
        bossGolem.curFireHP = bossGolem.maxFireHP;
        burningEffect.gameObject.SetActive(false);
        isBurning = false;
        Destroy(audioSource.gameObject);
    }

    IEnumerator StatusFrozen()
    {
        //Debug.Log("Frozen");
        bossGolem.anim.speed = 0;
        bossGolem.StartCoroutine(bossGolem.FrozenStaggering());
        effectAnim.StartCoroutine(effectAnim.MatLerpFloat(meshRenderer.materials[1], "_Dissolve", -1, 5f));
        Instantiate(frozenInEffect, transform.position, Quaternion.identity);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Frozen01, transform.position);

        yield return new WaitForSeconds(5f);
        bossGolem.maxIceHP = bossGolem.maxIceHP * 1.2f;
        bossGolem.curIceHP = bossGolem.maxIceHP;
        isFrozen = false;
        effectAnim.StartCoroutine(effectAnim.MatLerpFloat(meshRenderer.materials[1], "_Dissolve", 1.2f, 5f));
        Instantiate(frozenOutEffect, transform.position, Quaternion.identity);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact01, transform.position);
    }

    IEnumerator StatusShock()
    {
        isShock = true;
        shockStatusVal = 1.2f;
        shockEffect.gameObject.SetActive(true);
        AudioSource audioSource = SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.ElectricShock01, gameObject);

        yield return new WaitForSeconds(15f);
        shockStatusVal = 1;
        bossGolem.maxLightingHP = bossGolem.maxLightingHP * 1.2f;
        bossGolem.curLightingHP = bossGolem.maxLightingHP;
        shockEffect.gameObject.SetActive(false);
        isShock = false;
        Destroy(audioSource.gameObject);
    }

    void Dead()
    {
        golemAni.SetTrigger("doDie");
        ob_Golem.layer = 10;   //DeadEnemy
        enemyHitCollider.enabled = false;
        bossGolem.isDead = true;
        bossGolem.nav.isStopped = true;
        bossGolem.StopAllCoroutines();
        bossWeapon.StopAllCoroutines();
        StopAllCoroutines();
        player.ReleaseLockOn();
        Destroy(ob_Golem, 5f);

        switch(GameManager.Instance.difficultyType){
            case GameManager.DifficultyType.Easy:
                GameManager.Instance.gainGold += 1200;
                GameManager.Instance.playerData.money += 1200;
                break;

            case GameManager.DifficultyType.Normal:
                GameManager.Instance.gainGold += 1800;
                GameManager.Instance.playerData.money += 1800;
                break;

            case GameManager.DifficultyType.Hard:
                GameManager.Instance.gainGold += 2500;
                GameManager.Instance.playerData.money += 2500;
                break;
        }

        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyDead01_1, transform.position);

        GameManager.Instance.DungeonClear();
    }

}
