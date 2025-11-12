using UnityEngine;

[CreateAssetMenu(fileName = "TalkData", menuName = "Talk/TalkData")]
public class TalkData : ScriptableObject
{
    public NPCType NPCType;
    public TalkContent[] talkContent;

    public TalkContent GetTalkContent(TalkState state)
    {
        if(talkContent == null || talkContent.Length == 0)
            return null;
        
        foreach(var content in talkContent)
        {
            if (content != null && content.state == state)
                return content;
        }

        return talkContent[0];
    }
}

[System.Serializable]
public class TalkContent
{
    public TalkState state;
    public string[] scripts;
}