using UnityEngine;

public class ItemData : ScriptableObject
{
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public string Tooltip => tooltip;
    public int Price => price;
    public Transform Prefab => prefab;

    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Transform prefab;
    [Multiline]
    [SerializeField] private string tooltip;
    [SerializeField, Min(0)] private int price = 0;

    public virtual bool IsStackable => false;
    public virtual Item CreateItem(int amount = 1)
    {
        return new SimpleItem(this);
    }
}

public class SimpleItem : Item
{
    public SimpleItem(ItemData itemData) : base(itemData) { }
}