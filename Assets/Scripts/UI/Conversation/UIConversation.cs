using TMPro;
using UnityEngine;

public class UIConversation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI npcName;
    [SerializeField] TextMeshProUGUI conversationScript;

    public void SetName(string name)
    {
        npcName.text = name;
    }

    public void SetScript(string script)
    {
        conversationScript.text = script;
    }
}
