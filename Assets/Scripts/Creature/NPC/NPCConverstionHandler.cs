using TMPro;
using UnityEngine;

public class NPCConverstionHandler : MonoBehaviour
{
    [SerializeField] ConversationData data;

    [SerializeField] UIConversation conversationUI;

    public void ConversationEvent() // 대화 이벤트
    {
        if (data.ConversationComplete()) // 대화가 끝났는지 확인
        {
            // 끝났다면 대화 종료 및 초기화
            ControlConversationInterface(false);
            data.ResetConversation();
            return;
        }

        if (!conversationUI.gameObject.activeSelf) // 대화 창이 켜져있지 않다면 대화 시작
        {
            ControlConversationInterface(true);
        }

        // 대화 창 텍스트에 대화 저장
        conversationUI.SetName(gameObject.name);
        conversationUI.SetScript(data.GetConversation());
    }

    private void ControlConversationInterface(bool isTrue) // 대화 창 활성화/비활성화
    {
        conversationUI.gameObject.SetActive(isTrue);
    }

    // 대화 시작 메소드
    // TODO

    // 대화 종료 메소드
    // TODO
}