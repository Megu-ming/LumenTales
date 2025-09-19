using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class UIConversation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI npcName;
    [SerializeField] TextMeshProUGUI conversationScript;

    public bool isTypeEffect = false;
    string cacheScript;

    public void SetName(string name)
    {
        npcName.text = name;
    }

    public void SetScript(string script)
    {
        cacheScript = script;
        StartCoroutine(TypeTextEffect(script));
    }

    public void ShowAllScript()
    {
        StopAllCoroutines();
        isTypeEffect = false;
        conversationScript.text = cacheScript;
    }

    IEnumerator TypeTextEffect(string script)
    {
        conversationScript.text = string.Empty;
        isTypeEffect = true;
        StringBuilder stringBuilder = new StringBuilder();
        for(int i=0; i<script.Length; i++)
        {
            stringBuilder.Append(script[i]);
            conversationScript.text = stringBuilder.ToString();
            yield return new WaitForSeconds(0.05f);
        }
        isTypeEffect = false;
    }
}
