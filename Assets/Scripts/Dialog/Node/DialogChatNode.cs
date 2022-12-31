using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogChatNode
{
    public string id;
    // the name of current speaker
    public string name;
    // the image for the current speaker
    public Sprite sprite;
    // chat content
    [TextArea]
    public string content;

    public List<DialogDecisionNode> decisionNodes = new List<DialogDecisionNode>();
}
