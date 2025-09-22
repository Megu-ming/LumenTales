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
        // 새로운 줄 시작 전에 텍스트를 비워서 깜빡임 방지
        conversationScript.text = string.Empty;
        StartCoroutine(TypeTextEffect(script));
    }

    public void ShowAllScript()
    {
        StopAllCoroutines();
        isTypeEffect = false;
        conversationScript.text = cacheScript;
    }

    public void Clear()
    {
        StopAllCoroutines();
        isTypeEffect = false;
        cacheScript = null;
        if(conversationScript != null)
            conversationScript.text = string.Empty;
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
