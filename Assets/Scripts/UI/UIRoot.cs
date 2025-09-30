using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRoot : MonoBehaviour
{
    public static UIRoot instance;

    public IngameMenu ingameMenu;
    [SerializeField] RectTransform storeParentRt;
    [SerializeField] RectTransform inventoryRT;

    // 캐싱용 
    Transform invenOrigParent;
    int invenOrigSiblingIndex;
    bool invenDocked;

    private void Awake()
    {
        InitSingleton();

        if(ingameMenu == null)
            ingameMenu = GetComponentInChildren<IngameMenu>();
    }

    void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void AttachInvenToStore(StoreDataTable data)
    {
       if(invenDocked || inventoryRT == null || storeParentRt == null) return;

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
        storeParentRt.gameObject.SetActive(true);
        inventoryRT.gameObject.SetActive(true);
    }

    public void DetachInvenFromStore()
    {
        if(!invenDocked || inventoryRT == null) return;

        // 인벤토리 부모 변경
        inventoryRT.SetParent(invenOrigParent);
        inventoryRT.SetSiblingIndex(invenOrigSiblingIndex);
        invenDocked = false;

        // 상점 UI 비활성화
        storeParentRt.gameObject.SetActive(false);
        inventoryRT.gameObject.SetActive(false);
    }
}
