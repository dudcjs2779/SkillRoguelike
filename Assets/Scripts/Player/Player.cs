using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [Header("DebugMode")]
    public bool HP;
    public bool MP;
    public bool SP;

    [Header("Axis")]
    public Vector3 moveVec;
    public Vector3 slipWallVec;
    public Vector3 wallNormal;

    [Header("Keys")]
    //public bool dDown;
    //public bool fDown, f2Down, f2Up;
    bool testDown;

    [Header("Bools")]
    public bool doMovement;
    public bool isAction;
    public bool isDodge;
    public bool clickedDodge;
    public bool isAttacking;
    public bool isCharging, isSA, fullCharged;        //강공 기모으기
    public bool isHealing;
    public bool isStaggering;
    public bool isKD, isDead;
    public bool enemyLocked;
    public bool nonInputBool, preInputBool, attackInputBool, rollInputBool, skillInputBool, etcInputBool;
    public bool clicked;
    public bool isRootMotion;
    public bool isAnimRunning;
    public bool late_doVelocityZero;
    public bool donLockOnRotate;
    public bool isInputCancle;
    bool isDefenceAngle;
    public bool isIframes;
    public bool isNoHitFrame;
    public bool isGround;

    //플레이어 체력, 스테미너 등
    [Header("Player Status")]
    public float maxHealth;
    public float curHealth;
    public float maxStamina, curStamina;
    public float maxMP, curMP;
    public float maxEXP, curEXP;
    public float healAmount;
    public float bottleCount;


    //공격중 턴
    [Header("EXTRA")]
    public float moveSpeed;
    public float rotateSpeed;
    public float forceGravity;
    public float animLength, preInputTime, attackInputTime, rollInputTime, skillInputTime, etcInputTime, exitTime;
    float maxLockOnAngle = 60;
    [SerializeField] float lockOnDistance;
    [SerializeField] float lockOnMaxDistance;

    //Turn 변수
    float turnAmount;
    float rot;

    public float damageStack;

    public string prevAction = "";
    public string prevActionType;

    [SerializeField] GameObject healEffect;

    public Coroutine C_Action;
    public Coroutine C_Heal;
    public Coroutine C_ActionTiming;
    public Coroutine C_Staggering;

    public Rigidbody rigid;
    public Animator anim;
    SkinnedMeshRenderer meshRenderer;
    Color originColor;
    LayerMask enemyLayer = 1 << 9;
    LayerMask wallLayer = 1 << 7;

    public ParticleSystem charging;
    public ParticleSystem fullCharge;
    public BoxCollider swordArea;
    public Transform currentTarget;
    public List<Collider> hitColStack = new List<Collider>();

    [Header("For Tutorial")]
    public int dodgeCount;
    public int attackCount;
    public int strongAttackCount;

    PlayerAnim playerAnim;
    PlayerSkill playerSkill;
    PlayerState playerState;
    PlayerStateManager playerStateManager;
    PlayerEffect playerEffect;
    Weapon weapon;


    BossWeapon bossWeapon;

    void Awake()
    {
        var objs = FindObjectsOfType<Player>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        swordArea = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        originColor = meshRenderer.material.color;
        playerAnim = GetComponent<PlayerAnim>();
        weapon = GetComponentInChildren<Weapon>();
        playerSkill = GetComponent<PlayerSkill>();
        playerState = GameObject.Find("Canvas0").GetComponentInChildren<PlayerState>();
        playerStateManager = GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        if (GameManager.Instance.isDebugMode && Keyboard.current.f6Key.wasPressedThisFrame)
        {
            ActiveSkillLevel1();
        }

        if (GameManager.Instance.isDebugMode && Keyboard.current.f7Key.wasPressedThisFrame)
        {
            ActiveSkillLevel2();
        }

        if (GameManager.Instance.isDebugMode && Keyboard.current.f8Key.wasPressedThisFrame)
        {
            PassiveSkillLevel1();
        }

        if (GameManager.Instance.isDebugMode && Keyboard.current.f9Key.wasPressedThisFrame)
        {
            LevelUp();
        }

        if (GameManager.Instance.isDebugMode && Keyboard.current.f10Key.wasPressedThisFrame)
        {
            BuffOff();
        }


        if (isDead)
            return;

        //Debug.Log(moveVec);

        //if (Keyboard.current.tKey.wasPressedThisFrame)
        //{
        //    Debug.Log("t");
        //    playerInput.SwitchCurrentActionMap("UI");
        //}
        //if (Keyboard.current.yKey.wasReleasedThisFrame)
        //{
        //    Debug.Log("y");
        //    playerInput.SwitchCurrentActionMap("Player");
        //}

        if (HP) curHealth = maxHealth;
        if (MP) curMP = maxMP;
        if (SP) curStamina = 100;

        isRootMotion = anim.GetBool("isRootMotion");
        isAnimRunning = anim.GetBool("isAnimRunning");
        doMovement = anim.GetBool("doMovement");

        StartCoroutine(Dodge());
        //StartCoroutine(_Heal());
        Heal();
        AttackControl();
        //StartCoroutine(_AttackControl());
        StartCoroutine(StrongAttack_Down());
        //InteractObj();
        LockOnEnemy();

        if (hitColStack.Count > 0)
        {
            for (int i = hitColStack.Count - 1; i >= 0; i--)
            {
                if (hitColStack[i] == null || !hitColStack[i].enabled)
                {
                    //print(hitColStack[i].name + " is out");
                    hitColStack.Remove(hitColStack[i]);
                }
            }
        }
    }

    private void LateUpdate()
    {
        VelocityZero();
    }

    public void VelocityZero()
    {
        if (late_doVelocityZero)
        {
            late_doVelocityZero = false;
            rigid.velocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        //Debug.Log(transform.forward);

        if (isDead)
            return;

        Move();
        LockOnMovement();
        Turn();

    }


    void Move()
    {
        if (PlayerInputControls.Instance.hvInputVec == Vector3.zero) //여기 수정 했음
        {
            slipWallVec = Vector3.zero;
        }

        //Debug.Log(PlayerInputControls.Instance.hAxis);

        if (!isAttacking && !isDodge && !isCharging && !isSA && !isStaggering && !isAction && (!playerSkill.skilling || playerSkill.aiming))
        {
            moveVec = new Vector3(PlayerInputControls.Instance.hAxis, 0, PlayerInputControls.Instance.vAxis);
            moveVec = Vector3.ClampMagnitude(moveVec, 1);
            float speed = moveSpeed;

            Vector3 slideVec = SlideWallVec(wallNormal, moveVec);
            if (slideVec != Vector3.zero)
            {
                //Debug.Log("slide");
                moveVec = slideVec;
            }

            if (isHealing)
            {
                speed = moveSpeed * 0.5f;
            }
            Vector3 velocity = moveVec * speed;
            //Debug.Log(velocity);
            rigid.velocity = velocity;
            if (rigid.velocity.magnitude > 0.1f)
            {
                SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FootSteps_Grass_01, transform.position);
            }
        }
        else
        {
            moveVec = Vector3.zero;
        }

        if (!isGround)
        {
            rigid.AddForce(Vector3.down * forceGravity);
        }
    }

    public Vector3 SlideWallVec(Vector3 normal, Vector3 dir)
    {
        float angle = Vector3.Angle(normal, dir);
        Vector3 moveDirection = Vector3.zero;

        if (normal != Vector3.zero && angle <= 90)
        {
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

            //if (angle > 90)
            //{
            //    rigid.velocity = -wallNomal;
            //}

            moveDirection = Vector3.ProjectOnPlane(dir, normal);
            moveDirection = moveDirection * sin;
        }


        return moveDirection;
    }

    void LockOnMovement()
    {
        if (isAttacking || isDodge || isStaggering || (playerSkill.skilling && !playerSkill.aiming))
            return;

        if (enemyLocked)
        {
            //Quaternion rot = Quaternion.LookRotation(targetDir, Vector3.up);

            float angle = 360 - transform.rotation.eulerAngles.y;
            Quaternion rot = Quaternion.Euler(0, angle, 0);
            Vector3 moveAnimParaVec = rot * PlayerInputControls.Instance.hvInputVec;

            if (isHealing)
            {
                moveAnimParaVec = moveAnimParaVec * 0.5f;
            }

            //print(moveAnimParaVec);

            //Vector3 moveAnimParaVec = (Quaternion.AngleAxis(angle, Vector3.up) * moveVec).normalized;
            //Debug.Log(moveAnimParaVec);

            anim.SetFloat("MoveH", moveAnimParaVec.x, 0.1f, Time.deltaTime);
            anim.SetFloat("MoveV", moveAnimParaVec.z, 0.1f, Time.deltaTime);
        }
        else
        {
            Vector3 moveAnimParaVec = PlayerInputControls.Instance.hvInputVec;

            if (isHealing)
            {
                moveAnimParaVec = moveAnimParaVec * 0.5f;
            }

            anim.SetFloat("MoveH", 0);
            anim.SetFloat("MoveV", moveAnimParaVec.magnitude);
            //Debug.Log("moveAnimParaVec");
        }
    }

    void Turn()
    {
        //Debug.Log(doAttackTurn);

        if (PlayerInputControls.Instance.hvInputVec == Vector3.zero) return;
        turnAmount = Mathf.Atan2(PlayerInputControls.Instance.hvInputVec.x, PlayerInputControls.Instance.hvInputVec.z) * Mathf.Rad2Deg;   //실제 각도
        //Debug.Log("Mathf.Atan2 * Mathf.Rad2Deg : " + turnAmount);

        rot = Mathf.LerpAngle(transform.eulerAngles.y, turnAmount, Time.deltaTime * rotateSpeed);   //보간된 각도 값
        //Debug.Log("Mathf.LerpAngle : " + rot);

        if (!isAttacking && !isDodge && !isCharging && !isSA && !isStaggering && !playerSkill.skilling && !enemyLocked || playerSkill.aiming)
        {
            transform.eulerAngles = new Vector3(0, rot, 0);
        }

    }

    public IEnumerator ActionTurn(Vector3 arrow, float speed, float dur = 0.2f)
    {
        if (arrow == Vector3.zero)
            yield break;

        //Debug.Log(arrow);
        float time = 0;
        Vector3 dir = arrow;

        while (time < dur)
        {
            time = time + Time.deltaTime;

            Quaternion rot = Quaternion.LookRotation(dir);
            rigid.rotation = Quaternion.Slerp(rigid.rotation, rot, speed * Time.deltaTime);
            yield return null;
        }
    }

    void LockOnEnemy()
    {
        if (PlayerInputControls.Instance.lockOnDown)
        {
            PlayerInputControls.Instance.lockOnDown = false;
            Debug.Log("LockOnEnemy");

            //if (currentTarget)   //락온중에 한번 더 누를 경우 해제
            //{
            //    enemyLocked = false;
            //    currentTarget = null;
            //    return;
            //}

            if (currentTarget = ScanNearEnemy(false, false)) //함수가 실행된 후 currentTarget가 null이 아니면
            {
                enemyLocked = true;
            }
            else
            {
                ReleaseLockOn();
            }
        }
        else if (PlayerInputControls.Instance.mouseScrollLeft > 0)
        {
            //Debug.Log("SideScan left");
            PlayerInputControls.Instance.mouseScrollLeft = 0;
            if (currentTarget = ScanNearEnemy(true, true))
            {
                enemyLocked = true;
            }
            else
            {
                ReleaseLockOn();
            }

        }
        else if (PlayerInputControls.Instance.mouseScrollRight > 0)
        {
            //Debug.Log("SideScan right");
            PlayerInputControls.Instance.mouseScrollRight = 0;
            if (currentTarget = ScanNearEnemy(true, false))
            {
                enemyLocked = true;
            }
            else
            {
                ReleaseLockOn();
            }
        }

        if (enemyLocked)
        {
            if (!TargetOnRange())
            {
                ReleaseLockOn();
            }
            LookAtTarget();
        }
    }

    private Transform ScanNearEnemy(bool isSideScan, bool isLeft)
    {
        //Debug.Log("ScanNearEnemy");

        float maxDistance;
        if (isSideScan) maxDistance = lockOnMaxDistance;
        else maxDistance = lockOnDistance;

        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, maxDistance, enemyLayer);
        float closestAngle = maxLockOnAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0 || isSideScan && currentTarget == null)
        {
            return null;
        }

        if (isSideScan)
        {
            for (int i = 0; i < nearbyTargets.Length; i++)
            {
                Vector3 curTargetDir = currentTarget.position - transform.position;
                curTargetDir.y = 0;

                Vector3 newTargetDir = nearbyTargets[i].transform.position - transform.position;
                newTargetDir.y = 0;

                float _angle = Vector3.SignedAngle(curTargetDir, newTargetDir, Vector3.up);
                float absAnlge = Mathf.Abs(_angle);
                if (isLeft && _angle < 0 && absAnlge < closestAngle)
                {
                    closestTarget = nearbyTargets[i].transform;
                    closestAngle = absAnlge;
                }
                else if (!isLeft && _angle > 0 && _angle < closestAngle)
                {
                    closestTarget = nearbyTargets[i].transform;
                    closestAngle = _angle;
                }
            }

        }
        else
        {
            for (int i = 0; i < nearbyTargets.Length; i++)
            {
                Vector3 dir = nearbyTargets[i].transform.position - transform.position;
                dir.y = 0;

                Vector3 playerInputAxis = PlayerInputControls.Instance.hvRawInputVec != Vector3.zero
                ? PlayerInputControls.Instance.hvRawInputVec : transform.forward;
                float _angle = Vector3.Angle(playerInputAxis, dir);

                if (_angle < closestAngle)
                {
                    closestTarget = nearbyTargets[i].transform;
                    closestAngle = _angle;
                }
            }
        }

        if (!closestTarget) return null;
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2);
        Vector3 tarPos = closestTarget.position + new Vector3(0, half_h, 0);
        if (Blocked(tarPos)) return null;

        playerState.LockOn(closestTarget, half_h);
        return closestTarget;
    }

    bool Blocked(Vector3 target)
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, target, out hit, enemyLayer))
        {
            if (!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }

    private void LookAtTarget()
    {
        if (currentTarget == null || isDodge || donLockOnRotate)
        {
            return;
        }

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10);
    }

    bool TargetOnRange()
    {
        float dis = (transform.position - currentTarget.position).magnitude;
        if (dis > lockOnMaxDistance) return false; else return true;
    }

    public void ReleaseLockOn()
    {
        enemyLocked = false;
        currentTarget = null;
    }

    IEnumerator Dodge()
    {
        if (PlayerInputControls.Instance.dDown && curStamina > 0)
        {
            PlayerInputControls.Instance.dDown = false;
            if (nonInputBool || clicked)
            {
                Debug.Log("cancle");
                yield break;
            }

            if (preInputBool)
            {
                clicked = true;
                yield return new WaitUntil(() => rollInputBool);
                if (isDead)
                {
                    clicked = false;
                    yield break;
                }
            }
            clicked = false;



            if (prevAction == "")
            {
                //Debug.Log("Dodge1");
                Action("Dodge", "RollingForward", "Dodge", "Dodge", 0.1f);

                yield return new WaitForSeconds(0.06f);
                isIframes = true;
                yield return new WaitForSeconds(0.12f * playerStateManager.dodgePfVal);
                isIframes = false;

            }
            else if (rollInputBool)
            {
                //Debug.Log("Dodge2");

                Action("Dodge", "RollingForward", "Dodge", "Dodge", 0.1f);

                yield return new WaitForSeconds(0.06f);
                isIframes = true;
                yield return new WaitForSeconds(0.12f * playerStateManager.dodgePfVal);
                isIframes = false;
            }
        }
        else
        {
            PlayerInputControls.Instance.dDown = false;
        }
    }

    void Heal()
    {
        if (PlayerInputControls.Instance.healDown)
        {
            if (C_Action != null)
            {
                clicked = false;
                StopCoroutine(C_Action);
            }

            C_Action = StartCoroutine(_Heal());
        }
        else
        {
            PlayerInputControls.Instance.healDown = false;
        }
    }

    IEnumerator _Heal()
    {
        PlayerInputControls.Instance.healDown = false;
        if (nonInputBool || clicked)
        {
            Debug.Log("cancle");
            yield break;
        }
        if (preInputBool)
        {
            clicked = true;
            yield return new WaitUntil(() => etcInputBool);
            if (isDead)
            {
                clicked = false;
                yield break;
            }
        }
        clicked = false;
        Debug.Log("heal");

        if (bottleCount <= 0) yield break;

        Action("Heal", "Heal", "Heal", "Heal", 0.2f);

        yield return new WaitForSeconds(0.8f);
        float healAmount = this.healAmount * playerStateManager.healAmountVal;
        float time = 0.25f;
        float healSpeed = healAmount / time;
        float targetVal = curHealth + healAmount;
        bottleCount--;
        Canvas0.Instance.playerState.bottleCountText.text = bottleCount.ToString();
        Instantiate(healEffect, transform);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.Heal01, transform.position + transform.forward * 0.1f);
        while (time > 0)
        {
            curHealth += Time.deltaTime * healSpeed;
            if (curHealth > maxHealth) curHealth = maxHealth;

            time = time - Time.deltaTime;
            yield return null;
        }

        curHealth = targetVal;
        if (curHealth > maxHealth) curHealth = maxHealth;
    }

    void Action(string actionType, string animPara, string actionName, string nowAct, float tDuration)
    {
        //Debug.Log("Action");
        //Debug.Log(nowAct);
        ActionOutCheck(prevActionType);
        AnimLength(actionName);

        if (actionType == "Attack")
        {
            isAttacking = true;
            prevActionType = "Attack";
            playerAnim.PlayAnimation(animPara, true, tDuration);
            weapon.Use(actionName);
            attackCount++;
        }
        else if (actionType == "Dodge")
        {
            isDodge = true;
            prevActionType = "Dodge";
            playerAnim.PlayAnimation(animPara, true, tDuration);
            StartCoroutine(ActionTurn(PlayerInputControls.Instance.hvRawInputVec, 15, 0.3f));
            curStamina -= 20f;
            SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.Dodge01, gameObject)
            .transform.localPosition = -transform.forward * 0.1f;
            dodgeCount++;
        }
        else if (actionType == "SA")
        {
            if (actionName == "SA_Charge")
            {
                isCharging = true;
                nonInputBool = true;
                prevActionType = "SA";
                playerAnim.PlayAnimation(animPara, true, tDuration);
                prevAction = nowAct;
                if (C_ActionTiming != null) StopCoroutine(C_ActionTiming);
                return;
            }
            else
            {
                isSA = true;
                isCharging = false;
                prevActionType = "SA";
                playerAnim.PlayAnimation(animPara, true, tDuration);
                charging.Stop();
                weapon.Use(actionName);
                strongAttackCount++;
            }

        }
        else if (actionType == "Stagger")
        {
            if (actionName == "GetHit")
            {
                //Debug.Log("Action GetHit");
                isAction = true;
                prevActionType = "Stagger";

                anim.applyRootMotion = isRootMotion;
                anim.SetBool("isRootMotion", isRootMotion);
                anim.CrossFade(animPara, tDuration, 0, 0);
            }
            else if (actionName == "KnockDown")
            {
                isAction = true;
                prevActionType = "Stagger";

                anim.applyRootMotion = isRootMotion;
                anim.SetBool("isRootMotion", isRootMotion);
                anim.CrossFade(animPara, tDuration, 0, 0);
            }
        }
        else if (actionType == "Heal")
        {
            isHealing = true;
            prevActionType = "Heal";
            playerAnim.PlayAnimation(animPara, true, tDuration, 1);
        }

        prevAction = nowAct;
        StartActionTiming();
    }

    public void StartActionTiming()
    {
        //Debug.Log("StartActionTiming");
        if (C_ActionTiming != null)
        {
            StopCoroutine(C_ActionTiming);
        }
        C_ActionTiming = StartCoroutine(ActionTiming());
    }

    void AnimLength(string actionName)
    {
        if (actionName == "A1")
        {
            animLength = 2;
            preInputTime = 0.35f;
            attackInputTime = 0.55f;
            rollInputTime = 0.6f;
            skillInputTime = 0.7f;
            etcInputTime = 0.9f;
            exitTime = 1.5f;
        }
        else if (actionName == "A2")
        {
            animLength = 2;
            preInputTime = 0.35f;
            attackInputTime = 0.55f;
            rollInputTime = 0.6f;
            skillInputTime = 0.7f;
            etcInputTime = 0.9f;
            exitTime = 1.5f;
        }
        else if (actionName == "A3")
        {
            animLength = 2;
            preInputTime = 0.4f;
            attackInputTime = 1.45f;
            rollInputTime = 1.0f;
            skillInputTime = 1.45f;
            etcInputTime = 1.5f;
            exitTime = 1.5f;
        }
        else if (actionName == "Dodge")
        {
            animLength = 1.067f;
            preInputTime = 0.2f;
            attackInputTime = 0.55f;
            rollInputTime = 0.7f;
            skillInputTime = 0.7f;
            etcInputTime = 0.85f;
            exitTime = 1;
        }
        else if (actionName == "SA_Attack")
        {
            //Debug.Log("AnimLength SA_Attack");
            animLength = 1.667f;
            preInputTime = 0f;
            attackInputTime = 1f;
            rollInputTime = 0.6f;
            skillInputTime = 1f;
            etcInputTime = 1.2f;
            exitTime = 1.5f;
        }
        else if (actionName == "GetHit")
        {
            //Debug.Log("GetHit animLength");
            animLength = 0.667f;
            preInputTime = 0.1f;
            attackInputTime = 0.6f;
            rollInputTime = 0.5f;
            skillInputTime = 0.6f;
            etcInputTime = 0.6f;
            exitTime = 0.667f;
        }
        else if (actionName == "Heal")
        {
            //Debug.Log("animLength");
            animLength = 2f;
            preInputTime = 0.4f;
            attackInputTime = 1.8f;
            rollInputTime = 1.8f;
            skillInputTime = 1.8f;
            etcInputTime = 1.8f;
            exitTime = 1.9f;
        }

    }

    void AttackControl()
    {
        if (PlayerInputControls.Instance.f1Down && !PlayerInputControls.Instance.skillTrigger && curStamina > 0 && !playerSkill.aiming)
        {
            if (C_Heal != null)
            {
                clicked = false;
                StopCoroutine(C_Heal);
            }

            C_Heal = StartCoroutine(_AttackControl());
        }
        else
        {
            PlayerInputControls.Instance.f1Down = false;
        }
    }

    IEnumerator _AttackControl()
    {
        //Debug.Log("AttackControl: " + Time.frameCount);

        PlayerInputControls.Instance.f1Down = false;
        if (nonInputBool || clicked)
        {
            //Debug.Log("cancle");
            yield break;
        }

        if (preInputBool)
        {
            Debug.Log("preInput");
            clicked = true;
            yield return new WaitUntil(() => attackInputBool);
            if (isDead)
            {
                Debug.Log("preInput Cancle by Dead");
                clicked = false;
                yield break;
            }
        }
        clicked = false;

        Debug.Log("AttackControl");
        if (prevAction == "")
        {
            Action("Attack", "Attack1", "A1", "A1", 0.1f);
        }
        else if (prevAction == "A1" && attackInputBool)
        {
            Action("Attack", "Attack2", "A2", "A2", 0.1f);
        }
        else if (prevAction == "A2" && attackInputBool)
        {
            Action("Attack", "Attack3", "A3", "A3", 0.1f);
        }
        else if (prevAction == "A3" && attackInputBool)
        {
            Action("Attack", "Attack1", "A1", "A1", 0.1f);
        }
        else if (prevAction == "Dodge" && attackInputBool)
        {
            //DodgeOut();
            Action("Attack", "RollAttack", "A2", "Roll Attack", 0.1f);
        }
        else if (attackInputBool)
        {
            Debug.Log("else Combo");
            Action("Attack", "Attack1", "A1", "A1", 0.1f);
        }
    }

    public IEnumerator ActionTiming()
    {
        float time = 0;
        time = time + Time.deltaTime;
        nonInputBool = false;
        preInputBool = false;
        attackInputBool = false;
        rollInputBool = false;
        skillInputBool = false;
        etcInputBool = false;

        //Debug.Log(prevAction + "  AttackTiming || isStaggering: " + isStaggering.ToString());

        while (isAttacking || isDodge || playerSkill.skilling || isSA || isHealing || isAction)
        {
            //print(curAttack + " // " + time + " // " + exitTime);

            if (time >= attackInputTime && time < exitTime)
            {
                //Debug.Log("attackInputBool");
                attackInputBool = true;
            }

            if (time >= rollInputTime && time < exitTime)
            {
                rollInputBool = true;
            }

            if (time >= skillInputTime && time < exitTime)
            {
                skillInputBool = true;
            }

            if (time < preInputTime)
            {
                nonInputBool = true;
            }
            else if (time < etcInputTime)
            {
                preInputBool = true;
                nonInputBool = false;
            }
            else if (time < exitTime)
            {
                //Debug.Log("inputTime");
                etcInputBool = true;
                preInputBool = false;

                if (PlayerInputControls.Instance.hAxisRaw != 0 || PlayerInputControls.Instance.vAxisRaw != 0 || PlayerInputControls.Instance.dDown || PlayerInputControls.Instance.f1Down || PlayerInputControls.Instance.f2Down || PlayerInputControls.Instance.healDown)
                {
                    //Debug.Log("inputTimeOut");
                    if (isAttacking) AttackOut();
                    else if (isDodge) DodgeOut();
                    else if (playerSkill.skilling) SkillOut();
                    else if (isSA) StrongAttackOut();
                    else if (isAction) GetHitOut();
                    else if (isHealing)
                    {
                        HealOut();
                        yield break;
                    }

                    anim.SetBool("doMovement", true);
                    //Debug.Log("inputTimeOut: " + Time.frameCount);

                    yield break;
                }

            }
            else if (time >= exitTime)
            {
                //Debug.Log("exitTimeOut");
                if (isAttacking) AttackOut();
                else if (isDodge) DodgeOut();
                else if (playerSkill.skilling) SkillOut();
                else if (isSA) StrongAttackOut();
                else if (isAction) GetHitOut();
                else if (isHealing) HealOut();

                anim.SetBool("doMovement", true);
                //Debug.Log("exitTimeOut");

                yield break;
            }


            time = time + Time.deltaTime;
            yield return null;
        }
    }

    public void ActionOutCheck(string prevActType)
    {
        if (prevActType == "Attack")
        {
            AttackOut();
        }
        else if (prevActType == "Dodge")
        {
            DodgeOut();
        }
        else if (prevActType == "Skill")
        {
            SkillOut();
        }
        else if (prevActType == "SA")
        {
            StrongAttackOut();
        }
        else if (prevActType == "Stagger")
        {
            GetHitOut();
        }
        else if (prevActType == "Heal")
        {
            HealOut();
        }
    }

    void AttackOut()
    {
        //Debug.Log("AttackOut");
        anim.SetBool("isRootMotion", false);
        isAttacking = false;

        prevAction = "";
        prevActionType = "";
        nonInputBool = false;
        preInputBool = false;
        //attackInputBool = false;
        //rollInputBool = false;
        //skillInputBool = false;
        //etcInputBool = false;
        //clicked = false;
    }

    void DodgeOut()
    {
        //Debug.Log("DodgeOut");
        anim.SetBool("isRootMotion", false);
        isDodge = false;

        prevAction = "";
        prevActionType = "";
        nonInputBool = false;
        preInputBool = false;
        //attackInputBool = false;
        //rollInputBool = false;
        //skillInputBool = false;
        //etcInputBool = false;
        late_doVelocityZero = true;
        //clicked = false;
    }

    public void SkillOut()
    {
        //Debug.Log("SkillOut");
        anim.SetBool("isRootMotion", false);
        playerSkill.usingSkill = null;
        playerSkill.skilling = false;
        weapon.isStickedEnemy = false;

        prevAction = "";
        prevActionType = "";
        nonInputBool = false;
        preInputBool = false;
        //attackInputBool = false;
        //rollInputBool = false;
        //skillInputBool = false;
        //etcInputBool = false;
        //clicked = false;
    }

    void StrongAttackOut()
    {
        //Debug.Log("SAOut");
        anim.SetBool("isRootMotion", false);
        isSA = false;
        isCharging = false;
        fullCharged = false;
        PlayerInputControls.Instance.f2Up = false;
        charging.Stop();

        prevAction = "";
        prevActionType = "";
        nonInputBool = false;
        preInputBool = false;
        //attackInputBool = false;
        //rollInputBool = false;
        //skillInputBool = false;
        //etcInputBool = false;
        //clicked = false;
    }

    void GetHitOut()
    {
        Debug.Log("GetHitOut");
        isStaggering = false;
        isAction = false;
        isKD = false;

        prevAction = "";
        prevActionType = "";
        nonInputBool = false;
        preInputBool = false;
        //attackInputBool = false;
        //rollInputBool = false;
        //skillInputBool = false;
        //etcInputBool = false;
        //clicked = false;
    }

    void HealOut()
    {
        Debug.Log("HealOut");
        anim.SetBool("isRootMotion", false);
        isHealing = false;

        prevAction = "";
        prevActionType = "";
        nonInputBool = false;
        preInputBool = false;

        if (isStaggering)
        {
            anim.CrossFade("Empty", 0.05f, 1);
        }
        //attackInputBool = false;
        //rollInputBool = false;
        //skillInputBool = false;
        //etcInputBool = false;
    }

    IEnumerator StrongAttack_Down()
    {
        //차지 시작
        if (PlayerInputControls.Instance.f2Down && !PlayerInputControls.Instance.skillTrigger && curStamina > 0 && !playerSkill.aiming)
        {
            PlayerInputControls.Instance.f2Down = false;
            if (nonInputBool || clicked)
            {
                Debug.Log("cancle");
                PlayerInputControls.Instance.f2Up = false;
                yield break;
            }
            if (preInputBool)
            {
                clicked = true;
                yield return new WaitUntil(() => attackInputBool);
                if (isDead)
                {
                    clicked = false;
                    yield break;
                }
            }
            clicked = false;

            //Debug.Log("SA Down");
            if (prevAction == "")
            {
                Debug.Log("SA_Charge");
                Action("SA", "SA_Charge", "SA_Charge", "SA_Charge", 0.1f);
                StartCoroutine(StrongAttack_Up());
            }
            else if (attackInputBool)
            {
                Debug.Log("attackInputBool SA_Charge");
                //if (prevAction == "SA_Attack") yield return new WaitUntil(() => !isAnimRunning);
                Action("SA", "SA_Charge", "SA_Charge", "SA_Charge", 0.1f);
                StartCoroutine(StrongAttack_Up());
            }
        }
        else
        {
            PlayerInputControls.Instance.f2Down = false;
        }
    }

    IEnumerator StrongAttack_Up()
    {
        if (!charging.isPlaying)
            charging.Play();

        float chargingGauge = 0;
        while (true && !isStaggering)
        {
            chargingGauge += Time.deltaTime;
            //Debug.Log(chargingGauge);

            if (chargingGauge > 1.1 && !fullCharged)
            {
                fullCharged = true;
                charging.Stop();
                fullCharge.Play();
                SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.AirImpact02, transform.position + transform.forward * 0.1f);
            }

            if (PlayerInputControls.Instance.f2Up && chargingGauge > 0.6)
            {
                PlayerInputControls.Instance.f2Up = false;

                Action("SA", "SA_Attack", "SA_Attack", "SA_Attack", 0.1f);
                yield break;
            }
            yield return null;
        }
    }

    string hitColName;

    //피격 스크립트
    void OnTriggerEnter(Collider other)
    {
        if (isDead || isNoHitFrame || hitColStack.Contains(other) || GameManager.Instance.isGameOver || GameManager.Instance.isStageClear || GameManager.Instance.isCutScene)
        {
            return;
        }

        EnemyWeapon enemyWeapon = null;
        EnemyBullet enemyBullet = null;
        EnemyWeaponCol hitEnemyWeapon = null;

        if (other.CompareTag("EnemyWeapon"))
        {
            hitColName = other.name;

            if (other.GetComponent<EnemyWeapon>() != null)
            {
                enemyWeapon = other.GetComponent<EnemyWeapon>();
                hitEnemyWeapon = enemyWeapon.enemyWeaponColList.Find(x => x.col == other);

            }
            else if (other.transform.root.GetComponentInChildren<BossWeapon>() != null)
            {
                bossWeapon = other.transform.root.GetComponentInChildren<BossWeapon>();
                hitEnemyWeapon = bossWeapon.bossWeaponColList.Find(x => x.col == other);

            }

            //Debug.Log("hit");
            hitColStack.Add(other);
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            enemyBullet = other.GetComponent<EnemyBullet>();

            if (enemyBullet.isPierceBullet) hitColStack.Add(other);

            //Debug.Log("hit");
            hitEnemyWeapon = enemyBullet.enemyWeaponColList.Find(x => x.col == other);
        }

        if (other.CompareTag("EnemyWeapon") || other.CompareTag("EnemyBullet"))
        {
            if (playerStateManager.isParry)
            {
                Vector3 dir = other.transform.root.position - transform.position;
                float angle = Vector3.Angle(dir, transform.forward);

                if (angle <= 90) isDefenceAngle = true;
                else isDefenceAngle = false;
            }

            if (isIframes)
            {
                Debug.Log("isNoHit : " + isIframes);
                if (!hitEnemyWeapon.isBullet) hitColStack.Add(hitEnemyWeapon.col);

                PassiveSkill perfectDodge = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Perfect Dodge");
                if (perfectDodge != null && perfectDodge.skillLv != 0) playerStateManager.PassiveApply(perfectDodge);

                PassiveSkill dodgeRecover = GameManager.Instance.EquipPassiveList.Find(x => x.name == "Dodge Recover");
                if (dodgeRecover != null && dodgeRecover.skillLv != 0) playerStateManager.PassiveApply(dodgeRecover);

                return;
            }

            if (enemyBullet != null && !enemyBullet.isPierceBullet)
            {
                enemyBullet.DestroyBullet();
            }
            if (hitEnemyWeapon == null) return;
            TakeDamage(hitEnemyWeapon);
        }
    }

    public void TakeDamage(EnemyWeaponCol hit)
    {
        //Debug.Log("onhit");

        float hitDamage = hit.damage;
        bool isBlocked = false;

        // 파워실드 방어
        if (playerStateManager.isPowerShield)
        {
            curStamina -= hit.damage * weapon.skillVal;
            weapon.StartCoroutine(weapon.Shake());
            //print("스테 뎀지 : " + damage * weapon.skillVal);

            if (curStamina > 0)
            {
                isBlocked = true;

                GameObject instantBullet = Instantiate(weapon.powerShieldWave, transform.position + Vector3.up * 0.1f, Quaternion.identity);
                PlayerBullet playerBullet = instantBullet.GetComponent<PlayerBullet>();
                playerBullet.playerWeaponColList = weapon.playerWeaponColList;

                float shieldWaveDam = weapon.weaponeDamage * weapon.usingSkill.damage[weapon.usingSkill.skillLv - 1] * 0.15f;
                float shieldWaveStgDam = weapon.stgDamage * weapon.usingSkill.stgDamage[weapon.usingSkill.skillLv - 1] * 0.15f;

                PlayerWeaponCol playerWeaponCol = new PlayerWeaponCol(weapon.usingSkill, shieldWaveDam, shieldWaveStgDam, 0, false, false);
                playerWeaponCol.colList.Add(playerBullet.bulletCol);
                playerWeaponCol.attackName = "Power Shield Wave";
                weapon.playerWeaponColList.Add(playerWeaponCol);

                weapon.powerShieldHit.Play();
                damageStack += hitDamage;

                if (damageStack > 100) damageStack = 100;
                hitDamage = hitDamage * 0.1f;

            }
            else
            {
                playerStateManager.isPowerShield = false;
                weapon.powerShield.SetActive(false);
            }
        }

        //Debug.Log(isBlock && playerStateManager.isPerfectParry);
        //Debug.Log(isBlock && playerStateManager.isParry);

        // 페리 방어
        if (isDefenceAngle && playerStateManager.isPerfectParry)
        {
            Debug.Log("퍼펙트");
            //print("스테 뎀지 : " + dam * weapon.skillVal2);

            hitDamage = 0;
            playerStateManager.isPerfectParry = false;
            isBlocked = true;

            animLength = 1f;
            preInputTime = 0.25f;
            attackInputTime = 0.5f;
            rollInputTime = 0.5f;
            skillInputTime = 0.5f;
            etcInputTime = 0.6f;
            exitTime = 0.8f;

            ActiveSkill guardEnhance = GameManager.Instance.EquipActiveList.Find(x => x.name == "Guard Enhance");
            string path = guardEnhance.iconPath;
            Sprite icon = Resources.Load<Sprite>(path);

            GameObject[] effects = new GameObject[1];
            effects[0] = Instantiate(weapon.GuardEnhance, weapon.swordCol.transform);
            weapon.buffManager.CreateBuff("Guard Enhance", "Active", guardEnhance.duration[0], effects, icon);
            Instantiate(weapon.PF_ParrySuccess, transform.position + Vector3.up, Quaternion.identity);
            anim.CrossFade("Guard Enhance Hit", 0.15f);
            if (weapon.weaponCoroutine != null) weapon.StopCoroutine(weapon.weaponCoroutine);
            weapon.Use("Guard Enhance Hit");
            StartActionTiming();

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GuardHit02, hit.col.transform.position);
        }
        else if (isDefenceAngle && playerStateManager.isParry)
        {
            curStamina -= hitDamage * 0.5f;
            //print("스테 뎀지 : " + dam * weapon.skillVal2);

            if (curStamina <= 0)
            {
                playerStateManager.isParry = false;
                playerStateManager.isPerfectParry = false;
                if (weapon.weaponCoroutine != null) weapon.StopCoroutine(weapon.weaponCoroutine);
            }
            else
            {
                hitDamage = hitDamage * 0.1f;
                isBlocked = true;

                animLength = 1f;
                preInputTime = 0.25f;
                attackInputTime = 0.5f;
                rollInputTime = 0.5f;
                skillInputTime = 0.5f;
                etcInputTime = 0.6f;
                exitTime = 0.8f;

                anim.CrossFade("Guard Enhance Hit", 0.15f);
                Instantiate(weapon.guardEffect, weapon.shieldCol.transform.position, transform.rotation);
                if (weapon.weaponCoroutine != null) weapon.StopCoroutine(weapon.weaponCoroutine);
                weapon.Use("Guard Enhance Hit");
                StartActionTiming();
            }

            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.GuardHit01, hit.col.transform.position);
        }

        // 데미지 계산
        hitDamage = hitDamage * playerStateManager.defanceVal * playerStateManager.barrierVal;

        hitDamage = Mathf.Round(hitDamage);
        curHealth = curHealth - hitDamage;

        Debug.Log("damage : " + hitDamage);

        if (!isBlocked)
        {
            //Debug.Log("Red");
            meshRenderer.material.color = Color.red;
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.PlayerHit01, hit.col.transform.position);
        }

        if ((!isBlocked || hit.isBigAttack) && !playerStateManager.isSuperArmor)
        {
            if (C_Staggering != null) StopCoroutine(C_Staggering);
            C_Staggering = StartCoroutine(Staggering(hit.isBigAttack));
        }

        if (!isBlocked && hit.isKnockBack)
        {
            Vector3 dir = transform.position - hit.col.transform.root.position;
            rigid.AddForce(dir.normalized * hit.knockBackPower, ForceMode.Impulse);
        }

        Buff guardingBarrier;
        if (guardingBarrier = GameManager.Instance.Buffs_Playing.Find(x => x.buffName == "Guarding Barrier"))
        {
            guardingBarrier.donPlayEffect = true;
            Instantiate(guardingBarrier.buffEffects[2], guardingBarrier.buffEffects[0].transform.position, Quaternion.identity);
            guardingBarrier.DeActivation();
        }

        if (curHealth <= 0)
        {
            Dead();
        }

        StartCoroutine(HitColor());
    }

    IEnumerator HitColor()
    {
        yield return new WaitForSeconds(0.3f);
        meshRenderer.material.color = originColor;
    }

    void Dead()
    {
        //Debug.Log("Daed");
        isDead = true;

        if (C_ActionTiming != null) StopCoroutine(C_ActionTiming);
        if (C_Action != null) StopCoroutine(C_Action);
        if (C_Heal != null) StopCoroutine(C_Heal);

        if (isAttacking) AttackOut();
        if (isDodge) DodgeOut();
        if (isHealing) HealOut();
        if (playerSkill.skilling) SkillOut();
        if (isCharging || isSA) StrongAttackOut();

        clicked = false;

        if (weapon.weaponCoroutine != null) weapon.StopCoroutine(weapon.weaponCoroutine);
        weapon.WeaponOut();

        if (weapon.playerWeaponColList.Count > 0)
        {
            for (int i = weapon.playerWeaponColList.Count - 1; i >= 0; i--)
            {
                if (!weapon.playerWeaponColList[i].isBullet)
                {
                    weapon.playerWeaponColList.Remove(weapon.playerWeaponColList[i]);
                }
            }
        }

        anim.CrossFade("Die", 0.1f);
        ReleaseLockOn();
        gameObject.layer = LayerMask.NameToLayer("PlayerNoHit");

        GameManager.Instance.GameOver();
    }

    IEnumerator Staggering(bool isBigAttack)
    {
        if (curHealth > 0)
        {
            //Debug.Log("Staggering");
            isStaggering = true;
            if (C_ActionTiming != null) StopCoroutine(C_ActionTiming);
            if (C_Action != null) StopCoroutine(C_Action);
            if (C_Heal != null) StopCoroutine(C_Heal);

            if (isAttacking) AttackOut();
            if (isDodge) DodgeOut();
            if (isHealing) HealOut();
            if (playerSkill.skilling) SkillOut();
            if (isCharging || isSA) StrongAttackOut();

            clicked = false;

            if (weapon.weaponCoroutine != null) weapon.StopCoroutine(weapon.weaponCoroutine);
            weapon.WeaponOut();

            if (weapon.playerWeaponColList.Count > 0)
            {
                for (int i = weapon.playerWeaponColList.Count - 1; i >= 0; i--)
                {
                    if (!weapon.playerWeaponColList[i].isBullet)
                    {
                        weapon.playerWeaponColList.Remove(weapon.playerWeaponColList[i]);
                    }
                }
            }

            if (!isBigAttack | isKD)
            {
                Action("Stagger", "GetHit", "GetHit", "GetHit", 0.1f);
            }
            else
            {   // 녹다운 공격 맞았을때
                Vector3 enemyDir = Vector3.zero;

                Debug.Log("KD");
                isKD = true;
                nonInputBool = true;
                isNoHitFrame = true;
                anim.CrossFade("KnockDown", 0.1f);
                rigid.AddForce(-enemyDir.normalized * 50, ForceMode.Impulse);
                transform.LookAt(enemyDir + transform.position);

                yield return new WaitForSeconds(0.5f);
                isNoHitFrame = false;
                yield return new WaitForSeconds(1f);
                anim.CrossFade("DieRecover", 0.1f);
                yield return new WaitForSeconds(2f);
                GetHitOut();
            }
        }
    }

    public void ActiveSkillLevel1()
    {
        Debug.Log("skillLvUp");
        foreach (var item in GameManager.Instance.EquipActiveList)
        {
            item.skillLv = 1;
        }
    }

    public void ActiveSkillLevel2()
    {
        Debug.Log("skillLvUp");
        foreach (var item in GameManager.Instance.EquipActiveList)
        {
            item.skillLv = 2;
        }
    }

    public void PassiveSkillLevel1()
    {
        Debug.Log("skillLvUp");
        foreach (var item in GameManager.Instance.EquipPassiveList)
        {
            item.skillLv = 0;
        }
    }

    public void LevelUp()
    {
        Canvas5.Instance.OpenSkillUpgrade();
    }

    public void BuffOff()
    {
        for (int i = GameManager.Instance.Buffs_Playing.Count - 1; i >= 0; i--)
        {
            GameManager.Instance.Buffs_Playing[i].DeActivation();
        }

    }

}

