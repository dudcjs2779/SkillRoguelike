using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] GameObject exclamationMark;
    [SerializeField] private Animator emoteAnimator;
    
    [SerializeField] string keyboardKey;
    [SerializeField] string gamepadKey;
    [SerializeField] string explain;

    [Header("Ink JSON")]
    [SerializeField] protected private TextAsset inkJSON;

    private bool playerInRange;

    protected virtual void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(exclamationMark.activeSelf){
            exclamationMark.transform.LookAt(Camera.main.transform, Vector3.up);
        }

        Interact();
    }

    public void KeyHintUpdate(){
        if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
        {
            GameUI.Instance.keyHint.KeyHintUpdate(keyboardKey, explain);
        }
        else
        {
            GameUI.Instance.keyHint.KeyHintUpdate(gamepadKey, explain);
        }
    }

    void Interact(){
        if (PlayerInputControls.Instance.interactDown && GameUI.Instance.keyHint.gameObject.activeSelf && playerInRange)
        {
            PlayerInputControls.Instance.interactDown = false;
            DialogueManager.Instance.ExitDialogueAction += Activate;
            DialogueManager.Instance.EnterDialogueMode(inkJSON, emoteAnimator);

        }
    }

    protected virtual void Activate(){
        Canvas5.Instance.OpenSelectTab();
        DialogueManager.Instance.ExitDialogueAction -= Activate;
    }

    private void OnTriggerEnter(Collider other) {

        if(other.CompareTag("Player")){
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            {
                GameUI.Instance.keyHint.KeyHintUpdate(keyboardKey, explain);
            }
            else
            {
                GameUI.Instance.keyHint.KeyHintUpdate(gamepadKey, explain);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player"))
        {
            Vector3 dir = transform.position - other.transform.position;
            float angle = Vector3.Angle(dir, other.transform.forward);

            if(angle < 60){
                if(playerInRange) return;
                playerInRange = true;
                GameUI.Instance.keyHint.Activate();
            }
            else{
                playerInRange = false;
                GameUI.Instance.keyHint.Deactivate();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GameUI.Instance.keyHint.Deactivate();
        }
    }

    private void OnDestroy() {
        GameUI.Instance.keyHint.Deactivate();
    }
    
}
