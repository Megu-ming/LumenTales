using System.Collections;
using System.Text;
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
        StartCoroutine(TypeTextEffect(script));
    }

    IEnumerator TypeTextEffect(string script)
    {
        conversationScript.text = string.Empty;

        StringBuilder stringBuilder = new StringBuilder();
        for(int i=0; i<script.Length; i++)
        {
            stringBuilder.Append(script[i]);
            conversationScript.text = stringBuilder.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
