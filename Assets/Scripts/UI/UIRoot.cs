using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIRoot : MonoBehaviour
{
    [Header("UIRoot UI Prefabs")]
    [SerializeField] GameObject conversationBoxPrefab;
    [SerializeField] GameObject interactPanelPrefab;
    [SerializeField] GameObject ingameMenuPrefab;
    [SerializeField] GameObject expSliderPrefab;
    [SerializeField] GameObject tooltipPrefab;
    [SerializeField] GameObject storePrefab;
    [SerializeField] GameObject inventoryPrefab;
    [SerializeField] GameObject characterInfoPrefab;
    [SerializeField] GameObject dummyPrefab;

    [Header("Modal UI Prefabs")]
    [SerializeField] GameObject inputFieldModalPrefab;

    [Header("Revive UI Prefab")]
    [SerializeField] GameObject reviveUIPrefab;

    [Header("UI Instances(Read Only)")]
    public UIConversation conversationUI;
    public GameObject interactPanel;
    public IngameMenu ingameMenu;
    public GameObject expSlider;
    public StoreUI storeUI;
    public UIInventory inventoryUI;
    public UIEquipment characterInfoUI;
    public InputFieldModalUI inputFieldModal;
    public GameObject dummy;
    public ReviveUI reviveUI;

    [Header("Tooltip")]
    UIItemTooltip tooltip;
    RectTransform tooltipRect;
    RectTransform canvasRect;

    [SerializeField] RectTransform storeParentRt;
    [SerializeField] RectTransform inventoryRT;

    // 캐싱용 
    Transform invenOrigParent;
    int invenOrigSiblingIndex;
    bool invenDocked;
    Player player;

    [Header("StackUI")]
    public int baseSorting = 100;
    public int step = 10;
    readonly List<UIBase> UIStack = new();

    public void Init(Player player)
    {
        this.player = player;

        if (conversationUI == null)
        {
            Instantiate(conversationBoxPrefab, transform).TryGetComponent<UIConversation>(out conversationUI);
            conversationUI.gameObject.SetActive(false);
        }
        if (interactPanel == null)
        {
            interactPanel = Instantiate(interactPanelPrefab, transform);
            interactPanel.gameObject.SetActive(false);
        }
        if (ingameMenu == null)
        {
            Instantiate(ingameMenuPrefab, transform).TryGetComponent<IngameMenu>(out ingameMenu);
            ingameMenu.Init(this, player);
            ingameMenu.gameObject.SetActive(false);
        }
        if (expSlider == null) expSlider = Instantiate(expSliderPrefab, transform);
        if (tooltip == null)
        {
            Instantiate(tooltipPrefab, transform).TryGetComponent<UIItemTooltip>(out tooltip);
            tooltipRect = tooltip.GetComponent<RectTransform>();
            canvasRect = GetComponent<RectTransform>();
            tooltip.gameObject.SetActive(false);
        }
        if (inputFieldModal == null)
        {
            Instantiate(inputFieldModalPrefab, transform).TryGetComponent<InputFieldModalUI>(out inputFieldModal);
            inputFieldModal.Init(this);
        }
        if (storeUI == null)
        {
            Instantiate(storePrefab, transform).TryGetComponent<StoreUI>(out storeUI);
            storeUI.Init(this, player, inputFieldModal, null);
            storeUI.gameObject.SetActive(false);
        }
        if (inventoryUI == null)
        {
            Instantiate(inventoryPrefab, transform).TryGetComponent<UIInventory>(out inventoryUI);
            inventoryUI.Init(player.InventoryController, this);
        }
        if (characterInfoUI == null)
        {
            Instantiate(characterInfoPrefab, transform).TryGetComponent<UIEquipment>(out characterInfoUI);
            characterInfoUI.Init(this, player);
        }
        if (dummy == null)
        {
            dummy = Instantiate(dummyPrefab, transform);
        }

        player.Status.HandleExpChanged += OnExpChanged;
        player.Status.HandleOpenDeadUI += OpenDeadUI;
    }

    private void Update()
    {
        // ESC키 반응용
        if (Input.GetKeyDown(KeyCode.Escape)) CloseTopIfAllowed();
    }

    /// <summary>
    /// 상점 열릴 때 인벤토리랑 상점 UI 붙여주는 함수
    /// </summary>
    /// <param name="data">상점에서 판매하는 아이템 데이터</param>
    public void AttachInvenToStore(StoreDataTable data)
    {
        if (invenDocked) return;
        if (inventoryRT == null)
            inventoryUI.TryGetComponent<RectTransform>(out inventoryRT);
        if(storeParentRt == null)
            storeUI.TryGetComponent<RectTransform>(out storeParentRt);

        // 캐시(원상복구용)
        invenOrigParent = inventoryRT.parent;
        invenOrigSiblingIndex = inventoryRT.GetSiblingIndex();

        // 인벤토리를 스토어UI에 붙이기
        inventoryRT.SetParent(storeParentRt);

        invenDocked = true;

        // 상점 아이템 데이터 전달
        storeParentRt.TryGetComponent<StoreUI>(out var uiStore);
        if (uiStore != null) uiStore.Init(this, player, inputFieldModal, data);

        // 상점 UI 활성화
        inventoryRT.TryGetComponent<UIInventory>(out var uiInven);
        
        // 뭔가 열려 있었다면
        if(Top!=null)
        {
            CloseTopIfAllowed();
        }

        uiStore.Open();
        uiInven.gameObject.SetActive(true);
        if(player.InventoryController is not null) player.InventoryController.RefreshAllSlots();
    }

    /// <summary>
    /// 상점창 꺼질 때 호출되는 함수(인벤토리 탈착 함수)
    /// </summary>
    public void DetachInvenFromStore()
    {
        if(!invenDocked || inventoryRT == null) return;

        // 인벤토리 부모 변경
        inventoryRT.SetParent(invenOrigParent);
        inventoryRT.SetSiblingIndex(invenOrigSiblingIndex);
        invenDocked = false;

        inventoryRT.TryGetComponent<UIInventory>(out var uiInven);
        uiInven.gameObject.SetActive(false);        
    }

    /// <summary>
    /// 사망했을 때 UI띄워주는 함수
    /// </summary>
    public void OpenDeadUI()
    {
        if(Instantiate(reviveUIPrefab).TryGetComponent<ReviveUI>(out reviveUI))
            reviveUI.Init(player);
    }

    // StackManager에서 하던거
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
        if (top == null) ToggleInGameMenu();
    }

    void Reorder()
    {
        for (int i = 0; i < UIStack.Count; i++)
            UIStack[i].SetSortingOrder(baseSorting + (i + 1) * step);
    }
    #endregion

    // TooltipService에서 하던거
    #region Tooltip
    public void Show(string name, string desc, int price, Vector2 screenPos, int atk = 0, bool isAtk = false, bool isDef = false)
    {
        if (!tooltip) return;

        tooltip.SetupTooltip(name, desc, price, atk, isAtk, isDef);

        tooltip.transform.position = screenPos + new Vector2(70, -70);

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

    public void ToggleInGameMenu()
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

    public void OnExpChanged(float currentExp, float ManxExp)
    {
        expSlider.TryGetComponent<UnityEngine.UI.Slider>(out var slider);
        float expRatio = (float)currentExp / ManxExp;
        slider.value = expRatio;
    }
}