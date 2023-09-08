using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class OptionSetting
{
    // Display
    public Resolution resolution;
    public int frameRate;
    public int vSyncCount;
    public bool isFullScreen;
    public int qualityIndex;

    //Audio
    public float masterVolume;
    public float musicVolume;
    public float gameVolume;
    public float UIVolume;

    //Key
    public float mouseSensitivity;
    public float gamepadSensitivity;

}

// ===== 스텟 =====
public class GameManager : MonoBehaviour, IDataPersistence
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            // if (instance == null)
            // {
            //     var obj = FindObjectOfType<GameManager>();
            //     if (obj != null)
            //     {
            //         instance = obj;
            //     }
            //     else
            //     {
            //         var newObj = new GameObject().AddComponent<GameManager>();
            //         instance = newObj;
            //     }
            // }
            return instance;
        }
    }

    public bool isDebugMode;

    public enum EnemyType { Oak, Lich, Wolf };
    public enum EnemyActType { Stand, Walk, Acting, Fix };
    public enum SkillType { Active, Passive }
    public enum ShopItemType { Active, Passive, Item }
    public enum DifficultyType { Easy, Normal, Hard }
    public DifficultyType difficultyType;

    [Header("LIST")]
    public List<ActiveSkill> ActiveSkillList;
    public List<ActiveSkill> MyActiveSkillList, EquipActiveList;
    public List<PassiveSkill> PassiveSkillList;
    public List<PassiveSkill> MyPassiveSkillList, EquipPassiveList;
    public List<ShopUnlockData> ShopUnlockList;
    public List<ShopUnlockData> MyShopUnlockList;
    public List<ShopItem> ShopItemList;
    public List<ShopItem> MyShopItemList;
    public List<Buff> Buffs_Playing;

    Coroutine C_StageCheck;
    [Header("Tutorial")]

    [Header("SCRIPT")]
    public PlayerData playerData;
    public StatValData statValData = new StatValData();
    public OptionSetting optionSetting;
    Player player;

    [Header("CURSOR")]
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D selectCursor;
    [SerializeField] Texture2D skillCursor;

    [Header("BOOL")]
    public bool isInDungeon;
    public bool isTitle;
    public bool isNewGame;
    public bool isGamePause;
    public bool donContainPos;
    public bool isEnemyLoadDone;
    public bool isGameOver;
    public bool isStageClear;
    public bool goBossStage;
    public bool isCutScene;
    public bool isShowFPS;

    [Header("GAME PLAY TIME")]
    public DateTime lastSaveTime;
    public TimeSpan playTime;

    [Header("DUNGEON CONTROL")]
    public int playerLv;
    public int skillPoint;
    public List<int> expList;
    public List<string> Stage1_MapList;
    public List<string> curPlayMapList;
    public int stageLevel;
    public List<int> stageClearExpList_Easy = new List<int>() { 100, 100, 130, 130, 160, 160, 180, 200, 200, 200, 200, 200 };
    public List<int> stage_ClearExpList_Noraml = new List<int>() { 160, 160, 200, 200, 240, 240, 265, 300, 300, 300, 300, 300 };
    public List<int> stageClearExpList_Hard = new List<int>() { 200, 200, 250, 250, 300, 300, 325, 350, 350, 350, 350, 350 };
    public int enemyNum, restEnemy, spawnedEnemy;

    [Header("IN DUNGEON INFO")]
    public float dungeonPlayTime;
    public int stageCount;
    public int gainGold;
    public int oakKillCount, lichKillCount, wolfKillCount;

    public Animator sceneAnim;

    public List<EnemySpawnGroup> enemySpawnGroupList;
    public List<EnemySpawnZone> enemySpawnZoneList;

    public Action Init_playerState;
    public Action Action_Init_GameManager;

    [Header("Tutorial")]
    [SerializeField] GameObject tutorialManager;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("Destroyed GameManager instance: " + this.GetInstanceID());
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        Action_Init_GameManager += Init_GameManager;
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            isTitle = true;
        }
        else
        {
            Init_GameManager();
        }

        LoadOptionSetting();
    }

    void Start()
    {
        DefaultCursor();
        Cursor.lockState = CursorLockMode.Confined;

        if (isTitle) return;



        if (playerData.doTutorial)
        {
            Debug.Log("playerData.doTutorial");
            Instantiate(Resources.Load<TutorialManager>("Prefab/TutorialManager"));
        }
    }

    public void OnSceneLoaded_TitleToGame(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("OnSceneLoaded_TitleToGame");

        isTitle = false;
        Action_Init_GameManager.Invoke();
        SceneManager.sceneLoaded -= GameManager.Instance.OnSceneLoaded_TitleToGame;
    }

    public void Init_GameManager()
    {
        Debug.Log("Init_GameManager");
        player = GameObject.Find("Player")?.GetComponent<Player>();

        lastSaveTime = DateTime.Now;

        Stage1_MapList = new List<string>() { "Forest_Lake", "Forest_Flat1", "Forest_Flat2", "Forest_River", "Forest_WaterHole", "Forest_Bridge", "Forest_OakHouse", "Forest_StoneGrave", "Forest_Boss" };
        expList.Add(0);

        int exp = 140;
        for (int i = 0; i < 29; i++)
        {
            expList.Add(exp);
            exp += 20;
        }

        if (playerData.doTutorial)
        {
            Instantiate(tutorialManager);
        }

    }

    public void LoadOptionSetting()
    {
        Load_OptionSettingFromJson();
        StartCoroutine(LoadKeySetting());
    }

    void Update()
    {
        if (isTitle) return;

        if (Input.GetButtonDown("SaveTxt"))     //F1
        {
            //MyActiveSkillList.Find(x => x.name == "Lighting Split").skillLv = 2;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            //StartCoroutine(SceneIn());
            //ScreenCapture.CaptureScreenshot("testImage.png");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            // Debug.Log(Cursor.lockState);
            // Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            // Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }

        // if(PlayerInputControls.Instance.playerInputAction.Player.Fire1.WasReleasedThisFrame()){
        //     Debug.Log("파이어1");
        // }

        if (isShowFPS)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
        }

        //플레이 타임
        playTime = DateTime.Now - lastSaveTime;

        //if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    print(playerData.STR);
        //    print(playerData.AGL);
        //    print(playerData.VIT);
        //    print(playerData.INT);
        //    print(playerData.MP);
        //    print(playerData.MST);
        //}

    }

    public IEnumerator RefreshUI(ContentSizeFitter csf)
    {
        csf.enabled = false;
        yield return null;
        csf.enabled = true;
    }

    public void RefreshUI(LayoutGroup layoutGroup)
    {
        StartCoroutine(_RefreshUI(layoutGroup));
    }

    public IEnumerator _RefreshUI(LayoutGroup layoutGroup)
    {
        layoutGroup.enabled = false;
        yield return null;
        layoutGroup.enabled = true;
    }

    #region 세이브 로드 함수
    [ContextMenu("Save Json SkillData")]
    public void Save_SkillDataToJosn()
    {
        string jdata = JsonConvert.SerializeObject(MyActiveSkillList);
        string jdataFormatted = JValue.Parse(jdata).ToString(Formatting.Indented);
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ActiveSkillData.json");
        File.WriteAllText(path, jdataFormatted);

        jdata = JsonConvert.SerializeObject(MyPassiveSkillList);
        jdataFormatted = JValue.Parse(jdata).ToString(Formatting.Indented);
        path = Path.Combine(Application.streamingAssetsPath, "JSON/PassiveSkillData.json");
        File.WriteAllText(path, jdataFormatted);
    }

    [ContextMenu("Load Json SkillData")]
    public void Load_SkillDataFromJosn()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ActiveSkillData.json");
        string jdata = File.ReadAllText(path);
        MyActiveSkillList = JsonConvert.DeserializeObject<List<ActiveSkill>>(jdata);
        EquipActiveList = MyActiveSkillList.FindAll(x => x.isEquip);

        path = Path.Combine(Application.streamingAssetsPath, "JSON/PassiveSkillData.json");
        jdata = File.ReadAllText(path);
        MyPassiveSkillList = JsonConvert.DeserializeObject<List<PassiveSkill>>(jdata);
        EquipPassiveList = MyPassiveSkillList.FindAll(x => x.isEquip);
    }

    public void Load_DbSkillData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ActiveSkillData.json");
        string jdata = File.ReadAllText(path);
        ActiveSkillList = JsonConvert.DeserializeObject<List<ActiveSkill>>(jdata);

        path = Path.Combine(Application.streamingAssetsPath, "JSON/PassiveSkillData.json");
        jdata = File.ReadAllText(path);
        PassiveSkillList = JsonConvert.DeserializeObject<List<PassiveSkill>>(jdata);
    }

    [ContextMenu("Save Json ItemData")]
    public void Save_ItemDataToJosn()
    {
        string jdata = JsonConvert.SerializeObject(MyShopItemList);
        string jdataFormatted = JValue.Parse(jdata).ToString(Formatting.Indented);
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ItemData.json");
        File.WriteAllText(path, jdataFormatted);
    }

    [ContextMenu("Load Json ItemData")]
    public void Load_ItemDataFromJosn()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ItemData.json");
        string jdata = File.ReadAllText(path);
        MyShopItemList = JsonConvert.DeserializeObject<List<ShopItem>>(jdata);
    }

    public void Load_DbItemData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ItemData.json");
        string jdata = File.ReadAllText(path);
        ShopItemList = JsonConvert.DeserializeObject<List<ShopItem>>(jdata);
    }

    [ContextMenu("Save Json SkillUnlcokData")]
    public void Save_ShopUnlockDataToJosn()
    {
        string jdata = JsonConvert.SerializeObject(MyShopUnlockList);
        string jdataFormatted = JValue.Parse(jdata).ToString(Formatting.Indented);
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ShopUnlcokData.json");
        File.WriteAllText(path, jdataFormatted);
    }

    [ContextMenu("Load Json SkillUnlcokData")]
    public void Load_ShopUnlockDataFromJosn()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ShopUnlcokData.json");
        string jdata = File.ReadAllText(path);
        MyShopUnlockList = JsonConvert.DeserializeObject<List<ShopUnlockData>>(jdata);
    }

    public void Load_DbShopUnlockData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/ShopUnlcokData.json");
        string jdata = File.ReadAllText(path);
        ShopUnlockList = JsonConvert.DeserializeObject<List<ShopUnlockData>>(jdata);
    }

    [ContextMenu("Save Json PlayerData")]
    public void Save_PlayerDataToJosn()
    {
        string jdata = JsonConvert.SerializeObject(playerData);
        string jdataFormatted = JValue.Parse(jdata).ToString(Formatting.Indented);
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/PlayerData.json");
        File.WriteAllText(path, jdataFormatted);
    }

    [ContextMenu("Load Json PlayerData")]
    public void Load_PlayerDataFromJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JSON/PlayerData.json");
        string jdata = File.ReadAllText(path);
        playerData = JsonConvert.DeserializeObject<PlayerData>(jdata);
    }

    [ContextMenu("Save Json OptionSetting")]
    public void Save_OptionSettingToJosn()
    {
        string jdata = JsonConvert.SerializeObject(optionSetting);
        string jdataFormatted = JValue.Parse(jdata).ToString(Formatting.Indented);
        string path = Path.Combine(Application.persistentDataPath, "Config/OptionSetting.json");

        string dicPath = Path.Combine(Application.persistentDataPath, "Config");
        if (!Directory.Exists(dicPath)) Directory.CreateDirectory(dicPath);

        File.WriteAllText(path, jdataFormatted);
    }

    [ContextMenu("Load Json OptionSetting")]
    public void Load_OptionSettingFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "Config/OptionSetting.json");
        if (File.Exists(path))
        {
            string jdata = File.ReadAllText(path);
            optionSetting = JsonConvert.DeserializeObject<OptionSetting>(jdata);
            //Debug.Log("Load From File : " + optionSetting.resolution);
        }
        else
        {
            optionSetting = new OptionSetting();
            optionSetting.resolution = Screen.currentResolution;
            optionSetting.frameRate = 240;
            optionSetting.vSyncCount = 0;
            optionSetting.isFullScreen = false;
            optionSetting.qualityIndex = 3;
            optionSetting.masterVolume = 1f;
            optionSetting.musicVolume = 1f;
            optionSetting.gameVolume = 1f;
            optionSetting.UIVolume = 1f;
            optionSetting.mouseSensitivity = 100f;
            optionSetting.gamepadSensitivity = 1000f;
            Save_OptionSettingToJosn();
        }

        //디스플레이
        //Screen.SetResolution(optionSetting.resolution.width, optionSetting.resolution.height, FullScreenMode.Windowed);
        Application.targetFrameRate = optionSetting.frameRate;
        QualitySettings.SetQualityLevel(optionSetting.qualityIndex);
        QualitySettings.vSyncCount = optionSetting.vSyncCount;
        Screen.fullScreen = optionSetting.isFullScreen;

        //사운드
        StartCoroutine(LoadSetting());
    }

    IEnumerator LoadSetting()
    {
        yield return null;

        //사운드
        SoundManager.Instance.mixer.SetFloat("Master", Mathf.Log10(optionSetting.masterVolume) * 20);
        SoundManager.Instance.mixer.SetFloat("Music", Mathf.Log10(optionSetting.musicVolume) * 20);
        SoundManager.Instance.mixer.SetFloat("GameSFX", Mathf.Log10(optionSetting.gameVolume) * 20);
        SoundManager.Instance.mixer.SetFloat("UISFX", Mathf.Log10(optionSetting.UIVolume) * 20);

        // 감도
        PlayerInputControls.Instance.mouseSpeed = optionSetting.mouseSensitivity;
        PlayerInputControls.Instance.rightStick_Speed = optionSetting.gamepadSensitivity;
    }

    [ContextMenu("Save Json KeySetting")]
    public void Save_KeysToJson()
    {
        var rebinds = PlayerInputControls.Instance.playerInputAction.SaveBindingOverridesAsJson();
        string path = Path.Combine(Application.persistentDataPath, "Config/RebindKeys.json");

        string dicPath = Path.Combine(Application.persistentDataPath, "Config");
        if (!Directory.Exists(dicPath)) Directory.CreateDirectory(dicPath);

        File.WriteAllText(path, rebinds);
    }

    [ContextMenu("Load Json KeySetting")]
    public void Load_KeysFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "Config/RebindKeys.json");
        if (File.Exists(path))
        {
            string jdata = File.ReadAllText(path);
            PlayerInputControls.Instance.playerInputAction.LoadBindingOverridesFromJson(jdata);
        }
        else
        {
            Save_KeysToJson();

            string jdata = File.ReadAllText(path);
            PlayerInputControls.Instance.playerInputAction.LoadBindingOverridesFromJson(jdata);
            Debug.Log("Defalut Key Setting Loaded");
        }
    }

    IEnumerator LoadKeySetting()
    {
        yield return null;
        Load_KeysFromJson();
    }

    [ContextMenu("Temp Methode")]
    public void TempMethode()
    {
        Debug.Log(UnityEngine.Random.Range(3, 4));
    }

    void IDataPersistence.LoadData(GameData data)
    {
        Debug.Log(this.gameObject.name + " Data Load: " + this.GetInstanceID());

        Load_DbSkillData();
        Load_DbItemData();
        Load_DbShopUnlockData();

        UpdateMyData(data);
        this.EquipActiveList = this.MyActiveSkillList.FindAll(x => x.isEquip);
        this.EquipPassiveList = this.MyPassiveSkillList.FindAll(x => x.isEquip);
        this.playerData = data.playerData;
        this.statValData.StatUpdate(this.playerData);
    }

    void IDataPersistence.SaveData(GameData data)
    {
        //Debug.Log(this.gameObject.name + ": " + this.GetInstanceID());
        playerData.playTime += playTime.TotalSeconds;
        lastSaveTime = DateTime.Now;

        data.ActiveSkillList = this.MyActiveSkillList;
        data.PassiveSkillList = this.MyPassiveSkillList;
        data.ShopItemList = this.MyShopItemList;
        data.ShopUnlockList = this.MyShopUnlockList;
        data.playerData = this.playerData;
    }

    public void UpdateMyData(GameData data)
    {

        List<ActiveSkill> tempActiveSkillList = new List<ActiveSkill>();
        for (int i = 0; i < ActiveSkillList.Count; i++)
        {
            ActiveSkill copiedSkill = new ActiveSkill(ActiveSkillList[i]); // 깊은 복사

            copiedSkill.skillLv = data.ActiveSkillList[i].skillLv;
            copiedSkill.equipOrder = data.ActiveSkillList[i].equipOrder;
            copiedSkill.isBuy = data.ActiveSkillList[i].isBuy;
            copiedSkill.isEquip = data.ActiveSkillList[i].isEquip;
            tempActiveSkillList.Add(copiedSkill);
        }
        MyActiveSkillList = tempActiveSkillList;

        List<PassiveSkill> tempPassiveSkillList = new List<PassiveSkill>();
        for (int i = 0; i < PassiveSkillList.Count; i++)
        {

            PassiveSkill copiedSkill = new PassiveSkill(PassiveSkillList[i]); // 깊은 복사

            copiedSkill.skillLv = data.PassiveSkillList[i].skillLv;
            copiedSkill.equipOrder = data.PassiveSkillList[i].equipOrder;
            copiedSkill.isBuy = data.PassiveSkillList[i].isBuy;
            copiedSkill.isEquip = data.PassiveSkillList[i].isEquip;
            tempPassiveSkillList.Add(copiedSkill);

        }
        MyPassiveSkillList = tempPassiveSkillList;

        List<ShopItem> tempShopItemList = new List<ShopItem>();
        for (int i = 0; i < ShopItemList.Count; i++)
        {
            ShopItem copiedItem = new ShopItem(ShopItemList[i]);

            copiedItem.isUnlock = data.ShopItemList[i].isUnlock;
            copiedItem.isBuy = data.ShopItemList[i].isBuy;
            copiedItem.isEquip = data.ShopItemList[i].isEquip;
            tempShopItemList.Add(copiedItem);
        }
        MyShopItemList = tempShopItemList;

        List<ShopUnlockData> tempShopUnlockList = new List<ShopUnlockData>();
        for (int i = 0; i < ShopUnlockList.Count; i++)
        {
            ShopUnlockData copiedUnlockData = new ShopUnlockData(ShopUnlockList[i]);

            copiedUnlockData.isUnlock = data.ShopUnlockList[i].isUnlock;
            tempShopUnlockList.Add(copiedUnlockData);
        }
        MyShopUnlockList = tempShopUnlockList;
    }

    #endregion

    //=============== 스테이지 관리 ==================
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (!sceneAnim.gameObject.activeSelf)
        {
            sceneAnim.gameObject.SetActive(true);
        }
        sceneAnim.Play("Crossfade_End");

        switch (scene.name)
        {
            case "TitleScene":
                SoundManager.Instance.PlayMusicSound(SoundManager.MusicType.Title01);
                break;

            case "StartMap":
                PlayerInputControls.Instance.ChangeMapPlayer();
                SoundManager.Instance.PlayMusicSound(SoundManager.MusicType.StartMap01);
                break;

            case "TutorialScene":
                SoundManager.Instance.PlayMusicSound(SoundManager.MusicType.StartMap01);
                break;

            case "Forest_Boss":
                SoundManager.Instance.PlayMusicSound(SoundManager.MusicType.Boss01);
                break;

            default:
                SoundManager.Instance.PlayMusicSound(SoundManager.MusicType.StageType02);
                break;
        }
    }

    public void StartDungeon(DifficultyType type)
    {
        Init_playerState.Invoke();
        DataPersistenceManager.Instance.SaveGame();
        float openingWindowsCount = Canvas5.Instance.UISequenceList.Count;
        for (int i = 0; i < openingWindowsCount; i++)
        {
            //Debug.Log("for" + i);
            Canvas5.Instance.Escape();
        }

        difficultyType = type;
        switch (playerData.clearLv)
        {
            case 0:
                skillPoint = 1;
                //player.curEXP = 0;
                break;

            case 1:
                skillPoint = 3;
                //player.curEXP = 140;
                break;

            case 2:
            case 3:
                skillPoint = 5;
                //player.curEXP = 480;
                break;
        }
        Canvas0.Instance.playerState.playerLvText.text = string.Format("Lv {0}", playerLv);
        stageLevel = 0;
        stageCount = 0;
        curPlayMapList = MixStage();

        isEnemyLoadDone = false;
        LoadNextLevel();
        StartCoroutine(DungeonTimer());
    }

    IEnumerator DungeonTimer()
    {
        while (true)
        {
            dungeonPlayTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public void LoadNextLevel()
    {
        //Debug.Log("LoadNextLevel");
        isEnemyLoadDone = false;

        if (goBossStage)
        {
            goBossStage = false;
            StartCoroutine(LoadLevel("Forest_Boss"));
        }
        else
        {
            StartCoroutine(LoadLevel(curPlayMapList[stageLevel]));
            //StartCoroutine(LoadLevel("Forest_Boss"));
            //StartCoroutine(LoadLevel("Forest_Lake"));
            //StartCoroutine(LoadLevel("Forest_OakHouse"));

        }
    }

    public void LoadBossLevel()
    {
        isEnemyLoadDone = false;
        StartCoroutine(LoadLevel(curPlayMapList[curPlayMapList.Count - 1]));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        //Debug.Log("C_LoadNextLevel");

        if (!sceneAnim.gameObject.activeSelf) sceneAnim.gameObject.SetActive(true);
        sceneAnim.SetTrigger("Start");

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);

        stageLevel++;
        stageCount++;
        isInDungeon = true;
        isStageClear = false;

        yield return null;

        restEnemy = 0;
        if (SceneManager.GetActiveScene().name != "Forest_Boss") EnemySpawn();
        PlayerStartPosition();

        yield return new WaitUntil(() => isEnemyLoadDone);
        StageCheck();
    }

    public void SceneChange(string sceneName, Action afterFadeOutAction, Action afterSceneLoadAction)
    {
        StartCoroutine(_SceneChange(sceneName, afterFadeOutAction, afterSceneLoadAction));
    }

    IEnumerator _SceneChange(string sceneName, Action afterFadeOutAction, Action afterSceneLoadAction)
    {
        if (!sceneAnim.gameObject.activeSelf) sceneAnim.gameObject.SetActive(true);
        sceneAnim.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(1);
        //AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        afterFadeOutAction?.Invoke();
        yield return SceneManager.LoadSceneAsync(sceneName);
        afterSceneLoadAction?.Invoke();
    }

    List<string> MixStage()
    {
        List<string> tempList = new List<string>();
        List<string> list1 = new List<string>();
        List<string> list2 = new List<string>();

        tempList = Stage1_MapList.ToList();
        tempList.RemoveAt(tempList.Count - 1);
        int count = tempList.Count;

        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, tempList.Count);
            list1.Add(tempList[rand]);
            tempList.RemoveAt(rand);
        }

        tempList = Stage1_MapList.ToList();
        tempList.RemoveAt(tempList.Count - 1);

        for (int i = 0; i < 4; i++)
        {
            int rand = UnityEngine.Random.Range(0, tempList.Count);
            list2.Add(tempList[rand]);
            tempList.RemoveAt(rand);
        }

        tempList.Clear();
        tempList.AddRange(list1);
        tempList.AddRange(list2);
        tempList.Add(Stage1_MapList[Stage1_MapList.Count - 1]);

        return tempList;

        //for (int i = 0; i < Stage1_MapList.Count; i++)
        //{
        //    print(Stage1_MapList[i]);
        //}
    }

    public void StageCheck()
    {
        if (C_StageCheck == null && stageLevel != 0 && isEnemyLoadDone)
        {
            C_StageCheck = StartCoroutine(_StageCheck());
        }
    }

    IEnumerator _StageCheck()
    {
        Debug.Log("_StageCheck");
        // 리워드 보상
        if (restEnemy <= 0)
        {
            Debug.Log("Stage Clear");
            isStageClear = true;
            StartCoroutine(TimeScaleLerp(0.2f, 0.5f));
            SoundManager.Instance.SoundPitchLerp(SoundManager.Instance.bgmAudioSource, 0.2f, 0.5f);
            yield return new WaitForSecondsRealtime(3f);
            StartCoroutine(TimeScaleLerp(1, 1));
            SoundManager.Instance.SoundPitchLerp(SoundManager.Instance.bgmAudioSource, 1, 1f);
            yield return new WaitForSecondsRealtime(3f);

            Canvas5.Instance.OpenReward();

            yield return new WaitUntil(() => !Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.Reward));

            switch (difficultyType)
            {
                case DifficultyType.Easy:
                    player.curEXP += 120;
                    break;

                case DifficultyType.Normal:
                    player.curEXP += 180;
                    break;

                case DifficultyType.Hard:
                    player.curEXP += 240;
                    break;
            }
        }

        // 레벨업
        while (player.maxEXP <= player.curEXP)
        {
            bool isMaxLv = EquipActiveList.Find(x => x.skillLv < 2) == null && EquipPassiveList.Find(x => x.skillLv < 3) == null;
            if (isMaxLv) break;

            playerLv++;
            Canvas0.Instance.playerState.playerLvText.text = string.Format("Lv {0}", playerLv);

            skillPoint++;
            player.curEXP = player.curEXP - player.maxEXP;
            player.maxEXP = expList[playerLv];
            player.maxHealth += 1;
            player.maxMP += 1;
            PlayerStateManager.Instance.defanceVal -= 0.01f;
        }

        while (skillPoint > 0)
        {
            Canvas5.Instance.OpenSkillUpgrade();

            if (!isStageClear)
            {
                PauseGame();
            }
            yield return new WaitUntil(() => !Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.SkillUpgrade));
            skillPoint--;
            ResumeGame();
            Canvas0.Instance.hUD_Skill.Init_HUDSkill();
        }

        // 다음 스테이지
        if (isStageClear)
        {
            LoadNextLevel();
        }

        if (!isStageClear && restEnemy <= 0)
        {
            StartCoroutine(_StageCheck());
        }

        C_StageCheck = null;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePause = false;
    }

    public void DungeonClear()
    {
        StartCoroutine(_DungeonClear());
    }

    IEnumerator _DungeonClear()
    {
        StartCoroutine(TimeScaleLerp(0.2f, 0.5f));
        SoundManager.Instance.SoundPitchLerp(SoundManager.Instance.bgmAudioSource, 0.2f, 0.5f);
        yield return new WaitForSecondsRealtime(3f);
        StartCoroutine(TimeScaleLerp(1, 1));
        SoundManager.Instance.SoundPitchLerp(SoundManager.Instance.bgmAudioSource, 1f, 1f);
        yield return new WaitForSecondsRealtime(3f);

        Canvas5.Instance.OpenClearPanel();
        yield return new WaitUntil(() => !Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.ClearPanel));

        yield return new WaitForSeconds(1);
        if (!sceneAnim.gameObject.activeSelf) sceneAnim.gameObject.SetActive(true);
        sceneAnim.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        isInDungeon = false;
        stageLevel = 0;
        stageCount = 0;
        dungeonPlayTime = 0;
        gainGold = 0;
        oakKillCount = 0;
        lichKillCount = 0;
        wolfKillCount = 0;

        playerLv = 0;
        Canvas0.Instance.playerState.playerLvText.text = string.Format("Lv {0}", playerLv);
        skillPoint = 0;
        player.curEXP = 0;
        player.maxEXP = 100;

        switch (difficultyType)
        {
            case DifficultyType.Easy:
                if (playerData.clearLv < 1)
                {
                    playerData.clearLv = 1;
                    DialogueManager.Instance.SetVariableState("shopOwnerEvent", "clearEasy");
                }
                break;

            case DifficultyType.Normal:
                if (playerData.clearLv < 2)
                {
                    playerData.clearLv = 2;
                    DialogueManager.Instance.SetVariableState("shopOwnerEvent", "clearNormal");
                }
                break;

            case DifficultyType.Hard:
                if (playerData.clearLv < 3)
                {
                    playerData.clearLv = 3;
                    DialogueManager.Instance.SetVariableState("shopOwnerEvent", "clearHard");
                }
                break;

            default:
                break;
        }

        for (int i = Buffs_Playing.Count - 1; i >= 0; i--)
        {
            Buffs_Playing[i].DeActivation();
        }

        Init_playerState.Invoke();
        DataPersistenceManager.Instance.SaveGame();

        SceneManager.LoadScene("StartMap");
        yield return null;

        PlayerStartPosition();
    }

    public void GameOver()
    {
        isGameOver = true;
        StartCoroutine(_GameOver());
        Debug.Log("GameOver");
    }

    IEnumerator _GameOver()
    {
        yield return new WaitForSeconds(2f);

        while (Canvas5.Instance.UISequenceList.Count > 0)
        {
            Canvas5.Instance.Escape();
        }

        Canvas5.Instance.OpenClearPanel();
        yield return new WaitUntil(() => !Canvas5.Instance.UISequenceList.Contains(Canvas5.UIType.ClearPanel));

        yield return new WaitForSeconds(1);
        if (!sceneAnim.gameObject.activeSelf) sceneAnim.gameObject.SetActive(true);
        sceneAnim.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        isInDungeon = false;
        stageLevel = 0;
        stageCount = 0;
        dungeonPlayTime = 0;
        gainGold = 0;
        oakKillCount = 0;
        lichKillCount = 0;
        wolfKillCount = 0;

        playerLv = 0;
        Canvas0.Instance.playerState.playerLvText.text = string.Format("Lv {0}", playerLv);
        skillPoint = 0;
        player.curEXP = 0;
        player.maxEXP = 100;

        for (int i = Buffs_Playing.Count - 1; i >= 0; i--)
        {
            Buffs_Playing[i].DeActivation();
        }

        Init_playerState.Invoke();
        DataPersistenceManager.Instance.SaveGame();

        SceneManager.LoadScene("StartMap");
        yield return null;

        player.anim.Play("Movement", 0);

        PlayerStartPosition();
        isGameOver = false;
        player.isDead = false;
        player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void PlayerStartPosition()
    {
        Transform startTransform = GameObject.Find("StartPos").transform;
        Debug.Log(startTransform.position);
        player.transform.position = startTransform.position;
        player.transform.rotation = startTransform.rotation;
    }

    IEnumerator TimeScaleLerp(float target, float dur)
    {
        float start = Time.timeScale;
        float lerp = 0;
        float speed = 1 / dur;

        while (lerp < 1)
        {
            Time.timeScale = Mathf.Lerp(start, target, lerp);
            lerp += Time.unscaledDeltaTime * speed;
            yield return null;
        }
        Time.timeScale = target;
    }

    void EnemySpawn()
    {
        //Debug.Log("EnemySpawn");

        enemySpawnGroupList = new List<EnemySpawnGroup>();
        EnemySpawnGroup[] enemySpawnGroups = FindObjectsOfType<EnemySpawnGroup>();
        List<EnemySpawnGroup> enemySpawnGroupList_Backup;

        if (enemySpawnGroups != null)
        {
            foreach (var item in enemySpawnGroups)
            {
                enemySpawnGroupList.Add(item);
            }
        }

        enemySpawnGroupList_Backup = enemySpawnGroupList.ToList();

        switch (stageLevel)
        {
            case 1:
                enemyNum = UnityEngine.Random.Range(3, 5);
                //enemyNum = 13;
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                //UnityEngine.RandomSpawn(enemyNum, enemySpawnGroupList_Backup);
                break;

            case 2:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(3, 5);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                //UnityEngine.RandomSpawn(enemyNum, enemySpawnGroupList_Backup);
                break;

            case 3:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(4, 7);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                //UnityEngine.RandomSpawn(enemyNum, enemySpawnGroupList_Backup);
                break;

            case 4:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(4, 7);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 5:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(6, 9);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 6:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(6, 9);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 7:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(8, 11);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 8:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(9, 12);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 9:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(10, 13);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 10:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(10, 14);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 11:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(11, 14);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;

            case 12:
                // enemyNum = 13;
                enemyNum = UnityEngine.Random.Range(11, 14);
                StartCoroutine(RandomSpawn(enemyNum, enemySpawnGroupList_Backup));
                break;
        }
    }

    IEnumerator RandomSpawn(int enemyNum, List<EnemySpawnGroup> BackupList)
    {
        //Debug.Log("RandomSpawn");
        // 오크하우스 : 적들이 조금씩 스폰돼서 나오는 시스템의 맵
        if (SceneManager.GetActiveScene().name == "Forest_OakHouse")
        {
            yield return new WaitUntil(() => !GameManager.Instance.isCutScene);
            restEnemy = enemyNum;
            spawnedEnemy = 0;
            isEnemyLoadDone = true;

            int notYetSpawnCount = enemyNum;
            int totalEnemy = enemyNum;
            float waitScale = Remap(enemyNum, 3, 13, 0.5f, 1);

            for (int i = 0; i < enemyNum; i++)
            {
                EnemySpawnGroup enemySpwanGroup = enemySpawnGroupList[0];

                //랜덤 스폰존
                int ranZone = UnityEngine.Random.Range(0, enemySpwanGroup.EnemySpawnZoneList.Count);
                EnemySpawnZone enemySpwanZone = enemySpwanGroup.EnemySpawnZoneList[ranZone];

                int spawnRate = UnityEngine.Random.Range(0, 100);

                if (spawnRate < 50)
                {
                    // 오크 소환 확률 50%
                    GameObject InstantEnemy = Instantiate(enemySpwanZone.spawnEnemyType[0], enemySpwanZone.transform.position, enemySpwanZone.transform.rotation); ;
                    Enemy enemy = InstantEnemy.GetComponent<Enemy>();
                    enemy.trace_distance = 10000;
                    enemy.ChaseStart();
                    yield return new WaitForSeconds(6f);
                }
                else if (spawnRate < 85)
                {
                    // 리치 소환 확률 35%
                    GameObject InstantEnemy = Instantiate(enemySpwanZone.spawnEnemyType[1], enemySpwanZone.transform.position, enemySpwanZone.transform.rotation);
                    Enemy enemy = InstantEnemy.GetComponent<Enemy>();
                    enemy.trace_distance = 10000;
                    enemy.ChaseStart();
                    yield return new WaitForSeconds(6f);
                }
                else if (spawnRate < 100)
                {
                    // 늑대 소환 확률 15%
                    GameObject InstantEnemy = Instantiate(enemySpwanZone.spawnEnemyType[2], enemySpwanZone.transform.position, enemySpwanZone.transform.rotation);
                    Enemy enemy = InstantEnemy.GetComponent<Enemy>();
                    enemy.trace_distance = 10000;
                    enemy.ChaseStart();
                    yield return new WaitForSeconds(6f);
                }

                notYetSpawnCount--;
                spawnedEnemy++;

                if (notYetSpawnCount % (Mathf.RoundToInt(totalEnemy / 3)) == 0)
                {
                    yield return new WaitForSeconds(3f * waitScale);

                    if (spawnedEnemy > 2)
                    {
                        yield return new WaitUntil(() => spawnedEnemy < 3);
                    }
                }
            }
        }
        // 일반 맵들
        else
        {
            for (int i = 0; i < enemyNum; i++)
            {
                //랜덤 그룹
                int ranGroup = UnityEngine.Random.Range(0, enemySpawnGroupList.Count);
                EnemySpawnGroup enemySpwanGroup = enemySpawnGroupList[ranGroup];

                //랜덤 스폰존
                int ranZone = UnityEngine.Random.Range(0, enemySpwanGroup.EnemySpawnZoneList.Count);
                //print(enemySpwanGroup.name + " || " + enemySpwanGroup.EnemySpawnZoneList.Count);
                EnemySpawnZone enemySpwanZone = enemySpwanGroup.EnemySpawnZoneList[ranZone];

                //랜덤 적군타입
                int ranEnemy = UnityEngine.Random.Range(0, enemySpwanZone.spawnEnemyType.Length);
                GameObject spawnEnemy = enemySpwanZone.spawnEnemyType[ranEnemy];

                GameObject spawnedEnemy = Instantiate(spawnEnemy, enemySpwanZone.transform.position, enemySpwanZone.transform.rotation);
                Enemy enemy = spawnedEnemy.GetComponent<Enemy>();
                enemy.enemySpawnZone = enemySpwanZone;

                if (enemySpwanGroup.isGroupProbe)
                {
                    enemy.enemySpawnGroup = enemySpwanGroup;
                    enemySpwanGroup.groupMemberCount++;
                }
                //spawnedEnemy.GetComponent<Enemy>().actTpye = enemySpwanZone.actType;
                restEnemy++;

                //중복 방지
                enemySpawnGroupList.Remove(enemySpwanGroup);
                enemySpwanGroup.EnemySpawnZoneList.Remove(enemySpwanZone);

                // 그룹 카운트만큼 반복하고 그룹을 초기상태로 복구
                if (enemySpawnGroupList.Count == 0)
                {
                    enemySpawnGroupList = BackupList.ToList();

                    // 이미 모든 스폰존을 소진한 그룹 제외
                    for (int j = BackupList.Count - 1; j >= 0; j--)
                    {
                        if (enemySpawnGroupList[j].EnemySpawnZoneList.Count == 0)
                        {
                            enemySpawnGroupList.Remove(enemySpawnGroupList[j]);
                        }
                    }
                }
            }

            yield return null;
            isEnemyLoadDone = true;
        }


    }

    public void DefaultCursor()
    {
        //print("DefaultCursor");
        Cursor.SetCursor(defaultCursor, new Vector2(0, 3), CursorMode.Auto);
    }

    public void SelectCursor()
    {
        //print("SelectCursor");
        Cursor.SetCursor(selectCursor, new Vector2(0, 3), CursorMode.Auto);
    }

    public void SkillCursor()
    {
        Cursor.SetCursor(skillCursor, new Vector2(skillCursor.width / 2, skillCursor.height / 2), CursorMode.ForceSoftware);
    }

    bool isCursorVisible = true;
    bool isCursorLocked;

    public void ShowCursor()
    {
        if (isCursorVisible) return;

        isCursorVisible = true;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        if (!isCursorVisible) return;

        isCursorVisible = false;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        if (!isCursorLocked) return;

        isCursorLocked = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void LockCursor()
    {
        if (isCursorLocked) return;

        isCursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float Remap(float val, float in1, float in2, float out1, float out2)  //리맵하는 함수
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        DataPersistenceManager.Instance.RemoveDestroyedIDataPersistence(this);
    }
}
