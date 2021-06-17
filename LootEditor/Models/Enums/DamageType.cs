using System.ComponentModel;

namespace LootEditor.Models.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum DamageType
    {
        Slashing = 1,
        Piercing = 2,
        [Description("Slashing/Piercing")] Slashing_Piercing = Slashing | Piercing,
        Bludgeoning = 4,
        Frost = 8,
        Fire = 16,
        Acid = 32,
        Lightning = 64,
        Nether = 1024,
        Prismatic = 268435456
    }
}
