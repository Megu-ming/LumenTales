using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestData questData;
    public QuestState currentQuestState;

    public int currentCount;

    public void Evaluate()
    {
        //if (currentQuestState != QuestState.InProgress) return;

        //switch(questData.questType)
        //{
        //    case QuestType.Kill:
        //        if (currentCount >= questData.requiredKillCount && questData.requiredKillCount > 0)
        //            currentQuestState = QuestState.ReadyToComplete;

        //        break;
        //    case QuestType.Collect:
        //        int have = Player.instance.InventoryController.GetItemCount(questData.targetItem);
        //        if (have >= questData.requiredItemCount && questData.requiredItemCount > 0)
        //            currentQuestState = QuestState.ReadyToComplete;
        //        break;
        //}
    }

    public void Complete()
    {
        if (currentQuestState != QuestState.ReadyToComplete) return;

        // 보상 지급
        if(questData.reward.gold > 0) Player.instance.InventoryController.AddGold(questData.reward.gold);
        if (questData.reward.exp > 0) GameManager.instance.CurrentScene.AddExp(questData.reward.exp);
        if(questData.reward.itemData != null && questData.reward.itemCount > 0)
            Player.instance.InventoryController.Add(questData.reward.itemData, questData.reward.itemCount);

        currentQuestState = QuestState.Completed;
    }
}
