using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDT", menuName = "Data/ItemDT")]
public class ItemDT : ScriptableObject
{
    [System.Serializable]
    public class Items
    {
        public ItemData item;
        public int weight;
    }

    public List<Items> items = new List<Items>();

    protected ItemData PickItem()
    {
        int sum = 0;
        foreach (var item in items)
        {
            sum += item.weight;
        }

        var rnd = Random.Range(0, sum);

        for(int i=0;i<items.Count; i++)
        {
            var item = items[i];
            if (item.weight > rnd) return items[i].item;
            else rnd -= item.weight;
        }

        return null;
    }

    protected ItemData[] PickItem(int val)
    {
        int sum = 0;
        foreach (var item in items)
        {
            sum += item.weight;
        }

        ItemData[] newItems = new ItemData[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            var rnd = Random.Range(0, sum);
            var item = items[i];
            if (item.weight > rnd) 
                newItems[i] = item.item;
            else 
            { 
                newItems[i] = null;
                sum -= item.weight;
            }
        }

        return newItems;
    }



    public void ItemDrop(Vector3 position)
    {
        var item = PickItem();
        if (item == null)
        {
            Debug.Log("Drop nothing");
            return;
        }
        
        Instantiate(item.prefab, position, Quaternion.identity).GetComponent<Item>().Init(item);
    }

    public void ItemDropAll(Vector3 position)
    {
        var items = PickItem(1);
        foreach (var item in items)
        {
            if(item == null) continue;

            Instantiate(item.prefab, position, Quaternion.identity).GetComponent<Item>().Init(item);
        }
    }
}
