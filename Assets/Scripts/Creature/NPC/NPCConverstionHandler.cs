using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPCConverstionHandler : InteractiveObj
{
    [SerializeField] InteractionManager interactionManager;
    [SerializeField] NPCData data;

    [SerializeField] UIConversation conversationUI;

    private int conversationStep = 0;
    // 스킵으로 대화가 막 넘어갔는지?
    private bool justRevealed = false;

    public static event Action<bool> OnConversationToggle;

    private void Start()
    {
        if (UIManager.instance != null && UIManager.instance.conversationUI != null)
            conversationUI = UIManager.instance.conversationUI;
    }

    public override void OnInteraction() // 대화 이벤트
    {
        if(data == null || data.conversations == null || data.conversations.Length == 0)
        {
            Debug.LogWarning("No Conversation Data");
            return;
        }

        // 대화창이 꺼져있으면 시작
        if (!conversationUI.gameObject.activeSelf)
        {
            StartConversation();
            return;
        }

        // 타이핑 중이면 스킵
        if(conversationUI.isTypeEffect)
        {
            conversationUI.ShowAllScript();
            justRevealed = true; // 스킵으로 대화가 넘어갔음
            return;
        }
        // 방금 스킵했으면 다음 대화 진행
        if(justRevealed)
        {
            justRevealed = false;
            ProceedOrEnd();
            return;
        }
        // 대화 진행
        ProceedOrEnd();
    }

    private void StartConversation()
    {
        conversationStep = 0;
        justRevealed = false;

        // 카메라 줌인
        interactionManager.ZoomIn();

        ControlConversationInterface(true);
        SetLine(conversationStep);
        conversationStep++;
    }

    private void ProceedOrEnd()
    {
        if(conversationStep >= data.conversations.Length)
        {
            EndConversation();
            return;
        }

        SetLine(conversationStep);
        conversationStep++;
    }

    private void SetLine(int index)
    {
        string npcName = string.IsNullOrEmpty(data.Name) ? gameObject.name : data.Name;
        conversationUI.SetName(npcName);
        conversationUI.SetScript(data.conversations[index]);
    }

    private void EndConversation()
    {
        // 카메라 줌아웃
        interactionManager.ZoomOut();
        ControlConversationInterface(false);
        conversationStep = 0;
        justRevealed = false;

        // 대화 종료 시 퀘스트 할당
        // 만약 퀘스트가 이미 할당되어있다면 패스
        


        conversationUI.Clear();
    }

    private void ControlConversationInterface(bool isTrue)
    {
        conversationUI.gameObject.SetActive(isTrue);
        OnConversationToggle?.Invoke(isTrue);
    }

    // 대화 시작 메소드
    // TODO

    // 대화 종료 메소드
    // TODO
}