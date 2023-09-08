using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RebindingKey : MonoBehaviour
{
    public TextMeshProUGUI keyText;
    public int bindingIndex;
    public string keyName;
    public bool isFixedKey;

    Button btn;

    OptionMenu optionMenu;

    private void Awake()
    {
        btn = GetComponentInChildren<Button>();
        optionMenu = GetComponentInParent<OptionMenu>();

    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        Init_Key();
    }

    public void Init_Key(){
        if (isFixedKey)
        {
            btn.interactable = false;
        }

        InputAction action = PlayerInputControls.Instance.playerInputAction.FindAction(keyName);

        if (action == null) return;

        keyText.text = action.bindings[bindingIndex].ToDisplayString();
    }

    public void KeyRebinding()
    {
        InputAction action = PlayerInputControls.Instance.playerInputAction.FindAction(keyName);
        if (action == null)
        {
            Debug.Log("Action is not exist");
            return;
        }
        PlayerInputControls.Instance.playerInputAction.PlayerUI.Disable();
        PlayerInputControls.Instance.playerInputAction.UI.Escape.Disable();
        action.Disable();
        optionMenu.InteractableOptionMenu(false);
        // CanvasGroup keySetting_CanvasGroup = optionMenu.KeySettingRect.GetComponent<CanvasGroup>();
        // CanvasGroup optionTab_CanvasGroup = optionMenu.OptionTab.GetComponent<CanvasGroup>();
        // keySetting_CanvasGroup.interactable = false;
        // optionTab_CanvasGroup.interactable = false;
        // keySetting_CanvasGroup.blocksRaycasts = false;
        // optionTab_CanvasGroup.blocksRaycasts = false;

        keyText.text = "_";
        action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(callback =>
            {
                //Debug.Log(callback);
                Debug.Log("Complete: " + callback.action.bindings[bindingIndex].overridePath);
                callback.Dispose();

                PlayerInputControls.Instance.playerInputAction.PlayerUI.Enable();
                PlayerInputControls.Instance.playerInputAction.UI.Escape.Enable();
                action.Enable();

                optionMenu.InteractableOptionMenu(true);
                // keySetting_CanvasGroup.interactable = true;
                // optionTab_CanvasGroup.interactable = true;
                // keySetting_CanvasGroup.blocksRaycasts = true;
                // optionTab_CanvasGroup.blocksRaycasts = true;


                keyText.text = callback.action.bindings[bindingIndex].ToDisplayString();
                optionMenu.isOptionChanged = true;
            })
            .OnCancel(callback =>
            {
                //Debug.Log(callback);
                Debug.Log("escape: " + callback.action.bindings[bindingIndex].overridePath);
                callback.Dispose();

                PlayerInputControls.Instance.playerInputAction.PlayerUI.Enable();
                PlayerInputControls.Instance.playerInputAction.UI.Escape.Enable();
                action.Enable();

                optionMenu.InteractableOptionMenu(true);
                // keySetting_CanvasGroup.interactable = true;
                // optionTab_CanvasGroup.interactable = true;
                // keySetting_CanvasGroup.blocksRaycasts = true;
                // optionTab_CanvasGroup.blocksRaycasts = true;

                keyText.text = callback.action.bindings[bindingIndex].ToDisplayString();
            })
            .Start();
    }
}
