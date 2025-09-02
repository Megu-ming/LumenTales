using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropHelper : MonoBehaviour
{
    [Header("Data Table")]
    [SerializeField] private ItemDT dataTable;

    [Header("Scatter Pattern")]
    [SerializeField, Tooltip("부채꼴 반각(도). 35면 좌: 125°, 우: 55° 근처")]
    private float fanHalfAngle = 35f;
    [SerializeField, Tooltip("발사 기본 속도")]
    private float baseSpeed = 4.5f;
    [SerializeField, Tooltip("개별 생성 간 지연(연출용). 0이면 같은 프레임 생성")]
    private float staggerInterval = 0.06f;

    [Header("Physics Defaults")]
    [SerializeField] private float gravityScale = 2.8f;
    [SerializeField] private PhysicsMaterial2D itemMaterial;

    public void DropItem(Vector3 position, System.Action onComplete = null)
    {
        var items = SelectDropItem();
        if (items == null || items.Count == 0)
        {
            onComplete?.Invoke();
            return; 
        }

        StartCoroutine(DropScatterRoutine(position, items, onComplete));
    }

    private List<ItemData> SelectDropItem()
    {
        return dataTable ? dataTable.PickDropsIndependent() : new List<ItemData>();
    }

    private IEnumerator DropScatterRoutine(Vector3 origin, List<ItemData> items, System.Action onComplete = null)
    {
        int count = items.Count;
        Debug.Log($"[Drop] count={count}");

        for (int i = 0; i < count; i++)
        {
            var data = items[i];
            if (data == null || data.prefab == null) continue;

            // 각도 계산
            float angleDeg = GetAngleForIndex(i, count);

            // 초기 속도
            float spd = baseSpeed;
            Vector2 v0 = AngleToVelocity(angleDeg, spd);

            Debug.Log($"[Drop] i={i}/{count}, angle={angleDeg:F1}°, v0={v0}");

            // 3) 생성 + 초기화
            var go = Instantiate(data.prefab, origin, Quaternion.identity);
            go.GetComponent<Item>()?.Init(data);

            // 4) 물리 세팅(있을 때만)
            if (go.TryGetComponent(out Rigidbody2D rb))
            {
                rb.gravityScale = gravityScale;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                rb.linearVelocity = v0;
            }
            if (itemMaterial && go.TryGetComponent(out Collider2D col))
            {
                col.sharedMaterial = itemMaterial;
            }

            // 5) 연출용 지연(겹쳐 보이는 것 방지 + 타닥 느낌)
            if (staggerInterval > 0f) yield return new WaitForSeconds(staggerInterval);
            else yield return null;
        }

        onComplete?.Invoke();
    }

    // 규칙:
    // n=1 → 90°(수직)
    // n=2 → [왼쪽: 90+fan, 오른쪽: 90-fan] (왼쪽부터)
    // n>=3 → 90+fan .. 90-fan 사이를 왼→오 균등 분배
    private float GetAngleForIndex(int index, int count)
    {
        if (count <= 1) return 90f;

        float leftDeg = 90f + fanHalfAngle;
        float rightDeg = 90f - fanHalfAngle;

        if (count == 2)
            return (index == 0) ? leftDeg : rightDeg;

        float t = (float)index / (count - 1); // 0→left, 1→right
        return Mathf.Lerp(leftDeg, rightDeg, t);
    }

    private Vector2 AngleToVelocity(float angleDeg, float speed)
    {
        float r = angleDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(r) * speed, Mathf.Sin(r) * speed);
    }
}
