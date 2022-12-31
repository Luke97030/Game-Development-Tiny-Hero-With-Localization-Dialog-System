using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialogue/Dialogue Data")]
public class DialogData_SO : ScriptableObject
{
    public List<DialogChatNode> chatNodes = new List<DialogChatNode>();
    // the key will be the targetId of decision node
    // the value will be chatNode obj
    public Dictionary<string, DialogChatNode> dialogDictionary = new Dictionary<string, DialogChatNode>();

    // will be trigger if we made any change in unity editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        // update dictionay once the elements in the decision list changes 
        dialogDictionary.Clear();
        for (int i = 0; i < chatNodes.Count; ++i)
        {
            // if the key (tragetid) is not one of the id in the chatNode list
            if (!dialogDictionary.ContainsKey(chatNodes[i].id))
            {
                // adding the key and value pair to dictionary
                dialogDictionary.Add(chatNodes[i].id, chatNodes[i]);
            }
        }
    }
#else 
    private void Awake()
    {
        dialogDictionary.Clear();
         for (int i = 0; i < chatNodes.Count; ++i)
        {
            // if the key (tragetid) is not one of the id in the chatNode list
            if (!dialogDictionary.ContainsKey(chatNodes[i].id))
            {
                // adding the key and value pair to dictionary
                dialogDictionary.Add(chatNodes[i].id, chatNodes[i]);
            }
        }
    }
#endif
}
