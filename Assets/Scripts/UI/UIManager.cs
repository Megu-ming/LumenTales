using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;

    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas = FindAnyObjectByType<Canvas>();
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
    }

    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healReceived)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPos, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healReceived.ToString();
    }
}
