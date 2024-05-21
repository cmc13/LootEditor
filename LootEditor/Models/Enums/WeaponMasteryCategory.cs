using System.ComponentModel;

namespace LootEditor.Models.Enums;

[TypeConverter(typeof(EnumDescriptionConverter))]
public enum WeaponMasteryCategory
{
    [Description("Unarmed Weapon")] UnarmedWeapon = 1,
    [Description("Sword")] Sword = 2,
    [Description("Axe")] Axe = 3,
    [Description("Mace")] Mace = 4,
    [Description("Spear")] Spear = 5,
    [Description("Dagger")] Dagger = 6,
    [Description("Staff")] Staff = 7,
    [Description("Bow")] Bow = 8,
    [Description("Crossbow")] Crossbow = 9,
    [Description("Thrown")] Thrown = 10,
    [Description("Two Handed Combat")] TwoHandedCombat = 11,
}
