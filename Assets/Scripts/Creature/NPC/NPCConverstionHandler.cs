using System;
using TMPro;
using UnityEngine;

public class NPCConverstionHandler : MonoBehaviour
{
    [SerializeField] ConversationData data;

    [SerializeField] UIConversation conversationUI;

    // 스킵으로 대화가 막 넘어갔는지?
    bool justRevealed = false;

    public static event Action<bool> OnConversationToggle;

    public void ConversationEvent() // 대화 이벤트
    {
        // 대화창이 꺼져있으면 시작
        if (!conversationUI.gameObject.activeSelf)
        {
            ControlConversationInterface(true);
            justRevealed = false; // 새 대화 시작 시 리셋
        }

        if(conversationUI.isTypeEffect)
        {
            conversationUI.ShowAllScript();
            justRevealed = true; // 스킵으로 대화가 넘어갔음
            return;
        }

        if(justRevealed)
        {
            justRevealed = false;
            if(data.ConversationComplete())
            {
                EndConversation();
                return;
            }
        }

        conversationUI.SetName(gameObject.name);
        conversationUI.SetScript(data.GetConversation());
    }

    private void EndConversation()
    {
        ControlConversationInterface(false);
        conversationUI.Clear();
        data.ResetConversation();
        justRevealed = false;
    }

    private void ControlConversationInterface(bool isTrue) // 대화 창 활성화/비활성화
    {
        conversationUI.gameObject.SetActive(isTrue);
        OnConversationToggle?.Invoke(isTrue);
    }

    // 대화 시작 메소드
    // TODO

    // 대화 종료 메소드
    // TODO
}