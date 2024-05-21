using System.ComponentModel;

namespace LootEditor.Models.Enums;

[TypeConverter(typeof(EnumDescriptionConverter))]
public enum DoubleValueKey
{
    [Description("Mana Rate of Change")] ManaRateOfChange = 5,
    [Description("Melee Defense Bonus")] MeleeDefenseBonus = 29,
    [Description("Mana Transfer Efficiency")] ManaTransferEfficiency = 87,
    [Description("Healing Kit Restore Bonus")] HealingKitRestoreBonus = 100,
    [Description("Mana Stone Destruct Chance")] ManaStoneChanceDestruct = 137,
    [Description("Mana Conversion Bonus")] ManaCBonus = 144,
    [Description("Missile Defense Bonus")] MissileDBonus = 149,
    [Description("Magic Defense Bonus")] MagicDBonus = 150,
    [Description("Elemental Damage vs Monsters")] ElementalDamageVersusMonsters = 152,
    [Description("Slash Protection")] SlashProt = 167772160,
    [Description("Pierce Protection")] PierceProt = 167772161,
    [Description("Bludgeon Protection")] BludgeonProt = 167772162,
    [Description("Acid Protection")] AcidProt = 167772163,
    [Description("Lightning Protection")] LightningProt = 167772164,
    [Description("Fire Protection")] FireProt = 167772165,
    [Description("Cold Protection")] ColdProt = 167772166,
    Heading = 167772167,
    [Description("Approach Distance")] ApproachDistance = 167772168,
    [Description("Salvage Workmanship")] SalvageWorkmanship = 167772169,
    Scale = 167772170,
    Variance = 167772171,
    [Description("Attack Bonus")] AttackBonus = 167772172,
    Range = 167772173,
    [Description("Damage Bonus")] DamageBonus = 167772174,
}
