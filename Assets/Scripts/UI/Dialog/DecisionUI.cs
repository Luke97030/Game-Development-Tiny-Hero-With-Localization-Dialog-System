using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionUI : MonoBehaviour
{
    private DialogChatNode chatNode;
    private Button decisionBtn;
    public Text decisionTxt;

    // we need to get targetId and assign to nextChatNodeId which is where we go next
    private string nextChatNodeId;

    private void Awake()
    {
        decisionBtn = GetComponent<Button>();
        decisionBtn.interactable = true;
        decisionBtn.onClick.AddListener(onDecisionClick);
    }

    public void updateDecisionNode(DialogChatNode chat, DialogDecisionNode decision)
    {
        chatNode = chat;
        decisionTxt.text = decision.text;
        nextChatNodeId = decision.targetId;
    }

    public void onDecisionClick()
    {
        // check target id and start next nodes
        if (nextChatNodeId == "")
        {
            // if no target for the decison node, we quit the dialog
            DialogUI.singletonInstance.dialogPanel.SetActive(false);
            return;
        }
        else
        {
            // get the vaule(chatNode) based on the key from dictionary
            DialogUI.singletonInstance.updateChatDialogUI(DialogUI.singletonInstance.dialogDataSO.dialogDictionary[nextChatNodeId]);
        }
    }

}
