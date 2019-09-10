using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum LootCriteriaType
    {
        UnsupportedRequirement = -1,

        [Description("Spell Name Match")] SpellNameMatch = 0,
        [Description("String Value Match")] StringValueMatch = 1,
        [Description("Long Value Key <=")] LongValKeyLE = 2,
        [Description("Long Value Key >=")] LongValKeyGE = 3,
        [Description("Double Value Key <=")] DoubleValKeyLE = 4,
        [Description("Double Value Key >=")] DoubleValKeyGE = 5,
        [Description("Damage % >=")] DamagePercentGE = 6,
        [Description("Object Class")] ObjectClass = 7,
        [Description("Spell Count >=")] SpellCountGE = 8,
        [Description("Spell Match")] SpellMatch = 9,
        [Description("Min Damage >=")] MinDamageGE = 10,
        [Description("Long Value Key Flag Exists")] LongValKeyFlagExists = 11,
        [Description("Long Value Key ==")] LongValKeyE = 12,
        [Description("Long Value Key !=")] LongValKeyNE = 13,
        [Description("One Similar Color")] AnySimilarColor = 14,
        [Description("Similar Color Armor Type")] SimilarColorArmorType = 15,
        [Description("Slot Similar Color")] SlotSimilarColor = 16,
        [Description("Slot Exact Palette")] SlotExactPalette = 17,

        //Character reqs, not based on the item
        [Description("Character Skill >=")] CharacterSkillGE = 1000,
        [Description("Main Pack Empty Slots >=")] CharacterMainPackEmptySlotsGE = 1001,
        [Description("Character Level >=")] CharacterLevelGE = 1002,
        [Description("Character Level <=")] CharacterLevelLE = 1003,
        [Description("Character Base Skill")] CharacterBaseSkill = 1004,

        //Mag's requirement types
        [Description("Buffed Median Damage >=")] BuffedMedianDamageGE = 2000, // Melee Weapon
        [Description("Buffed Missile Damage >=")] BuffedMissileDamageGE = 2001,
        [Description("Buffed Long Value Key >=")] BuffedLongValKeyGE = 2003,
        [Description("Buffed Double Value Key >=")] BuffedDoubleValKeyGE = 2005,
        [Description("Calc'd Buffed Tinked Damage >=")] CalcdBuffedTinkedDamageGE = 2006,
        [Description("Total Ratings >=")] TotalRatingsGE = 2007,
        [Description("Calc'd Buffed Tinked Target Melee >=")] CalcedBuffedTinkedTargetMeleeGE = 2008,

        [Description("Enable/Disable")] DisabledRule = 9999,

    }
}
