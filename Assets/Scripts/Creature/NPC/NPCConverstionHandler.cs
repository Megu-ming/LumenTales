using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPCConverstionHandler : InteractiveObj
{
    [SerializeField] InteractionManager interactionManager;
    [SerializeField] TalkData talkData;
    public TalkState curState;

    public int id;
    private int conversationStep = 0;
    // 스킵으로 대화가 막 넘어갔는지?
    private bool justRevealed = false;

    public static event Action<bool> OnConversationToggle;

    public override void OnInteraction() // 대화 이벤트
    {
        if (talkData == null) return;
        foreach ( var talk in talkData.talkContent)
        {
            if(talk.state == curState)
            {
                
            }
        }

        //// 대화창이 꺼져있으면 시작
        //if (!UIManager.instance.conversationUI.gameObject.activeSelf)
        //{
        //    StartConversation();
        //    return;
        //}

        //// 타이핑 중이면 스킵
        //if(UIManager.instance.conversationUI.isTypeEffect)
        //{
        //    UIManager.instance.conversationUI.ShowAllScript();
        //    justRevealed = true; // 스킵으로 대화가 넘어갔음
        //    return;
        //}
        //// 방금 스킵했으면 다음 대화 진행
        //if(justRevealed)
        //{
        //    justRevealed = false;
        //    ProceedOrEnd();
        //    return;
        //}
        //// 대화 진행
        //ProceedOrEnd();
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
        //if(conversationStep >= data.conversations.Length)
        //{
        //    EndConversation();
        //    return;
        //}

        //SetLine(conversationStep);
        //conversationStep++;
    }

    private void SetLine(int index)
    {
        //string npcName = string.IsNullOrEmpty(data.Name) ? gameObject.name : data.Name;
        //UIManager.instance.conversationUI.SetName(npcName);
        //UIManager.instance.conversationUI.SetScript(data.conversations[index]);
    }

    private void EndConversation()
    {
        //// 카메라 줌아웃
        //interactionManager.ZoomOut();
        //ControlConversationInterface(false);
        //conversationStep = 0;
        //justRevealed = false;

        //UIManager.instance.conversationUI.Clear();
        
        //// 퀘스트 제공
        //if(data.quest is not null && 
        //    !GameManager.instance.questManager.questContainer.ContainsKey(data.quest.questID) &&
        //    !GameManager.instance.questManager.completedQuestContainer.Contains(data.quest.questID))
        //{
        //    data.quest.MakeQuest();

        //    return; 
        //}

        //GameManager.instance.questManager.TryCompleteQuest(data.quest.questID);

    }

    private void ControlConversationInterface(bool isTrue)
    {
        UIManager.instance.conversationUI.gameObject.SetActive(isTrue);
        OnConversationToggle?.Invoke(isTrue);
    }
}