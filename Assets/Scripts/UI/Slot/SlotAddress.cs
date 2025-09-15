
public enum SlotOwner
{
    Inventory,
    Equipment
}

public struct SlotAddress
{
    public SlotOwner owner;
    public int invIndex;                  // Inventory 소유자일 때
    public EquipmentSlotType equipSlot;   // Equipment 소유자일 때

    public static SlotAddress Inv(int index) => new SlotAddress { owner = SlotOwner.Inventory, invIndex = index };
    public static SlotAddress Eq(EquipmentSlotType slot) => new SlotAddress { owner = SlotOwner.Equipment, equipSlot = slot };
}