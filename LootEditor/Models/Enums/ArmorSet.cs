using System.ComponentModel;

namespace LootEditor.Models.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum ArmorSet
    {
        [Description("Protective Clothing")] ProtectiveClothing = 32,
        [Description("Gladiatorial Clothing")] GladiatorialClothing = 31,
        [Description("Dedication")] Dedication = 30,
        [Description("Lightning Proof")] LightningProof = 29,
        [Description("Cold Proof")] ColdProof = 28,
        [Description("Acid Proof")] AcidProof = 27,
        [Description("Flame Proof")] FlameProof = 26,
        [Description("Interlocking")] Interlocking = 25,
        [Description("Reinforced")] Reinforced = 24,
        [Description("Hardenend")] Hardenend = 23,
        [Description("Swift")] Swift = 22,
        [Description("Wise")] Wise = 21,
        [Description("Dexterous")] Dexterous = 20,
        [Description("Hearty")] Hearty = 19,
        [Description("Crafter's")] Crafters = 18,
        [Description("Tinker's")] Tinkers = 17,
        [Description("Defender's")] Defenders = 16,
        [Description("Archer's")] Archers = 15,
        [Description("Adept's")] Adepts = 14,
        [Description("Soldier's")] Soldiers = 13,
        [Description("Leggings of Perfect Light")] LeggingsOfPerfectLight = 12,
        [Description("Coat of the Perfect Light")] CoatOfThePerfectLight = 11,
        [Description("Arm, Mind, Heart")] ArmMindHeart = 10,
        [Description("Empyrean Rings")] EmpyreanRings = 9,
        [Description("Shou-jen")] Shoujen = 8,
        [Description("Relic Alduressa")] RelicAlduressa = 7,
        [Description("Ancient Relic")] AncientRelic = 6,
        [Description("Noble Relic")] NobleRelic = 5

    }
}
