using UnityEngine;

public class Store : InteractiveObj
{
    [SerializeField] StoreDataTable storeDatatable; // 판매하는 아이템 데이터 테이블
    UIRoot uiRoot;

    public void Init(UIRoot uiRoot)
    {
        this.uiRoot = uiRoot;
    }

    // 상호작용 키 입력 시에 호출되는 상점 UI Open 함수
    public override void OnInteraction()
    {
        if(uiRoot != null)
            uiRoot.AttachInvenToStore(storeDatatable);
    }
}
