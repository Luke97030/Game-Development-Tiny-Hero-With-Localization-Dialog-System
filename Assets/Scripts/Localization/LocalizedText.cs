using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    private LocalizationManager localizationManager;
    [Tooltip("The dropdown box to populate with flags")]
    public Dropdown dropdown;
    [Tooltip("The flags of the language that our game supports")]
    public Sprite[] flags;
    private Text currentTextComponent;
    private string filePath;
    public string key;
    private Button quitButton;

    public GameObject America;
    public GameObject China;
    // Start is called before the first frame update
    void Start()
    {
        
        currentTextComponent = GetComponent<Text>();
        quitButton = GetComponent<Button>();
        localizationManager = new LocalizationManager();

        // flag items handling
        dropdown.ClearOptions();
        List<Dropdown.OptionData> flagItems = new List<Dropdown.OptionData>();

        foreach (var flag in flags)
        {
            var flagOption = new Dropdown.OptionData(flag.name, flag);
            flagItems.Add(flagOption);
        }
        // adding the items into dropdown component
        dropdown.AddOptions(flagItems);
        // control the text change when the different language flag be selected
        DropDownItemSelected(dropdown);
        dropdown.onValueChanged.AddListener(delegate { DropDownItemSelected(dropdown); });

        // set the text when first start the scene, taking the default flag which is US (english)
        currentTextComponent.text = localizationManager.GetLocalizedValue(filePath, key);
    }

    private void DropDownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;

        switch (index)
        {
            case 0:
                filePath = "localizedText_en";
                America.SetActive(true);
                China.SetActive(false);
                break;
            case 1:
                filePath = "localizedText_cn";
                America.SetActive(false);
                China.SetActive(true);
                break;
        }

        // set the text when first start the scene, taking the default flag
        currentTextComponent.text = localizationManager.GetLocalizedValue(filePath, key);
    }
}
