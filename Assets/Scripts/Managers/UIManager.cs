using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Canvas gameCanvas;
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public float textHeight;

    [Header("Prefabs")]
    [SerializeField] GameObject UIRootPrefab;
    [SerializeField] GameObject pointerAreaPrefab;
    [SerializeField] GameObject interactPanelPrefab;
    [SerializeField] GameObject ingameMenuPrefab;
    [SerializeField] GameObject expSliderPrefab;

    // 인스턴스들
    public UIRoot uiRoot;
    public GameObject pointerArea;
    public GameObject interactPanel;
    public IngameMenu ingameMenu;
    public GameObject expSlider;

    [Header("StackUI")]
    public int baseSorting = 100;
    public int step = 10;
    readonly List<UIBase> UIStack = new();

    private void Awake()
    {
        // singleton
        if(instance && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitUI();
    }

    void InitUI()
    {
        // Prefab 생성
        if (uiRoot == null) Instantiate(UIRootPrefab).TryGetComponent<UIRoot>(out uiRoot);
        if (pointerArea == null) pointerArea = Instantiate(pointerAreaPrefab, uiRoot.transform);
        if (interactPanel == null) interactPanel = Instantiate(interactPanelPrefab, uiRoot.transform); 
        if (ingameMenu == null) Instantiate(ingameMenuPrefab, uiRoot.transform).TryGetComponent<IngameMenu>(out ingameMenu);
        if (expSlider == null) expSlider = Instantiate(expSliderPrefab, uiRoot.transform);
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

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();
        int damageInt = Mathf.RoundToInt(damageReceived);
        tmpText.text = damageInt.ToString();
    }

    public void CharacterHealed(GameObject character, float healReceived)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPos, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();
        int healInt = Mathf.RoundToInt(healReceived);
        tmpText.text = healInt.ToString();
    }

    // UIStackManager에서 하던거
    #region StackUI
    public void Push(UIBase ui)
    {
        int index = UIStack.IndexOf(ui);
        if (index >= 0) { BringToFront(ui); return; }
        UIStack.Add(ui);
        Reorder();
        ui.OnFocus();
    }

    public void Pop(UIBase ui)
    {
        int index = UIStack.IndexOf(ui);
        if (index < 0) return;
        UIStack.RemoveAt(index);
        Reorder();
    }

    public void BringToFront(UIBase ui)
    {
        int index = UIStack.IndexOf(ui);
        if (index < 0) return;
        UIStack.RemoveAt(index);
        UIStack.Add(ui);
        Reorder();
        ui.OnFocus();
    }

    public UIBase Top => UIStack.Count > 0 ? UIStack[^1] : null;

    public void CloseTopIfAllowed()
    {
        var top = Top;
        if (top != null && top.closeOnEsc) { top.Close(); return; }
        if (top == null) ToggleMenu();
    }

    void Reorder()
    {
        for (int i = 0; i < UIStack.Count; i++)
            UIStack[i].SetSortingOrder(baseSorting + (i + 1) * step);
    }

    private void ToggleMenu()
    {
        if (ingameMenu == null) return;
        if (ingameMenu.gameObject.activeSelf)
        {
            ingameMenu.Close();
        }
        else
        {
            ingameMenu.Open();
        }
    }
    #endregion
}
