using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class PlayerInputControls : MonoBehaviour
{
    private static PlayerInputControls instance;
    public static PlayerInputControls Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<PlayerInputControls>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<PlayerInputControls>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }

    [SerializeField] Player player;
    [SerializeField] PlayerSkill playerSkill;
    [Header("INPUT")]
    public PlayerInput playerInput;
    public PlayerInputAction playerInputAction;

    public enum ControlType { KeyboardMouse, GamePad }
    public ControlType controlType;

    [Header("MOUSE CURSOR")]
    public Mouse virtualMouse;
    private Camera mainCamera;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Canvas cursorCanvas;
    [SerializeField] RectTransform cursorRect;
    private Vector2 mousePosition;
    private Vector2 mouseDelta;
    public float mouseSpeed;
    public Vector2 rightStick_Axis;
    public float rightStick_Speed;

    [Header("MOVEMENT")]
    public float hAxisRaw;
    public float vAxisRaw, hAxis, vAxis;
    public float mouseScrollLeft;
    public float mouseScrollRight;
    public Vector3 hvInputVec;
    public Vector3 hvRawInputVec;
    public Vector2 smoothInputVec;
    public Vector2 smoothInputVel;

    [Header("PLAYER TRIGGER")]
    public bool f1Down;
    public bool f2Down;
    public bool f2Up;
    public bool skillTrigger, skill1, skill2, skill3, skill4;
    public bool skillChangeDown;
    public bool dDown;
    public bool healDown;
    public bool lockOnDown;
    public bool lockOnLeftDown;
    public bool lockOnRightDown;
    public bool interactDown;

    public bool menuDown;
    public bool statPanelDown;

    public bool skillWindowDown;
    public bool lvUpDown;
    public bool shopDown;
    public bool selectTabDown;

    // UI 전용
    [Header("UI")]
    Vector2 mouseAxis;
    Vector2 ui_AxisRaw;
    public float holdTime;
    public float holdDelayTime;
    public bool escDown;
    public bool doChangeTabLeft;
    public bool doChangeTabRight;
    public bool submitPressed;
    public bool skipCutSceneDown;

    private void Awake()
    {
        var objs = FindObjectsOfType<GameManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        GameManager.Instance.Action_Init_GameManager += Init_PlayerInputControls;

        playerInput = GetComponent<PlayerInput>();
        playerInputAction = new PlayerInputAction();

        playerInputAction.Player.Disable();
        playerInputAction.PlayerUI.Disable();
        playerInputAction.UI.Enable();
        playerInputAction.Gamepad.Enable();

        if (!GameManager.Instance.isTitle)
        {
            Init_PlayerInputControls();
        }

        //Player 컨트롤
        playerInputAction.Player.SkillTrigger.performed += SkillTrigger;
        playerInputAction.Player.SkillTrigger.canceled += SkillTrigger;

        playerInputAction.Player.SkillChange.performed += SkillChange;

        playerInputAction.Player.Fire1.performed += Fire1;
        playerInputAction.Player.Fire2.performed += Fire2;
        playerInputAction.Player.Fire2.canceled += Fire2;
        playerInputAction.Player.Fire3.performed += Fire3;
        playerInputAction.Player.Fire4.performed += Fire4;

        playerInputAction.Player.Dodge.performed += Dodge;
        playerInputAction.Player.Heal.performed += Heal;

        playerInputAction.Player.LockOn.performed += LockOn;
        playerInputAction.Player.LockOn.canceled += LockOn;

        playerInputAction.Player.LockOnLeft.performed += LockOnLeft;
        playerInputAction.Player.LockOnRight.performed += LockOnRight;

        playerInputAction.Player.Interact.performed += Interact;
        playerInputAction.Player.Interact.canceled += Interact;

        playerInputAction.Player.MousePosition.performed += x => mousePosition = x.ReadValue<Vector2>();
        playerInputAction.Player.MouseDelta.performed += x => mouseDelta = x.ReadValue<Vector2>();

        //Player & UI 컨트롤
        playerInputAction.PlayerUI.Menu.performed += Menu;
        playerInputAction.PlayerUI.StatPanel.performed += StatPanel;

        //UI 컨트롤
        //UI 입력
        playerInputAction.UI.Escape.performed += Escape;
        playerInputAction.UI.Escape.canceled += Escape;
        playerInputAction.UI.Axis.performed += UI_Axis;
        playerInputAction.UI.Axis.canceled += UI_Axis;
        playerInputAction.UI.TabChangeLeft.performed += TabChangeLeft;
        playerInputAction.UI.TabChangeLeft.canceled += TabChangeLeft;
        playerInputAction.UI.TabChageRight.performed += TabChangeRight;
        playerInputAction.UI.TabChageRight.canceled += TabChangeRight;
        playerInputAction.UI.DialogueSubmit.performed += DialogueSubmit;
        playerInputAction.UI.DialogueSubmit.canceled += DialogueSubmit;
        playerInputAction.UI.SkipCutScene.performed += SkipCutScene;
        playerInputAction.UI.SkipCutScene.canceled += SkipCutScene;

        playerInputAction.UI.MouseAxis.performed += MouseAxis;
    }

    private void SkipCutScene(InputAction.CallbackContext context)
    {
        skipCutSceneDown = context.performed;
    }

    public void Init_PlayerInputControls()
    {
        player = GameObject.Find("Player")?.GetComponent<Player>();
        playerSkill = player?.GetComponent<PlayerSkill>();
        playerInput.camera = Camera.main;
        cursorCanvas = FindObjectOfType<Canvas0>()?.GetComponent<Canvas>();
        canvasRect = cursorCanvas?.GetComponent<RectTransform>();
        cursorRect = cursorCanvas?.GetComponentInChildren<VirtualMouse>().GetComponent<RectTransform>();

        mainCamera = Camera.main;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorRect != null)
        {
            Vector2 position = cursorRect.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }
        InputSystem.onAfterUpdate += UpdateMotion;


        ChangeMapPlayer();
        // playerInputAction.Player.Enable();
        // playerInputAction.PlayerUI.Enable();
        // playerInputAction.UI.Disable();
    }

    private void OnEnable()
    {
        // if (GameManager.Instance.isTitle) return;

        // mainCamera = Camera.main;

        // if (virtualMouse == null)
        // {
        //     virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        // }
        // else if (!virtualMouse.added)
        // {
        //     InputSystem.AddDevice(virtualMouse);
        // }

        // InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        // if (cursorRect != null)
        // {
        //     Vector2 position = cursorRect.anchoredPosition;
        //     InputState.Change(virtualMouse.position, position);
        // }
        // InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void Start()
    {

    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    void Update()
    {
        if (controlType != ControlType.KeyboardMouse && Mouse.current.delta.IsPressed() && playerInputAction.Player.GamepadRightStickAxis.ReadValue<Vector2>() == Vector2.zero)
        {
            //Debug.Log("KeyboardMouse");
            controlType = ControlType.KeyboardMouse;
        }
        else if (Gamepad.current != null && controlType != ControlType.GamePad && playerInputAction.Gamepad.Any.WasPressedThisFrame())
        {
            //Debug.Log("Gamepad");
            controlType = ControlType.GamePad;
        }

        // Gamepad로 슬라이더 컨트롤시 가중치
        if (ui_AxisRaw.magnitude != 0)
        {
            holdDelayTime += Time.deltaTime;
            if (holdDelayTime > 1.5f)
            {
                holdTime += Time.deltaTime * 0.5f;
                holdTime = Mathf.Clamp(holdTime, 0, 1);
            }
        }

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            // Debug.Log("Player" + playerInputAction.Player.enabled);
            // Debug.Log("PlayerUI" + playerInputAction.PlayerUI.enabled);
        }

        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            // playerInputAction.Player.Enable();
            // playerInputAction.PlayerUI.Enable();
            // playerInputAction.UI.Disable();

            // Debug.Log("Player" + playerInputAction.Player.enabled);
            // Debug.Log("PlayerUI" + playerInputAction.PlayerUI.enabled);
            // Debug.Log("UI" + playerInputAction.UI.enabled);
        }

        if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            // playerInputAction.Player.Disable();
            // playerInputAction.PlayerUI.Disable();
            // playerInputAction.UI.Enable();

            // Debug.Log("Player" + playerInputAction.Player.enabled);
            // Debug.Log("PlayerUI" + playerInputAction.PlayerUI.enabled);
            // Debug.Log("UI" + playerInputAction.UI.enabled);
        }

        if (GameManager.Instance.isTitle) return;

        // Movement 백터
        Vector2 inputVec = playerInputAction.Player.Movement.ReadValue<Vector2>();
        hAxisRaw = inputVec.x;
        vAxisRaw = inputVec.y;
        hvRawInputVec = new Vector3(hAxisRaw, 0, vAxisRaw);

        smoothInputVec = Vector2.SmoothDamp(smoothInputVec, inputVec, ref smoothInputVel, 0.15f);
        hAxis = smoothInputVec.x;
        vAxis = smoothInputVec.y;
        hvInputVec = new Vector3(hAxis, 0, vAxis);

        //Debug.Log(hvInputVec);
        //Debug.Log(playerInputActions.Player.MouseScrollY.ReadValue<float>());
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || !playerSkill.aiming)
        {
            return;
        }

        Vector2 deltaValue = controlType == ControlType.KeyboardMouse ?
            Mouse.current.delta.ReadValue() : playerInputAction.Player.GamepadRightStickAxis.ReadValue<Vector2>();

        deltaValue *= controlType == ControlType.KeyboardMouse ?
            mouseSpeed * Time.deltaTime : rightStick_Speed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, position, cursorCanvas.renderMode ==
        RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);

        cursorRect.anchoredPosition = anchoredPosition;
    }

    public void VirtualCursorActivate(bool isOn)
    {
        cursorRect.GetComponent<UnityEngine.UI.Image>().enabled = isOn;
    }

    // ===== Player 컨트롤====
    void SkillTrigger(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("SkillTrigger Down");
            skillTrigger = true;

            //Gamepad.current.leftTrigger.wasPressedThisFrame &&
            if (Gamepad.all.Count > 0 && GameManager.Instance.playerData.clearLv != 0 && context.control.path == playerInputAction.Player.SkillTrigger.controls[1].path)
            {
                skillChangeDown = false;
            }
            else if (Gamepad.all.Count > 0 && GameManager.Instance.playerData.clearLv != 0 && context.control.path == playerInputAction.Player.SkillTrigger.controls[2].path)
            {
                skillChangeDown = true;
            }

            //Debug.Log(context.control.displayName);
        }
        else if (context.canceled)
        {
            //Debug.Log("SkillTrigger Up");
            skillTrigger = false;

            if (Gamepad.all.Count > 0 && (context.control.path == playerInputAction.Player.SkillTrigger.controls[1].path || context.control.path == playerInputAction.Player.SkillTrigger.controls[2].path))
            {
                skillChangeDown = false;
            }
        }
    }

    void SkillChange(InputAction.CallbackContext context)
    {
        Debug.Log("SkillChange");
        if (context.performed && GameManager.Instance.playerData.clearLv != 0)
        {
            skillChangeDown = !skillChangeDown;
        }

    }

    void Fire1(InputAction.CallbackContext context)
    {
        if (context.performed && !skillTrigger && !playerSkill.aiming)
        {
            //Debug.Log("Fire1");
            f1Down = true;
        }
        else if (context.performed && skillTrigger && !skill1 && !playerSkill.aiming)
        {
            //Debug.Log("Skill1");
            skill1 = true;
        }
    }

    void Fire2(InputAction.CallbackContext context)
    {
        if (context.performed && !player.isSA && !skillTrigger && !f2Down && !playerSkill.aiming && !player.isInputCancle)
        {
            //Debug.Log("Fire2Down");
            f2Down = true;
        }
        else if (context.canceled && player.isCharging)
        {
            //Debug.Log("Fire2Up");
            f2Up = true;
        }
        else if (context.performed && skillTrigger && !skill2 && !playerSkill.aiming)
        {
            //Debug.Log("Fire4");
            skill2 = true;
        }
    }

    void Fire3(InputAction.CallbackContext context)
    {
        if (context.performed && skillTrigger && !skill3 && !playerSkill.aiming)
        {
            //Debug.Log("Fire3");
            skill3 = true;
        }
    }

    void Fire4(InputAction.CallbackContext context)
    {
        if (context.performed && skillTrigger && !skill4 && !playerSkill.aiming)
        {
            //Debug.Log("Fire4");
            skill4 = true;
        }
    }

    void Dodge(InputAction.CallbackContext context)
    {
        if (context.performed && !skillTrigger && !player.isInputCancle)
        {
            //Debug.Log("Dodge");
            dDown = true;
        }
    }

    void Heal(InputAction.CallbackContext context)
    {
        if (context.performed && !skillTrigger)
        {
            //Debug.Log("Heal");
            healDown = true;
        }
    }

    void LockOn(InputAction.CallbackContext context)
    {
        //Debug.Log("LockOn");
        lockOnDown = context.performed;
    }

    void LockOnLeft(InputAction.CallbackContext context)
    {
        //Debug.Log("LockOnUp");
        if (context.performed)
        {
            mouseScrollLeft = context.ReadValue<float>();
            //Debug.Log(mouseScrollLeft);
        }

    }

    void LockOnRight(InputAction.CallbackContext context)
    {
        //Debug.Log("LockOnDown");
        if (context.performed)
        {
            mouseScrollRight = context.ReadValue<float>();
            //Debug.Log(mouseScrollRight);
        }

    }

    void Interact(InputAction.CallbackContext context)
    {
        //Debug.Log("Interact");
        interactDown = context.performed;
    }

    // ===== PlayerUI 컨트롤====
    void Menu(InputAction.CallbackContext context)
    {
        Debug.Log("Menu");

        if (context.performed && Canvas5.Instance.UISequenceList.Count == 0)
        {
            Canvas5.Instance.OpenMenu();
        }

    }

    void StatPanel(InputAction.CallbackContext context)
    {
        Debug.Log("StatTab");
        if (context.performed && Canvas5.Instance.UISequenceList.Count == 0 && !Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatPanel))
        {
            statPanelDown = true;
            Canvas5.Instance.OpenStatPanel();
        }
        else if (context.performed && Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatPanel))
        {
            statPanelDown = false;
            Canvas5.Instance.CloseStatPanel();
        }
    }

    // ===== UI 컨트롤====
    void Escape(InputAction.CallbackContext context)
    {
        //Debug.Log("Escape");
        escDown = context.performed;
    }

    void UI_Axis(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ui_AxisRaw = context.ReadValue<Vector2>();
            holdTime = 0;
            holdDelayTime = 0;
            //Debug.Log(ui_AxisRaw);
        }
        else if (context.canceled)
        {
            ui_AxisRaw = context.ReadValue<Vector2>();
            holdTime = 0;
            holdDelayTime = 0;
            //Debug.Log(ui_AxisRaw);
        }

    }

    void TabChangeLeft(InputAction.CallbackContext context)
    {
        doChangeTabLeft = context.performed;
        //Debug.Log("doChangeTabLeft");

    }

    void TabChangeRight(InputAction.CallbackContext context)
    {
        doChangeTabRight = context.performed;
        Debug.Log("doChangeTabRight " + doChangeTabRight);
    }

    private void MouseAxis(InputAction.CallbackContext context)
    {
        mouseAxis = context.ReadValue<Vector2>();
    }

    public void DialogueSubmit(InputAction.CallbackContext context)
    {
        //Debug.Log("DialogueSubmit: " + submitPressed.ToString());

        //Debug.Log("DialogueSubmit");
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }

    public bool GetDialogueSubmit()
    {
        //Debug.Log("GetDialogueSubmit");
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

    public void ToTitle()
    {
        playerInputAction.Player.Disable();
        playerInputAction.PlayerUI.Disable();
        playerInputAction.UI.Enable();
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    public void ChangeMapUI()
    {
        playerInputAction.Player.Disable();
        playerInputAction.PlayerUI.Enable();
        playerInputAction.UI.Enable();
    }

    public void ChangeMapPlayer()
    {
        playerInputAction.Player.Enable();
        playerInputAction.PlayerUI.Enable();
        playerInputAction.UI.Disable();

        if (!GameManager.Instance.isDebugMode && !GameManager.Instance.isInDungeon)
        {
            Debug.Log("ChangeMapStartMap");
            ChangeMapStartMap();
        }
    }

    public void ChangeMapDialogue()
    {
        playerInputAction.Player.Disable();
        playerInputAction.PlayerUI.Disable();
        playerInputAction.UI.Enable();
    }

    public void ChnageMapTrainingEnter()
    {
        playerInputAction.Player.Fire1.Enable();
        playerInputAction.Player.Fire2.Enable();
        playerInputAction.Player.Fire3.Enable();
        playerInputAction.Player.Fire4.Enable();
        playerInputAction.Player.Heal.Enable();
        playerInputAction.Player.LockOn.Enable();
    }

    public void ChangeMapStartMap()
    {
        playerInputAction.Player.Fire1.Disable();
        playerInputAction.Player.Fire2.Disable();
        playerInputAction.Player.Fire3.Disable();
        playerInputAction.Player.Fire4.Disable();
        playerInputAction.Player.Heal.Disable();
        playerInputAction.Player.LockOn.Disable();
    }
}
