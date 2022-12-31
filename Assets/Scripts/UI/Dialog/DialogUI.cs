using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq.Expressions;
using System.Runtime.Serialization.Json;

public class DialogUI : Singleton<DialogUI>
{

    [Header("Chat Info")]
    public Image characterImage;
    public Text chatText;
    public Button nextButton;

    // we need to control the visibility of dialogPanel 
    public GameObject dialogPanel;

    [Header("Decision Info")]
    // we want to get the decision panel (bottom one)
    public RectTransform decisionPanel;
    // create an instance of DecisionUI, so we can control the property of it
    public DecisionUI decisionUIPrefab;

    [Header("Data")]
    public DialogData_SO dialogDataSO;
    // this index to indicate which node are we going to show
    int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        // if the next button be clicked then move to the next chat node
        nextButton.onClick.AddListener(movetoNextNode);
    }

    public void updateDialogData(DialogData_SO data)
    {
        dialogDataSO = data;
        // make sure eveytime you start the conversation with NPC, it starts from beginning
        currentIndex = 0;
    }

    public void updateChatDialogUI(DialogChatNode dcn)
    {
        
        // show the dialogPanel
        dialogPanel.SetActive(true);
        currentIndex++;

        if (dcn.sprite != null)
        {
            characterImage.enabled = true;
            characterImage.sprite = dcn.sprite;
        }
        else 
        {
            characterImage.enabled = false;
        }

        // clean the test text content
        chatText.text = "";
        // print the chat content in 2 second word by word
        chatText.DOText(dcn.content, 2);

        // if we have the chats node and under it there is no any decision nodes, then we should the next button
        if (dcn.decisionNodes.Count == 0 && dialogDataSO.chatNodes.Count > 0)
        {
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);

        }
        // else the chatNode has options, we do not want to show the next button
        else
        {
            // cannot click the button
            nextButton.interactable = false;
            // only set the text to unactive, so it wont mess up with vertical layout
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
           
        }

        // create decison nodes 
        createDecisionNodes(dcn);
    }

    private void createDecisionNodes(DialogChatNode chatNode)
    {
        // there is alreay few decisions are showing, we want to clear them all before creating decision Node
        if (decisionPanel.childCount > 0)
        {
            for (int i = 0; i < decisionPanel.childCount; ++i)
            {
                Destroy(decisionPanel.GetChild(i).gameObject);
            }
        }

        // creating option buttons 
        for (int i = 0; i < chatNode.decisionNodes.Count; ++i)
        {
            var decision = Instantiate(decisionUIPrefab, decisionPanel);
            decision.updateDecisionNode(chatNode, chatNode.decisionNodes[i]);
        }
    }


    private void movetoNextNode()
    {
        if (currentIndex < dialogDataSO.chatNodes.Count)
        {
            // check if there is more english chat node or not 
            //if (isEnglishLocal && !isChineseLocal)
            //{
                // if there is more, we are updating currentIndex by +1 in the updateChatDialogUI function
                updateChatDialogUI(dialogDataSO.chatNodes[currentIndex]);
            //}
            //else if (isEnglishLocal && !isChineseLocal)
            //{

            //}
        }
        else
        {
            dialogPanel.SetActive(false);
        }
    }

 
}
