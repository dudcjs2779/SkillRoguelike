using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<SoundManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<SoundManager>();
                    instance = newObj;
                }
            }
            return instance;
        }
    }
    public AudioMixer mixer;
    public AudioSource bgmAudioSource;
    public GameObject oneShotGameObject;
    public AudioSource oneShotAudioSource;

    [SerializeField] private AnimationCurve gameAudioCurve;

    [SerializeField] AudioSource testAudio;

    [FormerlySerializedAs("gameSFXSounds")]
    public GameSFXAudioClip[] gameSFXSounds;
    [FormerlySerializedAs("uiSFXSounds")]
    public UISFXAudioClip[] uiSFXSounds;
    [FormerlySerializedAs("musicSounds")]
    public MusicAudioClip[] musicSounds;

    private Dictionary<Enum, float> soundTimerDictionary;

    private void Awake()
    {
        var objs = FindObjectsOfType<SoundManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        //LoadAudioSource();
        LoadAudioSource();
        Init_SoundTimerDictionary();

        //testAudio.rolloffMode = AudioRolloffMode.Custom;
        //testAudio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, gameAudioCurve);
    }

    private void Start() {
    }

    private void Update() {

    }

    public void Init_SoundTimerDictionary()
    {
        soundTimerDictionary = new Dictionary<Enum, float>();
        soundTimerDictionary[GameSFXType.FootSteps_Grass_01] = 0f;
        soundTimerDictionary[GameSFXType.Oak_Footsteps_Grass_01] = 0f;
        soundTimerDictionary[GameSFXType.Lich_Footsteps_Grass_01] = 0f;
        soundTimerDictionary[GameSFXType.Wolf_Footsteps_Grass_01] = 0f;
        soundTimerDictionary[GameSFXType.WolfGrow01] = 0f;
        soundTimerDictionary[GameSFXType.Footsteps_Walk_Grass_Big01] = 0f;
        soundTimerDictionary[GameSFXType.Footsteps_Walk_Grass_Big02] = 0f;
    }

    public AudioSource PlayGameSound(GameSFXType sound, Vector3 position, float spatialBlend = 1f)
    {
        if (CanPlayerSound(sound))
        {
            GameSFXAudioClip soundAudioClip = Array.Find(gameSFXSounds, x => x.sound == sound);

            if (soundAudioClip == null)
            {
                Debug.LogError("Sound" + sound + "Not Found!");
                return null;
            }
            else
            {
                GameObject soundGameObject = new GameObject("Sound");
                soundGameObject.transform.position = position;
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("GameSFX")[0];
                audioSource.clip = soundAudioClip.audioClip;
                audioSource.maxDistance = 25f;
                audioSource.spatialBlend = spatialBlend;
                audioSource.dopplerLevel = 0;
                audioSource.spread = 100;
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, gameAudioCurve);

                audioSource.Play();
                Destroy(soundGameObject, audioSource.clip.length);
                return audioSource;
            }
        }
        else{
            return null;
        }
    }

    public AudioSource AttachGameSound(GameSFXType sound, GameObject parentObject, float spatialBlend = 1f, bool isLoop = false)
    {
        if (CanPlayerSound(sound))
        {
            GameSFXAudioClip soundAudioClip = Array.Find(gameSFXSounds, x => x.sound == sound);

            if (soundAudioClip == null)
            {
                Debug.LogError("Sound" + sound + "Not Found!");
                return null;
            }
            else
            {
                GameObject soundGameObject = new GameObject(sound.ToString());
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("GameSFX")[0];
                audioSource.clip = soundAudioClip.audioClip;
                audioSource.maxDistance = 25f;
                audioSource.spatialBlend = spatialBlend;
                audioSource.dopplerLevel = 0;
                audioSource.spread = 100;
                audioSource.loop = isLoop;
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, gameAudioCurve);
                soundGameObject.transform.SetParent(parentObject.transform);
                audioSource.Play();

                if (!isLoop) Destroy(soundGameObject, audioSource.clip.length);
                return audioSource;
            }
            
        }
        else{
            return null;
        }
    }

    public AudioSource PlayDelayGameSound(GameSFXType sound, Vector3 position, float moveVal, float spatialBlend = 1f){
        if (CanPlayerSound(sound, moveVal))
        {
            GameSFXAudioClip soundAudioClip = Array.Find(gameSFXSounds, x => x.sound == sound);

            if (soundAudioClip == null)
            {
                Debug.LogError("Sound" + sound + "Not Found!");
                return null;
            }
            else
            {
                GameObject soundGameObject = new GameObject("Sound");
                soundGameObject.transform.position = position;
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("GameSFX")[0];
                audioSource.clip = soundAudioClip.audioClip;
                audioSource.maxDistance = 25f;
                audioSource.spatialBlend = spatialBlend;
                audioSource.dopplerLevel = 0;
                audioSource.spread = 100;
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, gameAudioCurve);
                audioSource.Play();

                
                Destroy(soundGameObject, audioSource.clip.length);

                return audioSource;
            }
        }
        else
        {
            return null;
        }
    }

    public void PlayUISound(UISFXType sound)
    {
        if (CanPlayerSound(sound))
        {
            UISFXAudioClip soundAudioClip = Array.Find(uiSFXSounds, x => x.sound == sound);

            if (soundAudioClip == null)
            {
                Debug.LogError("Sound" + sound + "Not Found!");
            }
            else
            {
                if (oneShotGameObject == null)
                {
                    oneShotGameObject = new GameObject("one Shot Sound");
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                    oneShotAudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("UISFX")[0];
                }
                oneShotAudioSource.PlayOneShot(soundAudioClip.audioClip);
            }
        }
    }

    public void PlayMusicSound(MusicType sound)
    {
        Debug.Log("PlayMusic");
        MusicAudioClip soundAudioClip = Array.Find(musicSounds, x => x.sound == sound);
        bgmAudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        bgmAudioSource.clip = soundAudioClip.audioClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = 0.5f;
        bgmAudioSource.Play();
    }

    public void SoundPitchLerp(AudioSource audioSource, float target, float dur){
        StartCoroutine(_SoundPitchLerp(audioSource, target, dur));
    }

    IEnumerator _SoundPitchLerp(AudioSource audioSource, float target, float dur){
        float start = audioSource.pitch;
        float lerp = 0;
        float speed = 1 / dur;

        while (lerp < 1)
        {
            audioSource.pitch = Mathf.Lerp(start, target, lerp);
            lerp += Time.unscaledDeltaTime * speed;
            yield return null;
        }
        audioSource.pitch = target;
    }

    private bool CanPlayerSound(Enum sound, float delayVal = 0)
    {
        switch (sound)
        {
            default:
                return true;

            case GameSFXType.FootSteps_Grass_01:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float delay = 0.3f / Mathf.Clamp(PlayerInputControls.Instance.hvInputVec.magnitude, 0.2f, 1f);
                    if (lastTimePlayed + delay < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case GameSFXType.Oak_Footsteps_Grass_01:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float delay = 0.3f / Mathf.Clamp(delayVal, 0.2f, 1f);
                    if (lastTimePlayed + delay < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case GameSFXType.Lich_Footsteps_Grass_01:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float delay = 0.3f / Mathf.Clamp(delayVal, 0.4f, 1f);
                    if (lastTimePlayed + delay < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case GameSFXType.Wolf_Footsteps_Grass_01:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float delay = 0.3f / Mathf.Clamp(delayVal, 0.2f, 1f);
                    if (lastTimePlayed + delay < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case GameSFXType.Footsteps_Walk_Grass_Big01:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float delay = 0.65f / Mathf.Clamp(delayVal, 0.2f, 1f);
                    if (lastTimePlayed + delay < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case GameSFXType.Footsteps_Walk_Grass_Big02:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float delay = 0.2f;
                    if (lastTimePlayed + delay < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
        }
    }

    public void SoundVolumeLerp(AudioSource audioSource, float target, float duration){
        StartCoroutine(_SoundVolumeLerp(audioSource, target, duration));
    }

    IEnumerator _SoundVolumeLerp(AudioSource audioSource, float target, float duration)
    {
        float lerp = 0;
        float start = audioSource.volume;
        //Debug.Log(audioSource.gameObject.name);

        duration = 1 / duration;

        while (lerp < 1)
        {
            audioSource.volume = Mathf.Lerp(start, target, lerp);
            lerp += Time.deltaTime * duration;
            //Debug.Log(audioSource.volume);
            yield return null;
        }
    }
    
    // [ContextMenu("Save Json SoundData")]
    // public void Save_ItemDataToJosn()
    // {
    //     #region 타입 string으로 저장
    //     foreach (var item in gameSFXSounds)
    //     {
    //         item.strType = item.sound.ToString();
    //     }

    //     foreach (var item in uiSFXSounds)
    //     {
    //         item.strType = item.sound.ToString();
    //     }

    //     foreach (var item in musicSounds)
    //     {
    //         item.strType = item.sound.ToString();
    //     }
    //     #endregion

    //     string jdata = JsonHelper.ToJson<GameSFXAudioClip>(gameSFXSounds, true);
    //     string path = Path.Combine(Application.streamingAssetsPath, "JSON/GameSound.json");
    //     File.WriteAllText(path, jdata);

    //     jdata = JsonHelper.ToJson<UISFXAudioClip>(uiSFXSounds, true);
    //     path = Path.Combine(Application.streamingAssetsPath, "JSON/UI_Sound.json");
    //     File.WriteAllText(path, jdata);

    //     jdata = JsonHelper.ToJson<MusicAudioClip>(musicSounds, true);
    //     path = Path.Combine(Application.streamingAssetsPath, "JSON/MusicSound.json");
    //     File.WriteAllText(path, jdata);
    // }

    // [ContextMenu("Load Json SoundData")]
    // public void Load_ItemDataFromJosn()
    // {
    //     string path = Path.Combine(Application.streamingAssetsPath, "JSON/GameSound.json");
    //     string jdata = File.ReadAllText(path);
    //     gameSFXSounds = JsonHelper.FromJson<GameSFXAudioClip>(jdata);

    //     path = Path.Combine(Application.streamingAssetsPath, "JSON/UI_Sound.json");
    //     jdata = File.ReadAllText(path);
    //     uiSFXSounds = JsonHelper.FromJson<UISFXAudioClip>(jdata);

    //     path = Path.Combine(Application.streamingAssetsPath, "JSON/MusicSound.json");
    //     jdata = File.ReadAllText(path);
    //     musicSounds = JsonHelper.FromJson<MusicAudioClip>(jdata);

    //     #region 스트링을 타입으로 변환
    //     foreach (var item in gameSFXSounds)
    //     {
    //         try{
    //             GameSFXType type = (GameSFXType)System.Enum.Parse(typeof(GameSFXType), item.strType);
    //             item.sound = type;
    //         }
    //         catch{
    //             item.sound = GameSFXType.None;
    //         }
    //     }

    //     foreach (var item in uiSFXSounds)
    //     {
    //         try{
    //             UISFXType type = (UISFXType)System.Enum.Parse(typeof(UISFXType), item.strType);
    //             item.sound = type;
    //         }
    //         catch{
    //             item.sound = UISFXType.None;
    //         }
    //     }

    //     foreach (var item in musicSounds)
    //     {
    //         try{
    //             MusicType type = (MusicType)System.Enum.Parse(typeof(MusicType), item.strType);
    //             item.sound = type;
    //         }
    //         catch{
    //             item.sound = MusicType.None;
    //         }
    //     }
    //     #endregion
    // }

    // void Init_Sound(){
    //     List<GameSFXAudioClip> tempGameSfxList = new List<GameSFXAudioClip>();
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerMove, GameAssets.Instance.GetGameSound(GameAudioSourceType.FootSteps_Grass_01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerStep01, GameAssets.Instance.GetGameSound(GameAudioSourceType.OneStep01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerStep02, GameAssets.Instance.GetGameSound(GameAudioSourceType.TwoStep01)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerA1, GameAssets.Instance.GetGameSound(GameAudioSourceType.SwordSwing01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerA2, GameAssets.Instance.GetGameSound(GameAudioSourceType.SwordSwing02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerA3, GameAssets.Instance.GetGameSound(GameAudioSourceType.SwordSwing03)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerHorizontal, GameAssets.Instance.GetGameSound(GameAudioSourceType.BigSlash01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerShieldShove, GameAssets.Instance.GetGameSound(GameAudioSourceType.BigSlash02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerPunch, GameAssets.Instance.GetGameSound(GameAudioSourceType.Punch01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerDashSting, GameAssets.Instance.GetGameSound(GameAudioSourceType.Dash01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerStepSlash, GameAssets.Instance.GetGameSound(GameAudioSourceType.Slash01)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerGroundWave, GameAssets.Instance.GetGameSound(GameAudioSourceType.GroundImpact02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerPowerShieldActivate, GameAssets.Instance.GetGameSound(GameAudioSourceType.GroundImpact01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerPowerShieldExplosion, GameAssets.Instance.GetGameSound(GameAudioSourceType.AirImpact01_1)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireballShot01, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireShot01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireBurst, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireBigShot01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireBurstExplosion01, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireExplosion01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireTornadoCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireCast03)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireTornadoShot, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireBigShot02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireTornado, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireBurning02)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIcicleCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceCast01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIcicleSpawn, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceImpact01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIcicleAmbient, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceAmbient01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIceSpearCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceCast02_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIceSpearSpawn, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceImpact02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIceTornado, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceWind01)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingCast01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingShot, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingShot01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingSplitBurning, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingBurning01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingSplitExplosion, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingExplosion01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingThunderShot, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingBigShot01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingThunderExplosion, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingExplosion02)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerDefenceWallCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.GroundShake01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerDefenceWallSpawn, GameAssets.Instance.GetGameSound(GameAudioSourceType.GroundCrack01)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireSwordCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireCast02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireSwordActivate, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireImpact01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerFireSwordBuff, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireBurning01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIceSwordCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceCast01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIceSwordActivate, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceImpact03)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerIceSwordBuff, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceAmbient02)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingSwordCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingCast01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerLightingSwordActivate, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingImpact01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerHeavySwordCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.MagicCast01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerHeavySwordActivate, GameAssets.Instance.GetGameSound(GameAudioSourceType.MagicActivate01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerGuardingBarrier, GameAssets.Instance.GetGameSound(GameAudioSourceType.MagicCast01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.PlayerGuardingBarrierCast, GameAssets.Instance.GetGameSound(GameAudioSourceType.MagicActivate01_1)));

    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.EnemyHit01, GameAssets.Instance.GetGameSound(GameAudioSourceType.EnemyHit01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.EnemyDie01, GameAssets.Instance.GetGameSound(GameAudioSourceType.EnemyDead01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.EnemyHitFire01, GameAssets.Instance.GetGameSound(GameAudioSourceType.FireHit01_1)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.EnemyHitIce01, GameAssets.Instance.GetGameSound(GameAudioSourceType.IceImpact01)));
    //     tempGameSfxList.Add(new GameSFXAudioClip(GameSFXType.EnemyHitLighting01, GameAssets.Instance.GetGameSound(GameAudioSourceType.LightingHit01_1)));

    //     gameSFXSounds = tempGameSfxList.ToArray();
    // }

    void LoadAudioSource()
    {
        gameSFXSounds = LoadSounds<GameSFXType, GameSFXAudioClip>("Sound/Game");
        uiSFXSounds = LoadSounds<UISFXType, UISFXAudioClip>("Sound/UI");
        musicSounds = LoadSounds<MusicType, MusicAudioClip>("Sound/BGM");
    }

    T[] LoadSounds<TEnum, T>(string folderPath) where TEnum : Enum
    {
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(folderPath);
        List<T> tempList = new List<T>();

        for (int i = 0; i < audioClips.Length; i++)
        {
            try
            {
                TEnum type = (TEnum)Enum.Parse(typeof(TEnum), audioClips[i].name);
                tempList.Add((T)Activator.CreateInstance(typeof(T), type, audioClips[i]));
            }
            catch
            {
                //Debug.LogWarning(audioClips[i].name + " not exist Type");
            }
        }

        return tempList.ToArray();
    }


    public enum GameSFXType
    {
        None,
        FootSteps_Grass_01,
        OneStep01,
        TwoStep01,
        Dodge01,
        Dash01,
        Dash02,

        Punch01_1,
        Swing01,
        Swing02,
        Swing03,
        LongSwing01,
        LongSwing02,
        LongSwing03,
        LongDoubleSwing01,
        BigSlash01,
        BigSlash02,
        BigSlash03,
        Slash01,
        Guard01,

        GroundShake01,
        GroundCrack01,
        GroundImpact01,
        GroundImpact02,
        GroundImpact03,
        GroundImpact04,
        GroundImpact05,
        GroundImpact06,
        GroundImpact07,
        Smash01,

        Charge01,

        FireCast02,
        FireCast03,
        FireShot01,
        FireBigShot01,
        FireBigShot02,
        FireImpact01,
        FireExplosion01,
        FireBurning01,
        FireBurning02,
        FireBurning03,
        FireBurning04,
        FireHit01_1,

        IceCast01_1,
        IceCast02_1,
        IceImpact01,
        IceImpact02,
        IceImpact03,
        IceAmbient01,
        IceAmbient02,
        IceWind01,
        IceHit01,
        Frozen01,

        LightingCast01,
        LightingShot01,
        LightingBigShot01,
        LightingBurning01,
        LightingExplosion01,
        LightingExplosion02,
        LightingImpact01,
        LightingHit01_1,
        ElectricShock01,

        Heal01,
        MagicCast01_1,
        MagicImpact01,
        MagicImpact02,
        MagicImpact03,
        DarkCast01,
        BlackHole01,

        AirImpact01_1,
        AirImpact02,
        AirImpact_Small01,

        PlayerHit01,
        PlayerHit02,
        GuardHit01,
        GuardHit02,

        Oak_Footsteps_Grass_01,
        Lich_Footsteps_Grass_01,
        Wolf_Footsteps_Grass_01,
        Footsteps_Walk_Grass_Big01,
        Footsteps_Walk_Grass_Big02,

        OakSwing01,
        OakSwing02,

        WolfGrow01,
        WolfBite01,

        Roar02,
        BossSwing01,

        EnemyHit01_1,
        EnemyHit02,
        EnemyGuardHit01,
        EnemyDead01_1,
    }

    public enum UISFXType
    {
        None,
        ButtonSelect01,
        ButtonClick01,
        SlotSelect01,
        SlotEquip01,
        SlotUnequip01,
        OpenFirst01,
        OpenSmall01,
        Error01,
        Escape01,
        Money01,

        SparkleImpact01,
    }

    public enum MusicType
    {
        None,
        Title01,
        StartMap01,
        StageType02,
        Boss01,

    }

    [System.Serializable]
    public class GameSFXAudioClip
    {
        public GameSFXAudioClip(GameSFXType _sound, AudioClip _audioClip)
        {
            sound = _sound;
            audioClip = _audioClip;
        }

        public GameSFXType sound;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class UISFXAudioClip
    {
        public UISFXAudioClip(UISFXType _sound, AudioClip _audioClip)
        {
            sound = _sound;
            audioClip = _audioClip;
        }

        public UISFXType sound;
        public string strType;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class MusicAudioClip
    {
        public MusicAudioClip(MusicType _sound, AudioClip _audioClip)
        {
            sound = _sound;
            audioClip = _audioClip;
        }

        public MusicType sound;
        public string strType;
        public AudioClip audioClip;
    }

    public void ButtonSelect01()
    {
        SoundManager.Instance.PlayUISound(SoundManager.UISFXType.ButtonSelect01);
    }

    public void ButtonClick01()
    {
        SoundManager.Instance.PlayUISound(SoundManager.UISFXType.ButtonClick01);
    }
}


