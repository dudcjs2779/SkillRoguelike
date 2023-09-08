using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage;
    public float enemyDamage;
    public string attackName;
    public string enemyType;

    public Coroutine C_enmeyWeapon;

    public Transform firePoint;
    [SerializeField] Transform[] IcicleFirePos;

    public GameObject lich_Fireball;
    public GameObject[] lich_Icicle;

    public List<EnemyWeaponCol> enemyWeaponColList = new List<EnemyWeaponCol>();

    public BoxCollider weaponCollider;
    Enemy enemy;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>();
        enemy = GetComponentInParent<Enemy>();
    }

    private void Update()
    {

    }

    public void Use(string AttackName)
    {
        if (C_enmeyWeapon != null)
            StopCoroutine(C_enmeyWeapon);
        C_enmeyWeapon = StartCoroutine(Swing(AttackName));
    }

    public IEnumerator Swing(string AttackName)
    {
        //Debug.Log("Swing : " + AttackName);
        attackName = AttackName;

        switch (enemy.enemyType)
        {
            case GameManager.EnemyType.Oak:
                enemyType = GameManager.EnemyType.Oak.ToString();

                if (AttackName == "A1")
                {
                    //Debug.Log("OakA1");

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(weaponCollider, enemyDamage, false);
                    enemyWeaponColList.Add(enemyWeaponCol);
                    enemy.AttackRotate(0.6f);

                    yield return new WaitForSeconds(0.48f);
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.OakSwing01, weaponCollider.gameObject);

                    yield return new WaitForSeconds(0.07f);
                    weaponCollider.enabled = true;

                    yield return new WaitForSeconds(0.2f);
                    weaponCollider.enabled = false;
                    enemyWeaponColList.Remove(enemyWeaponCol);
                }
                else if (AttackName == "A2")
                {
                    //Debug.Log("OakA2");

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(weaponCollider, enemyDamage * 1.3f, false);
                    enemyWeaponCol.isKnockBack = true;
                    enemyWeaponCol.knockBackPower = 10;
                    enemyWeaponColList.Add(enemyWeaponCol);

                    enemy.AttackRotate(0.9f);
                    yield return new WaitForSeconds(0.8f);
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.OakSwing02, weaponCollider.gameObject);

                    yield return new WaitForSeconds(0.08f);
                    weaponCollider.enabled = true;

                    yield return new WaitForSeconds(0.23f);
                    weaponCollider.enabled = false;

                    enemyWeaponColList.Remove(enemyWeaponCol);
                }
                break;

            case GameManager.EnemyType.Lich:
                if (AttackName == "A1")
                {
                    //Debug.Log("Lich_A1");

                    Transform root = transform.root;  //Lich 오브젝트(최상위)
                    enemy.AttackRotate(0.9f);
                    yield return new WaitForSeconds(0.9f);
                    GameObject instantBullet = Instantiate(lich_Fireball, firePoint.position, root.rotation);
                    Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                    EnemyBullet enemyBullet = instantBullet.GetComponent<EnemyBullet>();

                    enemyBullet.enemyType = GameManager.EnemyType.Lich.ToString();
                    enemyBullet.attackName = AttackName;
                    enemyBullet.damage = enemyDamage;
                    enemyBullet.hitSound = SoundManager.GameSFXType.FireHit01_1;
                    enemyBullet.enemyWeaponColList = enemyWeaponColList;

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(enemyBullet.bulletCol, enemyDamage, true);
                    enemyWeaponColList.Add(enemyWeaponCol);

                    instantBullet.transform.eulerAngles = new Vector3(-90, root.rotation.eulerAngles.y, 0);
                    rigidBullet.velocity = root.forward * 10;

                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireShot01, transform.position);
                }
                else if (AttackName == "A2")
                {
                    //Debug.Log("Lich_A2");

                    Transform root = transform.root;  //Lich 오브젝트(최상위)
                    Vector3 targetVec, shootingVec;
                    Quaternion rot;
                    enemy.AttackRotate(0.8f);

                    yield return new WaitForSeconds(0.85f);
                    targetVec = root.forward;

                    //왼쪽
                    yield return new WaitForSeconds(0.1f);

                    rot = Quaternion.Euler(0f, 20f, 0f);
                    shootingVec = rot * targetVec;

                    GameObject instantBullet = Instantiate(lich_Fireball, root.position + shootingVec.normalized * 2.5f + Vector3.up, root.rotation);
                    Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                    EnemyBullet enemyBullet = instantBullet.GetComponent<EnemyBullet>();

                    enemyBullet.enemyType = GameManager.EnemyType.Lich.ToString();
                    enemyBullet.attackName = AttackName;
                    enemyBullet.damage = enemyDamage * 0.75f;
                    enemyBullet.hitSound = SoundManager.GameSFXType.FireHit01_1;
                    enemyBullet.enemyWeaponColList = enemyWeaponColList;

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(enemyBullet.bulletCol, enemyDamage, true);
                    enemyWeaponColList.Add(enemyWeaponCol);

                    instantBullet.transform.eulerAngles = new Vector3(-90, Quaternion.LookRotation(shootingVec).eulerAngles.y, 0);
                    rigidBullet.velocity = shootingVec.normalized * 10;
                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireShot01, root.position + shootingVec.normalized + Vector3.up);
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning03, instantBullet, 1f, true);

                    //중앙
                    yield return new WaitForSeconds(0.1f);
                    shootingVec = targetVec;

                    //if (enemy.isDead)
                    //    break;
                    instantBullet = Instantiate(lich_Fireball, root.position + shootingVec.normalized * 2.5f + Vector3.up, root.rotation);
                    rigidBullet = instantBullet.GetComponent<Rigidbody>();
                    enemyBullet = instantBullet.GetComponent<EnemyBullet>();

                    enemyBullet.enemyType = GameManager.EnemyType.Lich.ToString();
                    enemyBullet.attackName = AttackName;
                    enemyBullet.damage = enemyDamage * 0.75f;
                    enemyBullet.hitSound = SoundManager.GameSFXType.FireHit01_1;
                    enemyBullet.enemyWeaponColList = enemyWeaponColList;

                    enemyWeaponCol = new EnemyWeaponCol(enemyBullet.bulletCol, enemyDamage, true);
                    enemyWeaponColList.Add(enemyWeaponCol);

                    instantBullet.transform.eulerAngles = new Vector3(-90, Quaternion.LookRotation(shootingVec).eulerAngles.y, 0);
                    rigidBullet.velocity = shootingVec.normalized * 10;
                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireShot01, root.position + shootingVec.normalized + Vector3.up);
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning03, instantBullet, 1f, true);

                    //오른쪽
                    yield return new WaitForSeconds(0.1f);
                    rot = Quaternion.Euler(0f, -20f, 0f);
                    shootingVec = rot * targetVec;

                    instantBullet = Instantiate(lich_Fireball, root.position + shootingVec.normalized * 2.5f + Vector3.up, root.rotation);
                    rigidBullet = instantBullet.GetComponent<Rigidbody>();
                    enemyBullet = instantBullet.GetComponent<EnemyBullet>();

                    enemyBullet.enemyType = GameManager.EnemyType.Lich.ToString();
                    enemyBullet.attackName = AttackName;
                    enemyBullet.damage = enemyDamage * 0.75f;
                    enemyBullet.hitSound = SoundManager.GameSFXType.FireHit01_1;
                    enemyBullet.enemyWeaponColList = enemyWeaponColList;

                    enemyWeaponCol = new EnemyWeaponCol(enemyBullet.bulletCol, enemyDamage, true);
                    enemyWeaponColList.Add(enemyWeaponCol);

                    instantBullet.transform.eulerAngles = new Vector3(-90, Quaternion.LookRotation(shootingVec).eulerAngles.y, 0);
                    rigidBullet.velocity = shootingVec.normalized * 10;
                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireShot01, root.position + shootingVec.normalized + Vector3.up);
                    SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning03, instantBullet, 1f, true);
                }
                else if (AttackName == "A3")
                {
                    //Debug.Log("Lich_A3");

                    Transform root = transform.root;  //Lich 오브젝트(최상위)

                    yield return new WaitForSeconds(1.666f);
                    StartCoroutine(IcicleSpawn("Lich_A3", 3));
                }
                break;

            case GameManager.EnemyType.Wolf:
                //Debug.Log(wolfRigid);

                enemyType = GameManager.EnemyType.Wolf.ToString();
                if (AttackName == "A1")
                {
                    Debug.Log("WolfA1");

                    EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(weaponCollider, enemyDamage, false);
                    enemyWeaponColList.Add(enemyWeaponCol);

                    Vector3 lookPos;
                    Vector3 attackDir = transform.forward;

                    Collider playerCol = enemy.T_target.GetComponent<Collider>();
                    Collider enemyCol = enemy.GetComponent<Collider>();
                    Vector3 startPos = enemy.T_target.position;
                    Player player = enemy.T_target.GetComponent<Player>();

                    if (enemy.unlockPattern)
                    {
                        float time = 0.3f;
                        while (time > 0)
                        {
                            lookPos = player.moveVec * 2.5f + enemy.T_target.position;
                            attackDir = (lookPos - transform.position).normalized;

                            if (Vector3.Angle(attackDir, transform.root.forward) > 90)
                            {
                                Debug.Log("Too big Rot");
                                attackDir = attackDir = (enemy.T_target.position - transform.position).normalized;
                            }

                            Quaternion rot = Quaternion.LookRotation(attackDir);
                            transform.root.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);

                            time -= Time.deltaTime;
                            yield return null;
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.3f);
                        attackDir = transform.forward;
                    }

                    yield return new WaitForSeconds(0.4f);
                    Physics.IgnoreCollision(playerCol, enemyCol);
                    weaponCollider.enabled = true;
                    enemy.nav.enabled = false;
                    enemy.rigid.constraints = RigidbodyConstraints.FreezeRotation;
                    enemy.rigid.AddForce(attackDir * 55f, ForceMode.Impulse);

                    SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.WolfBite01, transform.position);

                    yield return new WaitForSeconds(0.2f);
                    enemy.nav.enabled = true;
                    enemy.rigid.velocity = Vector3.zero;
                    enemy.rigid.constraints = RigidbodyConstraints.FreezeAll;
                    Physics.IgnoreCollision(playerCol, enemyCol, false);
                    weaponCollider.enabled = false;
                    enemyWeaponColList.Remove(enemyWeaponCol);


                }
                break;
        }
    }

    public void EnemyWeaponOut()
    {
        weaponCollider.enabled = false;

        if (enemy.enemyType == GameManager.EnemyType.Wolf)
        {
            enemy.rigid.constraints = RigidbodyConstraints.FreezeAll;
            enemy.nav.enabled = true;
        }
    }

    IEnumerator IcicleSpawn(string AttackName, int bulletCount)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            int ran = Random.Range(0, 2);

            GameObject instantBullet = Instantiate(lich_Icicle[ran], IcicleFirePos[i].position, transform.root.rotation, transform.root);
            EnemyBullet enemyBullet = instantBullet.GetComponent<EnemyBullet>();

            enemyBullet.enemyType = GameManager.EnemyType.Lich.ToString();
            enemyBullet.attackName = AttackName;
            enemyBullet.damage = enemyDamage * 0.6f;
            enemyBullet.rigid.isKinematic = true;
            enemyBullet.t_Target = enemy.T_player;
            enemyBullet.hitSound = SoundManager.GameSFXType.IceImpact01;
            enemyBullet.enemyWeaponColList = enemyWeaponColList;

            EnemyWeaponCol enemyWeaponCol = new EnemyWeaponCol(enemyBullet.bulletCol, 10, true);
            enemyWeaponColList.Add(enemyWeaponCol);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact01, instantBullet.transform.position);
            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.IceAmbient01, instantBullet, 1f, true);
            yield return new WaitForSeconds(0.15f);
        }
    }

}

public class EnemyWeaponCol
{
    public Collider col;
    public float damage;
    public float knockBackPower;

    public bool isBullet;
    public bool isBigAttack;
    public bool isKnockBack;

    public EnemyWeaponCol(Collider _col, float _damage, bool _isBullet)
    {
        col = _col;
        damage = _damage;
        isBullet = _isBullet;
    }
}