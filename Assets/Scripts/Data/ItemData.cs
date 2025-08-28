using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type; // Weapon, Armor, Consumable, Gold
    public int power;
    public string description;
}