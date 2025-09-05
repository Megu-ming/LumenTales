using UnityEngine;

public class ItemData : ScriptableObject
{
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public string Tooltip => tooltip;
    public Transform Prefab => prefab;

    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite icon;
    [SerializeField] private Transform prefab;
    [Multiline]
    [SerializeField] private string tooltip;

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