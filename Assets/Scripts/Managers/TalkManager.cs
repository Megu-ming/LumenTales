using UnityEngine;

public class TalkManager : MonoBehaviour
{
    string[] curTalkList;   // 현재 대화 스크립트
    int curTalkIndex = 0;   // 몇번째 대화를 하고 있는 지 저장

    UIConversation talkUI;      // 대화창 UI

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        talkUI.SetScript("");

        curTalkIndex = 0;
        curTalkList = null;
        if (Player.instance != null && Player.instance.PlayerController != null)
            Player.instance.PlayerController.isConversation = false;
        talkUI.gameObject.SetActive(false);
    }

    public void OpenTalkUI(string[] scripts)
    {
        curTalkList = scripts;
        NextTalk();
        if(Player.instance != null && Player.instance.PlayerController !=null)
            Player.instance.PlayerController.isConversation = true;
        talkUI.gameObject.SetActive(true);
    }

    public bool NextTalk()
    {
        if(curTalkIndex < curTalkList.Length)
        {
            talkUI.SetScript(curTalkList[curTalkIndex]);
            curTalkIndex++;
            return true;
        }
        else
        {
            Initialize();
            return false;
        }
    }
}
