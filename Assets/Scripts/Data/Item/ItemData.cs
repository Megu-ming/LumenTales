using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public Transform prefab;

    public int goldAmount;

    [Header("GoldPrice")]
    public int minGoldPrice;
    public int maxGoldPrice;

    [Multiline]
    public string description;
}