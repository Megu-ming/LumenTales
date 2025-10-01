using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Canvas Prefabs")]
    [SerializeField] GameObject UIRootPrefab;
    [SerializeField] GameObject GameHUDPrefab;

    [Header("UIRoot UI Prefabs")]
    [SerializeField] GameObject pointerAreaPrefab;
    [SerializeField] GameObject conversationBoxPrefab;
    [SerializeField] GameObject interactPanelPrefab;
    [SerializeField] GameObject ingameMenuPrefab;
    [SerializeField] GameObject expSliderPrefab;
    [SerializeField] GameObject tooltipPrefab;

    [Header("GameHUD UI Prefabs")]
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] GameObject healthTextPrefab;
    [SerializeField] float textHeight;


    [Header("Canvas Instance")]
    public UIRoot uiRoot;
    public Canvas gameHUD;

    public GameObject pointerArea;
    public UIConversation conversationUI;
    public GameObject interactPanel;
    public IngameMenu ingameMenu;
    public GameObject expSlider;

    [Header("StackUI")]
    public int baseSorting = 100;
    public int step = 10;
    readonly List<UIBase> UIStack = new();

    [Header("Tooltip")]
    UIItemTooltip tooltip;
    RectTransform tooltipRect;
    RectTransform canvasRect;

    // Events

    private void Awake()
    {
        // singleton
        if(instance && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseTopIfAllowed();
    }

    public void InitUI()
    {
        // Prefab 생성
        if (uiRoot == null) Instantiate(UIRootPrefab).TryGetComponent<UIRoot>(out uiRoot);
        if (gameHUD == null) Instantiate(GameHUDPrefab).TryGetComponent<Canvas>(out gameHUD);

        if (pointerArea == null) pointerArea = Instantiate(pointerAreaPrefab, uiRoot.transform);
        if (conversationUI == null)
        {
            Instantiate(conversationBoxPrefab, uiRoot.transform).TryGetComponent<UIConversation>(out conversationUI);
            conversationUI.gameObject.SetActive(false);
        }
        if (interactPanel == null)
        { 
            interactPanel = Instantiate(interactPanelPrefab, uiRoot.transform);
            interactPanel.gameObject.SetActive(false);
        }
        if (ingameMenu == null)
        {
            Instantiate(ingameMenuPrefab, uiRoot.transform).TryGetComponent<IngameMenu>(out ingameMenu);
            ingameMenu.gameObject.SetActive(false);
        }
        if (expSlider == null) expSlider = Instantiate(expSliderPrefab, uiRoot.transform);
        if (tooltip == null)
        {
            Instantiate(tooltipPrefab, uiRoot.transform).TryGetComponent<UIItemTooltip>(out tooltip);
            tooltipRect = tooltip.GetComponent<RectTransform>();
            canvasRect = uiRoot.GetComponent<RectTransform>();
        }
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

    // TooltipService에서 하던거
    #region Tooltip
    public void Show(string name, string desc, int price, Vector2 screenPos, int atk = 0, bool isAtk = false, bool isDef = false)
    {
        if (!tooltip) return;

        tooltip.SetupTooltip(name, desc, price, atk, isAtk, isDef);

        tooltip.transform.position = screenPos;

        float pivotX = tooltipRect.anchoredPosition.x + tooltipRect.sizeDelta.x > canvasRect.sizeDelta.x ? 1f : 0f; // anchor 11
        float pivotY = tooltipRect.anchoredPosition.y - tooltipRect.sizeDelta.y < -canvasRect.sizeDelta.y ? 0f : 1f; // anchor 00

        tooltipRect.pivot = new Vector2(pivotX, pivotY);

        tooltip.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (tooltip) tooltip.gameObject.SetActive(false);
    }
    #endregion

    // Event Functions
    public void OnExpChanged(float currentExp, float ManxExp)
    {
        expSlider.TryGetComponent<Slider>(out var slider);
        float expRatio = (float)currentExp / ManxExp;
        slider.value = expRatio;
    }
}
