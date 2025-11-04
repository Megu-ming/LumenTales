using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StoreUI : UIBase
{
    StoreDataTable currentData;

    List<StoreSlotUI> storeSlots = new List<StoreSlotUI>();
    private StoreSlotUI choicedSlot;
    [Header("컴포넌트 참조")]
    [SerializeField] InputFieldModalUI inputFieldModal;
    [SerializeField] RectTransform slotParent;

    [Header("슬롯 프리팹")]
    [SerializeField] StoreSlotUI slotPrefab;

    Player player;
    bool isInit = false;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void Init(UIRoot uiRoot, Player player, InputFieldModalUI inputFieldModal, StoreDataTable data)
    {
        if(isInit is false)
        {
            this.uiRoot = uiRoot;
            this.player = player;
            this.inputFieldModal = inputFieldModal;
            currentData = data;

            if (currentData is not null)
            {
                int count = currentData.items.Length;
                for (int i = 0; i < count; i++)
                {
                    var slot = Instantiate(slotPrefab, slotParent.transform);
                    slot.Init(this, player.InventoryController);
                    slot.SetItem(data.items[i]);

                    storeSlots.Add(slot);
                }

                // slotParent Height 슬롯 높이만큼 더해줘야 드래그나 마우스 휠 돌렸을 때 상점 잘 작동함
                Vector2 size = slotParent.sizeDelta;
                
            }
            isInit = true;
        }
    }

    protected override void OnOpen()
    {
        if (uiRoot.interactPanel != null)
            uiRoot.interactPanel.SetActive(false);
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
        uiRoot.inventoryUI.SetSortingOrder(111);
    }

    protected override void OnClose()
    {
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Player");
        if (uiRoot != null)
            uiRoot.DetachInvenFromStore();
    }

    public void OpenModal() 
    {
        var modal = inputFieldModal;
        modal.owner = From.Store;
        modal.Open(); 
    }

    public StoreSlotUI GetChoicedSlot() => choicedSlot;

    public void ChoiceSlot(StoreSlotUI slot)
    {
        if (choicedSlot is not null && choicedSlot != slot)
        {
            var modal = inputFieldModal;
            modal.HandleYesButton -= choicedSlot.TryBuyItem;
            choicedSlot.borderImage.color = Color.white;
        }
        choicedSlot = slot;
    }
}
