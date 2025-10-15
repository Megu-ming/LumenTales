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

    public void AttachInvenToStore(StoreDataTable data)
    {
        if (invenDocked) return;
        if (inventoryRT == null)
            UIManager.instance.inventoryUI.TryGetComponent<RectTransform>(out inventoryRT);
        if(storeParentRt == null)
            UIManager.instance.storeUI.TryGetComponent<RectTransform>(out storeParentRt);

        // 캐시(원상복구용)
        invenOrigParent = inventoryRT.parent;
        invenOrigSiblingIndex = inventoryRT.GetSiblingIndex();

        // 인벤토리를 스토어UI에 붙이기
        inventoryRT.SetParent(storeParentRt);

        invenDocked = true;

        // 상점 아이템 데이터 전달
        storeParentRt.TryGetComponent<StoreUI>(out var uiStore);
        if(uiStore != null) uiStore.InitStore(data);

        // 상점 UI 활성화
        inventoryRT.TryGetComponent<UIInventory>(out var uiInven);
        
        // 뭔가 열려 있었다면
        if(UIManager.instance!=null && UIManager.instance.Top!=null)
        {
            UIManager.instance.CloseTopIfAllowed();
        }

        uiStore.Open();
        uiInven.gameObject.SetActive(true);
        if(Player.instance?.InventoryController is not null) Player.instance?.InventoryController.RefreshAllSlots();
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
