using UnityEngine;

public class Store : InteractiveObj
{
    [SerializeField] StoreDataTable storeDatatable; // 판매하는 아이템 데이터 테이블
    UIManager uiManager;

    public void Init(UIManager uiManager)
    {
        this.uiManager = uiManager;
    }

    // 상호작용 키 입력 시에 호출되는 상점 UI Open 함수
    public override void OnInteraction()
    {
        if(uiManager.uiRoot != null)
            uiManager.uiRoot.AttachInvenToStore(storeDatatable);
    }
}
