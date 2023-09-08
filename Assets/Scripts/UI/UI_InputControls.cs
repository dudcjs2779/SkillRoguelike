using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InputControls : MonoBehaviour
{
    public UI_InputControls Instance { get; private set; }

    public UI_InputAction ui_InputAction;

    Vector2 mouseAxis;

    Vector2 ui_AxisRaw;
    public bool escDown;
    public bool doChangeTabLeft;
    public bool doChangeTabRight;
    public float holdTime;

    private void Awake() {
        if (Instance == null) Instance = this;
        ui_InputAction = new UI_InputAction();

        ui_InputAction.UI.Disable();

        //UI 입력
        ui_InputAction.UI.Escape.performed += Escape;
        ui_InputAction.UI.Escape.canceled += Escape;
        ui_InputAction.UI.Axis.performed += UI_Axis;
        ui_InputAction.UI.Axis.canceled += UI_Axis;
        ui_InputAction.UI.TabChangeLeft.performed += TabChangeLeft;
        ui_InputAction.UI.TabChangeLeft.canceled += TabChangeLeft;
        ui_InputAction.UI.TabChageRight.performed += TabChangeRight;
        ui_InputAction.UI.TabChageRight.canceled += TabChangeRight;

        ui_InputAction.UI.MouseAxis.performed += MouseAxis;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }

    private void MouseLeft(InputAction.CallbackContext context)
    {
    }

    private void MouseAxis(InputAction.CallbackContext context)
    {
        mouseAxis = context.ReadValue<Vector2>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.wasUpdatedThisFrame || mouseAxis.magnitude != 0){
            //Debug.Log("KeyboardMouse");
            PlayerInputControls.Instance.controlType = PlayerInputControls.ControlType.KeyboardMouse;
        }
        else if(Gamepad.all.Count > 0 && Gamepad.current.wasUpdatedThisFrame){
            //Debug.Log("Gamepad");
            PlayerInputControls.Instance.controlType = PlayerInputControls.ControlType.GamePad;
        }

        if (ui_AxisRaw.magnitude != 0)
        {
            holdTime += Time.deltaTime * 2f;
            holdTime = Mathf.Clamp(holdTime, 1, 4);
        }
    }

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
            holdTime = 1;
            //Debug.Log(ui_AxisRaw);
        }
        else if (context.canceled)
        {
            ui_AxisRaw = context.ReadValue<Vector2>();
            holdTime = 1;
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

    
}
