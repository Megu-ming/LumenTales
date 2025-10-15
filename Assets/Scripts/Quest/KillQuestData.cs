using UnityEngine;

[CreateAssetMenu(fileName = "KillQuestData", menuName = "Quest/KillQuestData")]
public class KillQuestData : QuestData
{
    // 퀘스트 목표 (예: "몬스터 10마리 처치", "아이템 5개 수집" 등)
    [Header("Goal - Kill")]
    public string targetName;       // 목표 대상 이름
    public int requiredKillCount;   // 목표 대상 처치 수
}
