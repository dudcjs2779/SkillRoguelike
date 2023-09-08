using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillStat : MonoBehaviour
{
    Animator anim;
    [SerializeField] Toggle tabChangeToggle;
   
    [SerializeField] SelectTab selectTab;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable(){
        Debug.Log("OnStatSkill");
        tabChangeToggle.isOn = false;
        anim.Play("CloseSkillTab", 0, 1);
    }

    private void Start() {
        if (!Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.StatSkill))
            gameObject.SetActive(false);
        
    }

    private void Update() {
        TabChange();
    }

    public void OnClickSkillStatToggle(bool isOpen){
        Debug.Log("OnClickSkillStatToggle");
        anim.SetBool("OpenSkillTab", isOpen);
        if(isOpen){
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.skillTabFirst);
        }
        else{
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectTab.statTabFirst);
        }
        
    }

    void TabChange(){
        if(Canvas5.Instance.UISequenceList.Count > 0 && Canvas5.Instance.UISequenceList[Canvas5.Instance.UISequenceList.Count - 1] != Canvas5.UIType.StatSkill) return;
        if (PlayerInputControls.Instance.doChangeTabLeft)
        {
            PlayerInputControls.Instance.doChangeTabLeft = false;
            tabChangeToggle.isOn = false;
        }
        else if (PlayerInputControls.Instance.doChangeTabRight)
        {
            PlayerInputControls.Instance.doChangeTabRight = false;
            tabChangeToggle.isOn = true;
        }
    }
}
