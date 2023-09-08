using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas0 : MonoBehaviour
{
    private static Canvas0 instance;
    public static Canvas0 Instance
    {
        get
        {
            return instance;
        }
    }

    CanvasGroup canvasGroup;
    public HUD_Skill hUD_Skill;
    public PlayerState playerState;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Destroyed Canvas0 instance: " + this.GetInstanceID());
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        canvasGroup = GetComponent<CanvasGroup>();
    }


    public void UiActivate(bool isOn){
        if(isOn){
            StartCoroutine(CanvasGroupAlphaLerp(canvasGroup, 1f, 0.5f));
        }
        else{
            StartCoroutine(CanvasGroupAlphaLerp(canvasGroup, 0f, 0f));
        }
    }

    IEnumerator CanvasGroupAlphaLerp(CanvasGroup canvasGroup, float alpha, float duration){
        if (duration == 0){
            canvasGroup.alpha = alpha;
            yield break;
        }

        float start = canvasGroup.alpha;
        float lerp = 0;
        float speed = 1 / duration;

    
        while(lerp < 1){
            canvasGroup.alpha = Mathf.Lerp(start, alpha, lerp);

            lerp += Time.deltaTime * speed;
            yield return null;
        }
    }
}
