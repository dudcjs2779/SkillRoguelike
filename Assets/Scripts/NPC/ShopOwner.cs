using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class ShopOwner : InteractableObject
{
    [SerializeField] string shopOwnerEvent;

    private void Start() {
        WhichEvent();
    }

    protected override void Activate(){
        //Debug.Log("ShopOwner");
        Canvas5.Instance.OpenSelectTab();
        DialogueManager.Instance.ExitDialogueAction -= Activate;
    }

    void WhichEvent(){
        int clearLv = GameManager.Instance.playerData.clearLv;
        // switch (clearLv)
        // {
        //     case 1:
        //         break;
        //     case 1:
        //         break;
        //     case 1:
        //         break;
        //     case 1:
        //         break;
        //     default:
        // }

        shopOwnerEvent = ((Ink.Runtime.StringValue)DialogueManager.Instance.GetVariableState("shopOwnerEvent")).value;
        //Debug.Log(shopOwnerEvent);
    }

    [ContextMenu("Change Valriable")]
    public void ChangeValriable(){
        DialogueManager.Instance.SetVariableState("shopOwnerEvent", shopOwnerEvent);
    }

    [ContextMenu("ShowShopOwnerEvent")]
    public void ShowShopOwnerEvent(){
        DialogueManager.Instance.dialogueVariables.DebugSetVariable();
    }
}
