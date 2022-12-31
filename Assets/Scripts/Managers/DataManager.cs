using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : Singleton<DataManager>
{
    private string scene = "";
    public string Scene { get { return PlayerPrefs.GetString(scene); } }
    protected override void Awake()
    {
        // keep whatever base.Awake has 
        base.Awake();
        // do not destory the sceneController when load a new scene
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            savePlayerData();
            StartCoroutine(SceneController.singletonInstance.loadMainMenuScene());
        }
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    savePlayerData();
        //}
    }

    public void savePlayerData()
    {
        // using characterDataSO as key 
        saveData(GameManager.singletonInstance.playerData.characterDataSO, GameManager.singletonInstance.playerData.characterDataSO.name);
    }

    public void loadPlayerData()
    {
        // Load the key we just saved 
        loadData(GameManager.singletonInstance.playerData.characterDataSO, GameManager.singletonInstance.playerData.characterDataSO.name);
    }

    // save data 
    public void saveData(Object obj,string key)
    {
        // convert the obt to json string 
        var jsonString = JsonUtility.ToJson(obj, true);
        // save the key value pair to PlayerPrefs
        PlayerPrefs.SetString(key, jsonString);
        // save the name of the scene as well in order to continue game in the game menu
        PlayerPrefs.SetString(scene, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
       
    }

    // load data 
    public void loadData(Object obj, string key)
    {
        // if cannot find the key, do not load 
        if (PlayerPrefs.HasKey(key))
        {
            // load the data from Prefab to obj
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), obj);
        }
    }
}
