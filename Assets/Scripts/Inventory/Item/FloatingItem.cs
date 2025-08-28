using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [SerializeField] float amplitude = 0.005f; // 위아래 진폭(미터) - PPU=32면 0.03 ≈ 1픽셀
    [SerializeField] float frequency = 0.2f;   // 초당 진동 수(Hz)
    [SerializeField] float phase = 1f;         // 개체마다 랜덤 위상 주면 자연스러움

    Vector3 baseLocalPos;

    void Awake()
    {
        baseLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        float y = Mathf.Sin((Time.time + phase) * Mathf.PI * 2f * frequency) * amplitude;
        transform.localPosition = new Vector3(baseLocalPos.x, baseLocalPos.y + y, baseLocalPos.z);
    }
}
