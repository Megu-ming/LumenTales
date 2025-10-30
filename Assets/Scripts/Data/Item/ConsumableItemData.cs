using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
public class ConsumableItemData : CountableItemData
{
    [Header("Effect")]
    public float strength;
    public float agility;
    public float currentHealth;
    public float maxHealth;
    [TextArea] public string effectDesc = "Head 20 HP";

    public virtual void ApplyEffect(Player owner)
    {
        owner.Status.AddCurrentHealth(currentHealth);
        owner.Status.SpAddedStr += strength;
        owner.Status.SpAddedAgi += agility;
        owner.Status.BaseMaxHealth += maxHealth;
    }

    public override Item CreateItem(int amount =1)
    {
        return new ConsumableItem(this, amount);
    }
}