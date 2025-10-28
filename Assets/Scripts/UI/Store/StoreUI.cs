using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StoreUI : UIBase
{
    StoreDataTable currentData;

    [SerializeField] StoreSlotUI[] storeSlots;
    private StoreSlotUI choicedSlot;
    [Header("컴포넌트 참조")]
    InputFieldModalUI inputFieldModal;

    Player player;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void Init(UIRoot uiRoot, Player player, InputFieldModalUI inputFieldModal, StoreDataTable data)
    {
        this.uiRoot = uiRoot;
        this.player = player;
        this.inputFieldModal = inputFieldModal;
        currentData = data;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        if(currentData is not null)
        {
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
