using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    // make sure this is only one LocalizationManager object through the game life span
    public static LocalizationManager localizationManager;
    private Dictionary<string, string> localizedText;
    private bool isLocalized = false;

    private void Awake()
    {
        // initialize the LocalizationManager instance if it does not has been initialized
        if (localizationManager == null)
            localizationManager = this;
        else if (localizationManager != this)
            Destroy(gameObject);

        // Do not destroy the target Object when loading a new Scene.
        DontDestroyOnLoad(gameObject);
    }

    // 
    public void LoadLocalizedText(string fileName)
    {
        // initialize the localizedText
        localizedText = new Dictionary<string, string>();

        // get the relative file path 
        string filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);
        // Debug.Log(filePath);
        // if the file name has no extension be given, appending json extension to it
        if (!Path.HasExtension(filePath))
            filePath += ".json";
        // Debug.Log(filePath);
        // if the file path is valid 
        if (File.Exists(filePath))
        {
            // read the content of the file and store it into a variable 
            string dataAsJson = File.ReadAllText(filePath);

            // mapping the JSON data with LocalizationData class properties 
            LocalizationData loadData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            // assigning the key-value pair into local dictionary
            for (int i = 0; i < loadData.localizationData.Length; i++)
                localizedText.Add(loadData.localizationData[i].key, loadData.localizationData[i].value);

            //Debug.Log("Data loaded. Dictionary has: " + localizedText.Count + " pairs");
            isLocalized = true;
        }
        else
            Debug.LogError("The localization file cannot be found!");
    }

    public string GetLocalizedValue(string jsonFile, string key)
    {
        string result = "";
        // local the local location json file based on the file name
        LoadLocalizedText(jsonFile);
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;

    }

    public bool GetIsLocalized()
    {
        return isLocalized;
    }
}
