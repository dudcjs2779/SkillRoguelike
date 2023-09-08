using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

public class BossSceneDirector : SceneDirector
{
    GameObject obj_Player;
    public Transform obj_Golem;
    Animator player_Anim;
    public CinemachineVirtualCamera dollyCam_Cvc;
    public CinemachineVirtualCamera sceneCam_Cvc;

    bool canSkip;

    override protected void Awake()
    {
        base.Awake();
        obj_Player = GameObject.Find("Player");
        player_Anim = obj_Player.GetComponent<Animator>();
        player_Anim.applyRootMotion = false;
        dollyCam_Cvc.LookAt = obj_Golem.transform;
        sceneCam_Cvc.Follow = obj_Player.transform;
        sceneCam_Cvc.LookAt = obj_Player.transform;
    }

    override protected void Director_Played(PlayableDirector obj)
    {
        Debug.Log("start Director");
        GameManager.Instance.isCutScene = true;
        UiActivate.Invoke(false);

        PlayerInputControls.Instance.playerInputAction.Player.Disable();
        PlayerInputControls.Instance.playerInputAction.PlayerUI.Disable();
        PlayerInputControls.Instance.playerInputAction.UI.Enable();

        if (obj_Player != null)
        {
            PlayerAnim playerAnim = obj_Player.GetComponent<PlayerAnim>();
            if (playerAnim != null) Destroy(playerAnim);
        }
    }

    override protected void Director_Stopped(PlayableDirector obj)
    {
        Debug.Log("end Director");
        if (obj_Player != null)
        {
            UiActivate.Invoke(true);

            PlayerInputControls.Instance.playerInputAction.Player.Enable();
            PlayerInputControls.Instance.playerInputAction.PlayerUI.Enable();
            PlayerInputControls.Instance.playerInputAction.UI.Disable();

            obj_Player.AddComponent<PlayerAnim>();
            PlayerAnim playerAnim = obj_Player.GetComponent<PlayerAnim>();
            playerAnim.animMoveSpeed = 1.2f;

        }

        GameManager.Instance.isCutScene = false;
    }

    override public void Binding()
    {
        TimelineAsset ta = director.playableAsset as TimelineAsset;
        IEnumerable<TrackAsset> temp = ta.GetOutputTracks();

        foreach (var item in temp)
        {
            Debug.Log(item.name);
        }

        var track = ta.GetOutputTrack(1);
        director.SetGenericBinding(track, obj_Player);

        track = ta.GetOutputTrack(2);
        director.SetGenericBinding(track, player_Anim);

        track = ta.GetOutputTrack(3);
        director.SetGenericBinding(track, player_Anim);

        track = ta.GetOutputTrack(4);
        director.SetGenericBinding(track, GameObject.Find("Game Camera").GetComponent<CinemachineBrain>());

        track = ta.GetOutputTrack(5);
        director.SetGenericBinding(track, GameObject.Find("DollyCam").GetComponent<Animator>());
    }
}
