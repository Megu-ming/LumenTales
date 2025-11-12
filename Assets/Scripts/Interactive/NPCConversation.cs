using UnityEngine;

public class NPCConversation : InteractiveObj
{
    [SerializeField] string npcName;
    [SerializeField] TalkData talkData;
    [SerializeField] TalkState talkState = TalkState.Default;
    [SerializeField] UIRoot uiRoot;
    [SerializeField] PlayerController playerController;

    int currentLineIndex = -1;
    TalkContent currentContent;
    bool conversationActive;

    private void Awake()
    {
        CacheTalkContent();
    }

    private void OnValidate()
    {
        CacheTalkContent();
    }

    void CacheTalkContent()
    {
        if(talkData == null)
        {
            currentContent = null;
            return;
        }

        currentContent = talkData.GetTalkContent(talkState);
        currentLineIndex = -1;
    }

    public override void OnInteraction()
    {
        if (talkData == null || uiRoot == null || uiRoot.conversationUI == null)
            return;
        if(currentContent == null)
            CacheTalkContent();

        var conversationUI = uiRoot.conversationUI;

        if(!conversationUI.gameObject.activeSelf)
        {
            conversationUI.gameObject.SetActive(true);
            conversationUI.SetName(string.IsNullOrEmpty(npcName) ? name : npcName);
            BeginConversation();
        }

        if(conversationUI.isTypeEffect)
        {
            conversationUI.ShowAllScript();
            return;
        }

        if(currentContent == null || currentContent.scripts==null|| currentContent.scripts.Length ==0)
        {
            CloseConversation();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < currentContent.scripts.Length)
            conversationUI.SetScript(currentContent.scripts[currentLineIndex]);
        else
            CloseConversation();
    }

    void CloseConversation()
    {
        if(uiRoot && uiRoot.conversationUI)
        {
            uiRoot.conversationUI.Clear();
            uiRoot.conversationUI.gameObject.SetActive(false);
        }
        currentLineIndex = -1;

        EndConversation();
    }

    private void OnDisable()
    {
        CloseConversation();
    }

    void BeginConversation()
    {
        if (conversationActive) return;

        playerController.SetConversationsState(true);
        conversationActive = true;
    }

    void EndConversation()
    {
        if(!conversationActive) return;
        playerController.SetConversationsState(false);
        conversationActive = false;
    }
}
