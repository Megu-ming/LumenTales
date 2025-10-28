using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIRoot uiRoot;

    [Header("Canvas Prefabs")]
    [SerializeField] GameObject GameHUDPrefab;

    [Header("GameHUD UI Prefabs")]
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] GameObject healthTextPrefab;
    [SerializeField] float textHeight;

    [Header("Canvas Instances")]
    public Canvas mainMenuCanvas;
    public Canvas gameHUD;

    public void Init(Player player)
    {
        if (GameManager.instance.CurrentScene is MenuScene) return;
        if(player == null) return;

        // Prefab 생성
        if (gameHUD == null) Instantiate(GameHUDPrefab).TryGetComponent<Canvas>(out gameHUD);

        uiRoot.Init(player);
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

    public void CharacterTookDamage(GameObject character, float damageReceived)
    {
        Vector3 spawnPos = 
            Camera.main.WorldToScreenPoint(new Vector3(character.transform.position.x, character.transform.position.y + textHeight, character.transform.position.z));

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, gameHUD.transform).GetComponent<TMP_Text>();
        int damageInt = Mathf.RoundToInt(damageReceived);
        tmpText.text = damageInt.ToString();
    }

    public void CharacterHealed(GameObject character, float healReceived)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPos, Quaternion.identity, gameHUD.transform).GetComponent<TMP_Text>();
        int healInt = Mathf.RoundToInt(healReceived);
        tmpText.text = healInt.ToString();
    }

}
