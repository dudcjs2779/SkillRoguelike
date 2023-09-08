using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyCollision : MonoBehaviour
{
    [Header("HIT PARA")]
    string attackName;
    // float damage;
    // float stgDamage;
    // float stgPower;
    // float fireVal, iceVal, lightingVal;
    // PlayerWeaponCol hitPlayerWeapon;

    public float enemyColScale;

    public bool isBurning;
    public bool isFrozen;
    public bool isShock;
    float shockStatusVal;

    private SkinnedMeshRenderer meshRenderer;
    private Color originColor;
    CapsuleCollider enemyHitCollider;
    GameObject ob_enemy;
    [SerializeField] VisualEffect burningEffect;
    [SerializeField] VisualEffect shockEffect;
    [SerializeField] GameObject frozenInEffect;
    [SerializeField] GameObject frozenOutEffect;

    List<Collider> hitColStack = new List<Collider>();
    List<Collider> groupHitColStack = new List<Collider>();

    Enemy enemy;
    EnemyWeapon enemyWeapon;
    Weapon weapon;
    PlayerBullet playerBullet;
    Player player;
    PlayerStateManager playerStateManager;
    UpgradeSkill upgradeSkill;
    EffectAnim effectAnim;

    // Start is called before the first frame update
    void Awake()
    {
        ob_enemy = transform.parent.gameObject;
        meshRenderer = ob_enemy.GetComponentInChildren<SkinnedMeshRenderer>();
        enemyHitCollider = GetComponent<CapsuleCollider>();

        originColor = meshRenderer.material.color;

        enemy = ob_enemy.GetComponent<Enemy>();
        enemyWeapon = ob_enemy.GetComponentInChildren<EnemyWeapon>();

        player = GameObject.Find("Player").GetComponent<Player>();
        playerStateManager = player.GetComponent<PlayerStateManager>();
        upgradeSkill = GameObject.Find("Canvas5").GetComponentInChildren<UpgradeSkill>();
        effectAnim = enemy.GetComponent<EffectAnim>();

    }

    private void Start()
    {
        shockStatusVal = 1;
    }

    private void Update()
    {
        if (GameManager.Instance.isDebugMode && Input.GetKeyDown(KeyCode.Keypad2))
        {
            Dead();
        }

        if (Input.GetKey(KeyCode.Keypad3))
        {

        }


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

        //if (weapon != null && weapon.weaponColGroup.Count == 0)
        //    isAlreadyHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }

    public void OnHit(Collider other){
        if (hitColStack.Contains(other) || groupHitColStack.Contains(other) || GameManager.Instance.isGameOver)
            return;

        PlayerWeaponCol hitPlayerWeapon;
        if (other.CompareTag("Sword"))
        {
            weapon = other.transform.root.GetComponentInChildren<Weapon>();
            hitColStack.Add(other);

            hitPlayerWeapon = weapon.playerWeaponColList.Find(x => x.colList.Find(x => x == other));
            if (hitPlayerWeapon == null)
                return;

            playerStateManager.GainMpByAttack(hitPlayerWeapon.attackName, enemy.isStaggering);
            Debug.Log(hitPlayerWeapon.attackName);


            if (weapon.weaponColGroup.Count > 0)
            {
                groupHitColStack = weapon.weaponColGroup;
            }

            // 실드러쉬 적 끌고가기
            if (hitPlayerWeapon.isStickAttck && !weapon.isStickedEnemy)
            {
                weapon.shieldRushCol1.enabled = false;
                weapon.isStickedEnemy = true;
                StartCoroutine(StickToPlayer());
            }

            //Debug.Log(hitPlayerWeapon);
            //Debug.Log(other);
            StartCoroutine(TakeDamage(hitPlayerWeapon, other));

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

            StartCoroutine(TakeDamage(hitPlayerWeapon, other));
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

    IEnumerator TakeDamage(PlayerWeaponCol hit, Collider hitCol)
    {
        //Debug.Log(hit.attackName);
        bool isBlock = false;
        attackName = hit.attackName;
        float damage = hit.damage;
        float stgDamage = hit.stgDamage;
        float stgPower = hit.stgPower;
        float fireVal = hit.fireVal;
        float iceVal = hit.iceVal;
        float lightingVal = hit.lightingVal;
        if (enemy.isGuarding)
        {
            Vector3 dir = hitCol.transform.position - transform.parent.position;
            float angle = Vector3.Angle(dir, transform.parent.forward);

            if (angle <= 90) isBlock = true;
            else isBlock = false;
        }

        enemy.NearEnemyWakeUp(7f);

        // 데미지 및 경직 보정치 계산
        if (!hit.isMagiceAttack)
        {
            damage = damage * playerStateManager.meleeDamVal * playerStateManager.guardEnhanceVal + playerStateManager.extraDamage;
            stgDamage = stgDamage * playerStateManager.meleeStgDamVal * shockStatusVal + playerStateManager.extraStgDam;
            stgPower = stgPower * playerStateManager.meleeStgPowerVal + playerStateManager.extraStgPower;
            fireVal = fireVal * playerStateManager.elementalVal + playerStateManager.extraFireVal;
            iceVal = iceVal * playerStateManager.elementalVal + playerStateManager.extraIceVal;
            lightingVal = lightingVal * playerStateManager.elementalVal + playerStateManager.extraLightingVal;

            Buff guardEnhance = GameManager.Instance.Buffs_Playing.Find(x => x.buffName == "Guard Enhance");
            if (guardEnhance  != null)
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
        damage = damage * (enemy.isStaggering ? 1.3f : 1) ;
        fireVal = fireVal * (enemy.isStaggering ? 1.2f : 1);
        iceVal = iceVal * (enemy.isStaggering ? 1.2f : 1);
        lightingVal = lightingVal * (enemy.isStaggering ? 1.2f : 1);

        // 데미지 계산
        damage = Mathf.Round(damage * 10) / 10;
        if (isBlock) damage = 0;
        enemy.curHP = enemy.curHP - damage;

        // 경직 데미지 계산
        stgDamage = Mathf.Round(stgDamage * 10) / 10;
        if (isBlock) stgDamage = stgDamage * 0.65f;
        if (!enemy.isStaggering && !enemy.canStagger && !isFrozen)
            enemy.curSHP = enemy.curSHP - stgDamage;

        // 속성 데미지 계산
        fireVal = Mathf.Round(fireVal * 10) / 10;
        iceVal = Mathf.Round(iceVal * 10) / 10;
        lightingVal = Mathf.Round(lightingVal * 10) / 10;
        if (!isBurning) enemy.curFireHP -= fireVal;
        if (!isFrozen) enemy.curIceHP -= iceVal;
        if (!isShock) enemy.curLightingHP -= lightingVal;

        Debug.Log("damage : " + damage);
        //Debug.Log("stgVal : " + stgDamage);
        //Debug.Log("stgPower : " + stgPower);
        //Debug.Log("fireVal : " + fireVal);
        //Debug.Log("iceVal : " + iceVal);
        //Debug.Log("lightingVal : " + lightingVal);


        // 사망
        if (enemy.curHP <= 0)
        {
            Dead();
            yield break;
        }

        if (!isBurning && enemy.curFireHP <= 0)
        {
            StartCoroutine(StatusBurning());

            PassiveSkill elementalDrain = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Elemental Drain");
            if (elementalDrain != null && elementalDrain.skillLv != 0) playerStateManager.PassiveApply(elementalDrain);
        }

        if (!isFrozen && enemy.curIceHP <= 0)
        {
            isFrozen = true;
            StartCoroutine(StatusFrozen());

            PassiveSkill elementalDrain = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Elemental Drain");
            if (elementalDrain != null && elementalDrain.skillLv != 0) playerStateManager.PassiveApply(elementalDrain);
        }

        if (!isShock && enemy.curLightingHP <= 0)
        {
            StartCoroutine(StatusShock());

            PassiveSkill elementalDrain = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Elemental Drain");
            if (elementalDrain != null && elementalDrain.skillLv != 0) playerStateManager.PassiveApply(elementalDrain);
        }

        // 경직 성능
        if (!isFrozen && (enemy.curSHP <= 0 || enemy.canStagger))
        {
            float stgAnimSpeed = 20 / stgPower;

            if (stgPower == 0)
            {
                enemy.canStagger = true;
                meshRenderer.gameObject.layer = LayerMask.NameToLayer("OutlineObject");
            }
            else if (stgPower < 25)
            {
                if (!enemy.canStagger || stgPower == 0)
                {
                    enemy.canStagger = true;
                    meshRenderer.gameObject.layer = LayerMask.NameToLayer("OutlineObject");
                }
                else if (enemy.canStagger)
                {
                    enemy.canStagger = false;
                    enemy.curSHP = enemy.maxSHP;
                    meshRenderer.gameObject.layer = 0;
                    enemy.StartCoroutine(enemy.Staggering(stgAnimSpeed));

                    playerStateManager.GainMpByStg();

                    PassiveSkill postureMending = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Posture Mending");
                    if (postureMending != null && postureMending.skillLv != 0) playerStateManager.PassiveApply(postureMending);

                    PassiveSkill chanceOfVictory = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Chance of Victory");
                    if (chanceOfVictory != null && chanceOfVictory.skillLv != 0) playerStateManager.PassiveApply(chanceOfVictory);

                    PassiveSkill completeDown = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Complete Down");
                    if (completeDown != null && completeDown.skillLv != 0) playerStateManager.PassiveApply(completeDown, stgPower);

                    if(hit.isBullet)
                    {
                        PassiveSkill mpReturn = GameManager.Instance.EquipPassiveList.Find(x => x.name == "MP Return");
                        if (mpReturn != null && mpReturn.skillLv != 0) playerStateManager.PassiveApply(mpReturn);
                    }

                }
            }
            else
            {
                enemy.canStagger = false;
                enemy.curSHP = enemy.maxSHP + enemy.curSHP;
                meshRenderer.gameObject.layer = 0;
                enemy.StartCoroutine(enemy.Staggering(stgAnimSpeed));

                playerStateManager.GainMpByStg();

                PassiveSkill postureMending = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Posture Mending");
                if (postureMending != null && postureMending.skillLv != 0) playerStateManager.PassiveApply(postureMending);

                PassiveSkill chanceOfVictory = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Chance of Victory");
                if (chanceOfVictory != null && chanceOfVictory.skillLv != 0) playerStateManager.PassiveApply(chanceOfVictory);

                PassiveSkill completeDown = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Complete Down");
                if (completeDown != null && completeDown.skillLv != 0) playerStateManager.PassiveApply(completeDown, stgPower);

                if (hit.isBullet)
                {
                    PassiveSkill mpReturn = GameManager.Instance.EquipPassiveList.Find(x => x.name == "MP Return");
                    if (mpReturn != null && mpReturn.skillLv != 0) playerStateManager.PassiveApply(mpReturn);
                }
            }
            //print(stgAnimSpeed);
        }

        // 넉백
        if (hit.isKnockBack)
        {
            //print("특수스킬");
            Vector3 dir = enemy.transform.position - hitCol.transform.position;
            HitBackOutAttack(dir, hit.knockBackPower, hit.donStgKnockBack);
        }

        StartCoroutine(HitColor(isBlock));
        if(!isBlock) SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyHit01_1, transform.position);
        else SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyGuardHit01, transform.position);
    }

    IEnumerator HitColor(bool isBlock)
    {
        if (!isBlock) meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        meshRenderer.material.color = originColor;
    }

    void HitBackOutAttack(Vector3 dir,float power, bool donStgKnockBack)
    {
        if (donStgKnockBack && enemy.isStaggering)
            return;

        enemy.nav.velocity = dir.normalized * power;
    }

    IEnumerator StickToPlayer()
    {
        while (weapon.isStickedEnemy)
        {
            if (enemy.isWallSlip)
                break;

            transform.root.position = player.transform.position + (player.transform.forward * enemyColScale);

            yield return null;
        }
    }

    IEnumerator StatusBurning()
    {
        float time = 15f;
        float frequency = 0.3f;
        isBurning = true;
        burningEffect.gameObject.SetActive(true);
        AudioSource audioSource = SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning04, gameObject, 1f, true);

        while (time > 0)
        {
            enemy.curHP -= 1;

            // 사망
            if (enemy.curHP <= 0)
            {
                Dead();
            }
            
            time -= frequency;
            yield return new WaitForSeconds(frequency);
        }

        enemy.maxFireHP = enemy.maxFireHP * 1.2f;
        enemy.curFireHP = enemy.maxFireHP;
        burningEffect.gameObject.SetActive(false);
        isBurning = false;
        Destroy(audioSource.gameObject);
    }

    IEnumerator StatusFrozen()
    {
        //Debug.Log("Frozen");
        enemy.anim.speed = 0;
        enemy.StartCoroutine(enemy.FrozenStaggering());
        effectAnim.StartCoroutine(effectAnim.MatLerpFloat(meshRenderer.materials[1], "_Dissolve", -1, 5f));
        Instantiate(frozenInEffect, transform.position, Quaternion.identity);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Frozen01, transform.position);

        yield return new WaitForSeconds(5f);
        enemy.maxIceHP = enemy.maxIceHP * 1.2f;
        enemy.curIceHP = enemy.maxIceHP;
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
        AudioSource audioSource = SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.ElectricShock01, gameObject, 1f, true);

        yield return new WaitForSeconds(15f);
        shockStatusVal = 1;
        enemy.maxLightingHP = enemy.maxLightingHP * 1.2f;
        enemy.curLightingHP = enemy.maxLightingHP;
        shockEffect.gameObject.SetActive(false);
        isShock = false;
        Destroy(audioSource.gameObject);
    }

    void Dead()
    {
        enemy.anim.CrossFade("Empty", 0, 1);
        enemy.anim.SetTrigger("doDie");
        ob_enemy.layer = 10;   //DeadEnemy
        meshRenderer.gameObject.layer = 0;
        enemyHitCollider.enabled = false;
        enemy.isDead = true;
        enemy.isChase = false;
        enemy.nav.isStopped = true;
        enemy.anim.speed = 1f;
        GameManager.Instance.restEnemy--;
        if(GameManager.Instance.spawnedEnemy != 0) GameManager.Instance.spawnedEnemy--;
        StopAllCoroutines();
        enemyWeapon.StopAllCoroutines();
        enemy.StopAllCoroutines();
        playerStateManager.FinishHealing();
        if (player.currentTarget == transform.root) player.enemyLocked = false;

        if(!enemy.isTraining){
            if (GameManager.Instance.difficultyType == GameManager.DifficultyType.Easy)
            {
                switch (enemy.enemyType)
                {
                    case GameManager.EnemyType.Oak:
                        player.curEXP += 30;
                        GameManager.Instance.gainGold += 110;
                        GameManager.Instance.playerData.money += 110;
                        GameManager.Instance.oakKillCount++;
                        GameManager.Instance.playerData.easyOak++;
                        break;
                    case GameManager.EnemyType.Lich:
                        player.curEXP += 30;
                        GameManager.Instance.gainGold += 110;
                        GameManager.Instance.playerData.money += 110;
                        GameManager.Instance.lichKillCount++;
                        GameManager.Instance.playerData.easyLich++;
                        break;
                    case GameManager.EnemyType.Wolf:
                        player.curEXP += 30;
                        GameManager.Instance.gainGold += 110;
                        GameManager.Instance.playerData.money += 110;
                        GameManager.Instance.wolfKillCount++;
                        GameManager.Instance.playerData.easyWolf++;
                        break;
                    default:
                        break;
                }

            }
            else if (GameManager.Instance.difficultyType == GameManager.DifficultyType.Normal)
            {
                switch (enemy.enemyType)
                {
                    case GameManager.EnemyType.Oak:
                        player.curEXP += 42;
                        GameManager.Instance.gainGold += 150;
                        GameManager.Instance.playerData.money += 150;
                        GameManager.Instance.oakKillCount++;
                        GameManager.Instance.playerData.normalOak++;
                        break;
                    case GameManager.EnemyType.Lich:
                        player.curEXP += 42;
                        GameManager.Instance.gainGold += 150;
                        GameManager.Instance.playerData.money += 150;
                        GameManager.Instance.lichKillCount++;
                        GameManager.Instance.playerData.normalLich++;
                        break;
                    case GameManager.EnemyType.Wolf:
                        player.curEXP += 42;
                        GameManager.Instance.gainGold += 150;
                        GameManager.Instance.playerData.money += 150;
                        GameManager.Instance.wolfKillCount++;
                        GameManager.Instance.playerData.normalWolf++;
                        break;
                    default:
                        break;
                }

            }
            else if (GameManager.Instance.difficultyType == GameManager.DifficultyType.Hard)
            {
                switch (enemy.enemyType)
                {
                    case GameManager.EnemyType.Oak:
                        player.curEXP += 54;
                        GameManager.Instance.gainGold += 200;
                        GameManager.Instance.playerData.money += 200;
                        GameManager.Instance.oakKillCount++;
                        GameManager.Instance.playerData.hardOak++;
                        break;
                    case GameManager.EnemyType.Lich:
                        player.curEXP += 54;
                        GameManager.Instance.gainGold += 200;
                        GameManager.Instance.playerData.money += 200;
                        GameManager.Instance.lichKillCount++;
                        GameManager.Instance.playerData.hardLich++;
                        break;
                    case GameManager.EnemyType.Wolf:
                        player.curEXP += 54;
                        GameManager.Instance.gainGold += 200;
                        GameManager.Instance.playerData.money += 200;
                        GameManager.Instance.wolfKillCount++;
                        GameManager.Instance.playerData.hardWolf++;
                        break;
                    default:
                        break;
                }

            }
        }

        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyDead01_1, transform.position);
        GameManager.Instance.StageCheck();

        Destroy(ob_enemy, 5f);

        StartCoroutine(HitColor(false));

    }


}
