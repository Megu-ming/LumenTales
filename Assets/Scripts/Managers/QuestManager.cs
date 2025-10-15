using System.Collections.Generic;

[System.Serializable]
public class QuestManager
{
    public Dictionary<int, Quest> questContainer = new Dictionary<int, Quest>();                      // 진행중인 퀘스트
    public List<int> completedQuestContainer = new List<int>();     // 완료한 퀘스트 (퀘스트 ID)

    public void NotifyEnemyKilled(string enemyName)
    {
        foreach (var quest in questContainer)
        {
            if (quest.Value.currentQuestState != QuestState.InProgress) continue;
            if (quest.Value.questData.questType != QuestType.Kill) continue;
            //if (quest.Value.questData.targetName != enemyName) continue;

            quest.Value.currentCount++;
            quest.Value.Evaluate();
        }
    }

    public void NotifyInventoryChanged(ItemData item)
    {
        foreach (var quest in questContainer)
        {
            if (quest.Value.currentQuestState != QuestState.InProgress) continue;
            if (quest.Value.questData.questType != QuestType.Collect) continue;
            //if (quest.Value.questData.targetItem != item) continue;
            quest.Value.Evaluate();
        }
    }

    public bool TryCompleteQuest(int target)
    {
        if(!questContainer.ContainsKey(target)) return false;
        if (questContainer[target] is null) return false;

        if (questContainer[target].currentQuestState != QuestState.ReadyToComplete) return false;

        questContainer[target].Complete();
        if (questContainer[target].currentQuestState == QuestState.Completed)
        {
            questContainer.Remove(target);
            completedQuestContainer.Add(target);
            return true;
        }

        return false;
    }

    public void ApplyQuest(QuestSnapshot qs)
    {
        if (qs == null) return;
        foreach(var q in qs.inProgressQuests)
        {
            if(q.questData && questContainer.ContainsKey(q.questData.questID)) continue;
            questContainer.Add(q.questData.questID, q);
        }

        completedQuestContainer = qs.completedQuests;

    }

    public QuestSnapshot GetQuestSnapshot()
    {
        var snapshot = new QuestSnapshot();
        foreach(var q in questContainer)
        {
            snapshot.inProgressQuests.Add(q.Value);
        }

        snapshot.completedQuests = new List<int>(completedQuestContainer);


        return snapshot;
    }
}
