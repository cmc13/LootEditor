using System;
using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [Flags]
    public enum Coverage
    {
        Head = 16384,
        [Description("Underwear Chest")] UnderChest = 8,
        [Description("Underwear Abdomen")] UnderAbdomen = 2,
        [Description("Underwear Upper Arms")] UnderUpperArms = 32,
        [Description("Underwear Lower Arms")] UnderLowerArms = 64,
        Hands = 32768,
        [Description("Underwear Upper Legs")] UnderUpperLegs = 16,
        [Description("Underwear Lower Legs")] UnderLowerLegs = 4,
        Feet = 65536,
        Chest = 1024,
        Girth = 2048,
        [Description("Upper Arms")] UpperArms = 4096,
        [Description("Lower Arms")] LowerArms = 8192,
        [Description("Upper Legs")] UpperLegs = 256,
        [Description("Lower Legs")] LowerLegs = 512
    }
}
