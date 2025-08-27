using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    // Pixels per second
    public Vector3 moveSpeed = new Vector3(0, 100, 0);
    public float timeToLive = 3f;

    RectTransform textTransform;
    TextMeshProUGUI textMesh;

    private float timeElapsed = 0f;
    private Color startColor;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMesh = GetComponent<TextMeshProUGUI>();
        startColor = textMesh.color;
    }

    private void Update()
    {
        textTransform.position += moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;

        if (timeElapsed<timeToLive)
        {
            float newAlpha = startColor.a * (1 - (timeElapsed / timeToLive));
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
