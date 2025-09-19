using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager I { get; private set; }

    [SerializeField] GameObject cam;
    [SerializeField] GameObject interactNPC;
    [SerializeField] UIConversation conversationUI;

    private float camSize;
    private Vector3 camOffset;
    float t = 5f;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public bool ConversationStart()
    {
        bool result = false;
        cam.TryGetComponent<CinemachineCamera>(out CinemachineCamera cineCam);
        cam.TryGetComponent<CinemachinePositionComposer>(out CinemachinePositionComposer cinePC);

        if(cineCam && cinePC)
        {
            // 시네카메라 기존 값 캐싱
            camSize = cineCam.Lens.OrthographicSize;
            camOffset = cinePC.TargetOffset;
            // 카메라 줌인
            cineCam.Lens.OrthographicSize = Mathf.SmoothStep(camSize, 1.5f, t);
            cinePC.TargetOffset.y = Mathf.SmoothStep(camOffset.y, 0, t);

            result = true;
        }
        
        if(conversationUI && interactNPC)
        {
            // NPC의 대화 시작 메소드 호출
            // conversationUI 세팅
            conversationUI.SetName(interactNPC.name);
            conversationUI.gameObject.SetActive(true);

            result = true;
        }

        return result;
    }

    public bool ConversationEnd()
    {
        cam.TryGetComponent<CinemachineCamera>(out CinemachineCamera cineCam);
        cam.TryGetComponent<CinemachinePositionComposer>(out CinemachinePositionComposer cinePC);

        if (cineCam && cinePC)
        {
            // 카메라 줌아웃
            cineCam.Lens.OrthographicSize = camSize;
            cinePC.TargetOffset = camOffset;
        }

        if(conversationUI)
        {
            // NPC 대화종료 메소드 호출
            conversationUI.gameObject.SetActive(false);
        }

        return false;
    }

    public void SetInteractNPC(GameObject obj) => interactNPC = obj;
}
