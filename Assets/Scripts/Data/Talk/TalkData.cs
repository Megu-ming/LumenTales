using UnityEngine;

[CreateAssetMenu(fileName = "TalkData", menuName = "Talk/TalkData")]
public class TalkData : ScriptableObject
{
    public NPCType NPCType;
    public TalkContent[] talkContent;
}

[System.Serializable]
public class TalkContent
{
    public TalkState state;
    public string[] scripts;
}