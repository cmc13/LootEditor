using System;
using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    [Flags]
    public enum ArmorSlot
    {
        Head = 1,
        UnderChest = 2,
        UnderAbdomen = 4,
        UnderUpperArms = 8,
        UnderLowerArms = 16,
        Hands = 32,
        UnderUpperLegs = 64,
        UnderLowerLegs = 128,
        Feet = 256,
        Chest = 512,
        Girth = 1024,
        UpperArms = 2048,
        LowerArms = 4096,
        UpperLegs = 8192,
        LowerLegs = 16384
    }
}
