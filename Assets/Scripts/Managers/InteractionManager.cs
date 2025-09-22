using Unity.Cinemachine;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager I { get; private set; }

    [SerializeField] GameObject cam;

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

        return false;
    }
}
