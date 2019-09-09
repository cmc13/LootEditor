using System;
using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    [Flags]
    public enum ArmorSlot
    {
        Head = 1,
        [Description("Underwear Chest")] UnderChest = 2,
        [Description("Underwear Abdomen")] UnderAbdomen = 4,
        [Description("Underwear Upper Arms")] UnderUpperArms = 8,
        [Description("Underwear Lower Arms")] UnderLowerArms = 16,
        Hands = 32,
        [Description("Underwear Upper Legs")] UnderUpperLegs = 64,
        [Description("Underwear Lower Legs")] UnderLowerLegs = 128,
        Feet = 256,
        Chest = 512,
        Girth = 1024,
        [Description("Upper Arms")] UpperArms = 2048,
        [Description("Lower Arms")] LowerArms = 4096,
        [Description("Upper Legs")] UpperLegs = 8192,
        [Description("Lower Legs")] LowerLegs = 16384
    }
}
