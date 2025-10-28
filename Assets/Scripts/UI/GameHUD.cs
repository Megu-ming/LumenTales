using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameHUD : MonoBehaviour
{
    [Header("GameHUD UI Prefabs")]
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] GameObject healthTextPrefab;
    [SerializeField] float textHeight;

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

    public void CharacterTookDamage(GameObject character, float damageReceived)
    {
        Vector3 spawnPos =
            Camera.main.WorldToScreenPoint(new Vector3(character.transform.position.x, character.transform.position.y + textHeight, character.transform.position.z));

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, transform).GetComponent<TMP_Text>();
        int damageInt = Mathf.RoundToInt(damageReceived);
        tmpText.text = damageInt.ToString();
    }

    public void CharacterHealed(GameObject character, float healReceived)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPos, Quaternion.identity, transform).GetComponent<TMP_Text>();
        int healInt = Mathf.RoundToInt(healReceived);
        tmpText.text = healInt.ToString();
    }
}
