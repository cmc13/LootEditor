using System;
using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    [Flags]
    public enum EquippableSlot
    {
        Head = 1 << 0,
        [Description("Underwear Chest")] UnderChest = 1 << 1,
        [Description("Underwear Abdomen")] UnderAbdomen = 1 << 2,
        [Description("Underwear Upper Arms")] UnderUpperArms = 1 << 3,
        [Description("Underwear Lower Arms")] UnderLowerArms = 1 << 4,
        Hands = 1 << 5,
        [Description("Underwear Upper Legs")] UnderUpperLegs = 1 << 6,
        [Description("Underwear Lower Legs")] UnderLowerLegs = 1 << 7,
        Feet = 1 << 8,
        Chest = 1 << 9,
        Girth = 1 << 10,
        [Description("Upper Arms")] UpperArms = 1 << 11,
        [Description("Lower Arms")] LowerArms = 1 << 12,
        [Description("Upper Legs")] UpperLegs = 1 << 13,
        [Description("Lower Legs")] LowerLegs = 1 << 14,
        Necklace = 1 << 15,
        Bracelet = 196608,
        Ring = 786432,
        [Description("Melee Weapon")] MeleeWeapon = 1 << 20,
        Shield = 1 << 21,
        [Description("Missile Weapon")] MissileWeapon = 1 << 22,
        [Description("Magic Caster")] MagicCaster = 1 << 24,
        [Description("Two-Handed Weapon")]TwoHandedWeapon = 1 << 25,
        Trinket = 1 << 26,
        Cloak = 1 << 27,
        [Description("Blue Aetheria")] BlueAetheria = 1 << 28,
        [Description("Yellow Aetheria")] YellowAetheria = 1 << 29,
        [Description("Red Aetheria")] RedAetheria = 1 << 30
    }
}
