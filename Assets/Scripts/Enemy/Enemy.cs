using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;
using UnityEngine.EventSystems;
using System;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public bool HpFull;
    public bool isTraining;

    public GameManager.EnemyType enemyType;
    public GameManager.DifficultyType difficulty;

    public float maxHP, curHP;
    public float maxSHP, curSHP;
    public float maxFireHP, curFireHP;
    public float maxIceHP, curIceHP;
    public float maxLightingHP, curLightingHP;
    float animLength;
    public float runSpeed, walkSpeed, strafeSpeed;
    float moveHori, moveVert;
    float thinkDelay = 0;

    public float hitLength;
    
    [SerializeField] float sight_angle;
    [SerializeField] float sight_distance;
    public float trace_distance;
    [SerializeField] float detect_distance;
    [SerializeField] float nearEnemy_distance;
    [SerializeField] float thinkRange;
    [SerializeField] float thinkOutRange;

    public bool isChase;
    public bool isDead;
    public bool isAttack;
    public bool unlockPattern;
    public bool isGuarding;
    public bool doGuard;
    public bool canGuard;
    public bool isMoveAttack;
    public bool isFixedEnemy;
    public bool isStaggering, canStagger;
    public bool isThinking;
    public bool isThinkRange;
    public bool isAttackRange;
    public bool isLootAt;
    public bool doAimMove;
    public bool doLockRotate;
    public bool doAttackRotate;
    public bool isStrafe;
    public bool isProving;
    public bool isHitWall;
    public bool isWallSlip;
    public bool isPlayerTouched;

    public List<string> ThinkStack = new List<string>();

    Coroutine C_Think;
    Coroutine C_hori, C_vert;
    Coroutine C_Attack;
    Coroutine C_Probe;

    public Transform T_player;
    public Transform T_target;
    public Transform T_HP_Bar;
    public Transform T_Eyes;
    [SerializeField] LayerMask player_LayerMask;
    LayerMask enemy_layerMask = 1 << 9;
    [SerializeField] LayerMask raySightMask;

    RaycastHit[] attackRange_Rayhits;

    public Rigidbody rigid;
    public Animator anim;
    public NavMeshAgent nav;

    EnemyCollision enemyCollision;
    EnemyWeapon enemyWeapon;
    EnemyStateBar enemyStateBar;
    public EnemySpawnZone enemySpawnZone;
    public EnemySpawnGroup enemySpawnGroup;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        T_player = GameObject.Find("Player").transform;

        enemyCollision = GetComponentInChildren<EnemyCollision>();
        enemyWeapon = GetComponentInChildren<EnemyWeapon>();
        enemyStateBar = GameObject.Find("Canvas0").GetComponentInChildren<EnemyStateBar>();
    }

    private void Start()
    {
        //체력바 추가
        enemyStateBar.SpawnEnemyBar(this);

        if(enemySpawnZone != null) {
            isFixedEnemy = enemySpawnZone.actType == EnemyActType.Fix;
            if (isFixedEnemy)
            {
                sight_distance = 20;
                thinkRange = 20;
                thinkOutRange = 20;
            }
        }
        
        nav.updateRotation = false;
        DiffcultySetting(GameManager.Instance.difficultyType);
        C_Probe = StartCoroutine(Probe());
    }

    public void ChaseStart()
    {
        T_target = T_player;
        isChase = true;
        nav.isStopped = false;
        if(C_Probe != null) StopCoroutine(C_Probe);

        StartMovementAnim(0f, 1.5f);
    }


    void Update()
    {
        if (HpFull) curHP = maxHP;

        if(isTraining){
            return;
        }

        if (!isDead)
        {
            if (T_target != null && isChase && !isAttack && !isStaggering && nav.enabled == true && !isStrafe && !enemyCollision.isFrozen && !isFixedEnemy)
            {
                //Debug.Log("chase");
                nav.SetDestination(T_target.position);
            }

            if(isChase){
                float delayVal = GameManager.Instance.Remap(nav.velocity.magnitude, 0, runSpeed, 0, 1);
                switch(enemyType){
                    case EnemyType.Oak:
                        SoundManager.Instance.PlayDelayGameSound(SoundManager.GameSFXType.Oak_Footsteps_Grass_01, transform.position, delayVal);
                        break;
                    case EnemyType.Lich:
                        SoundManager.Instance.PlayDelayGameSound(SoundManager.GameSFXType.Lich_Footsteps_Grass_01, transform.position, delayVal);
                        break;
                    case EnemyType.Wolf:
                        SoundManager.Instance.PlayDelayGameSound(SoundManager.GameSFXType.Wolf_Footsteps_Grass_01, transform.position, delayVal);
                        break;
                }
            }

            Sight();
            Targeting();
        }

        if (Input.GetButtonDown("TestKey"))
        {

        }
    }

    void FixedUpdate()
    {
        //FreezeVelocity();

        //Debug.Log(nav.updatePosition);
        

        if (!isDead && !enemyCollision.isFrozen)
        {
            EnemyRotate();
        }
    }


    void EnemyRotate()
    {
        // 회전 코드
        if (isChase && !isAttack && !isStaggering && nav.enabled == true && !isThinkRange && !doLockRotate)
        {
            Vector3 targetVec = T_player.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(targetVec);
            //rigid.rotation = Quaternion.RotateTowards(rigid.rotation, rot, rotSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
        else if (!isAttack && !isStaggering && nav.enabled == true && isThinkRange && !doLockRotate)
        {
            //print("ThinkRange rot");
            Vector3 targetVec = T_player.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(targetVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }

    public void AttackRotate(float duration){
        StartCoroutine(_AttackRotate(duration));
    }

    IEnumerator _AttackRotate(float duration)
    {
        while(duration > 0 && !isStaggering){
            Vector3 targetVec = T_player.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(targetVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
            duration -= Time.deltaTime;
            yield return null;
        }
    }

    void Sight()
    {
        if (enemyCollision.isFrozen) return;
        
        Collider[] sight_cols = Physics.OverlapSphere(transform.position, sight_distance, player_LayerMask);    //정찰 거리
        Collider[] detected_cols = Physics.OverlapSphere(transform.position, detect_distance, player_LayerMask);    //근접시 발각 거리

        if (T_target != null)
        {
            // 추격 거리 밖으로 나가면 추격 중지
            Collider[] trace_cols = Physics.OverlapSphere(transform.position, trace_distance, player_LayerMask);    //추격 거리
            if (trace_cols.Length == 0)
            {
                Debug.Log("추격 종료");
                T_target = null;
                isChase = false;
                nav.isStopped = true;

                if(enemyType == EnemyType.Oak)
                {
                    isGuarding = false;
                    anim.CrossFade("GuardDown", 0.1f, 1);
                }

                StartMovementAnim(0f, 0f);
                C_Probe = StartCoroutine(Probe());
                return;
            }

            if (Vector3.Distance(T_player.position, transform.position) < thinkRange || isThinkRange && Vector3.Distance(T_player.position, transform.position) < thinkOutRange)
            {
                isThinkRange = true;
                if(C_Think == null && !isAttack && !isStaggering && !enemyCollision.isFrozen)
                {
                    C_Think = StartCoroutine(Think());
                }
            }
            else
            {
                isThinkRange = false;
                if (ThinkStack.Count == 0)
                {
                    StartMovementAnim(0f, 1.5f);
                    T_target = T_player;
                    nav.speed = runSpeed;
                }
            }
        }

        switch (enemyType)
        {
            case GameManager.EnemyType.Oak:
            case GameManager.EnemyType.Lich:
            case GameManager.EnemyType.Wolf:
                if (!isChase && !isAttack && !isStaggering && !isThinking && nav.enabled == true)
                {
                    Debug.DrawRay(transform.position, (T_player.position - transform.position).normalized * 10f, Color.red);

                    //정찰 거리 이내
                    if (sight_cols.Length > 0)
                    {
                        Vector3 targetPos = sight_cols[0].transform.position + Vector3.up * 1.5f;
                        Vector3 t_direction = (targetPos - T_Eyes.position).normalized;
                        float t_angle = Vector3.Angle(t_direction, transform.forward);

                        //정찰 시야각 이내
                        if (t_angle < sight_angle * 0.5f)
                        {
                            if (Physics.Raycast(T_Eyes.position, t_direction, out RaycastHit t_hit, sight_distance, raySightMask))
                            {
                                //플레이어만 탐지
                                if (t_hit.transform.name == "Player")
                                {
                                    Debug.Log("시야각 탐지");
                                    ChaseStart();
                                    nav.speed = runSpeed;

                                    NearEnemyWakeUp(6f);
                                }
                            }
                        }
                        //근접 발각 거리 이내
                        else if (isPlayerTouched)
                        {
                            //Debug.Log("근접 탐지");
                            ChaseStart();
                            nav.speed = runSpeed;
                            NearEnemyWakeUp(6f);
                        }
                    }
                }
                break;
        }
    }

    public void NearEnemyWakeUp(float detectRange){
        Collider[] nearEnemyCols = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Enemy"));    //정찰 거리
        //주변 적 합세
        if (!isTraining && nearEnemyCols.Length > 0)
        {
            foreach (Collider nearEnemyCol in nearEnemyCols)
            {
                //Debug.Log(nearEnemyCol.transform.root.name);
                Enemy nearEnemy = nearEnemyCol.GetComponent<Enemy>();
                if (!nearEnemy.isTraining && nearEnemy.T_target == null)
                {
                    nearEnemy.ChaseStart();
                }
            }
        }
    }

    void Targeting()
    {
        if (!isDead && !isAttack && !isStaggering)
        {
            float attackRange = 0;

            switch (enemyType)
            {
                case GameManager.EnemyType.Oak:
                    attackRange = 2.4f;
                    break;

                case GameManager.EnemyType.Lich:
                    attackRange = 20f;
                    break;

                case GameManager.EnemyType.Wolf:
                    attackRange = 9f;
                    break;
            }

            //attackRange_Rayhits = Physics.RaycastAll(transform.position, transform.forward, attackRange, LayerMask.GetMask("Player"));
            attackRange_Rayhits = Physics.SphereCastAll(transform.position, 0.2f, transform.forward, attackRange, LayerMask.GetMask("Player"));

            //Debug.DrawRay(transform.position, transform.forward * targetRange, Color.blue);
        }
    }

    IEnumerator Probe()
    {
        //혼자 정찰
        if (enemySpawnZone != null && enemySpawnZone.actType == GameManager.EnemyActType.Walk && T_target == null)
        {
            //Debug.Log("Probe");
            isProving = true;

            if (!enemySpawnZone.isNonStopProbe)
                yield return new WaitForSeconds(2f);
            
            nav.isStopped = false;
            StartMovementAnim(0f, 1f);
            nav.speed = walkSpeed;

            if (enemySpawnGroup == null)
            {
                //Debug.Log("Nomal Probe");
                
                for (int i = 0; i < enemySpawnZone.probePoints.Length; i++)
                {
                    nav.SetDestination(enemySpawnZone.probePoints[i].transform.position);
                    Quaternion rotation = Quaternion.LookRotation(enemySpawnZone.probePoints[i].transform.position - transform.position);
                    StartCoroutine(_ProbeTurn(rotation, 1f));

                    yield return new WaitUntil(() => Vector3.Distance(transform.position, enemySpawnZone.probePoints[i].transform.position) < 0.5f);
                }

                if (!enemySpawnZone.isNonStopProbe)
                {
                    StartMovementAnim(0f, 0f);
                    StartCoroutine(_ProbeTurn(enemySpawnZone.probePoints[enemySpawnZone.probePoints.Length - 1].transform.rotation, 1f));
                    yield return new WaitForSeconds(5f);
                    StartMovementAnim(0f, 1f);
                }

                for (int i = enemySpawnZone.probePoints.Length - 1; i >= 0; i--)
                {
                    nav.SetDestination(enemySpawnZone.probePoints[i].transform.position);
                    Quaternion rotation = Quaternion.LookRotation(enemySpawnZone.probePoints[i].transform.position - transform.position);
                    StartCoroutine(_ProbeTurn(rotation, 1f));

                    yield return new WaitUntil(() => Vector3.Distance(transform.position, enemySpawnZone.probePoints[i].transform.position) < 0.5f);
                }

                if (!enemySpawnZone.isNonStopProbe)
                {
                    StartMovementAnim(0f, 0f);
                    StartCoroutine(_ProbeTurn(enemySpawnZone.probePoints[0].transform.rotation, 1f));
                    yield return new WaitForSeconds(5f);
                }

            }   //그룹 정찰
            else if (enemySpawnGroup != null && enemySpawnGroup.isGroupProbe)
            {
                //Debug.Log(enemySpawnGroup.gameObject.name + " : Group Probe");

                for (int i = 0; i < enemySpawnZone.probePoints.Length; i++)
                {
                    nav.SetDestination(enemySpawnZone.probePoints[i].transform.position);
                    Quaternion rotation = Quaternion.LookRotation(enemySpawnZone.probePoints[i].transform.position - transform.position);
                    StartCoroutine(_ProbeTurn(rotation, 1f));

                    yield return new WaitUntil(() => Vector3.Distance(transform.position, enemySpawnZone.probePoints[i].transform.position) < 0.5f);
                    enemySpawnGroup.arrivedStack++;

                    if(i == enemySpawnZone.probePoints.Length - 1 && !enemySpawnZone.isNonStopProbe)
                    {
                        StartMovementAnim(0f, 0f);
                        StartCoroutine(_ProbeTurn(enemySpawnZone.probePoints[enemySpawnZone.probePoints.Length - 1].transform.rotation, 1f));
                    }

                    yield return new WaitUntil(() => enemySpawnGroup.isAllArrive);
                    enemySpawnGroup.arrivedStack = 0;
                }

                if (!enemySpawnZone.isNonStopProbe)
                {
                    yield return new WaitForSeconds(5f);
                    StartMovementAnim(0f, 1f);
                }

                for (int i = enemySpawnZone.probePoints.Length - 1; i >= 0; i--)
                {
                    nav.SetDestination(enemySpawnZone.probePoints[i].transform.position);
                    Quaternion rotation = Quaternion.LookRotation(enemySpawnZone.probePoints[i].transform.position - transform.position);
                    StartCoroutine(_ProbeTurn(rotation, 1f));

                    yield return new WaitUntil(() => Vector3.Distance(transform.position, enemySpawnZone.probePoints[i].transform.position) < 0.5f);
                    enemySpawnGroup.arrivedStack++;

                    if (i == 0 && !enemySpawnZone.isNonStopProbe)
                    {
                        StartMovementAnim(0f, 0f);
                        StartCoroutine(_ProbeTurn(enemySpawnZone.probePoints[0].transform.rotation, 1f));
                    }

                    yield return new WaitUntil(() => enemySpawnGroup.isAllArrive);
                    enemySpawnGroup.arrivedStack = 0;
                }

                if (!enemySpawnZone.isNonStopProbe)
                {
                    yield return new WaitForSeconds(5f);
                }
            }

            if(T_target == null){
                C_Probe = StartCoroutine(Probe());
            }
            else{
                isProving = false;
            }
        }
        else if(enemySpawnZone != null && enemySpawnZone.actType == GameManager.EnemyActType.Stand && T_target == null){

            isProving = true;

            nav.isStopped = false;
            StartMovementAnim(0f, 1f);
            nav.speed = walkSpeed;

            nav.SetDestination(enemySpawnZone.transform.position);
            Quaternion rotation = Quaternion.LookRotation(enemySpawnZone.transform.position - transform.position);
            StartCoroutine(_ProbeTurn(rotation, 1f));

            yield return new WaitUntil(() => Vector3.Distance(transform.position, enemySpawnZone.transform.position) < 0.5f);
            StartMovementAnim(0f, 0f);
            StartCoroutine(_ProbeTurn(enemySpawnZone.transform.rotation, 1f));

            nav.isStopped = true;
            isProving = false;
        }
    }

    IEnumerator _ProbeTurn(Quaternion tagetRot, float time){
        Quaternion nowRot = transform.rotation;
        float lerp = 0;

        while(lerp < 1 && !isChase){
            transform.rotation = Quaternion.Slerp(nowRot, tagetRot, lerp);
            lerp += Time.deltaTime * 1 / time;
            yield return null;
        }
    }

    IEnumerator Think()
    {
        //Debug.Log("Think");
        //Debug.Log(isAttackRange);

        isThinking = true;
        yield return new WaitForSeconds(thinkDelay);
        
        if(!isStaggering && !enemyCollision.isFrozen)
        {
            if (enemyType == GameManager.EnemyType.Oak)
            {
                int ranAction = Random.Range(0, 100);
                if (ranAction < 40)
                {
                    StartCoroutine(MellePattern_RunAndAttact());
                }
                else if (ranAction < 70)
                {
                    StartCoroutine(MelleStrafe());
                }
                else if (ranAction < 100)
                {
                    StartCoroutine(MellePattern_WalkAndAttact());
                }
            }
            else if (enemyType == GameManager.EnemyType.Lich)
            {
                int ranAction = Random.Range(0, 100);

                if (ranAction < 60 && !isFixedEnemy)
                {
                    StartCoroutine(RangeStrafe());
                }
                else if (ranAction < 100)
                {
                    StartCoroutine(StandAttack());
                }
            }
            else if (enemyType == GameManager.EnemyType.Wolf)
            {
                int ranAction = Random.Range(0, 100);
                if (ranAction < 20 && attackRange_Rayhits.Length > 0)
                {
                    C_Attack = StartCoroutine(Attack1());
                }
                else if (ranAction < 60)
                {
                    StartCoroutine(Wolf_Strafe());
                }
                else if (ranAction < 100)
                {
                    StartCoroutine(Wolf_WalkAndAttact());
                }
            }
        }

        yield return new WaitUntil(() => ThinkStack.Count == 0);

        isThinking = false;
        C_Think = null;
    }

    IEnumerator Attack1()
    {
        //print("Attack1");
        ThinkStack.Add("Attack1");

        isAttack = true;
        isChase = false;
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("Attack1", 0.25f);

        switch (enemyType)
        {
            case EnemyType.Oak:
                animLength = 2.033f;
                thinkDelay = 0.1f;
                break;

            case EnemyType.Lich:
                animLength = 1.333f;
                thinkDelay = 0.1f;
                break;

            case EnemyType.Wolf:
                animLength = 1.7f;
                thinkDelay = 0.1f;
                break;
        }

        animLength = animLength - 0.25f;
        enemyWeapon.Use("A1");

        yield return new WaitForSeconds(animLength);

        if(enemyType == EnemyType.Wolf){
            int ranRun = Random.Range(0, 100);
            if (ranRun < 60 && Vector3.Distance(transform.position, T_target.position) < 4f)
            {
                isAttack = false;
                if (T_target != null && nav.enabled)
                {
                    isChase = true;
                    nav.isStopped = false;
                }
                ThinkStack.Remove("Attack1");

                StartCoroutine(Wolf_RunAway());
                yield break;
            }
        }

        Vector3 targetVec = T_player.position - transform.position;
        targetVec.y = 0;

        if (Vector3.Angle(transform.forward, targetVec) > 90)
        {
            StartCoroutine(Turn(targetVec));
        }

        yield return new WaitUntil(() => !ThinkStack.Contains("Turn"));

        isAttack = false;
        if (T_target != null && nav.enabled)
        {
            isChase = true;
            nav.isStopped = false;
        }
        ThinkStack.Remove("Attack1");
    }

    IEnumerator Attack2()
    {
        //print("Attack2");
        ThinkStack.Add("Attack2");

        isAttack = true;
        isChase = false;
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        //anim.SetTrigger("attack2");
        anim.CrossFade("Attack2", 0.25f);

        if (enemyType == GameManager.EnemyType.Oak)
        {
            animLength = 2.967f;
            thinkDelay = 0.1f;
        }
        else if (enemyType == GameManager.EnemyType.Lich)
        {
            animLength = 2.333f;
            thinkDelay = 0.1f;
        }
        animLength = animLength - 0.25f;
        enemyWeapon.Use("A2");

        yield return new WaitForSeconds(animLength);

        Vector3 targetVec = T_player.position - transform.position;
        targetVec.y = 0;

        if (Vector3.Angle(transform.forward, targetVec) > 90)
        {
            StartCoroutine(Turn(targetVec));
        }

        yield return new WaitUntil(() => !ThinkStack.Contains("Turn"));
        
        isAttack = false;
        if (T_target != null)
        {
            isChase = true;
            nav.isStopped = false;
        }
        ThinkStack.Remove("Attack2");
    }

    IEnumerator Attack3()
    {
        //print("Attack3");
        ThinkStack.Add("Attack3");

        isAttack = true;
        isChase = false;
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        //anim.SetTrigger("attack2");
        anim.CrossFade("Attack3", 0.25f);

        switch (enemyType)
        {
            case EnemyType.Oak:
                break;
            case EnemyType.Lich:
                animLength = 2.467f;
                thinkDelay = 0.2f;
                break;
            case EnemyType.Wolf:
                break;
            default:
                break;
        }

        animLength = animLength - 0.25f;
        enemyWeapon.Use("A3");

        yield return new WaitForSeconds(animLength);

        Vector3 targetVec = T_player.position - transform.position;
        targetVec.y = 0;

        if (Vector3.Angle(transform.forward, targetVec) > 90)
        {
            StartCoroutine(Turn(targetVec));
        }

        yield return new WaitUntil(() => !ThinkStack.Contains("Turn"));

        isAttack = false;
        if (T_target != null)
        {
            isChase = true;
            nav.isStopped = false;
        }
        ThinkStack.Remove("Attack3");
    }

    IEnumerator MellePattern_RunAndAttact()
    {
        //print("MellePattern_RunAndAttact");

        ThinkStack.Add("MellePattern_RunAndAttact");

        if (isGuarding)
        {
            isGuarding = false;
            anim.CrossFade("GuardDown", 0.1f, 1);
        }

        float time = 0;
        float ranTime = Random.Range(20, 50);
        ranTime = ranTime / 10;
        nav.speed = runSpeed;
        StartMovementAnim(0f, 1.5f);

        while (time < ranTime && !isStaggering)
        {
            //print(time);
            time = time + Time.deltaTime;

            if (attackRange_Rayhits.Length > 0)
            {
                //print("공격1 발동");
                C_Attack = StartCoroutine(Attack1());
                break;
            }
            yield return null;
        }

        ThinkStack.Remove("MellePattern_RunAndAttact");
    }

    IEnumerator MellePattern_WalkAndAttact()
    {
        //Debug.Log("WalkAndAttact");
        //print("MellePattern_WalkAndAttact");
        ThinkStack.Add("MellePattern_WalkAndAttact");

        float ranAttack = Random.Range(0, 10);
        float time = 0;
        float ranTime = Random.Range(15, 30);
        ranTime = ranTime / 10;
        nav.speed = walkSpeed;
        StartMovementAnim(0f, 0.5f);

        StartCoroutine(GuardUp());

        while (time < ranTime && !isStaggering)
        {
            //print(time);
            time = time + Time.deltaTime;

            if (attackRange_Rayhits.Length > 0)
            {
                if (doGuard || isGuarding)
                {
                    GuardDown();
                }

                if (ranAttack < 5)
                {
                    C_Attack = StartCoroutine(Attack1());
                }
                else
                {
                    C_Attack = StartCoroutine(Attack2());
                }

                break;
            }
            yield return null;
        }

        nav.speed = runSpeed;
        canGuard = false;
        ThinkStack.Remove("MellePattern_WalkAndAttact");
    }

    IEnumerator MelleStrafe()
    {
        //print("MelleStrafe");
        ThinkStack.Add("MelleStrafe");
        isStrafe = true;

        float ran = Random.Range(0, 2);
        float time = 0;
        float ranTime = Random.Range(15, 25);
        ranTime = ranTime / 10;
        nav.speed = strafeSpeed;

        StartCoroutine(GuardUp());

        if (ran == 0)
        {
            StartMovementAnim(-0.5f, 0f);
        }
        else
        {
            StartMovementAnim(0.5f, 0f);
        }

        while (time < ranTime && !isStaggering)
        {
            time = time + Time.deltaTime;

            Vector3 targetVec = T_player.position - transform.position;
            Vector3 moveDir = Vector3.Cross(targetVec, ran == 0 ? Vector3.up : Vector3.down);
            Vector3 lookPos = targetVec;

            nav.SetDestination(transform.position + moveDir);
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);

            yield return null;
        }

        float ranPat = Random.Range(0, 10);
        if (attackRange_Rayhits.Length > 0 && !isStaggering)
        {
            if (doGuard || isGuarding)
            {
                GuardDown();
            }

            if (ranPat < 5)
            {
                C_Attack = StartCoroutine(Attack1());
            }
            else
            {
                C_Attack = StartCoroutine(Attack2());
            }
        }

        nav.speed = runSpeed;
        isStrafe = false;
        ThinkStack.Remove("MelleStrafe");
    }

    IEnumerator GuardUp(){
        float ranGuard = Random.Range(0, 10);

        if (unlockPattern && ranGuard < 7)
        {
            doGuard = true;
            anim.CrossFade("GuardUp", 0.1f, 1);
            yield return new WaitForSeconds(0.2f);
            doGuard = false;
            if(!isStaggering && !isAttack) {
                //Debug.Log("isGuarding true");
                isGuarding = true;
            }
        }
        else if (isGuarding)
        {
            isGuarding = false;
            anim.CrossFade("GuardDown", 0.1f, 1);
        }
    }

    void GuardDown(){
        doGuard = false;
        isGuarding = false;
        anim.CrossFade("GuardDown", 0.05f, 1);
    }

    IEnumerator RangeStrafe()
    {
        //print("RangeStrafe");
        ThinkStack.Add("RangeStrafe");
        isStrafe = true;

        int ran = Random.Range(0, 361);
        Vector3 moveAnimParaVec = (Quaternion.AngleAxis(ran, Vector3.up) * Vector3.forward).normalized * 0.5f;

        float time = 0;
        float ranTime = Random.Range(20, 40);

        ranTime = ranTime / 10;
        nav.speed = strafeSpeed;

        StartMovementAnim(moveAnimParaVec.x, moveAnimParaVec.z);
        StartCoroutine(KeepAttack());
        
        while (time < ranTime && !isStaggering && !enemyCollision.isFrozen)
        {
            Vector3 targetVec = T_player.position - transform.position;
            Vector3 strafeDir = Quaternion.AngleAxis(ran, Vector3.up) * targetVec;
            Vector3 lookPos = targetVec;

            nav.SetDestination(transform.position + strafeDir.normalized * 3f);
            if (!isAttack)
            {
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
            }

            time = time + Time.deltaTime;
            yield return null;
        }

        isStrafe = false;
        ThinkStack.Remove("RangeStrafe");
    }

    IEnumerator KeepAttack()
    {
        //print("KeepAttack");
        ThinkStack.Add("KeepAttack");

        int ranAttack = Random.Range(0, 100);
        string animStr = "";
        string attackName = "";

        if (ranAttack < 35)
        {
            animStr = "Attack1";
            attackName = "A1";
            animLength = 1.333f;
            thinkDelay = 0.1f;
        }
        else if (ranAttack < 70)
        {
            animStr = "Attack2";
            attackName = "A2";
            animLength = 2.333f;
            thinkDelay = 0.1f;
        }
        else if(unlockPattern && ranAttack < 100)
        {
            animStr = "Attack3";
            attackName = "A3";
            animLength = 2.467f;
            thinkDelay = 0.2f;
        }
        else{
            ThinkStack.Remove("KeepAttack");
            StartCoroutine(KeepAttack());
            yield break;
        }
        
        float time = 0;
        float ranWaitTime = Random.Range(0, 30);
        ranWaitTime = ranWaitTime / 10;

        float ranMaxTime = Random.Range(60, 100);
        ranMaxTime = ranMaxTime / 10;

        while (ranMaxTime > time && !isStaggering && !enemyCollision.isFrozen)
        {
            if(time > ranWaitTime && attackRange_Rayhits.Length > 0)
            {
                C_Attack = StartCoroutine(MoveAttack(animStr, attackName, animLength));
                break;
            }

            time = time + Time.deltaTime;
            yield return null;
        }

        ThinkStack.Remove("KeepAttack");
    }

    IEnumerator MoveAttack(string animStr, string attackName, float animLength)
    {
        //print("MoveAttack: " + animStr);

        ThinkStack.Add("MoveAttack");
        isAttack = true;
        anim.CrossFade(animStr, 0.25f, 1);

        animLength = animLength - 0.25f;

        enemyWeapon.Use(attackName);

        yield return new WaitForSeconds(animLength);

        Vector3 targetVec = T_player.position - transform.position;
        targetVec.y = 0;

        if (Vector3.Angle(transform.forward, targetVec) > 90)
        {
            StartCoroutine(Turn(targetVec));
        }

        yield return new WaitUntil(() => !ThinkStack.Contains("Turn"));

        isAttack = false;
        ThinkStack.Remove("MoveAttack");
    }

    IEnumerator StandAttack()
    {
        //print("StandAttack");
        ThinkStack.Add("StandAttack");

        float time = 0;
        float ranWaitTime = Random.Range(0, 30);
        ranWaitTime = ranWaitTime / 10;

        float ranMaxTime = Random.Range(50, 80);
        ranMaxTime = ranMaxTime / 10;

        isChase = false;
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        StartMovementAnim(0, 0);

        int ranAttack = Random.Range(0, 100);
        Action doAttack;
        if (ranAttack < 50)
        {
            doAttack = () => { C_Attack = StartCoroutine(Attack1()); };

        }
        else if (ranAttack < 75)
        {
            doAttack = () => { C_Attack = StartCoroutine(Attack2());};
        }
        else if (unlockPattern && ranAttack < 100)
        {
            doAttack = () => { C_Attack = StartCoroutine(Attack3());};
        }
        else
        {
            ThinkStack.Remove("StandAttack");
            StartCoroutine(StandAttack());
            yield break;
        }

        while (time < ranMaxTime && !isStaggering && !enemyCollision.isFrozen)
        {
            if (time > ranWaitTime && attackRange_Rayhits.Length > 0)
            {
                doAttack?.Invoke();
                break;
            }
            time = time + Time.deltaTime;
            yield return null;
        }

        ThinkStack.Remove("StandAttack");
    }

    IEnumerator Wolf_Strafe()
    {
        //print("Wolf_Strafe");
        ThinkStack.Add("Wolf_Strafe");
        isStrafe = true;

        float ran = Random.Range(0, 2);
        float time = 0;
        nav.speed = strafeSpeed;
        if(Random.Range(0, 10) < 6) SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.WolfGrow01, gameObject);

        //정 90도 횡이동
        if (ran == 0)
        {
            //print("일반 스트라페");
            float ranTime = Random.Range(3, 30);
            ranTime = ranTime / 10;

            if (ran == 0) StartMovementAnim(-0.5f, 0f);
            else StartMovementAnim(0.5f, 0f);
            
            while (time < ranTime && !isStaggering && !enemyCollision.isFrozen)
            { 
                time = time + Time.deltaTime;

                Vector3 targetVec = T_player.position - transform.position;
                Vector3 moveDir = Vector3.Cross(targetVec, ran == 0 ? Vector3.up : Vector3.down);
                Vector3 lookPos = targetVec;

                if (nav.enabled)
                    nav.SetDestination(transform.position + moveDir);

                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);

                yield return null;
            }
        }
        else    //사선 횡이동
        {
            //print("사선 스트라페");
            float ranTime = Random.Range(3, 25);
            ranTime = ranTime / 10;
            int ranNum = Random.Range(0, 2);
            int ranAngle;

            if (ranNum == 0)
            {
                ranAngle = Random.Range(30, 81);
                Vector3 moveAnimParaVec = (Quaternion.AngleAxis(ranAngle, Vector3.up) * Vector3.forward).normalized * 0.5f;
                StartMovementAnim(moveAnimParaVec.x, moveAnimParaVec.z);
            }
            else
            {
                ranAngle = Random.Range(-80, -31);
                Vector3 moveAnimParaVec = (Quaternion.AngleAxis(ranAngle, Vector3.up) * Vector3.forward).normalized * 0.5f;
                StartMovementAnim(moveAnimParaVec.x, moveAnimParaVec.z);
            }
            

            while (time < ranTime && !isStaggering && !enemyCollision.isFrozen)
            {
                time = time + Time.deltaTime;

                Vector3 targetVec = T_player.position - transform.position;
                Vector3 strafeDir = Quaternion.AngleAxis(ranAngle, Vector3.up) * targetVec;
                Vector3 lookPos = targetVec;

                nav.SetDestination(transform.position + strafeDir);

                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
                
                yield return null;
            }
        }

        if (attackRange_Rayhits.Length > 0 && !isStaggering && !enemyCollision.isFrozen)
        {
            float ranPat = Random.Range(0, 100);
            if (ranPat < 80)
            {
                C_Attack = StartCoroutine(Attack1());
            }
            else if (ranPat < 100 && Vector3.Distance(transform.position, T_target.position) < 4f)
            {
                StartCoroutine(Wolf_RunAway());
            }
        }
        
        isStrafe = false;
        ThinkStack.Remove("Wolf_Strafe");
    }

    IEnumerator Wolf_WalkAndAttact()
    {
        //print("Wolf_WalkAndAttact");
        ThinkStack.Add("Wolf_WalkAndAttact");

        float time = 0f;
        float ranTime = Random.Range(3, 20);
        ranTime = ranTime / 10;
        nav.speed = strafeSpeed;
        StartMovementAnim(0f, 0.5f);

        if (Random.Range(0, 10) < 7) SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.WolfGrow01, gameObject);

        if (attackRange_Rayhits.Length > 0 && !isStaggering && !enemyCollision.isFrozen)
        {
            float ranPat = Random.Range(0, 100);
            if (ranPat < 60)
            {
                C_Attack = StartCoroutine(Attack1());
                ThinkStack.Remove("Wolf_WalkAndAttact");
                yield break;
            }
        }

        while (time < ranTime && !isStaggering && !enemyCollision.isFrozen)
        {
            time = time + Time.deltaTime;

            if (Vector3.Distance(transform.position, T_player.position) <= 4.5f)
            {
                nav.speed = 0;
                StartMovementAnim(0f, 0);
            }

            yield return null;
        }

        if (attackRange_Rayhits.Length > 0 && !isStaggering && !enemyCollision.isFrozen)
        {
            float ranPat = Random.Range(0, 100);
            if (ranPat < 80)
            {
                C_Attack = StartCoroutine(Attack1());
            }
            else if (ranPat < 100 && Vector3.Distance(transform.position, T_target.position) < 4f)
            {
                StartCoroutine(Wolf_RunAway());
            }
        }

        ThinkStack.Remove("Wolf_WalkAndAttact");
    }

    IEnumerator Wolf_RunAway()
    {
        //print("Wolf_RunAway");
        ThinkStack.Add("Wolf_RunAway");

        float time = 0;
        float ran_RunAwayTime = Random.Range(25, 40);
        ran_RunAwayTime = ran_RunAwayTime / 10;
        nav.speed = 0;
        isChase = false;
        doLockRotate = true;

        #region//뒤로 돌기
        StartMovementAnim(0, 0.5f);
        Vector3 dir = transform.position - T_target.position;
        Quaternion rotation1 = Quaternion.LookRotation(dir);
        StartCoroutine(_ProbeTurn(rotation1, 0.3f));
        yield return new WaitForSeconds(0.3f);
        #endregion

        #region//도망가기
        nav.speed = runSpeed;
        StartMovementAnim(0, 1.5f);

        Vector3 turnDir = Vector3.zero;
        while (ran_RunAwayTime > 0 && !isStaggering && !enemyCollision.isFrozen)
        {
            Vector3 runVec = transform.forward * 3f;
            RaycastHit[] rays = Physics.RaycastAll(transform.position + Vector3.up * 0.5f, transform.forward, 3f, LayerMask.GetMask("Wall", "InvisibleWall"));
            if(rays.Length > 0)
            {
                turnDir = Vector3.ProjectOnPlane(transform.forward, rays[0].normal);
            }

            if(turnDir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(turnDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 3);
            }
            
            if (nav.enabled)
            {
                nav.SetDestination(transform.position + runVec);
            }

            if(Vector3.Distance(T_player.position ,transform.position) > 15)
            {
                break;
            }

            float delayVal = GameManager.Instance.Remap(nav.velocity.magnitude, 0, runSpeed, 0, 1);
            SoundManager.Instance.PlayDelayGameSound(SoundManager.GameSFXType.Wolf_Footsteps_Grass_01, transform.position, delayVal);
            ran_RunAwayTime -= Time.deltaTime;
            yield return null;
        }
        #endregion

        #region//회전
        time = 0f;
        nav.speed = runSpeed * 0.6f;

        while (time < 2f && !isStaggering && !enemyCollision.isFrozen)
        {
            time = time + Time.deltaTime;

            Vector3 runVec = transform.forward * 2;

            if (nav.enabled)
            {
                nav.SetDestination(transform.position + runVec);
            }

            Vector3 targetVec = T_player.position - transform.position;
            Vector3 lookPos = targetVec;
            lookPos.y = 0;
            Quaternion rotation3 = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation3, Time.deltaTime * 240f);

            if(time > 0.5f)
            {
                if(Vector3.Angle(transform.forward, lookPos) < 10)
                {
                    break;
                }
            }

            yield return null;
        }
        #endregion

        isChase = true;
        doLockRotate = false;
        ThinkStack.Remove("Wolf_RunAway");
    }

    IEnumerator Turn(Vector3 targetVec)
    {
        //print("Trun");
        ThinkStack.Add("Turn");
        Quaternion rotation = Quaternion.LookRotation(targetVec);
        StartMovementAnim(0, 0.5f);

        float turnLength;
        if (enemyType == GameManager.EnemyType.Oak)
        {
            turnLength = 0.5f;
        }
        else if (enemyType == GameManager.EnemyType.Lich)
        {
            turnLength = 0.7f;
        }
        else if(enemyType == GameManager.EnemyType.Wolf)
        {
            turnLength = 0.5f;
        }
        else
        {
            turnLength = 0.6f;
        }

        StartCoroutine(_ProbeTurn(rotation, turnLength));
        yield return new WaitForSeconds(turnLength); ;

        ThinkStack.Remove("Turn");
    }

    public IEnumerator Staggering(float animSpeed)
    {
        //Debug.Log("Staggering");
        isStaggering = true;
        isChase = false;
        isAttack = false;
        isMoveAttack = false;
        isStrafe = false;
        doLockRotate = false;
        nav.speed = runSpeed;
        if (C_Attack != null) StopCoroutine(C_Attack);
        doGuard = false;
        isGuarding = false;
        anim.CrossFade("Empty", 0.1f, 1);

        switch (enemyType)
        {
            case EnemyType.Oak:
                animLength = 1.333f;
                break;

            case EnemyType.Lich:
                animLength = 1.333f;
                break;

            case EnemyType.Wolf:
                animLength = 1;
                break;
        }

        if (enemyWeapon.C_enmeyWeapon != null) 
        {
            Debug.Log("Stop Enemy Weapon");
            enemyWeapon.StopCoroutine(enemyWeapon.C_enmeyWeapon);
        }

        if (enemyType == GameManager.EnemyType.Oak || enemyType == GameManager.EnemyType.Wolf)
            enemyWeapon.EnemyWeaponOut();

        if (enemyWeapon.enemyWeaponColList.Count > 0)
        {
            for (int i = enemyWeapon.enemyWeaponColList.Count - 1; i >= 0; i--)
            {
                if (!enemyWeapon.enemyWeaponColList[i].isBullet)
                {
                    enemyWeapon.enemyWeaponColList.Remove(enemyWeapon.enemyWeaponColList[i]);
                }
            }
        }

        if (C_Think != null)
        {
            StopCoroutine(C_Think);
            C_Think = null;
            ThinkStack.Clear();
            isThinking = false;
        }
        
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        anim.CrossFade("GetHit", 0.1f, 0);
        anim.speed = animSpeed;

        float waitTime = 1 / animSpeed * animLength;
        //Debug.Log("애니 : " + animSpeed + "  대기 : " + waitTime);

        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.EnemyHit02, transform.position);

        yield return new WaitForSeconds (waitTime - 0.2f);
        //Debug.Log("스턴 끝");

        if(!isTraining){
            isChase = true;
            nav.isStopped = false;
            StartMovementAnim(0f, 1.5f);
        }

        anim.speed = 1f;
        isStaggering = false;
    }

    public IEnumerator FrozenStaggering()
    {
        //Debug.Log("FrozenStaggering");
        isStaggering = true;
        isChase = false;
        isAttack = false;
        isMoveAttack = false;
        isStrafe = false;
        doLockRotate = false;
        nav.speed = runSpeed;
        if (C_Attack != null) StopCoroutine(C_Attack);
        isGuarding = false;
        anim.CrossFade("GuardDown", 0.1f, 1);

        switch (enemyType)
        {
            case EnemyType.Oak:
                animLength = 1.333f;
                break;

            case EnemyType.Lich:
                animLength = 1.333f;
                break;

            case EnemyType.Wolf:
                animLength = 1;
                break;
        }

        if (enemyWeapon.C_enmeyWeapon != null)
        {
            enemyWeapon.StopCoroutine(enemyWeapon.C_enmeyWeapon);
        }

        if (enemyType == GameManager.EnemyType.Oak || enemyType == GameManager.EnemyType.Wolf)
            enemyWeapon.EnemyWeaponOut();

        if (enemyWeapon.enemyWeaponColList.Count > 0)
        {
            for (int i = enemyWeapon.enemyWeaponColList.Count - 1; i >= 0; i--)
            {
                if (!enemyWeapon.enemyWeaponColList[i].isBullet)
                {
                    enemyWeapon.enemyWeaponColList.Remove(enemyWeapon.enemyWeaponColList[i]);
                }
            }
        }

        if (C_Think != null)
        {
            StopCoroutine(C_Think);
            C_Think = null;
            ThinkStack.Clear();
            isThinking = false;
        }

        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        yield return new WaitUntil(() => !enemyCollision.isFrozen);

        anim.speed = 1f;
        anim.CrossFade("GetHit", 0.1f);
        yield return new WaitForSeconds(animLength - 0.2f);

        //Debug.Log("스턴 끝");
        if (!isTraining){
            isChase = true;
            nav.isStopped = false;
            StartMovementAnim(0f, 1.5f);
        }
        
        isStaggering = false;

    }

    public void DiffcultySetting(GameManager.DifficultyType difficultyType)
    {
        difficulty = difficultyType;
        if(difficultyType == DifficultyType.Easy)
        {
            //Debug.Log("Easy");
            switch (enemyType)
            {
                case EnemyType.Oak:
                    maxHP = 150;
                    curHP = 150;
                    maxSHP = 80;
                    curSHP = 80;
                    maxFireHP = 70;
                    curFireHP = 70;
                    maxIceHP = 70;
                    curIceHP = 70;
                    maxLightingHP = 60;
                    curLightingHP = 60;
                    unlockPattern = false;
                    enemyWeapon.enemyDamage = 32;

                    break;

                case EnemyType.Lich:
                    maxHP = 110;
                    curHP = 110;
                    maxSHP = 60;
                    curSHP = 60;
                    maxFireHP = 80;
                    curFireHP = 80;
                    maxIceHP = 80;
                    curIceHP = 80;
                    maxLightingHP = 80;
                    curLightingHP = 80;
                    unlockPattern = false;
                    enemyWeapon.enemyDamage = 26;

                    break;

                case EnemyType.Wolf:
                    maxHP = 100;
                    curHP = 100;
                    maxSHP = 50;
                    curSHP = 50;
                    maxFireHP = 50;
                    curFireHP = 50;
                    maxIceHP = 50;
                    curIceHP = 50;
                    maxLightingHP = 60;
                    curLightingHP = 60;
                    unlockPattern = false;
                    enemyWeapon.enemyDamage = 32;

                    break;
            }
        }
        else if (difficultyType == DifficultyType.Normal)
        {
            //Debug.Log("Normal");
            switch (enemyType)
            {
                case EnemyType.Oak:
                    maxHP = 190;
                    curHP = 190;
                    maxSHP = 95;
                    curSHP = 95;
                    maxFireHP = 80;
                    curFireHP = 80;
                    maxIceHP = 80;
                    curIceHP = 80;
                    maxLightingHP = 70;
                    curLightingHP = 70;
                    unlockPattern = true;
                    enemyWeapon.enemyDamage = (int)(enemyWeapon.enemyDamage * 1.25f);

                    break;

                case EnemyType.Lich:
                    maxHP = 150;
                    curHP = 150;
                    maxSHP = 70;
                    curSHP = 70;
                    maxFireHP = 90;
                    curFireHP = 90;
                    maxIceHP = 90;
                    curIceHP = 90;
                    maxLightingHP = 90;
                    curLightingHP = 90;
                    unlockPattern = true;
                    enemyWeapon.enemyDamage = (int)(enemyWeapon.enemyDamage * 1.25f);
                    break;

                case EnemyType.Wolf:
                    maxHP = 130;
                    curHP = 130;
                    maxSHP = 60;
                    curSHP = 60;
                    maxFireHP = 60;
                    curFireHP = 60;
                    maxIceHP = 60;
                    curIceHP = 60;
                    maxLightingHP = 70;
                    curLightingHP = 70;
                    unlockPattern = true;
                    enemyWeapon.enemyDamage = (int)(enemyWeapon.enemyDamage * 1.25f);
                    break;
            }
        }
        else if (difficultyType == DifficultyType.Hard)
        {
            //Debug.Log("Hard");
            switch (enemyType)
            {
                case EnemyType.Oak:
                    maxHP = 230;
                    curHP = 230;
                    maxSHP = 105;
                    curSHP = 105;
                    maxFireHP = 90;
                    curFireHP = 90;
                    maxIceHP = 90;
                    curIceHP = 90;
                    maxLightingHP = 80;
                    curLightingHP = 80;
                    unlockPattern = true;

                    enemyWeapon.enemyDamage = (int)(enemyWeapon.enemyDamage * 1.5f);
                    break;

                case EnemyType.Lich:
                    maxHP = 175;
                    curHP = 175;
                    maxSHP = 80;
                    curSHP = 80;
                    maxFireHP = 100;
                    curFireHP = 100;
                    maxIceHP = 100;
                    curIceHP = 100;
                    maxLightingHP = 100;
                    curLightingHP = 100;
                    unlockPattern = true;
                    enemyWeapon.enemyDamage = (int)(enemyWeapon.enemyDamage * 1.5f);

                    break;

                case EnemyType.Wolf:
                    maxHP = 155;
                    curHP = 155;
                    maxSHP = 70;
                    curSHP = 70;
                    maxFireHP = 65;
                    curFireHP = 65;
                    maxIceHP = 65;
                    curIceHP = 65;
                    maxLightingHP = 75;
                    curLightingHP = 75;
                    unlockPattern = true;
                    enemyWeapon.enemyDamage = (int)(enemyWeapon.enemyDamage * 1.5f);

                    break;
            }
        }
    }

    IEnumerator MovementAnimParam(float end, string type)
    {
        //print("HoriVert Start");
        if (type == "h")
        {
            while (Mathf.Abs(moveHori - end) > 0.01f)
            {
                moveHori = Mathf.Lerp(moveHori, end, Time.deltaTime * 3f);
                //hori = Mathf.MoveTowards(hori, end, Time.deltaTime * 2f);
                //print("h : " + moveHori);

                anim.SetFloat("horizontal", moveHori);

                InfiniteLoopDetector.Run();
                yield return null;
            }
            C_hori = null;
        }
        else if (type == "v")
        {
            while (Mathf.Abs(moveVert - end) > 0.01f)
            {
                moveVert = Mathf.Lerp(moveVert, end, Time.deltaTime * 3f);
                //vert = Mathf.MoveTowards(vert, end, Time.deltaTime * 2f);
                //print("v : " + moveVert);

                anim.SetFloat("vertical", moveVert);

                InfiniteLoopDetector.Run();
                yield return null;
            }
            C_vert = null;
        }
        //print("HoriVert End");
    }

    void StartMovementAnim(float h, float v)
    {
        if (C_hori != null)
        {
            StopCoroutine(C_hori);
            C_hori = null;
        }

        if (C_vert != null)
        {
            StopCoroutine(C_vert);
            C_vert = null;
        }

        C_hori = StartCoroutine(MovementAnimParam(h, "h"));
        C_vert = StartCoroutine(MovementAnimParam(v, "v"));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isWallSlip = true;
        }

        if (collision.collider.CompareTag("Player"))
        {
            isPlayerTouched = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isWallSlip = true;
        }
        else
        {
            isWallSlip = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isWallSlip = false;
        }

        if (collision.collider.CompareTag("Player"))
        {
            isPlayerTouched = false;
        }
    }

    private void OnDestroy()
    {

    }

}
