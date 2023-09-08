using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class CutSceneManager : MonoBehaviour
{
    private static CutSceneManager instance;
    public static CutSceneManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private PlayableDirector director;
    [SerializeField] private SceneDirector sceneDirector;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private Canvas0 Canvas0;

    bool canSkip;
    Coroutine C_SkipMsg;
    Coroutine C_TmpAlphaLerp;

    [SerializeField] TextMeshProUGUI skipMsgText;

    private void Awake() {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Destroyed CutSceneManager instance: " + this.GetInstanceID());
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // brain = Camera.main.GetComponent<CinemachineBrain>();
        // playerCamera = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode){
        //StartCutScene();
        sceneDirector = FindObjectOfType<SceneDirector>();

        if(sceneDirector != null){
            sceneDirector.UiActivate = Canvas0.UiActivate;
            sceneDirector.SetPlayerCamera(brain, playerCamera);
            sceneDirector.Binding();
            sceneDirector.StartTimeLine();
        }
    }

    public void StartCutScene()
    {
        StartCoroutine(_StartCutScene());
    }


    IEnumerator _StartCutScene(){
        yield return new WaitUntil(() => GameManager.Instance.isEnemyLoadDone);

        sceneDirector = FindObjectOfType<SceneDirector>();

        if (sceneDirector != null)
        {
            sceneDirector.UiActivate = Canvas0.UiActivate;
            sceneDirector.SetPlayerCamera(brain, playerCamera);
            sceneDirector.Binding();
            sceneDirector.StartTimeLine();
        }
    }

    
    private void Update() {
        if(GameManager.Instance.isCutScene && PlayerInputControls.Instance.playerInputAction.UI.AnyKey.WasPerformedThisFrame()){
            Debug.Log("SkipCutScene pressed");

            if(canSkip && PlayerInputControls.Instance.playerInputAction.UI.SkipCutScene.WasPerformedThisFrame()){
                SkipCutScene();
            }

            ShowSkipMsg();
        }
    }

    void SkipCutScene(){
        //Debug.Log("CutSceneSkiped");
        sceneDirector.SkipTimeLine();
        skipMsgText.gameObject.SetActive(false);
        canSkip = false;
        return;
    }

    void ShowSkipMsg(){
        if (C_SkipMsg != null) StopCoroutine(C_SkipMsg);
        C_SkipMsg = StartCoroutine(SkipMsg());
    }

    IEnumerator SkipMsg()
    {
        canSkip = true;

        if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
        {
            skipMsgText.text = "ESC: 스킵";
        }
        else
        {
            skipMsgText.text = "<sprite=46>: 스킵";
        }

        if (C_TmpAlphaLerp != null) StopCoroutine(C_TmpAlphaLerp);
        C_TmpAlphaLerp = StartCoroutine(TextAlphaLerp(skipMsgText, 1f, 0.5f, false));

        yield return new WaitForSeconds(3f);
        if (C_TmpAlphaLerp != null) StopCoroutine(C_TmpAlphaLerp);
        C_TmpAlphaLerp = StartCoroutine(TextAlphaLerp(skipMsgText, 0f, 0.5f, true));

        canSkip = false;
    }

    IEnumerator TextAlphaLerp(TextMeshProUGUI tmp, float targetAlpha, float duration, bool offAtEnd)
    {
        if(!tmp.gameObject.activeSelf) tmp.gameObject.SetActive(true);

        Color startColor = tmp.color;
        Color targetColor = tmp.color;
        targetColor.a = targetAlpha;

        float lerp = 0;
        float speed = 1 / duration;

        while (lerp < 1)
        {
            tmp.color = Color.Lerp(startColor, targetColor, lerp);
            lerp += Time.deltaTime * speed;
            yield return null;
        }

        if(offAtEnd){
            tmp.gameObject.SetActive(false);
        }
    }


    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
