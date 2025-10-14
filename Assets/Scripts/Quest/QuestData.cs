using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string questTitle;       // 퀘스트 제목
    public QuestType questType;     // 퀘스트 종류
    [TextArea] public string description;      // 퀘스트 설명

    // 퀘스트 목표 (예: "몬스터 10마리 처치", "아이템 5개 수집" 등)
    [Header("Goal - Kill")]
    public string targetName;       // 목표 대상 이름
    public int requiredKillCount;           // 목표 대상 처치 수

    [Header("Goal - Collect")]
    public ItemData targetItem;     // 목표 아이템
    public int requiredItemCount;   // 목표 아이템 수량

    public Reward reward;           // 퀘스트 보상

    /// <summary>
    /// 퀘스트 생성 및 플레이어의 퀘스트 컨테이너에 추가
    /// </summary>
    public void MakeQuest()
    {
        Quest newQuest = new Quest();
        newQuest.questData = this;
        newQuest.currentQuestState = QuestState.InProgress;
        GameManager.instance.questManager.questContainer.Add(newQuest);
    }
}

[System.Serializable]
public struct Reward
{
    public int gold;                // 골드 보상
    public int exp;                 // 경험치 보상
    public ItemData itemData;       // 아이템 보상 (아이템 ID)
    public int itemCount;           // 아이템 수량
}