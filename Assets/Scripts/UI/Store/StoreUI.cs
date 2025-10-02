using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StoreUI : UIBase
{
    StoreDataTable currentData;

    [SerializeField] StoreSlotUI[] storeSlots;
    private StoreSlotUI choicedSlot;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        if (UIManager.instance != null && UIManager.instance.interactPanel != null)
            UIManager.instance.interactPanel.SetActive(false);
        var playerInput = Player.instance.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
        UIManager.instance.inventoryUI.SetSortingOrder(111);
    }

    protected override void OnClose()
    {
        var playerInput = Player.instance.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Player");
        if (UIManager.instance != null && UIManager.instance.uiRoot != null)
            UIManager.instance.uiRoot.DetachInvenFromStore();
    }

    public void InitStore(StoreDataTable data)
    {
        currentData = data;

        for (int i = 0; i < storeSlots.Length; i++)
        {
            if (i < currentData.items.Length)
                storeSlots[i].SetItem(currentData.items[i]);
            else
                storeSlots[i].Clear();
        }
    }

    public void OpenModal() 
    {
        var modal = UIManager.instance.inputFieldModal;
        modal.Open(); 
    }

    public StoreSlotUI GetChoicedSlot() => choicedSlot;

    public void ChoiceSlot(StoreSlotUI slot)
    {
        if (choicedSlot is not null && choicedSlot != slot)
        {
            var modal = UIManager.instance.inputFieldModal;
            modal.HandleYesButton -= choicedSlot.TryBuyItem;
            choicedSlot.borderImage.color = Color.white;
        }
        choicedSlot = slot;
    }
}
