using UnityEngine;

[System.Serializable]
public class ConversationData
{
    public int conversationStep = 0;        // 현재 대화 단계
    public string[] conversations;  // 대화 스크립트 배열

    public string GetConversation() // 대화 반환
    {
        if (conversations.Length <= conversationStep)
        {
            return ".....";
        }

        return conversations[conversationStep++];
    }

    public void ResetConversation() // 대화 단계 초기화
    {
        conversationStep = 0;
    }

    public bool ConversationComplete() // 대화가 끝났는지?
    {
        return conversations.Length <= conversationStep;
    }
}
