public enum WalkableDirection { Right, Left };

public enum QuestGoalType
{
    Kill,
    Gather,
    Talk
}

public enum EnemyType
{
    None,
    Object,
    Normal,
    Boss
}

public enum EquipmentSlotType
{
    Weapon,
    Head,
    Chest,
    Legs,
    Gloves,
    Boots,
}

public enum SceneType
{
    Menu,
    Town,
    Battle,
    Boss
}

public enum From
{
    Store,
    Inventory,
}
// -----------Talk-----------------
public enum NPCType
{
    Default = 0,
    TestNPC = 100,  
}

public enum TalkState
{
    Default = 0,
    Talk = 1,       
    Hint = 2,
    Quest = 3,      
}
// ---------------------------------

public enum QuestType
{
    Kill,       // 몬스터 처치
    Collect,    // 아이템 수집
}

public enum QuestState
{
    NotStarted,         // 퀘스트 미시작
    InProgress,         // 퀘스트 진행 중
    ReadyToComplete,    // 퀘스트 완료 가능
    Completed,          // 퀘스트 완료
}