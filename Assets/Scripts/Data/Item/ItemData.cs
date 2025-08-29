using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public Transform prefab;

    public int price;

    [Multiline]
    public string description;
}