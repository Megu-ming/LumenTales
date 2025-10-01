using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] StoreDataTable storeDatatable; // 판매하는 아이템 데이터 테이블

    // 상호작용 키 입력 시에 호출되는 상점 UI Open 함수
    public void OpenStoreUI()
    {
        if(UIManager.instance != null&&UIManager.instance.uiRoot != null)
            UIManager.instance.uiRoot.AttachInvenToStore(storeDatatable);
    }
}
