using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StoreUI : UIBase
{
    StoreDataTable currentData;

    [SerializeField] StoreSlotUI[] storeSlots;
    private StoreSlotUI choicedSlot;

    Player player;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void Init(UIManager uiManager, Player player, StoreDataTable data)
    {
        this.uiManager = uiManager;
        this.player = player;

        currentData = data;

        for (int i = 0; i < storeSlots.Length; i++)
        {
            if (i < currentData.items.Length)
            {
                storeSlots[i].Init(player.InventoryController);
                storeSlots[i].SetItem(currentData.items[i]); 
            }
            else
                storeSlots[i].Clear();
        }
    }

    protected override void OnOpen()
    {
        if (uiManager.interactPanel != null)
            uiManager.interactPanel.SetActive(false);
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
        uiManager.inventoryUI.SetSortingOrder(111);
    }

    protected override void OnClose()
    {
        var playerInput = player.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Player");
        if (uiManager.uiRoot != null)
            uiManager.uiRoot.DetachInvenFromStore();
    }

    public void OpenModal() 
    {
        var modal = uiManager.inputFieldModal;
        modal.owner = From.Store;
        modal.Open(); 
    }

    public StoreSlotUI GetChoicedSlot() => choicedSlot;

    public void ChoiceSlot(StoreSlotUI slot)
    {
        if (choicedSlot is not null && choicedSlot != slot)
        {
            var modal = uiManager.inputFieldModal;
            modal.HandleYesButton -= choicedSlot.TryBuyItem;
            choicedSlot.borderImage.color = Color.white;
        }
        choicedSlot = slot;
    }
}
