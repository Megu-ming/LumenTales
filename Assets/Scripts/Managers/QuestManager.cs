using System.Collections.Generic;

[System.Serializable]
public class QuestManager
{
    public List<Quest> questContainer = new List<Quest>();                      // 진행중인 퀘스트
    public List<QuestData> completedQuestContainer = new List<QuestData>();     // 완료한 퀘스트


    public void NotifyEnemyKilled(string enemyName)
    {
        foreach (var quest in questContainer)
        {
            if (quest.currentQuestState != QuestState.InProgress) continue;
            if (quest.questData.questType != QuestType.Kill) continue;
            if (quest.questData.targetName != enemyName) continue;

            quest.currentCount++;
            quest.Evaluate();
        }
    }

    public void NotifyInventoryChanged(ItemData item)
    {
        foreach (var quest in questContainer)
        {
            if (quest.currentQuestState != QuestState.InProgress) continue;
            if (quest.questData.questType != QuestType.Collect) continue;
            if (quest.questData.targetItem != item) continue;
            quest.Evaluate();
        }
    }

    public bool TryCompleteQuest(Quest target)
    {
        if (target is null) return false;
        if (!questContainer.Contains(target)) return false;

        if (target.currentQuestState != QuestState.ReadyToComplete) return false;

        target.Complete();
        if (target.currentQuestState == QuestState.Completed)
        {
            questContainer.Remove(target);
            completedQuestContainer.Add(target.questData);
            return true;
        }

        return false;
    }

    public void ApplyQuest(QuestSnapshot qs)
    {
        if (qs == null) return;
        questContainer = new List<Quest>(qs.inProgressQuests);
        completedQuestContainer = new List<QuestData>(qs.completedQuests);
    }

    public QuestSnapshot GetQuestSnapshot()
    {
        var snapshot = new QuestSnapshot();
        snapshot.inProgressQuests = new List<Quest>(questContainer);
        snapshot.completedQuests = new List<QuestData>(completedQuestContainer);

        return snapshot;
    }
}
