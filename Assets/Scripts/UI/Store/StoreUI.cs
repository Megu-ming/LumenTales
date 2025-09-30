using UnityEngine;
using UnityEngine.InputSystem;

public class StoreUI : UIBase
{
    StoreDataTable currentData;

    [SerializeField] StoreSlotUI[] storeSlots;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        if (UIStackManager.Instance != null && UIStackManager.Instance.interactPanel != null)
            UIStackManager.Instance.interactPanel.SetActive(false);
        var playerInput = Player.instance.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
    }

    protected override void OnClose()
    {
        var playerInput = Player.instance.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Player");
        UIRoot.instance.DetachInvenFromStore();
    }

    public void InitStore(StoreDataTable data)
    {
        currentData = data;

        for(int i=0;i<storeSlots.Length; i++)
        {
            if (i < currentData.items.Length)
                storeSlots[i].SetItem(currentData.items[i]);
            else
                storeSlots[i].Clear();
        }
    }
}
