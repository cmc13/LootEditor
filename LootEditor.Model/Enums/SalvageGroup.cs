using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum SalvageGroup
    {
        [Description("Armor Imbues")] ArmorImbues,
        [Description("Armor Tinkering")] ArmorTinkering,
        [Description("Basic Tinkering")] BasicTinkering,
        [Description("Brass/Granite/Iron/Mahogany")] BrassGraniteIronMahogany,
        Gearcrafting,
        [Description("Item Tinkering")] ItemTinkering,
        [Description("Magic Item Tinkering")] MagicItemTinkering,
        [Description("Protection Tinks")] ProtectionTinks,
        [Description("RG/BG/Jet")] RGBGJet,
        [Description("Weapon Imbues")] WeaponImbues,
        [Description("Weapon Tinkering")] WeaponTinkering
    }
}
