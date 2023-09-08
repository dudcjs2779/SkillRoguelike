using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public enum Type { Golem };
    public Type bossType;

    public float bossDamage;
    public float damage;
    public string attackName;
    public string enemyType;
    public GameObject bullet;

    public Coroutine C_bossWeapon;

    public List<EnemyWeaponCol> bossWeaponColList = new List<EnemyWeaponCol>();
    public List<Collider> weaponColliders;
    public Collider leftHandCol;
    public Collider rightHandCol;
    public Collider leftHandCol_Big;
    public Collider rightHandCol_Big;
    public Collider rushCol;
    public Collider rushWaveCol;
    public Collider rushHitWallCol;
    public Collider jumpHitCol;
    public Collider smashCol;
    public Collider smashWaveCol;

    public GameObject jumpAttackEffect;
    public GameObject smashAttackEffect;
    public GameObject rushHitWallEffect;

    public GameObject risingWall;


    BossGolem bossGolem;

    private void Awake()
    {
        bossGolem = GetComponentInParent<BossGolem>();
    }

    private void Start()
    {

    }

    public void Use(string AttackName)
    {
        if(C_bossWeapon != null) StopCoroutine(C_bossWeapon);
        C_bossWeapon = StartCoroutine(Swing(AttackName));
    }

    public IEnumerator Swing(string AttackName)
    {
        //Debug.Log("Swing : " + AttackName);
        attackName = AttackName;

        switch (bossType)
        {
            case Type.Golem:
                if (AttackName == "Attack1")
                {
                    //Debug.Log("OakA1");
                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(leftHandCol, bossDamage, false);
                    bossWeaponColList.Add(enemyWeaponCol);

                    bossGolem.isAttackAiming = true;

                    yield return new WaitForSeconds(0.6f);
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.LongSwing01, leftHandCol.gameObject);
                    yield return new WaitForSeconds(0.1f);
                    leftHandCol.enabled = true;

                    yield return new WaitForSeconds(0.15f);
                    bossGolem.isAttackAiming = false;

                    yield return new WaitForSeconds(0.15f);
                    leftHandCol.enabled = false;

                    bossWeaponColList.Remove(enemyWeaponCol);
                }
                else if (AttackName == "Attack2")
                {
                    //Debug.Log("OakA2");

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(leftHandCol, bossDamage, false);
                    bossWeaponColList.Add(enemyWeaponCol);

                    bossGolem.isAttackAiming = true;
                    yield return new WaitForSeconds(0.4f);

                    yield return new WaitForSeconds(0.3f);
                    leftHandCol.enabled = true;
                    bossGolem.isAttackAiming = false;
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.LongSwing02, leftHandCol.gameObject);

                    yield return new WaitForSeconds(0.23f);
                    leftHandCol.enabled = false;
                    bossWeaponColList.Remove(enemyWeaponCol);
                }
                else if (AttackName == "Attack2-2")
                {
                    //Debug.Log("OakA2");
                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(rightHandCol, bossDamage, false);
                    bossWeaponColList.Add(enemyWeaponCol);

                    yield return new WaitForSeconds(0.3f);
                    bossGolem.StartCoroutine(bossGolem.AttackMove());

                    yield return new WaitForSeconds(0.15f);
                    rightHandCol.enabled = true;
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.LongSwing03, rightHandCol.gameObject);

                    yield return new WaitForSeconds(0.18f);
                    bossGolem.anim.speed = 1f;
                    rightHandCol.enabled = false;
                    bossWeaponColList.Remove(enemyWeaponCol);
                }
                else if (AttackName == "TurnAttack")
                {
                    //Debug.Log("OakA2");

                    EnemyWeaponCol enemyWeaponCol1 = new EnemyWeaponCol(leftHandCol_Big, bossDamage * 1.5f, false);
                    enemyWeaponCol1.isBigAttack = true;
                    enemyWeaponCol1.isKnockBack = true;
                    enemyWeaponCol1.knockBackPower = 25;
                    bossWeaponColList.Add(enemyWeaponCol1);

                    EnemyWeaponCol enemyWeaponCol2 = new EnemyWeaponCol(rightHandCol_Big, bossDamage * 1.5f, false);
                    enemyWeaponCol2.isBigAttack = true;
                    enemyWeaponCol2.isKnockBack = true;
                    enemyWeaponCol2.knockBackPower = 10;
                    bossWeaponColList.Add(enemyWeaponCol2);

                    yield return new WaitForSeconds(0.86f);
                    leftHandCol_Big.enabled = true;
                    rightHandCol_Big.enabled = true;
                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LongDoubleSwing01, transform.position);

                    yield return new WaitForSeconds(0.74f);
                    leftHandCol_Big.enabled = false;
                    rightHandCol_Big.enabled = false;
                    bossWeaponColList.Remove(enemyWeaponCol1);
                    bossWeaponColList.Remove(enemyWeaponCol2);
                }
                else if (AttackName == "AttackSmash")
                {
                    //Debug.Log("OakA2");

                    EnemyWeaponCol enemyWeaponCol1 = new EnemyWeaponCol(smashCol, bossDamage * 1.8f, false);
                    enemyWeaponCol1.isBigAttack = true;
                    bossWeaponColList.Add(enemyWeaponCol1);

                    EnemyWeaponCol enemyWeaponCol2 = new EnemyWeaponCol(smashWaveCol, bossDamage * 1.5f, false);
                    bossWeaponColList.Add(enemyWeaponCol2);

                    bossGolem.isAttackAiming = true;
                    yield return new WaitForSeconds(1.3f);
                    bossGolem.isAttackAiming = false;

                    yield return new WaitForSeconds(0.13f);
                    smashCol.enabled = true;
                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Smash01, transform.position);

                    yield return new WaitForSeconds(0.17f);
                    smashCol.enabled = false;

                    smashWaveCol.enabled = true;
                    CinemachineShake.Instance.ShakeCamera(10, 1.5f, 0.2f);

                    GameObject instantBullet = Instantiate(smashAttackEffect, smashWaveCol.transform.position + (Vector3.up * 0.01f), Quaternion.identity);

                    yield return new WaitForSeconds(0.1f);
                    smashWaveCol.enabled = false;

                    bossWeaponColList.Remove(enemyWeaponCol1);
                    bossWeaponColList.Remove(enemyWeaponCol2);

                }
                else if (AttackName == "AttackRush")
                {
                    //Debug.Log("OakA2");

                    EnemyWeaponCol enemyWeaponCol1 = new EnemyWeaponCol(rushCol, bossDamage * 1.7f, false);
                    enemyWeaponCol1.isBigAttack = true;
                    enemyWeaponCol1.isKnockBack = true;
                    enemyWeaponCol1.knockBackPower = 15;

                    bossWeaponColList.Add(enemyWeaponCol1);
                    EnemyWeaponCol enemyWeaponCol2 = new EnemyWeaponCol(rushWaveCol, bossDamage, false);
                    bossWeaponColList.Add(enemyWeaponCol2);

                    
                    yield return new WaitForSeconds(0.1f);
                    rushCol.enabled = true;
                    rushHitWallCol.enabled = true;

                    yield return new WaitUntil(() => bossGolem.rush_Hit  || bossGolem.rush_HitWall || bossGolem.doStopRush);

                    bossGolem.doStopRush = false;
                    rushCol.enabled = false;
                    rushHitWallCol.enabled = false;

                    if (bossGolem.rush_HitWall)
                    {
                        rushWaveCol.enabled = true;
                        Instantiate(rushHitWallEffect, rushHitWallCol.transform.position, Quaternion.identity);
                        yield return new WaitForSeconds(0.066f);
                        rushWaveCol.enabled = false;
                    }

                    bossWeaponColList.Remove(enemyWeaponCol1);
                    bossWeaponColList.Remove(enemyWeaponCol2);
                }
                else if (AttackName == "AttackJump")
                {
                    //Debug.Log("OakA1");

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(jumpHitCol, bossDamage * 1.4f, false);
                    bossWeaponColList.Add(enemyWeaponCol);

                    yield return new WaitUntil(() => bossGolem.isEndJump);
                    jumpHitCol.enabled = true;
                    Instantiate(jumpAttackEffect, transform.position + (Vector3.up * 0.01f), Quaternion.identity);
                    CinemachineShake.Instance.ShakeCamera(10, 1f, 0.15f);

                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact04, transform.position);

                    yield return new WaitForSeconds(0.1f);
                    jumpHitCol.enabled = false;
                    bossGolem.isEndJump = false;

                    bossWeaponColList.Remove(enemyWeaponCol);
                }
                else if (AttackName == "RisingWall")
                {
                    //Debug.Log("OakA1");

                    yield return new WaitForSeconds(0.65f);
                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact06, transform.position);

                    yield return new WaitForSeconds(0.1f);

                    Vector3 dir = transform.root.position - bossGolem.target.position;
                    dir.y = 0;

                    GameObject instantBullet = Instantiate(risingWall, bossGolem.target.position - dir.normalized * 0.01f, Quaternion.LookRotation(dir));
                    EnemyBullet enemyBullet = instantBullet.GetComponent<EnemyBullet>();

                    enemyBullet.attackName = AttackName;
                    enemyBullet.damage = 50;
                    enemyBullet.enemyWeaponColList = bossWeaponColList;

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(enemyBullet.bulletCol, bossDamage * 1.7f, true);
                    enemyWeaponCol.isKnockBack = true;
                    enemyWeaponCol.knockBackPower = 30;
                    bossWeaponColList.Add(enemyWeaponCol);

                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact07, bossGolem.target.position);

                }
                break;
        }

    }

    public void BossWeaponOut()
    {
        for (int i = 0; i < weaponColliders.Count; i++)
        {
            weaponColliders[i].enabled = false;
        }

        C_bossWeapon = null;
    }
    
}
