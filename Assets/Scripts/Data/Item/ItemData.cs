using UnityEngine;

public abstract class ItemData : ScriptableObject
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

    public abstract Item CreateItem();
}