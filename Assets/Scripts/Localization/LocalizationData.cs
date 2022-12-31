using System;
using UnityEngine;

// Serializable class, we need to store the data using customerized object and also key and value pair s
[Serializable]
public class LocalizationData
{
    public LocalizationDataArray[] localizationData;
}

[Serializable]
public class LocalizationDataArray
{
    // we are trying to store the data into json object, which is follwing key-value manner
    public string key;
    public string value;

}

