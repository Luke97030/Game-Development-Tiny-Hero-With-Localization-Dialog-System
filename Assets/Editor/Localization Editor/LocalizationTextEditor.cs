using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

// localizationEditor window implementation 
[CanEditMultipleObjects]
[CustomEditor(typeof(LocalizationData))]
public class LocalizationTextEditor : EditorWindow
{
    public LocalizationData localizationData;
    SerializedProperty _serializedProperty;
    SerializedObject _serializedObject;

    // create a new editor window (Localization Editor Window) under Tools
    [MenuItem("Tools/Localization Text Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LocalizationTextEditor)).Show();
    }

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
        // find the customerized class and get all property of it
        _serializedProperty = _serializedObject.FindProperty("localizationData");
    }


    private void OnGUI()
    {

        if (localizationData != null)
        {
            // using SerializedOject and SerializedProperty becase we are going to create a customized array 
            _serializedObject.Update();
            SerializedProperty serializedProperty = _serializedObject.FindProperty("localizationData");


            EditorGUILayout.PropertyField(serializedProperty, true);
            _serializedObject.ApplyModifiedProperties();

            // make a button to save localization data
            if (GUILayout.Button("Save data"))
            {
                saveLocalizationData();
            }
          
        }

        // Loading localization data from one of the JSON file under the streamingAssets directory
        if (GUILayout.Button("Load data"))
        {
            loadLocalizationData();
        }

        // this allow user to create an empty LocalizationData instead of delete the record one by one 
        if (GUILayout.Button("Create new data"))
        {
            createNewLocalizationData();
        }
    }

    // this function will allow user to save the localization data into a JSON file under the streamingAssets directory
    // this function can either modify the existing data after load and then save
    // or save the new data into a new localization data JSON file 
    private void saveLocalizationData()
    {

        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);
        }
    }
    private void loadLocalizationData()
    {

        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            _serializedObject.Update();
            string dataAsJson = File.ReadAllText(filePath);

            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            EditorGUILayout.PropertyField(_serializedProperty, true);
            _serializedObject.ApplyModifiedProperties();
        }
    }

    private void createNewLocalizationData()
    {
        localizationData = new LocalizationData();
    }
}