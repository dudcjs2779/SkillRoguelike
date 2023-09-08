using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;
using Random = UnityEngine.Random;

public class BossGolem : MonoBehaviour
{
    string attackName;

    public float movementPara;

    public float maxHP, curHP;
    public float maxSHP, curSHP;
    public float maxFireHP, curFireHP;
    public float maxIceHP, curIceHP;
    public float maxLightingHP, curLightingHP;

    float animLength;
    public float patternDelay;

    public bool isDead;
    public bool unlockPatternNormal;
    public bool unlockPatternHard;
    public bool isAttack;
    public bool isJump;
    public bool isStaggering, canStagger;
    public bool isThinking;
    public bool rush_HitWall;
    public bool rush_Hit;
    public bool doStopRush;
    bool isMaxH;
    public bool isEndJump;
    public bool isWalk;
    public bool isAttackAiming;

    private Vector3 startPos, endPos;   //포물선
    public List<string> patternStack = new List<string>();
    public LayerMask playerMask = 1 << 8;

    public Transform target;
    Rigidbody rigid;
    public Animator anim;
    public NavMeshAgent nav;
    SkinnedMeshRenderer meshRenderer;
    Color originColor;
    Coroutine c_AnimMovement;
    Coroutine c_Attack;
    Coroutine c_RetainPattern;

    BossCollision bossCollision;
    BossWeapon bossWeapon;
    EnemyStateBar enemyStateBar;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        originColor = meshRenderer.material.color;
        bossCollision = GetComponentInChildren<BossCollision>();
        bossWeapon = GetComponentInChildren<BossWeapon>();
        enemyStateBar = GameObject.Find("Canvas0").GetComponentInChildren<EnemyStateBar>();
        target = GameObject.Find("Player").transform;
        GetComponentInChildren<RushHit>().player = target.GetComponent<Player>();

