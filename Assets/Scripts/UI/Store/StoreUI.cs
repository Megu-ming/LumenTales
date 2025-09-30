using UnityEngine;

public class StoreUI : MonoBehaviour
{
    StoreDataTable currentData;

    [SerializeField] StoreSlotUI[] storeSlots;

    private void Awake()
    {
        gameObject.SetActive(false);
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
