using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;
using UnityEngine.Events;
using UnityEditor;

public class SceneDirector : MonoBehaviour
{
    public PlayableDirector director;
    public UnityAction<bool> UiActivate;
    CinemachineBrain brain;
    CinemachineVirtualCamera playerCamera;

    protected virtual void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.played += Director_Played;
        director.stopped += Director_Stopped;
    }

    protected virtual void Director_Played(PlayableDirector obj)
    {
        Debug.Log("start Director");
        GameManager.Instance.isCutScene = true;
        UiActivate.Invoke(false);
        PlayerInputControls.Instance.playerInputAction.Player.Disable();
        PlayerInputControls.Instance.playerInputAction.PlayerUI.Disable();
        PlayerInputControls.Instance.playerInputAction.UI.Enable();
    }

    protected virtual void Director_Stopped(PlayableDirector obj)
    {
        Debug.Log("end Director");
        UiActivate.Invoke(true);

        PlayerInputControls.Instance.playerInputAction.Player.Enable();
        PlayerInputControls.Instance.playerInputAction.PlayerUI.Enable();
        PlayerInputControls.Instance.playerInputAction.UI.Disable();

        GameManager.Instance.isCutScene = false;
    }

    public void StartTimeLine()
    {
        director.Play();
    }

    public void StopTimeLine()
    {
        director.Stop();
    }

    public void SkipTimeLine()
    {
        director.time = director.duration;
    }

    public virtual void Binding()
    {
        TimelineAsset ta = director.playableAsset as TimelineAsset;
        IEnumerable<TrackAsset> tracks = ta.GetOutputTracks();
        CinemachineTrack cinemachineTrack = null;

        foreach (var trackItem in tracks)
        {
            //Debug.Log(trackItem.name);

            if (trackItem is CinemachineTrack)
            {
                cinemachineTrack = trackItem as CinemachineTrack;
                director.SetGenericBinding(cinemachineTrack, brain);
            }
        }

        foreach (var clip in cinemachineTrack.GetClips())
        {
            if(clip.displayName == "FollowCam"){
                CinemachineShot shot = clip.asset as CinemachineShot;
                //shot.VirtualCamera.exposedName = GUID.Generate().ToString();
                director.SetReferenceValue(shot.VirtualCamera.exposedName, playerCamera);
                //Debug.Log(shot.VirtualCamera.exposedName);
            }
        }
    }

    public void SetPlayerCamera(CinemachineBrain brain, CinemachineVirtualCamera playerCamera){
        this.brain = brain;
        this.playerCamera = playerCamera;
    }
}
