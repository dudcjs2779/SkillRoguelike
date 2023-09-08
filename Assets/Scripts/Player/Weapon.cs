using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.VFX;
using System.Linq;
using UnityEngine.VFX.Utility;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    [Header("DEBUG")]
    public float debugFloat1;
    public float debugFloat2;
    public float debugFloat3;
    public bool debugBool1;

    public Coroutine weaponCoroutine;

    public string attackName;

    [Header("WEAPON PARA")]
    public float weaponeDamage;
    public float weaponeStaggerVal;
    
    [Header("ATTACK PARA")]
    public ActiveSkill usingSkill;
    public float damage;
    public float stgDamage;
    public float stgPower;
    public float fireVal, iceVal, lightingVal;
    public float skillVal;
    public float skillVal2;
    public float skillDuration;
    public float knockBackPower;

    // 애니메이션 속도
    public float aniSpeed;

    [Header("BOOL PARA")]
    public bool isStickedEnemy;
    bool isWeaponAttack;

    Camera gameCamera;

    // 공격 콜라이더
    [Header("COLLIDER")]
    public Collider swordCol;
    public Collider leftHandCol;
    public Collider backStepCol;
    public Collider horizontalSlashCol01;
    public Collider horizontalSlashCol02;
    public Collider doubleSlashCol1;
    public Collider doubleSlashCol2;
    public Collider dashStingCol;
    public Collider stepSlashCol;
    public Collider finalSmashCol;
    public Collider shieldCol;
    public Collider shieldShoveCol; 
    public Collider shieldRushCol1;
    public Collider shieldRushCol2;
    public Collider ShieldRushSwordCol;
    public MeshCollider groundWaveCol;

    [SerializeField] Transform[] IcicleFirePos;

    // 공격 콜라이더 리스트
    public List<Collider> weaponColGroup;
    public List<PlayerWeaponCol> playerWeaponColList = new List<PlayerWeaponCol>();

    Player player;
    PlayerSkill playerSkill;
    PlayerAnim playerAnim;
    PlayerStateManager playerStateManager;
    EffectControl effectControl;

    [Header("BULLET")]
    public GameObject defenceWall;
    public GameObject fireBall;
    public GameObject fireBurst;
    public GameObject fireTornado;
    public GameObject[] icicle;
    public GameObject iciSpear;
    public GameObject iceStorm;
    public GameObject lightingSpear;
    public GameObject lightingSplit;
    public GameObject lightingThunder;
    public GameObject gravityHole;
    public GameObject powerShieldWave;

    [Header("SWORD EFFECT")]
    public GameObject backStepSlash;
    public GameObject horizontalSlash;
    public GameObject shieldShoveSlash;
    public GameObject doubleSlash1;
    public GameObject doubleSlash2;
    public GameObject stepSlash;
    public GameObject finalSmashSlash;
    public GameObject finalSmashCrack;
    public GameObject ShieldRushSlash1;
    public GameObject ShieldRushSlash2;

    [Header("EFFECT")]
    public GameObject hitEffect;
    public GameObject guardEffect;
    public GameObject meleeEffect;
    public VisualEffect dashEffect;
    public GameObject HolyMagicBuff;
    public GameObject RoundAura;
    public GameObject LightExplosion;
    public GameObject LightExtinction;
    public ParticleSystem LightExplosionBlue;
    public ParticleSystem DefanceEffect;
    public ParticleSystem castingFire;
    public ParticleSystem castingIce;
    public ParticleSystem castingLighting;
    public ParticleSystem castingBlack;
    public GameObject fireSword;
    public GameObject fireSwordCasting;
    public GameObject iceSword;
    public GameObject iceSwordCasting;
    public GameObject lightingSword;
    public GameObject lightingSwordCasting;
    public GameObject HeavySword;
    public GameObject HeavySwordCasting;
    public GameObject crackGround;
    public GameObject powerShield;
    public VisualEffect powerShieldHit;
    public GameObject PowerShieldExplosion;
    public GameObject PF_ParrySuccess;
    public GameObject GuardEnhance;
    public GameObject magicRecover;
    public GameObject magicRecoverCasting;

    public BuffManager buffManager;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        playerAnim = player.GetComponent<PlayerAnim>();
        playerSkill = GetComponentInParent<PlayerSkill>();
        playerStateManager = GetComponentInParent<PlayerStateManager>();
        effectControl = GetComponentInParent<EffectControl>();
        gameCamera = Camera.main;
    }

    private void Start()
    {

    }

    void Update()
    {
        TestFrameRate();
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            // Debug.Log(playerWeaponColList.Count);

            // foreach (var item in playerWeaponColList)
            // {
            //     Debug.Log(item.attackName);
            // }
        }


    }

    private void OnDrawGizmos() {
    }

    public IEnumerator Shake(float duration = 0.2f)
    {
        Vector3 oriPlayerPos = transform.root.position;
        Vector3 prevShakeVal = Vector3.zero;

        float time = 0;
        while (time < duration)
        {
            time = time + Time.deltaTime;
            Vector3 playerPos = transform.root.position;

            float shakeRange = 0.05f;

            float posX = Random.value * shakeRange * 2 - shakeRange + prevShakeVal.x;
            float posZ = Random.value * shakeRange * 2 - shakeRange + prevShakeVal.z;

            playerPos.x += posX;
            playerPos.z += posZ;

            prevShakeVal.x += -posX;
            prevShakeVal.z += -posZ;

            transform.root.position = playerPos;
            yield return null;
        }

        transform.root.position = oriPlayerPos;
    }

    public void Use(string AttackName)
    {
        if (isWeaponAttack)
        {
            StopCoroutine(weaponCoroutine);
            WeaponOut();
        }
        weaponCoroutine = StartCoroutine(WeaponAttack(AttackName));
    }

    public IEnumerator WeaponAttack(string AttackName)
    {
        isWeaponAttack = true;
        attackName = AttackName;
        //print("staminaVal : " + staminaVal);
        //print("mpVal : " + mpVal);
        Debug.Log(AttackName);

        usingSkill = GameManager.Instance.EquipActiveList.Find(x=> x.name == AttackName);
        //print(usingSkill.krName);

        hitEffect = meleeEffect;

        if (AttackName == "A1")
        {
            //Debug.Log("A1");

            damage = weaponeDamage;
            stgDamage = weaponeStaggerVal;
            stgPower = 20;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(null, damage, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(swordCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.3f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.1f);
            swordCol.enabled = true;
            player.curStamina -= 24;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing01, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 3f);

            yield return new WaitForSeconds(0.1f);
            swordCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
        }
        else if (AttackName == "A2")
        {
            //Debug.Log("A2");

            damage = weaponeDamage;
            stgDamage = weaponeStaggerVal;
            stgPower = 20;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(null, damage, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(swordCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.2f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.13f);
            swordCol.enabled = true;
            player.curStamina -= 24;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing02, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 3f);

            yield return new WaitForSeconds(0.13f);
            swordCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
        }
        else if (AttackName == "A3")
        {
            //Debug.Log("A3");

            damage = weaponeDamage * 1.3f;
            stgDamage = weaponeStaggerVal * 1.3f;
            stgPower = 25;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(null, damage, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(swordCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.4f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.13f);
            swordCol.enabled = true;
            player.curStamina -= 28;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing03, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 3f);

            yield return new WaitForSeconds(0.087f);
            swordCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
        }
        else if (AttackName == "SA_Attack")
        {
            Debug.Log("SA");
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            if (player.fullCharged)
            {
                damage = weaponeDamage * 1.7f;
                stgDamage = weaponeStaggerVal * 1.8f;
                stgPower = 40;
            }
            else
            {
                damage = weaponeDamage * 1.3f;
                stgDamage = weaponeStaggerVal * 1.5f;
                stgPower = 40;
            }

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(null, damage, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(swordCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            swordCol.enabled = true;
            player.curStamina -= 35;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing03, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 3.5f);

            yield return new WaitForSeconds(0.166f);
            swordCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);

        }
        else if (AttackName == "Punch")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    break;
            }

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(leftHandCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);
            
            yield return new WaitForSeconds(0.16f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.1f);
            leftHandCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Punch01_1, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 2f);

            yield return new WaitForSeconds(0.14f);
            leftHandCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);

        }
        else if (AttackName == "Backstep Slash")
        {
            //Debug.Log(AttackName);
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    player.anim.speed = 1 + playerSkill.skillSpeed;
                    break;

            }

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(backStepCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.14f * (1 - playerSkill.skillSpeed));
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.12f * (1 - playerSkill.skillSpeed));
            GameObject instantSlash = Instantiate(backStepSlash, transform.position, transform.root.rotation * backStepSlash.transform.rotation, transform);
            instantSlash.transform.localPosition += backStepSlash.transform.position;
            backStepCol.enabled = true;
            Debug.Log(usingSkill.staminaPoint);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            player.isIframes = true;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing02, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 3f);

            yield return new WaitForSeconds(0.14f * (1 - playerSkill.skillSpeed));
            backStepCol.enabled = false;
            player.isIframes = false;

            yield return new WaitForSeconds(0.2f * (1 - playerSkill.skillSpeed));
            player.anim.speed = 1f;
            playerWeaponColList.Remove(playerWeaponCol);

        }
        else if (AttackName == "Guarding Barrier")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillDuration = usingSkill.duration[1];
                    break;
            }
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicCast01_1,leftHandCol.transform.position);
            yield return new WaitForSeconds(0.15f);
            Instantiate(HolyMagicBuff, transform);

            yield return new WaitForSeconds(0.65f);
            string path = GameManager.Instance.EquipActiveList.Find(x => x.name == AttackName).iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[3];
            effects[0] = Instantiate(RoundAura, transform);
            effects[1] = LightExtinction;
            effects[2] = LightExplosion;

            buffManager.CreateBuff(AttackName, "Active", skillDuration, effects, icon);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicImpact01, leftHandCol.transform.position);
        }
        else if (AttackName == "Fire Sword")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillDuration = usingSkill.duration[1];
                    break;
            }
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireCast02, transform.position);

            yield return new WaitForSeconds(0.25f);

            Instantiate(fireSwordCasting, swordCol.transform);

            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireImpact01, swordCol.transform.position);

            yield return new WaitForSeconds(0.6f);
            string path = GameManager.Instance.EquipActiveList.Find(x => x.name == AttackName).iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[1];
            effects[0] = Instantiate(fireSword, swordCol.transform);

            buffManager.CreateBuff(AttackName, "Active", skillDuration, effects, icon);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

        }
        else if (AttackName == "Fireball")
        {
            //Debug.Log(AttackName);

            float curveVal = 1f;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    fireVal = usingSkill.elementalVal[0];
                    curveVal = 60f;
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    fireVal = usingSkill.elementalVal[1];
                    curveVal = 100f;
                    break;
            }

            yield return new WaitForSeconds(0.1f);
            castingFire.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.3f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));

            yield return new WaitForSeconds(0.1f);  //발사

            GameObject instantBullet = Instantiate(fireBall, leftHandCol.transform.position, transform.root.rotation);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.fireVal = fireVal;
            playerBullet.t_Target = player.currentTarget;
            playerBullet.curveVal = curveVal;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.fireVal = fireVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            rigidBullet.velocity = transform.root.forward * 25f;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireShot01, leftHandCol.transform.position);
            WeaponSound(swordCol.transform.position, 3f);

        }
        else if (AttackName == "Ground Wave")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    groundWaveCol.transform.localScale = new Vector3(8.5f, 0.5f, 8.5f);
                    crackGround.transform.GetChild(0).transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    crackGround.transform.GetChild(1).transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    crackGround.transform.GetChild(2).transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    groundWaveCol.transform.localScale = new Vector3(10.5f, 0.5f, 10.5f);
                    crackGround.transform.GetChild(0).transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
                    crackGround.transform.GetChild(1).transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
                    crackGround.transform.GetChild(2).transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);
                    break;
            }

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(groundWaveCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.78f);     //찍는 타이밍
            groundWaveCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            GameObject InstantEffect = Instantiate(crackGround, transform.position, transform.rotation);
            CinemachineShake.Instance.ShakeCamera(5, 1f, 0.15f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact02, transform.position);
            WeaponSound(swordCol.transform.position, 5.5f);

            yield return new WaitForSeconds(0.1f);
            groundWaveCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);

            yield return new WaitForSeconds(0.45f);
        }
        else if (AttackName == "Horizontal Slash")
        {
            //Debug.Log(AttackName);
            Collider horizontalCol = horizontalSlashCol01;
            float slashScale = 1;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;
                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    horizontalCol = horizontalSlashCol02;
                    slashScale = 1.3f;
                    break;
            }

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(horizontalCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.3f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.35f);
            //if(debugBool1) EditorApplication.isPaused = true;
            
            GameObject instantSlash = Instantiate(horizontalSlash, transform.position, transform.root.rotation * horizontalSlash.transform.rotation, transform);
            instantSlash.transform.localPosition += horizontalSlash.transform.position;
            instantSlash.transform.localScale *= slashScale;

            horizontalCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.BigSlash01, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 6f);

            yield return new WaitForSeconds(0.17f);
            horizontalCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
        }
        else if (AttackName == "Shield Shove")
        {
            //Debug.Log(AttackName);

            bool donStgKnockBack = false;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    donStgKnockBack = false;
                    break;
                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    donStgKnockBack = true;
                    break;
            }
            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.attackName = AttackName;
            playerWeaponCol.isKnockBack = true;
            playerWeaponCol.donStgKnockBack = donStgKnockBack;
            playerWeaponCol.knockBackPower = 15;
            playerWeaponCol.colList.Add(shieldShoveCol);
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.5f);

            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.1f);
            GameObject instantSlash = Instantiate(shieldShoveSlash, transform.position, transform.root.rotation * shieldShoveSlash.transform.rotation, transform);
            instantSlash.transform.localPosition += shieldShoveSlash.transform.position;

            shieldShoveCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.BigSlash02, transform.position);
            WeaponSound(transform.position, 4.5f);

            yield return new WaitForSeconds(0.11f);
            shieldShoveCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);

        }
        else if (AttackName == "Power Shield")
        {
            //Debug.Log(AttackName);
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    skillVal = 0.4f;
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    skillVal = 0.2f;

                    aniSpeed = 0.3f;
                    player.anim.speed = 1 + playerSkill.skillSpeed;
                    break;
            }

            yield return new WaitForSeconds(0.2f * (1 - playerSkill.skillSpeed));
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.58f * (1 - playerSkill.skillSpeed));     //찍는 타이밍
            powerShield.SetActive(true);
            playerStateManager.isPowerShield = true;
            player.curMP -= usingSkill.mpPoint;
            player.curStamina -= usingSkill.staminaPoint;
            player.damageStack = 0;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact01, transform.position);
 
            yield return new WaitForSeconds(0.1f * (1 - playerSkill.skillSpeed));
            CinemachineShake.Instance.ShakeCamera(5, 1f, 0.15f);
            //StartCoroutine(PowerShield_StaminaReduce());

            player.isInputCancle = true;
            yield return new WaitUntil(() => Input.GetButtonDown("Dodge") || Input.GetButtonDown("Fire2") || player.isStaggering
                                        || player.curStamina <= 0 || !playerSkill.skilling || !playerStateManager.isPowerShield);

            player.isInputCancle = false;
            player.anim.speed = 1f;
            playerStateManager.isPowerShield = false;

            player.StartActionTiming();
            player.anim.CrossFade("Power Shield2", 0.15f);

            damage = damage * player.damageStack / 100;
            stgDamage = stgDamage * player.damageStack / 100;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.AirImpact01_1, transform.position);
            WeaponSound(swordCol.transform.position, 3f);
            yield return new WaitForSeconds(0.15f);

            powerShield.SetActive(false);

            GameObject instantBullet = Instantiate(PowerShieldExplosion, transform.position, Quaternion.identity);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);
        }
        else if (AttackName == "Double Slash")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;
                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    break;
            }

            PlayerWeaponCol playerWeaponCol1 = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol1.colList.Add(doubleSlashCol1);
            playerWeaponCol1.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol1);

            PlayerWeaponCol playerWeaponCol2 = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol2.colList.Add(doubleSlashCol2);
            playerWeaponCol2.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol2);

            yield return new WaitForSeconds(0.2f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.16f);
            doubleSlashCol1.enabled = true;
            GameObject instantSlash1 = Instantiate(doubleSlash1, transform.position, transform.root.rotation * doubleSlash1.transform.rotation, transform);
            instantSlash1.transform.localPosition += doubleSlash1.transform.position;

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing01, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 4f);

            yield return new WaitForSeconds(0.14f);
            doubleSlashCol1.enabled = false;

            yield return new WaitForSeconds(0.16f);
            doubleSlashCol2.enabled = true;
            GameObject instantSlash2 = Instantiate(doubleSlash2, transform.position, transform.root.rotation * doubleSlash2.transform.rotation, transform);
            instantSlash2.transform.localPosition += doubleSlash2.transform.position;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing02, swordCol.transform.position);

            yield return new WaitForSeconds(0.15f);
            doubleSlashCol2.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol1);
            playerWeaponColList.Remove(playerWeaponCol2);

        }
        else if (AttackName == "Defence Wall")
        {
            Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;
                case 2:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;
            }

            //Debug.Log(AttackName);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundShake01, playerSkill.aimPos);

            yield return new WaitForSeconds(0.75f);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            GameObject instantBullet = Instantiate(defenceWall, playerSkill.aimPos, playerSkill.aimRot);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponCol.isKnockBack = true;
            playerWeaponCol.knockBackPower = 8;
            playerWeaponColList.Add(playerWeaponCol);

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundCrack01, playerSkill.aimPos);

        }
        else if (AttackName == "Dash Sting")
        {
            //Debug.Log(AttackName);

            float dashPower = 1;

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    dashPower = 1;
                    break;
                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    dashPower = 1.5f;
                    break;
            }

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(dashStingCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.45f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));
            playerAnim.StartCoroutine(playerAnim.SetAnimMoveSpeed(dashPower, 0.3f));

            yield return new WaitForSeconds(0.12f);
            dashStingCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            dashEffect.gameObject.SetActive(true);
            EffectControl effectControl = dashEffect.GetComponent<EffectControl>();
            effectControl.StartCoroutine(effectControl.VfxDuration(0, 0.15f));

            player.donLockOnRotate = true;
            transform.root.gameObject.layer = LayerMask.NameToLayer("PlayerPass");
            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.Dash01, swordCol.gameObject);

            yield return new WaitForSeconds(0.1f);
            transform.root.gameObject.layer = LayerMask.NameToLayer("Player");

            yield return new WaitForSeconds(0.3f);
            dashStingCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
            WeaponSound(swordCol.transform.position, 4f);

            yield return new WaitForSeconds(0.55f);
            player.donLockOnRotate = false;


        }
        else if (AttackName == "Step Slash")
        {
            Debug.Log(AttackName + usingSkill.skillLv);
            float stepPower = 1;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    stepPower = 1f;
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    stepPower = 1.3f;
                    break;
            }

            playerAnim.StartCoroutine(playerAnim.SetAnimMoveSpeed(stepPower, 0.3f));

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol.colList.Add(stepSlashCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.TwoStep01, transform.position);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));

            yield return new WaitForSeconds(0.1f);
            dashEffect.gameObject.SetActive(true);
            EffectControl effectControl = dashEffect.GetComponent<EffectControl>();
            effectControl.StartCoroutine(effectControl.VfxDuration(0, 0.15f));

            yield return new WaitForSeconds(0.333f);
            stepSlashCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            GameObject instantSlash = Instantiate(stepSlash, transform.position, transform.root.rotation * stepSlash.transform.rotation, transform);
            instantSlash.transform.localPosition += stepSlash.transform.position;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Slash01, swordCol.transform.position);
            WeaponSound(swordCol.transform.position, 4f);

            yield return new WaitForSeconds(0.13f);
            stepSlashCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
        }
        else if (AttackName == "Icicle")
        {
            //Debug.Log(AttackName);

            int bulletCount = 3;

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    iceVal = usingSkill.elementalVal[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    iceVal = usingSkill.elementalVal[1];
                    bulletCount = 5;
                    break;
            }
            castingIce.gameObject.SetActive(true);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceCast01_1, leftHandCol.transform.position);
            yield return new WaitForSeconds(0.51f);

            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));

            yield return new WaitForSeconds(0.15f);

            StartCoroutine(IcicleSpawn(usingSkill, AttackName, bulletCount));

        }
        else if (AttackName == "Lighting Spear")
        {
            //Debug.Log(AttackName);
            int chainCount = 3;
            float chainDistance = 9;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    lightingVal = usingSkill.elementalVal[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    lightingVal = usingSkill.elementalVal[1];
                    chainCount = 4;
                    chainDistance = 13;
                    break;
            }

            castingLighting.gameObject.SetActive(true);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingCast01, leftHandCol.transform.position);
            yield return new WaitForSeconds(0.44f);

            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));
            yield return new WaitForSeconds(0.16f);  //발사

            GameObject instantBullet = Instantiate(lightingSpear, new Vector3(transform.position.x, leftHandCol.transform.position.y, transform.position.z), transform.rotation);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.lightingVal = lightingVal;
            playerBullet.t_Target = player.currentTarget;
            playerBullet.chainCount = chainCount;
            playerBullet.detect_Distance = chainDistance;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.lightingVal = lightingVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingShot01, leftHandCol.transform.position);
            WeaponSound(leftHandCol.transform.position, 3f);

        }
        else if (AttackName == "Fire Burst")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    fireVal = usingSkill.elementalVal[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    fireVal = usingSkill.elementalVal[1];
                    break;
            }

            yield return new WaitForSeconds(0.1f);
            castingFire.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.53f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));

            yield return new WaitForSeconds(0.2f);  //발사

            GameObject instantBullet = Instantiate(fireBurst, transform.position + transform.forward + Vector3.up * 1.2f, transform.root.rotation);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.fireVal = fireVal;
            playerBullet.t_Target = player.currentTarget;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;
            
            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.fireVal = fireVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBigShot01, instantBullet);
        }
        else if (AttackName == "Ice Spear")
        {
            //Debug.Log(AttackName);
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    iceVal = usingSkill.elementalVal[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    iceVal = usingSkill.elementalVal[1];
                    break;
            }
            castingIce.gameObject.SetActive(true);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact02, leftHandCol.transform.position);

            yield return new WaitForSeconds(0.9f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));

            yield return new WaitForSeconds(0.1f);

            GameObject instantBullet = Instantiate(iciSpear, transform.position + transform.forward * 1.5f, transform.rotation);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.iceVal = iceVal;
            playerBullet.usingSkill = usingSkill;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList = playerBullet.bulletColGroup;
            playerWeaponCol.iceVal = iceVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            playerBullet.bulletSoundAction = WeaponSound;
        }
        else if (AttackName == "Lighting Split")
        {
            //Debug.Log(AttackName);
            float skillScale = 1;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    lightingVal = usingSkill.elementalVal[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    lightingVal = usingSkill.elementalVal[1];
                    skillScale = 1.3f;
                    break;
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingCast01, leftHandCol.transform.position);
            castingLighting.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.67f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 30f));
            yield return new WaitForSeconds(0.16f);  //발사

            GameObject instantBullet = Instantiate(lightingSplit, transform.position + lightingSplit.transform.position, transform.rotation);
            instantBullet.transform.localScale = Vector3.one * skillScale;
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.lightingVal = lightingVal;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList = playerBullet.bulletColGroup;
            playerWeaponCol.lightingVal = lightingVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingBurning01, leftHandCol.transform.position);
            WeaponSound(leftHandCol.transform.position, 4f);
        }
        else if (AttackName == "Fire Tornado")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = 0;
                    fireVal = usingSkill.elementalVal[0];
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = 0;
                    fireVal = usingSkill.elementalVal[1];
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            castingFire.gameObject.SetActive(true);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireCast03, leftHandCol.transform.position);
            yield return new WaitForSeconds(0.83f);

            GameObject instantBullet = Instantiate(fireTornado, playerSkill.aimPos, fireTornado.transform.rotation);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();

            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.fireVal = fireVal;
            playerBullet.duration = skillDuration;
            playerBullet.t_Target = player.currentTarget;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.fireVal = fireVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireBigShot02, playerSkill.aimPos);
        }
        else if (AttackName == "Ice Storm")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = 0;
                    iceVal = usingSkill.elementalVal[0];
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = 0;
                    iceVal = usingSkill.elementalVal[1];
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            castingIce.gameObject.SetActive(true);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceCast02_1, leftHandCol.transform.position);
            yield return new WaitForSeconds(0.83f);

            GameObject instantBullet = Instantiate(iceStorm, playerSkill.aimPos, iceStorm.transform.rotation);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();

            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.iceVal = iceVal;
            playerBullet.duration = skillDuration;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.iceVal = iceVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact01, playerSkill.aimPos);
            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.IceWind01, instantBullet);
        }
        else if (AttackName == "Lighting Thunder")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = 0;
                    lightingVal = usingSkill.elementalVal[0];
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = 0;
                    lightingVal = usingSkill.elementalVal[1];
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingCast01, leftHandCol.transform.position);
            castingLighting.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.2f);

            GameObject instantBullet = Instantiate(lightingThunder, playerSkill.aimPos, Quaternion.identity);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();

            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.lightingVal = lightingVal;
            playerBullet.duration = skillDuration;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.lightingVal = lightingVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingBigShot01, playerSkill.aimPos);
        }
        else if (AttackName == "Gravity Hole")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);

            float skillScale = 1f;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = 0;
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = 0;
                    skillDuration = usingSkill.duration[1];
                    skillScale = 1.4f;
                    break;
            }
            
            castingBlack.gameObject.SetActive(true);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.DarkCast01, leftHandCol.transform.position);
            yield return new WaitForSeconds(0.8f);

            GameObject instantBullet = Instantiate(gravityHole, playerSkill.aimPos, Quaternion.identity);
            instantBullet.transform.localScale = Vector3.one * skillScale;
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();

            playerBullet.usingSkill = usingSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.duration = skillDuration;
            playerBullet.bulletSoundAction = WeaponSound;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
        }
        else if (AttackName == "Lighting Sword")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingCast01, leftHandCol.transform.position);

            yield return new WaitForSeconds(0.25f);
            Instantiate(lightingSwordCasting, swordCol.transform);

            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingImpact01, leftHandCol.transform.position);

            yield return new WaitForSeconds(0.6f);
            string path = GameManager.Instance.EquipActiveList.Find(x => x.name == AttackName).iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[1];
            effects[0] = Instantiate(lightingSword, swordCol.transform);

            buffManager.CreateBuff(AttackName, "Active", skillDuration, effects, icon);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

        }
        else if (AttackName == "Ice Sword")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceCast01_1, swordCol.transform.position);
            yield return new WaitForSeconds(0.25f);
            Instantiate(iceSwordCasting, swordCol.transform);

            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact03, swordCol.transform.position);

            yield return new WaitForSeconds(0.6f);
            string path = GameManager.Instance.EquipActiveList.Find(x => x.name == AttackName).iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[1];
            effects[0] = Instantiate(iceSword, swordCol.transform);

            buffManager.CreateBuff(AttackName, "Active", skillDuration, effects, icon);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

        }
        else if (AttackName == "Heavy Sword")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicCast01_1, swordCol.transform.position);

            yield return new WaitForSeconds(0.25f);
            Instantiate(HeavySwordCasting, swordCol.transform);

            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicImpact01, swordCol.transform.position);

            yield return new WaitForSeconds(0.6f);
            string path = GameManager.Instance.EquipActiveList.Find(x => x.name == AttackName).iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[1];
            effects[0] = Instantiate(HeavySword, swordCol.transform);

            buffManager.CreateBuff(AttackName, "Active", skillDuration, effects, icon);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

        }
        else if (AttackName == "Final Smash")
        {
            Debug.Log(AttackName + usingSkill.skillLv);
            float chargeGauge = 1;
            int mouseBtnNum;
            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    break;
            }

            switch (usingSkill.equipOrder)
            {
                case 1:
                    mouseBtnNum = 0;
                    break;
                case 2:
                    mouseBtnNum = 1;
                    break;
                case 3:
                    mouseBtnNum = 3;
                    break;
                case 4:
                    mouseBtnNum = 4;
                    break;
                default:
                    mouseBtnNum = 0;
                    break;
            }

            playerAnim.PlayAnimation("Final Smash1", true, 0.15f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.OneStep01, transform.position);
            yield return new WaitForSeconds(0.5f);

            AudioSource chargeSound = SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Charge01, swordCol.transform.position);

            float time = 0;
            while (time < 1.833f && player.curStamina >= 0)
            {
                time = time + Time.deltaTime;

                player.curStamina -= Time.deltaTime * 50;
                chargeGauge += Time.deltaTime * 0.9F;

                if(chargeGauge > 2.4f){
                    chargeGauge = 2.4f;
                    break;
                }

                if (player.isStaggering || Input.GetMouseButtonUp(mouseBtnNum) || !Input.GetMouseButton(mouseBtnNum) )
                    break;

                yield return null;
            }
            SoundManager.Instance.SoundVolumeLerp(chargeSound, 0f, 0.25f);
            //Debug.Log(chargeGauge);

            damage = damage * chargeGauge;
            stgDamage = stgDamage * chargeGauge;
            stgPower = stgPower * chargeGauge;
            playerAnim.PlayAnimation("Final Smash2", true, 0.15f);
            player.StartActionTiming();

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(usingSkill, damage, stgDamage, stgPower, false, false);

            playerWeaponCol.colList.Add(finalSmashCol);
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            yield return new WaitForSeconds(0.36f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.07f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.BigSlash03, swordCol.transform.position);

            yield return new WaitForSeconds(0.07f);
            finalSmashCol.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            GameObject instantSlash = Instantiate(finalSmashSlash, transform.position, transform.root.rotation * finalSmashSlash.transform.rotation, transform);
            instantSlash.transform.localPosition += finalSmashSlash.transform.position;

            yield return new WaitForSeconds(0.05f);
            GameObject instantCrack = Instantiate(finalSmashCrack, transform.position, transform.root.rotation);
            CinemachineShake.Instance.ShakeCamera(10, 1f, 0.15f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact03, swordCol.transform.position);
            WeaponSound(playerSkill.aimPos, 5f);

            yield return new WaitForSeconds(0.15f);
            finalSmashCol.enabled = false;
            playerWeaponColList.Remove(playerWeaponCol);
        }
        else if (AttackName == "Guard Enhance")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);
            switch (usingSkill.skillLv)
            {
                case 1:
                    skillVal = usingSkill.damage[0];
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillVal = usingSkill.damage[1];
                    skillDuration = usingSkill.duration[1];
                    break;
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Guard01, shieldCol.transform.position);

            yield return new WaitForSeconds(0.06f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));
            playerStateManager.isPerfectParry = true;
            playerStateManager.isParry = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            yield return new WaitForSeconds(0.25f);
            if (!player.isStaggering) playerStateManager.isPerfectParry = false;

            yield return new WaitForSeconds(0.5f);
            playerStateManager.isParry = false;
        }
        else if (AttackName == "Guard Enhance Hit")
        {
            //Debug.Log(AttackName);
            playerStateManager.isParry = true;

            yield return new WaitForSeconds(0.5f);
            playerStateManager.isParry = false;
        }
        else if (AttackName == "Shield Rush")
        {
            //Debug.Log(AttackName + usingSkill.skillLv);

            switch (usingSkill.skillLv)
            {
                case 1:
                    damage = weaponeDamage * usingSkill.damage[0];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[0];
                    stgPower = usingSkill.stgPower[0];
                    break;

                case 2:
                    damage = weaponeDamage * usingSkill.damage[1];
                    stgDamage = weaponeStaggerVal * usingSkill.stgDamage[1];
                    stgPower = usingSkill.stgPower[1];
                    break;
            }

            PlayerWeaponCol playerWeaponCol1 = new PlayerWeaponCol(usingSkill, damage / 1.5f * usingSkill.effectByStat, stgDamage * 1.5f, stgPower * 1.35f, false, false);
            playerWeaponCol1.colList.Add(shieldRushCol1);
            playerWeaponCol1.isStickAttck = true;
            playerWeaponCol1.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol1);

            PlayerWeaponCol playerWeaponCol2 = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol2.colList.Add(shieldRushCol2);
            playerWeaponCol2.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol2);

            PlayerWeaponCol playerWeaponCol3 = new PlayerWeaponCol(usingSkill, damage * usingSkill.effectByStat, stgDamage, stgPower, false, false);
            playerWeaponCol3.colList.Add(ShieldRushSwordCol);
            playerWeaponCol3.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol3);

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Guard01, shieldCol.transform.position);

            yield return new WaitForSeconds(0.4f);
            player.StartCoroutine(player.ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 20f));

            yield return new WaitForSeconds(0.16f);
            shieldRushCol1.enabled = true;
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;
            player.donLockOnRotate = true;

            dashEffect.gameObject.SetActive(true);
            EffectControl effectControl = dashEffect.GetComponent<EffectControl>();
            effectControl.StartCoroutine(effectControl.VfxDuration(0, 0.25f));

            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.Dash02, gameObject).transform.localPosition = -transform.forward * 0.1f;
            WeaponSound(transform.position, 4f);

            yield return new WaitForSeconds(0.4f);
            shieldRushCol1.enabled = false;
            isStickedEnemy = false;

            yield return new WaitForSeconds(0.07f);
            shieldRushCol2.enabled = true;
            player.donLockOnRotate = false;

            GameObject instantSlash1 = Instantiate(ShieldRushSlash1, transform.position, transform.root.rotation * ShieldRushSlash1.transform.rotation, transform);
            instantSlash1.transform.localPosition += ShieldRushSlash1.transform.position;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.BigSlash02, shieldCol.transform.position);
            WeaponSound(transform.position, 4f);

            yield return new WaitForSeconds(0.13f);
            shieldRushCol2.enabled = false;

            yield return new WaitForSeconds(0.2f);
            ShieldRushSwordCol.enabled = true;
            GameObject instantSlash2 = Instantiate(ShieldRushSlash2, transform.position, transform.root.rotation * ShieldRushSlash2.transform.rotation, transform);
            instantSlash2.transform.localPosition += ShieldRushSlash2.transform.position;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Swing03, swordCol.transform.position);

            yield return new WaitForSeconds(0.12f);
            ShieldRushSwordCol.enabled = false;

            playerWeaponColList.Remove(playerWeaponCol1);
            playerWeaponColList.Remove(playerWeaponCol2);
            playerWeaponColList.Remove(playerWeaponCol3);
        }
        else if (AttackName == "Magic Recover")
        {
            //Debug.Log(AttackName);

            switch (usingSkill.skillLv)
            {
                case 1:
                    skillDuration = usingSkill.duration[0];
                    break;

                case 2:
                    skillDuration = usingSkill.duration[1];
                    break;
            }
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicCast01_1, leftHandCol.transform.position);

            yield return new WaitForSeconds(0.15f);
            Instantiate(magicRecoverCasting, transform);

            yield return new WaitForSeconds(0.65f);
            string path = GameManager.Instance.EquipActiveList.Find(x => x.name == AttackName).iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[1];
            effects[0] = Instantiate(magicRecover, transform);

            buffManager.CreateBuff(AttackName, "Active", skillDuration, effects, icon);
            player.curStamina -= usingSkill.staminaPoint;
            player.curMP -= usingSkill.mpPoint;

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.MagicImpact02, leftHandCol.transform.position);
        }

        //else yield break;

        attackName = "";
        usingSkill = null;
        isStickedEnemy = false;
        fireVal = 0;
        iceVal = 0;
        lightingVal = 0;
        weaponColGroup.Clear();
        isWeaponAttack = false;
    }

    public void WeaponOut()
    {
        attackName = "";
        usingSkill = null;
        fireVal = 0;
        iceVal = 0;
        lightingVal = 0;

        isStickedEnemy = false;

        swordCol.enabled = false;
        leftHandCol.enabled = false;
        backStepCol.enabled = false;
        horizontalSlashCol01.enabled = false;
        horizontalSlashCol02.enabled = false;
        doubleSlashCol1.enabled = false;
        doubleSlashCol2.enabled = false;
        dashStingCol.enabled = false;
        stepSlashCol.enabled = false;
        finalSmashCol.enabled = false;
        shieldCol.enabled = false;
        shieldShoveCol.enabled = false;
        shieldRushCol1.enabled = false;
        shieldRushCol2.enabled = false;
        ShieldRushSwordCol.enabled = false;
        groundWaveCol.enabled = false;

        player.anim.speed = 1f;
        player.donLockOnRotate = false;
        player.isIframes = false;

        transform.root.gameObject.layer = LayerMask.NameToLayer("Player");

        weaponColGroup.Clear();
    }

    void WeaponSound(Vector3 soundPos, float soundRange){
        //Debug.Log("WeaponSound");
        Collider[] detectedEnemyCols = Physics.OverlapSphere(soundPos, soundRange, LayerMask.GetMask("Enemy"));

        if(detectedEnemyCols.Length > 0){
            foreach (var enemyCol in detectedEnemyCols)
            {
                Enemy enemy = enemyCol.GetComponent<Enemy>();
                if(enemy != null && !enemy.isTraining && enemy.T_target == null && !enemy.isChase){
                    enemy.ChaseStart();
                }
            }
        }
    }

    IEnumerator IcicleSpawn(ActiveSkill activeSkill, string AttackName, int bulletCount)
    {
        float iceVal = this.iceVal;
        for (int i = 0; i < bulletCount; i++)
        {
            int ran = Random.Range(0, 2);

            GameObject instantBullet = Instantiate(icicle[ran], IcicleFirePos[i].position, transform.rotation, transform);
            PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();

            playerBullet.usingSkill = activeSkill;
            playerBullet.attackName = AttackName;
            playerBullet.damage = damage;
            playerBullet.stgDamage = stgDamage;
            playerBullet.stgPower = stgPower;
            playerBullet.iceVal = iceVal;

            playerBullet.rigid.isKinematic = true;
            playerBullet.t_Target = player.currentTarget;
            playerBullet.playerWeaponColList = playerWeaponColList;

            PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(activeSkill, damage * activeSkill.effectByStat, stgDamage, stgPower, true, true);
            playerWeaponCol.colList.Add(playerBullet.bulletCol);
            playerWeaponCol.iceVal = iceVal;
            playerWeaponCol.attackName = AttackName;
            playerWeaponColList.Add(playerWeaponCol);

            if (i == 0)
            {
                player.curStamina -= activeSkill.staminaPoint;
                player.curMP -= activeSkill.mpPoint;
            }
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact01, leftHandCol.transform.position);
            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.IceAmbient01, instantBullet, 1f, true);
            WeaponSound(leftHandCol.transform.position, 3.5f);

            yield return new WaitForSeconds(0.15f);
        }
    }

    IEnumerator PowerShield_StaminaReduce()
    {
        while (playerStateManager.isPowerShield)
        {
            player.curStamina -= Time.deltaTime * 5f;

            if (player.curStamina <= 0)
            {
                playerStateManager.isPowerShield = false;
            }

            yield return null;
        }
    }

    IEnumerator WeaopnRayCast(Collider weaponCol){
        Debug.Log(weaponCol.transform.localScale);
        while(weaponCol.enabled){
            Collider[] hitCols = Physics.OverlapBox(weaponCol.transform.position, weaponCol.transform.localScale / 2, weaponCol.transform.rotation, LayerMask.GetMask("EnemyCollision"));
            //Debug.Log("WeaopnRayCast");

            if(hitCols.Length > 0){
                Debug.Log("WeaopnRayCast Detect");
                foreach (var hitCol in hitCols)
                {
                    EnemyCollision enemyCollision = hitCol.GetComponent<EnemyCollision>();
                    enemyCollision.OnHit(weaponCol);
                }
            }
            
            yield return null;
        }
    }

    float lastFrame = 0;
    bool doCountFrame;
    void TestFrameRate()
    {
        if (doCountFrame)
        {
            if (lastFrame == 0)
            {
                lastFrame = Time.frameCount;
            }
            Debug.Log(Time.frameCount - lastFrame);
        }
        else
        {
            lastFrame = 0;
        }
    }

}

public class PlayerWeaponCol
{
    public List<Collider> colList = new List<Collider>();
    public string attackName;

    public float damage;
    public float stgDamage;
    public float stgPower;
    public float fireVal, iceVal, lightingVal;
    public float skillVal;
    public float skillVal2;
    public float skillDuration;
    public float knockBackPower;

    public bool isBullet;
    public bool isMagiceAttack;
    public bool isKnockBack;
    public bool donStgKnockBack;
    public bool isStickAttck;

    public PlayerWeaponCol(ActiveSkill _activeSkill, float _damage, float _stgDamage, float _stgPower, bool _isBullet, bool _isMagiceAttack)
    {
        damage = _damage;
        stgDamage = _stgDamage;
        stgPower = _stgPower;

        isBullet = _isBullet;
        isMagiceAttack = _isMagiceAttack;
    }

    
}
