using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
     private Text levelTxt;
     private Image hpBarSlider;
     private Image expBarSlider;

    private void Awake()
    {

        // we wanna get the slider property, which is the child(0) of child(0) of Canvas 
        hpBarSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        // we wanna get the slider property, which is the child(0) of child(1) of Canvas 
        expBarSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        // the Text component is the third child of Canvas componnet  
        levelTxt = transform.GetChild(2).GetComponent<Text>();
    }

    private void Update()
    {
        levelTxt.text = "Level " + GameManager.singletonInstance.playerData.characterDataSO.level;
        updatePlayerHpBar();
        updatePlayerExpBar();
    }

    private void updatePlayerHpBar()
    {
          // the fill amount value for currentHp image 
          // get the float value for the ho bar slider 
          float hpSliderPercentage = (float)GameManager.singletonInstance.playerData.CurrentHealth / GameManager.singletonInstance.playerData.MaxHealth;
          hpBarSlider.fillAmount = hpSliderPercentage;
        
    }
    private void updatePlayerExpBar()
    {
        // the fill amount value for currentHp image 
        // get the float value for the ho bar slider 
        float expSliderPercentage = (float)GameManager.singletonInstance.playerData.characterDataSO.currentExp / GameManager.singletonInstance.playerData.characterDataSO.levelUpNeededExp;
        expBarSlider.fillAmount = expSliderPercentage;

    }


}
