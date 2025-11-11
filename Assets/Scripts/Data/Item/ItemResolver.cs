public interface IItemResolver
{
    bool TryGetItemData(string itemName, out ItemData data);
}