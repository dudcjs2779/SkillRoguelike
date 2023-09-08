using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressAnyKey : MonoBehaviour
{
    [SerializeField] TitleMenu titleMenu;

    Animator anim;

    float time;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Start() {
        
    }

    bool IsAnimating(Animator animator, int layerIndex){
        if(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime <= 1 && !animator.IsInTransition(layerIndex)){
            return true;
        }
        else{
            return false;
        }
    }

    void Update()
    {
        if(!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Menu) && !IsAnimating(anim, 0) && PlayerInputControls.Instance.playerInputAction.UI.AnyKey.WasPerformedThisFrame()){
            OpenTitleMenu();
        }
    }

    void OpenTitleMenu(){
        anim.SetTrigger("FadeOut");
        titleMenu.gameObject.SetActive(true);
    }
}
