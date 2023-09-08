using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerSkill : MonoBehaviour
{
    public bool skilling;
    public float skillSpeed;

    public ActiveSkill usingSkill;

    [Header("AIMING SKILL")]
    public GameObject defenceWallSkillRange;
    public GameObject skillAimDistanceImage;
    public GameObject skillAimRangeImage;
    public bool aiming, aimingTrigger;
    public Quaternion aimRot;
    public Vector3 aimPos;

    Coroutine C_SkillLogic;

    Animator anim;

    Player player;
    PlayerAnim playerAnim;
    Weapon weapon;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerAnim = GetComponent<PlayerAnim>();
        anim = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();
    }

    void Start()
    {

    }

    void Update()
    {
        SkillLogic();
        //StartCoroutine(_SkillLogic());

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //for (int i = 0; i < GameManager.Instance.EquiqActInSlot.Count; i++)
            //{
            //    print(GameManager.Instance.EquiqActInSlot[i].name + " // " + GameManager.Instance.EquiqActInSlot[i].skillLv);
            //}
        }
    }

    
    private void FixedUpdate()
    {
        
    }


    void SkillLogic(){
        //  스킬 가져오기
        ActiveSkill activeSkill = null;
        int changeIndex;

        if (PlayerInputControls.Instance.skillChangeDown) changeIndex = 4;
        else changeIndex = 0;

        if (PlayerInputControls.Instance.skill1)
        {
            //activeSkill = GameManager.Instance.EquipActInSlot[0];
            activeSkill = GameManager.Instance.EquipActiveList.Find(x => x.equipOrder == 1 + changeIndex);
        }
        else if (PlayerInputControls.Instance.skill2)
        {
            activeSkill = GameManager.Instance.EquipActiveList.Find(x => x.equipOrder == 2 + changeIndex);
        }
        else if (PlayerInputControls.Instance.skill3)
        {
            activeSkill = GameManager.Instance.EquipActiveList.Find(x => x.equipOrder == 3 + changeIndex);
        }
        else if (PlayerInputControls.Instance.skill4)
        {
            activeSkill = GameManager.Instance.EquipActiveList.Find(x => x.equipOrder == 4 + changeIndex);
        }

        if(activeSkill != null && activeSkill.skillLv != 0){
            if (C_SkillLogic != null)
            {
                StopCoroutine(C_SkillLogic);
                player.clicked = false;
            }
            C_SkillLogic = StartCoroutine(_SkillLogic(activeSkill));
        }
        else{
            PlayerInputControls.Instance.skill1 = false;
            PlayerInputControls.Instance.skill2 = false;
            PlayerInputControls.Instance.skill3 = false;
            PlayerInputControls.Instance.skill4 = false;
        }
    }

    IEnumerator _SkillLogic(ActiveSkill activeSkill)
    {
        if (!aiming)
        {
            // 스킬 시전
            PlayerInputControls.Instance.skill1 = false;
            PlayerInputControls.Instance.skill2 = false;
            PlayerInputControls.Instance.skill3 = false;
            PlayerInputControls.Instance.skill4 = false;

            // 스킬 선입력
            if (player.nonInputBool || player.clicked)
            {
                Debug.Log("cancle");
                yield break;
            }
            if (player.preInputBool)
            {
                player.clicked = true;
                yield return new WaitUntil(() => player.skillInputBool);
                if (player.isDead) yield break;
            }
            player.clicked = false;

            // 조준 스킬 사용시
            if (activeSkill.isAiming && player.curMP >= activeSkill.mpPoint)
            {
                aiming = true;
                StartAiming(activeSkill);
                yield return new WaitUntil(() => aimingTrigger || !aiming);

                if (!aimingTrigger)
                {
                    aimingTrigger = false;
                    aiming = false;
                    yield break;
                }
                else
                {
                    aimingTrigger = false;
                    aiming = false;
                }
            }

            //스킬 사용
            if (player.prevAction == "")
            {
                StartCoroutine(UseSkill(activeSkill));
            }
            else if (player.skillInputBool)
            {
                StartCoroutine(UseSkill(activeSkill));
            }
        }
    }
    
    IEnumerator UseSkill(ActiveSkill thisSkill)
    {
        player.rigid.velocity = Vector3.zero;

        if (player.curStamina > 0 && player.curMP >= thisSkill.mpPoint)
        {
            //print("UseSkill");
            player.ActionOutCheck(player.prevActionType);

            skilling = true;
            usingSkill = thisSkill;

            #region ANIMATION LENGTH
            if (thisSkill.name == "Punch")
            {
                player.animLength = 1.5f;
                player.preInputTime = 0.3f;
                player.attackInputTime = 0.7f;
                player.rollInputTime = 0.7f;
                player.skillInputTime = 0.7f;
                player.etcInputTime = 0.75f;
                player.exitTime = 1.3f;
            }
            else if(thisSkill.name == "Backstep Slash")
            {
                if(thisSkill.skillLv == 2) skillSpeed = 0.2f;
                else skillSpeed = 0;

                player.animLength = 0.883f * (1 - skillSpeed);
                player.preInputTime = 0.25f * (1 - skillSpeed);
                player.attackInputTime = 0.6f * (1 - skillSpeed);
                player.rollInputTime = 0.6f * (1 - skillSpeed);
                player.skillInputTime = 0.6f * (1 - skillSpeed);
                player.etcInputTime = 0.7f * (1 - skillSpeed);
                player.exitTime = 0.8f * (1 - skillSpeed);
            }
            else if (thisSkill.name == "Guarding Barrier")
            {
                player.animLength = 1.5f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.3f;
            }
            else if (thisSkill.name == "Fire Sword")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.45f;
            }
            else if (thisSkill.name == "Fireball")
            {
                player.animLength = 1.333f;
                player.preInputTime = 0.3f;
                player.attackInputTime = 0.8f;
                player.rollInputTime = 0.8f;
                player.skillInputTime = 0.8f;
                player.etcInputTime = 0.85f;
                player.exitTime = 1.1f;
            }
            else if (thisSkill.name == "Ground Wave")
            {
                player.animLength = 2.167f;
                player.preInputTime = 0.7f;
                player.attackInputTime = 1.45f;
                player.rollInputTime = 1.45f;
                player.skillInputTime = 1.45f;
                player.etcInputTime = 1.517f;
                player.exitTime = 1.9f;
            }
            else if (thisSkill.name == "Horizontal Slash")
            {
                player.animLength = 2.033f;
                player.preInputTime = 0.5f;
                player.attackInputTime = 1.3f;
                player.rollInputTime = 1.3f;
                player.skillInputTime = 1.3f;
                player.etcInputTime = 1.35f;
                player.exitTime = 1.8f;

            }
            else if (thisSkill.name == "Shield Shove")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.5f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.45f;

            }
            else if (thisSkill.name == "Power Shield")
            {
                if (thisSkill.skillLv == 2) skillSpeed = 0.3f;
                else skillSpeed = 0;

                player.animLength = 0.9f * (1 - skillSpeed);
                player.preInputTime = 0.2f * (1 - skillSpeed);
                player.attackInputTime = 0.5f * (1 - skillSpeed);
                player.rollInputTime = 0.5f * (1 - skillSpeed);
                player.skillInputTime = 0.5f * (1 - skillSpeed);
                player.etcInputTime = 0.6f * (1 - skillSpeed);
                player.exitTime = 0.7f * (1 - skillSpeed);

                player.prevActionType = "Skill";
                player.prevAction = "Skill";

                //print(thisSKill.name);
                weapon.Use(thisSkill.name);
                anim.CrossFade(thisSkill.name, 0.15f);
                yield break;
            }
            else if (thisSkill.name == "Double Slash")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.3f;
                player.attackInputTime = 0.9f;
                player.rollInputTime = 0.9f;
                player.skillInputTime = 0.9f;
                player.etcInputTime = 1.02f;
                player.exitTime = 1.5f;

            }
            else if (thisSkill.name == "Defence Wall")
            {
                player.animLength = 2.333f;
                player.preInputTime = 0.75f;
                player.attackInputTime = 0.95f;
                player.rollInputTime = 0.95f;
                player.skillInputTime = 0.95f;
                player.etcInputTime = 1.017f;
                player.exitTime = 2f;
            }
            else if (thisSkill.name == "Dash Sting")
            {
                player.animLength = 1.833f;
                player.preInputTime = 0.8f;
                player.attackInputTime = 1.45f;
                player.rollInputTime = 1.45f;
                player.skillInputTime = 1.45f;
                player.etcInputTime = 1.517f;
                player.exitTime = 1.517f;
            }
            else if (thisSkill.name == "Step Slash")
            {
                player.animLength = 2f;
                player.preInputTime = 0.35f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.2f;
                player.exitTime = 1.7f;
            }
            else if (thisSkill.name == "Icicle")
            {
                player.animLength = 1.4f;
                player.preInputTime = 0.5f;
                player.attackInputTime = 1f;
                player.rollInputTime = 1f;
                player.skillInputTime = 1f;
                player.etcInputTime = 1.1f;
                player.exitTime = 1.2f;
            }
            else if (thisSkill.name == "Lighting Spear")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.45f;
                player.attackInputTime = 0.95f;
                player.rollInputTime = 0.95f;
                player.skillInputTime = 0.95f;
                player.etcInputTime = 1.05f;
                player.exitTime = 1.3f;
            }
            else if (thisSkill.name == "Fire Burst")
            {
                player.animLength = 2.167f;
                player.preInputTime = 0.66f;
                player.attackInputTime = 1.5f;
                player.rollInputTime = 1.5f;
                player.skillInputTime = 1.5f;
                player.etcInputTime = 1.6f;
                player.exitTime = 1.8f;
            }
            else if (thisSkill.name == "Ice Spear")
            {
                player.animLength = 2.3f;
                player.preInputTime = 0.8f;
                player.attackInputTime = 1.66f;
                player.rollInputTime = 1.66f;
                player.skillInputTime = 1.66f;
                player.etcInputTime = 1.76f;
                player.exitTime = 2;
            }
            else if (thisSkill.name == "Lighting Split")
            {
                player.animLength = 2.1f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.4f;
                player.rollInputTime = 1.4f;
                player.skillInputTime = 1.4f;
                player.etcInputTime = 1.5f;
                player.exitTime = 1.8f;
            }
            else if (thisSkill.name == "Fire Tornado")
            {
                player.animLength = 2.167f;
                player.preInputTime = 0.66f;
                player.attackInputTime = 1.5f;
                player.rollInputTime = 1.5f;
                player.skillInputTime = 1.5f;
                player.etcInputTime = 1.6f;
                player.exitTime = 1.8f;
            }
            else if (thisSkill.name == "Ice Storm")
            {
                player.animLength = 2.167f;
                player.preInputTime = 0.66f;
                player.attackInputTime = 1.5f;
                player.rollInputTime = 1.5f;
                player.skillInputTime = 1.5f;
                player.etcInputTime = 1.6f;
                player.exitTime = 1.8f;
            }
            else if (thisSkill.name == "Lighting Thunder")
            {
                player.animLength = 2.433f;
                player.preInputTime = 0.9f;
                player.attackInputTime = 1.76f;
                player.rollInputTime = 1.76f;
                player.skillInputTime = 1.76f;
                player.etcInputTime = 1.9f;
                player.exitTime = 2f;
            }
            else if (thisSkill.name == "Gravity Hole")
            {
                player.animLength = 2.1f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.4f;
                player.rollInputTime = 1.4f;
                player.skillInputTime = 1.4f;
                player.etcInputTime = 1.5f;
                player.exitTime = 1.8f;
            }
            else if (thisSkill.name == "Lighting Sword")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.45f;
            }
            else if (thisSkill.name == "Ice Sword")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.45f;
            }
            else if (thisSkill.name == "Heavy Sword")
            {
                player.animLength = 1.667f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.45f;
            }
            else if (thisSkill.name == "Final Smash")
            {
                player.animLength = 2.2f;
                player.preInputTime = 0.7f;
                player.attackInputTime = 1.5f;
                player.rollInputTime = 1.5f;
                player.skillInputTime = 1.5f;
                player.etcInputTime = 1.6f;
                player.exitTime = 1.9f;

                weapon.Use(thisSkill.name);
                yield break;
            }
            else if (thisSkill.name == "Guard Enhance")
            {
                player.animLength = 1.5f;
                player.preInputTime = 0.4f;
                player.attackInputTime = 0.9f;
                player.rollInputTime = 0.9f;
                player.skillInputTime = 0.9f;
                player.etcInputTime = 1f;
                player.exitTime = 1.3f;
            }
            else if (thisSkill.name == "Shield Rush")
            {
                player.animLength = 2.467f;
                player.preInputTime = 1f;
                player.attackInputTime = 1.8f;
                player.rollInputTime = 1.8f;
                player.skillInputTime = 1.8f;
                player.etcInputTime = 1.9f;
                player.exitTime = 2.1f;
            }
            else if (thisSkill.name == "Magic Recover")
            {
                player.animLength = 1.5f;
                player.preInputTime = 0.6f;
                player.attackInputTime = 1.1f;
                player.rollInputTime = 1.1f;
                player.skillInputTime = 1.1f;
                player.etcInputTime = 1.183f;
                player.exitTime = 1.3f;
            }

            #endregion

            StartSkillAction(thisSkill);
        }
    }

    public void StartSkillAction(ActiveSkill skill)
    {
        //Debug.Log("StartSkillAction");

        player.prevActionType = "Skill";
        player.prevAction = "Skill";

        player.StartActionTiming();
        //print(thisSKill.name);
        weapon.Use(skill.name);
        playerAnim.PlayAnimation(skill.name, true, 0.1f);
    }

    public void StartAiming(ActiveSkill skill){
        GameObject skillRangeImg = skillAimRangeImage;
        GameObject skillDistanceImg = skillAimDistanceImage;
        float maxDistance = 10f;
        float skillScale = 1f;
        bool isDrag = false;

        switch(skill.name){
            case "Defence Wall":
                skillRangeImg = defenceWallSkillRange;
                skillDistanceImg = skillAimDistanceImage;
                maxDistance = 12f;
                skillScale = 1f;
                isDrag = true;
                break;

            case "Fire Tornado":
                skillRangeImg = skillAimRangeImage;
                skillDistanceImg = skillAimDistanceImage;
                maxDistance = 10f;
                skillScale = 0.85f;
                isDrag = false;
                break;

            case "Ice Storm":
                skillRangeImg = skillAimRangeImage;
                skillDistanceImg = skillAimDistanceImage;
                maxDistance = 10f;
                skillScale = 1.75f;
                isDrag = false;
                break;

            case "Lighting Thunder":
                skillRangeImg = skillAimRangeImage;
                skillDistanceImg = skillAimDistanceImage;
                maxDistance = 12f;
                skillScale = 0.7f;
                isDrag = false;
                break;

            case "Gravity Hole":
                skillRangeImg = skillAimRangeImage;
                skillDistanceImg = skillAimDistanceImage;
                maxDistance = 8f;
                skillScale = skill.skillLv == 1 ? 1.6f : 1.6f * 1.4f;
                isDrag = false;
                break;

            default:
                Debug.Log("No Aiming Skill");
                break;
        }

        StartCoroutine(SkillAiming(skillRangeImg, skillDistanceImg, maxDistance, skillScale, isDrag));
    }

    IEnumerator SkillAiming(GameObject _aimRange, GameObject _aimDistance, float maxDistance, float rangeScale, bool isDrag)
    {
        bool isDragging = false;

        LayerMask groundLayer = 1 << 6;
        InputState.Change(PlayerInputControls.Instance.virtualMouse.position, new Vector2(Screen.width/2, Screen.height/2));

        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputControls.Instance.virtualMouse.position.ReadValue())
            , out hit, Mathf.Infinity, groundLayer);

        Vector3 startPos = hit.point;
        Vector3 endPos = Vector3.zero;
        Vector3 dragDir = Vector3.zero;

        Vector3 dir = hit.point - transform.position;
        dir.y = 0;

        Quaternion rot = Quaternion.LookRotation(dir);
        GameObject aimingCursor = Instantiate(_aimRange, hit.point, rot);
        aimingCursor.transform.localScale = Vector3.one * rangeScale;
        GameObject aimDistance = Instantiate(_aimDistance, transform.position, Quaternion.identity, transform);
        aimDistance.transform.localScale = Vector3.one * maxDistance * 0.11f;
        PlayerInputControls.Instance.VirtualCursorActivate(isDrag);

        //Debug.Log(aimingCursor.transform.position);

        yield return null;

        // 드래그 스킬
        while(isDrag){

            // 캔슬
            if (PlayerInputControls.Instance.playerInputAction.Player.Fire2.WasPerformedThisFrame() || player.isStaggering)
            {
                GameManager.Instance.DefaultCursor();
                PlayerInputControls.Instance.VirtualCursorActivate(false);
                Destroy(aimingCursor);
                Destroy(aimDistance);
                aiming = false;
                player.SkillOut();
                print("aim cancle");
                yield break;
            }

            Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputControls.Instance.virtualMouse.position.ReadValue())
                , out hit, Mathf.Infinity, groundLayer);

            if(!isDragging){
                dir = hit.point - transform.position;
                dir.y = 0;
                rot = Quaternion.LookRotation(dir);

                startPos = transform.position + Vector3.ClampMagnitude(hit.point - transform.position, maxDistance);
            }

            if(PlayerInputControls.Instance.playerInputAction.Player.Fire1.WasPerformedThisFrame()){
                Debug.Log("WasPerformedThisFrame");
                isDragging = true;
                
                dir = hit.point - transform.position;
                dir.y = 0;

                rot = Quaternion.LookRotation(dir);
                startPos = transform.position + Vector3.ClampMagnitude(startPos - transform.position, maxDistance);
            }
            else if(PlayerInputControls.Instance.playerInputAction.Player.Fire1.IsPressed()){

                // 드래그
                if (Vector3.Distance(startPos, hit.point) > 0.4f)
                {
                    endPos = hit.point;
                    dir = endPos - startPos;
                    dir.y = 0;

                    rot = Quaternion.LookRotation(dir);
                    startPos = transform.position + Vector3.ClampMagnitude(startPos - transform.position, maxDistance);
                    
                    aimRot = rot;
                }
            }
            else if(isDragging && PlayerInputControls.Instance.playerInputAction.Player.Fire1.WasReleasedThisFrame()
            && (player.preInputBool || player.skillInputBool|| player.prevAction == "")){
                Debug.Log("WasReleasedThisFrame");

                aimPos = startPos;
                aimRot = rot;

                Destroy(aimingCursor);
                Destroy(aimDistance);
                GameManager.Instance.DefaultCursor();
                PlayerInputControls.Instance.VirtualCursorActivate(false);
                aimingTrigger = true;
                yield break;
            }
            else{
                isDragging = false;
            }

            aimingCursor.transform.position = startPos;
            aimingCursor.transform.rotation = rot;
            yield return null;
        }

        // 노 드래그 스킬
        while (!isDrag)
        {
            // 캔슬
            if (PlayerInputControls.Instance.playerInputAction.Player.Fire2.WasPerformedThisFrame() || player.isStaggering)
            {
                GameManager.Instance.DefaultCursor();
                PlayerInputControls.Instance.VirtualCursorActivate(false);
                Destroy(aimingCursor);
                Destroy(aimDistance);
                aiming = false;
                player.SkillOut();
                print("aim cancle");
                yield break;
            }

            Physics.Raycast(Camera.main.ScreenPointToRay(PlayerInputControls.Instance.virtualMouse.position.ReadValue())
                , out hit, Mathf.Infinity, groundLayer);

            dir = hit.point - transform.position;
            dir.y = 0;
            
            startPos = transform.position + Vector3.ClampMagnitude(hit.point - transform.position, maxDistance);
            rot = Quaternion.LookRotation(dir);

            aimingCursor.transform.position = startPos;
            aimingCursor.transform.rotation = rot;

            if (PlayerInputControls.Instance.playerInputAction.Player.Fire1.WasPerformedThisFrame() 
            && (player.preInputBool || player.skillInputBool || player.prevAction == ""))
            {
                aimPos = startPos;
                aimRot = rot;

                Destroy(aimingCursor);
                Destroy(aimDistance);
                GameManager.Instance.DefaultCursor();
                PlayerInputControls.Instance.VirtualCursorActivate(false);
                aimingTrigger = true;
                yield break;
            }

            yield return null;
        }
    }

    
}
