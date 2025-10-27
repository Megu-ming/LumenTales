using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRoot : MonoBehaviour
{
    [SerializeField] RectTransform storeParentRt;
    [SerializeField] RectTransform inventoryRT;

    // 캐싱용 
    Transform invenOrigParent;
    int invenOrigSiblingIndex;
    bool invenDocked;
    UIManager uiManager;
    Player player;

    public void Init(UIManager uiManager, Player player)
    {
        this.uiManager = uiManager;
        this.player = player;
    }

    public void AttachInvenToStore(StoreDataTable data)
    {
        if (invenDocked) return;
        if (inventoryRT == null)
            uiManager.inventoryUI.TryGetComponent<RectTransform>(out inventoryRT);
        if(storeParentRt == null)
            uiManager.storeUI.TryGetComponent<RectTransform>(out storeParentRt);

        // 캐시(원상복구용)
        invenOrigParent = inventoryRT.parent;
        invenOrigSiblingIndex = inventoryRT.GetSiblingIndex();

        // 인벤토리를 스토어UI에 붙이기
        inventoryRT.SetParent(storeParentRt);

        invenDocked = true;

        // 상점 아이템 데이터 전달
        storeParentRt.TryGetComponent<StoreUI>(out var uiStore);
        if(uiStore != null) uiStore.Init(uiManager, player, data);

        // 상점 UI 활성화
        inventoryRT.TryGetComponent<UIInventory>(out var uiInven);
        
        // 뭔가 열려 있었다면
        if(uiManager.Top!=null)
        {
            uiManager.CloseTopIfAllowed();
        }

        uiStore.Open();
        uiInven.gameObject.SetActive(true);
        if(player.InventoryController is not null) player.InventoryController.RefreshAllSlots();
    }

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
}
