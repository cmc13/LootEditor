using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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

    public static class SalvageGroupExtensions
    {
        public static IEnumerable<Material> GetMaterials(this SalvageGroup salvageGroup)
        {
            return Enum.GetValues(typeof(Material)).Cast<Material>()
                .Where(m => typeof(Material).GetMember(m.ToString()).Single()
                .GetCustomAttributes<SalvageGroupAttribute>().Any(g => g.SalvageGroup == salvageGroup));
        }
    }
}
