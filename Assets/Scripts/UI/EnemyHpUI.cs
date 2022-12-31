using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpUI : MonoBehaviour
{
    // Getting the HP Bar prefab
    public GameObject hPBarPrefab;
    public Transform hpBarPosition;
    public bool showHpBar;
    public float hpBarVisibleTime;
    private float hpBarVisibleTimeLeft;

    // this will control what hp bar look like
    Image hpSlider;

    private Transform hpBarUI;
    // we should be able to see the hp bar from different position. 
    // we need to get the opposite direction of camera.Forward
    private Transform cameraPosition;
    // trigger updateHPBarOnAttack
    private CharacterData characterData;



    private void Awake()
    {
        // get the CharacterData from the attched game object 
        characterData = GetComponent<CharacterData>();
        characterData.updateHPBarOnAttack += updateEnemyHPBar;
    }

    private void OnEnable()
    {
        // getting the main Camera transform
        cameraPosition = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            // getting the all WorldSpace canvase
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                hpBarUI = Instantiate(hPBarPrefab, canvas.transform).transform;
                hpSlider = hpBarUI.GetChild(0).GetComponent<Image>();
                hpSlider.fillAmount = 1;
                hpBarUI.gameObject.SetActive(showHpBar);

            }
        }
    }

    private void updateEnemyHPBar(int currentHp, int maxHp)
    {
        // destory hp ban when hp reach 0
        if (currentHp <= 0)
        {         
            Destroy(hpBarUI.gameObject);
        }
        else
        {
            // show the hpBar
            hpBarUI.gameObject.SetActive(true);
            hpBarVisibleTimeLeft = hpBarVisibleTime;
            // the fill amount value for currentHp image 
            float hpSliderPercentage = (float)currentHp / maxHp;
            hpSlider.fillAmount = hpSliderPercentage;
        }   
    }

    private void LateUpdate()
    {
        if (hpBarUI != null)
        {
            hpBarUI.position = hpBarPosition.position;
            // the HP UI bar should always be facing to the camera
            hpBarUI.forward = -cameraPosition.forward;

            if (hpBarVisibleTimeLeft <= 0 && showHpBar == false)
            {
                hpBarUI.gameObject.SetActive(false);
            }
            else
            {
                hpBarVisibleTimeLeft -= Time.deltaTime;
            }
        }
    }
}
