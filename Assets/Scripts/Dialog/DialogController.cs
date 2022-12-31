using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    // we should have a list of dialogData
    // know the game only support two language, so we have 2 elements in the dialogData_SO List, index to indicate which one we will use
    public List<DialogData_SO> dialogDataSO;
    // when talkFlag is true, player can talk to the NPC when he enter the range of collider
    public bool talkFlag = false;

    PlayerController tempPlayerController;

    // get the language selection from the main menu
    bool languageEN = true;
    bool languageCN = false;

    private void OnEnable()
    {
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.name == "DropdownCanvas")
            {
                // get the dropdown game object
                var america = canvas.transform.GetChild(0).Find("America").gameObject;
                var china = canvas.transform.GetChild(0).Find("China").gameObject;
                languageEN = america.activeInHierarchy;
                languageCN = china.activeInHierarchy;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && dialogDataSO != null)
        {
            //PlayerController playerController = other.GetComponent<PlayerController>();
            //playerController.canMove = false;
            //tempPlayerController = playerController;
            talkFlag = true;
        }
    }


    // we need to set the talkFlag to false once player is not in the collision range
    private void OnTriggerExit(Collider other)
    {
        talkFlag = false;
    }


    private void Update()
    {
        // when the talkFlag is true and user right click on mouse
        if (talkFlag && Input.GetMouseButtonDown(1))
        {
            showDialogCanvas();
        }
    }

    private void showDialogCanvas() 
    {      
        if (languageEN && !languageCN)
        {
            // open English Dialog Panel 
            DialogUI.singletonInstance.updateDialogData(dialogDataSO[0]);
            // using English
            // show the chat dialog UI English version
            DialogUI.singletonInstance.updateChatDialogUI(dialogDataSO[0].chatNodes[0]);
        }
        else if (languageCN && !languageEN)
        {
            // open Chinese Dialog Panel 
            DialogUI.singletonInstance.updateDialogData(dialogDataSO[1]);
            // using Chinese 
            // show the chat dialog UI Chinese verion
            DialogUI.singletonInstance.updateChatDialogUI(dialogDataSO[1].chatNodes[0]);
        }
    }
}