        AnimMovement(1);
    }

    private void Start()
    {
        nav.updateRotation = false;
        DiffcultySetting();
    }
    
    void Update()
    {
        if (GameManager.Instance.isCutScene) return;

        if (!isDead)
        {
            if (!isAttack && !isStaggering && !bossCollision.isFrozen || isWalk)
            {
                nav.SetDestination(target.position);
                //Debug.Log("navSetDes");
            }

            Turn();
            if (c_RetainPattern == null && !isStaggering) c_RetainPattern = StartCoroutine(RetainPattern());

            if(movementPara > 0.2f) SoundManager.Instance.PlayDelayGameSound(SoundManager.GameSFXType.Footsteps_Walk_Grass_Big01, transform.position, movementPara);
            anim.SetFloat("move", movementPara);
        }

        enemyStateBar.BossBar(this);

        if (Input.GetButtonDown("TestKey"))
        {
            //isAttackTurn = true;
            //rigid.AddForce(transform.forward * 30, ForceMode.Impulse);
            //Debug.Log("P");
            //rigid.velocity = transform.forward * 10;
        }
    }

    private void AnimMovement(float para)
    {
        if (c_AnimMovement != null) StopCoroutine(c_AnimMovement);
        c_AnimMovement = StartCoroutine(_AnimMovement(para));
    }

    IEnumerator _AnimMovement(float para)
    {
        float start = movementPara;
        float lerp = 0;

        while (lerp < 1)
        {
            movementPara = Mathf.Lerp(start, para, lerp);
            lerp += Time.deltaTime * 3f;
            yield return null;
        }
    }

    //적이 정면에 있을 경우 전진공격을 하기위함
    bool TargetFront(float range)
    {
        bool isFront;
        Vector3 t_direction = (target.position - transform.position).normalized;
        float t_angle = Vector3.Angle(t_direction, transform.forward);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.2f, t_direction, range, playerMask);

        if (hits.Length == 0) return false;

        if (t_angle < 70)
        {
            isFront = true;
        }
        else
        {
            isFront = false;
        }

        return isFront;
    }

    //Attack2 공격하면서 조금 앞으로 나가게 하기 위함
    public IEnumerator AttackMove(float duration = 0.5f)
    {
        //포지션
        Vector3 targetDir = target.position - transform.position;
        Vector3 desDir;
        Vector3 desPos;

        if (targetDir.magnitude > 2) desDir = targetDir - targetDir.normalized * 2;
        else if (targetDir.magnitude > 4) desDir = targetDir.normalized * 2f;
        else desDir = targetDir;
        desPos = desDir + transform.position;

        //각도
        float angle = Vector3.SignedAngle(transform.forward, desDir, Vector3.up);
        angle = Mathf.Clamp(angle, -45, 45);
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        Quaternion desRot = Quaternion.LookRotation(rot * transform.forward); 

        float time = 0;
        while (time < duration && !isStaggering)
        {
            time = time + Time.deltaTime;

            if(desDir.magnitude > 2)
                transform.position = Vector3.Lerp(transform.position, desPos, 3 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desRot, 20f * Time.deltaTime);

            yield return null;
        }
    }

    string ChooseAttack()
    {
        string patternName = "";

        Collider[] meleeAreaCols = Physics.OverlapSphere(transform.position, 8f, playerMask);    //정찰 거리
        //Collider[] longAreaCols = Physics.OverlapSphere(transform.position, 22f, playerMask);    //정찰 거리

        if (meleeAreaCols.Length > 0)
        {
            int meleeRan = Random.Range(0, 100);

            if (meleeRan < 10 && !unlockPatternHard)
            {
                patternName = "Attack1";
            }
            else if (meleeRan < 45)
            {
                patternName = "Attack2";
            }
            else if (meleeRan < 70 && (unlockPatternNormal || unlockPatternHard))
            {
                patternName = "TurnAttack";
            }
            else if (meleeRan < 90)
            {
                patternName = "AttackSmash";
            }
            else if (meleeRan < 95)
            {
                patternName = "AttackJump";
            }
            else if (meleeRan < 100 && unlockPatternHard)
            {
                patternName = "RisingWall";
            }
            else
            {
                patternName = "";
            }
        }
        else
        {
            int longRan = Random.Range(75, 100);

            if (longRan < 15)
            {
                patternName = "Attack2";
            }
            else if (longRan < 20 && (unlockPatternNormal || unlockPatternHard))
            {
                patternName = "TurnAttack";
            }
            else if (longRan < 25)
            {
                patternName = "AttackSmash";
            }
            else if (longRan < 50)
            {
                patternName = "AttackRush";
            }
            else if (longRan < 75)
            {
                patternName = "AttackJump";
            }
            else if (longRan < 100 && unlockPatternHard)
            {
                patternName = "RisingWall";
            }
            else
            {
                patternName = "";
            }
        }

        return patternName;

    }

    TargetingType AttackRangeReturn(string patternName)
    {
        TargetingType targetingType = new TargetingType();

        if (patternName == "Attack1")
        {
            targetingType.range = 3;
            targetingType.isForwardTargeting = true;
        }
        else if (patternName == "Attack2")
        {
            targetingType.range = 3.5f;
            targetingType.isForwardTargeting = true;
        }
        else if (patternName == "TurnAttack")
        {
            targetingType.range = 3f;
            targetingType.isForwardTargeting = false;
        }
        else if (patternName == "AttackSmash")
        {
            targetingType.range = 5f;
            targetingType.isForwardTargeting = true;
        }
        else if (patternName == "AttackRush")
        {
            targetingType.range = 20f;
            targetingType.isForwardTargeting = false;
        }
        else if (patternName == "AttackJump")
        {
            targetingType.range = 22f;
            targetingType.isForwardTargeting = true;
        }
        else if (patternName == "RisingWall")
        {
            targetingType.range = 13f;
            targetingType.isForwardTargeting = false;
        }

        return targetingType;
    }

    // 패턴 컨트롤
    IEnumerator RetainPattern()
    {
        //Debug.Log("RetainPattern");

        string patternName = ChooseAttack();
        TargetingType targetingType = AttackRangeReturn(patternName);

        int ranNum = Random.Range(20, 41);
        float retainTime = ranNum / 10;
        AnimMovement(1);

        while (patternName != "" && retainTime > 0 && !isStaggering)
        {
            Vector3 dir;

            if (targetingType.isForwardTargeting)
                dir = transform.forward;
            else
                dir = target.position - transform.position;

            RaycastHit[] rayHit_attackRange = Physics.SphereCastAll(transform.position, 0.2f, dir, targetingType.range, playerMask);

            if (rayHit_attackRange.Length > 0)
            {
                c_Attack = StartCoroutine(patternName, patternName);
                break;
            }

            retainTime -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitUntil(() => patternStack.Count == 0);
        c_RetainPattern = null;
    }

    //랜덤 패턴 AI
    //=================패턴들=================
    IEnumerator Attack1(string patternName)
    {
        patternStack.Add("Attack1");
        //Debug.Log((transform.position - target.position).magnitude);
        isAttack = true;
        patternDelay = 0.5f;

        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack1", 0.1f);
        //Debug.Log("Golem_" + attackType.ToString());

        bossWeapon.Use(patternName);
        AnimMovement(0);

        animLength = 2;
        yield return new WaitForSeconds(animLength);
        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        isAttack = false;
        patternStack.Remove("Attack1");
    }

    IEnumerator Attack2(string patternName)
    {
        patternStack.Add("Attack2");
        isAttack = true;
        patternDelay = 0.5f;

        nav.isStopped = true;
        nav.velocity = Vector3.zero;


        anim.CrossFade("Attack2", 0.1f);
        bossWeapon.Use(patternName);
        AnimMovement(0);

        yield return new WaitForSeconds(1.967f * 0.6f);

        if (TargetFront(8) && (unlockPatternNormal || unlockPatternHard))
        {
            anim.speed = 1.5f;
            anim.CrossFade("Attack2-2", 0.1f);
            bossWeapon.Use(patternName + "-2");
            yield return new WaitForSeconds(0.11f);
            yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2-2"));
        }
        else
        {
            animLength = 1.967f * 0.4f;
            yield return new WaitForSeconds(animLength);
        }

        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        isAttack = false;
        patternStack.Remove("Attack2");
    }

    IEnumerator TurnAttack(string patternName)
    {
        patternStack.Add(patternName);
        //Debug.Log((transform.position - target.position).magnitude);
        isAttack = true;
        patternDelay = 2f;

        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack_TurnAttack", 0.1f);
        //Debug.Log("Golem_" + attackType.ToString());

        bossWeapon.Use(patternName);
        AnimMovement(0);
        animLength = 2.467f;
        yield return new WaitForSeconds(animLength);

        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        isAttack = false;
        patternStack.Remove(patternName);
    }

    IEnumerator AttackSmash(string patternName)
    {
        patternStack.Add(patternName);
        //Debug.Log((transform.position - target.position).magnitude);
        isAttack = true;
        patternDelay = 1f;

        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack_Smash", 0.1f);
        //Debug.Log("Golem_" + attackType.ToString());

        bossWeapon.Use(patternName);
        AnimMovement(0);

        animLength = 3.967f;
        yield return new WaitForSeconds(animLength);

        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        isAttack = false;
        patternStack.Remove(patternName);
    }

    IEnumerator AttackRush(string patternName)
    {
        // 러쉬 준비
        patternStack.Add(patternName);
        isAttack = true;

        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack_RushReady", 0.1f);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.AirImpact_Small01, transform.position);
        //Debug.Log("Golem_RushReady");

        #region//방향 맞추기 턴
        float time = 0.467f;
        while (time > 0 && !isStaggering)
        {
            Vector3 targetVec = target.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(targetVec);
            transform.rotation = Quaternion.Slerp(rigid.rotation, rot, 6 * Time.deltaTime);
            time -= Time.deltaTime;
            yield return null;
        }
        #endregion

        #region // 러쉬 달리기
        anim.CrossFade("Attack_RushRun", 0.1f);
        bossWeapon.Use(patternName);
        AnimMovement(0);

        float ranTime = Random.Range(4, 6);
        float speed = 0;
        while (ranTime > 0 && !isStaggering)
        {
            if (rush_Hit || rush_HitWall)
                break;

            Vector3 targetVec = target.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(targetVec);
            transform.rotation = Quaternion.RotateTowards(rigid.rotation, rot, 60 * Time.deltaTime);

            speed += Time.deltaTime * 10;
            speed = Mathf.Clamp(speed, 0, 20);
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;

            SoundManager.Instance.PlayDelayGameSound(SoundManager.GameSFXType.Footsteps_Walk_Grass_Big02, transform.position, 1f);

            ranTime -= Time.deltaTime;
            yield return null;
        }
        #endregion

        doStopRush = true;  //벽이나 플레이어를 박지 않았을 경우를 위해서

        // 벽충돌
        if (rush_HitWall)
        {
            patternDelay = 3f;
            anim.CrossFade("Attack_RushHit", 0.1f);
            animLength = 1.8f;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact04, transform.position);
        }
        else
        {
            patternDelay = 1f;
            anim.CrossFade("Attack_RushStop", 0.1f);
            animLength = 0.967f;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.PlayerHit02, transform.position);
        }

        yield return new WaitForSeconds(animLength);
        rush_HitWall = false;
        rush_Hit = false;

        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        isAttack = false;
        patternStack.Remove(patternName);
    }

    IEnumerator AttackJump(string patternName)
    {
        patternStack.Add(patternName);
        isAttack = true;
        isJump = true;
        patternDelay = 0.5f;

        nav.isStopped = true;
        nav.updatePosition = false;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack_Jump1", 0.1f);
        AnimMovement(0);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GroundImpact05, transform.position);

        //jumpVec = transform.position + transform.up * 10;
        yield return new WaitForSeconds(0.78f);
        JumpMove();

        //print("Waiting");
        yield return new WaitUntil(() => isMaxH);   //최고 높이 도달할 시
        animLength = 1.633f;
        yield return new WaitForSeconds(animLength);

        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        nav.updatePosition = true;
        isMaxH = false;
        isAttack = false;
        isJump = false;
        patternStack.Remove(patternName);
    }

    //포물선 운동
    void JumpMove()
    {
        //print("JumpMove");
        startPos = transform.position;
        endPos = startPos + (target.position - transform.position);
        StartCoroutine(ParabolaMove());
    }

    protected static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    protected IEnumerator ParabolaMove()
    {
        float time = 0;
        while (time <= 1 && !isStaggering)
        {
            time += Time.deltaTime;
            Vector3 tempPos = Parabola(startPos, endPos, 10f, time * 1f);
            transform.position = tempPos;
            yield return new WaitForEndOfFrame();

            if (time > 0.5f && !isMaxH)
            {
                isMaxH = true;
                anim.CrossFade("Attack_Jump2", 0.1f);
            }
        }

        transform.position = endPos;
        anim.CrossFade("Attack_Jump3", 0.1f);
        bossWeapon.Use("AttackJump");

        nav.nextPosition = transform.position;  //네비메시 다음 포지션 설정
        isEndJump = true;
    }

    IEnumerator RisingWall(string patternName)
    {
        patternStack.Add(patternName);
        //Debug.Log((transform.position - target.position).magnitude);
        isAttack = true;
        patternDelay = 1f;

        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack_RisingWall", 0.1f);
        //Debug.Log("Golem_" + attackType.ToString());

        bossWeapon.Use(patternName);
        AnimMovement(0);

        animLength = 1.667f;
        yield return new WaitForSeconds(animLength);
        yield return new WaitForSeconds(patternDelay);

        nav.isStopped = false;
        isAttack = false;
        patternStack.Remove(patternName);
    }

    void Turn()
    {
        if ((!isAttack || isAttackAiming) && !isStaggering && !bossCollision.isFrozen)
        {
            Vector3 targetVec = target.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(targetVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }

    public IEnumerator Staggering(float animSpeed)
    {
        isStaggering = true;

        patternStack.Clear();
        if (c_RetainPattern != null) StopCoroutine(c_RetainPattern);
        if (c_Attack != null) StopCoroutine(c_Attack);
        
        isAttack = false;
        rush_Hit = false;
        rush_HitWall = false;
        doStopRush = false;
        isMaxH = false;
        isEndJump = false;
        isWalk = false;
        AnimMovement(0);

        if (bossWeapon.C_bossWeapon != null)
        {
            Debug.Log("bossWeapon StopCoroutine");
            bossWeapon.StopCoroutine(bossWeapon.C_bossWeapon);
        }
        bossWeapon.BossWeaponOut();

        if (bossWeapon.bossWeaponColList.Count > 0)
        {
            for (int i = bossWeapon.bossWeaponColList.Count - 1; i > 0; i--)
            {
                if (!bossWeapon.bossWeaponColList[i].isBullet)
                {
                    bossWeapon.bossWeaponColList.Remove(bossWeapon.bossWeaponColList[i]);
                }
            }
        }

        nav.isStopped = true;
        nav.velocity = Vector3.zero;
        anim.CrossFade("GetHit", 0.1f);
        anim.speed = animSpeed;
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyHit02, transform.position);

        float waitTime = 1 / animSpeed * 1.333f;
        yield return new WaitForSeconds(waitTime);

        c_RetainPattern = null;
        c_Attack = null;
        anim.speed = 1f;
        isStaggering = false;
        nav.isStopped = false;
    }

    public IEnumerator FrozenStaggering()
    {
        isStaggering = true;

        patternStack.Clear();
        if (c_RetainPattern != null) StopCoroutine(c_RetainPattern);
        if (c_Attack != null) StopCoroutine(c_Attack);

        isAttack = false;
        rush_Hit = false;
        rush_HitWall = false;
        doStopRush = false;
        isMaxH = false;
        isEndJump = false;
        isWalk = false;
        AnimMovement(0);

        bossWeapon.BossWeaponOut();

        if (bossWeapon.C_bossWeapon != null)
        {
            bossWeapon.StopCoroutine(bossWeapon.C_bossWeapon);
        }

        if (bossWeapon.bossWeaponColList.Count > 0)
        {
            for (int i = bossWeapon.bossWeaponColList.Count - 1; i >= 0; i--)
            {
                if (!bossWeapon.bossWeaponColList[i].isBullet)
                {
                    bossWeapon.bossWeaponColList.Remove(bossWeapon.bossWeaponColList[i]);
                }
            }
        }
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        yield return new WaitUntil(() => !bossCollision.isFrozen);
        
        anim.speed = 1;
        anim.CrossFade("GetHit", 0.1f);

        yield return new WaitForSeconds(1.333f);

        c_RetainPattern = null;
        c_Attack = null;
        anim.speed = 1f;
        isStaggering = false;
        nav.isStopped = false;
    }

    void DiffcultySetting()
    {
        if (GameManager.Instance.difficultyType == DifficultyType.Easy)
        {
            maxHP = 1200;
            curHP = 1200;
            maxSHP = 250;
            curSHP = 250;
            maxFireHP = 180;
            curFireHP = 180;
            maxIceHP = 170;
            curIceHP = 170;
            maxLightingHP = 180;
            curLightingHP = 180;
            unlockPatternNormal = false;
            unlockPatternHard = false;
            bossWeapon.bossDamage = 45;
        }
        else if (GameManager.Instance.difficultyType == DifficultyType.Normal)
        {
            maxHP = 1500;
            curHP = 1500;
            maxSHP = 290;
            curSHP = 290;
            maxFireHP = 215;
            curFireHP = 215;
            maxIceHP = 190;
            curIceHP = 190;
            maxLightingHP = 215;
            curLightingHP = 215;
            unlockPatternNormal = true;
            unlockPatternHard = false;

            bossWeapon.bossDamage = (int)(bossWeapon.bossDamage * 1.25f);
        }
        else if (GameManager.Instance.difficultyType == DifficultyType.Hard)
        {
            maxHP = 1800;
            curHP = 1800;
            maxSHP = 340;
            curSHP = 340;
            maxFireHP = 250;
            curFireHP = 250;
            maxIceHP = 230;
            curIceHP = 230;
            maxLightingHP = 250;
            curLightingHP = 250;
            unlockPatternNormal = false;
            unlockPatternHard = true;

            bossWeapon.bossDamage = (int)(bossWeapon.bossDamage * 1.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isJump && collision.collider.CompareTag("Player"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider, false);
        }
    }


    private void OnDestroy()
    {
        enemyStateBar.bossHealthBar.gameObject.SetActive(false);
        enemyStateBar.bossSTGBar.gameObject.SetActive(false);
    }

}

class TargetingType
{
    public float range;
    public bool isForwardTargeting;
}
