using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.InputSystem;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disalbeDataPersistence = false;   //세이브 로드기능 작동 중지
    [SerializeField] private bool initializeDataIfNull = false; // 초기 상태로 시작

    //테스트용 세이브 파일 가동
    [SerializeField] private bool overrideSelectedProfileId = false;    
    [SerializeField] private string testSelectedProfileId = "test";     


    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    public static DataPersistenceManager Instance {get; private set;}

    private void Awake(){
        if(Instance != null){
            Debug.Log("Found more than one Data Persistence Manager in the Scene, Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if(disalbeDataPersistence){
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        string path = Path.Combine(Application.persistentDataPath, "SaveFile");
        this.dataHandler = new FileDataHandler(path, fileName, useEncryption);
        
        InitializeSelectedProfileId();
    }

    private void Update() {
        
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded_LoadData;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded_LoadData;
    }

    public void OnSceneLoaded_LoadData(Scene scene, LoadSceneMode mode){
        Debug.Log("OnSceneLoaded_LoadData");
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        if(scene.name == "StartMap"){
            SceneManager.sceneLoaded -= OnSceneLoaded_LoadData;
        }
    }

    public void LoadDataFromObjects()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileId(string newProfileId){
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void DeleteProfileData(string profileId){
        dataHandler.Delete(profileId);

        InitializeSelectedProfileId();

        LoadGame();

    }

    private void InitializeSelectedProfileId(){
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.Log("Override selected profile id with test id" + testSelectedProfileId);
        }
    }
    
    public void NewGame(){
        this.gameData = new GameData();
        this.gameData.createDate = System.DateTime.Now.ToBinary();

        GameManager.Instance.Load_SkillDataFromJosn();
        GameManager.Instance.Load_ItemDataFromJosn();
        GameManager.Instance.Load_ShopUnlockDataFromJosn();
        GameManager.Instance.Load_PlayerDataFromJson();

        gameData.ActiveSkillList = GameManager.Instance.MyActiveSkillList;
        gameData.PassiveSkillList = GameManager.Instance.MyPassiveSkillList;
        gameData.ShopItemList = GameManager.Instance.MyShopItemList;
        gameData.ShopUnlockList = GameManager.Instance.MyShopUnlockList;
        gameData.playerData = GameManager.Instance.playerData;
        GameManager.Instance.playerData.doTutorial = true;
    }

    public void LoadGame(){
        if(disalbeDataPersistence){
            return;
        }

        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if(this.gameData == null){
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded");
            return;
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects){
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame(){
        if (disalbeDataPersistence)
        {
            return;
        }

        if(this.gameData == null){
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        dataHandler.Save(gameData, selectedProfileId);
    }
    
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void RemoveDestroyedIDataPersistence(IDataPersistence iDataPersistence){
        dataPersistenceObjects.Remove(iDataPersistence);
    }

    public bool HasGameData(){
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData(){
        return dataHandler.LoadAllProfiles();
    }

    public void DestoyObjectsForTitle(){
        Destroy(Canvas5.Instance.gameObject);
        Destroy(FindObjectOfType<Player>().gameObject);
        Destroy(FindObjectOfType<Canvas0>().gameObject);
        Destroy(FindObjectOfType<Canvas7>().gameObject);
        Destroy(FindObjectOfType<DialogueManager>().gameObject);
        Destroy(FindObjectOfType<MainCamera>().gameObject);
        Destroy(FindObjectOfType<Minimap>().gameObject);
        Destroy(FindObjectOfType<CutSceneManager>().gameObject);
    }

    public void DestoyObjectsForGame()
    {
        Destroy(Canvas5.Instance.gameObject);
        //Destroy(GameManager.Instance.gameObject);
    }
}
