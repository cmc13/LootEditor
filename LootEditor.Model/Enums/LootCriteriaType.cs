namespace LootEditor.Model
{
    public enum LootCriteriaType
    {
        UnsupportedRequirement = -1,

        SpellNameMatch = 0,
        StringValueMatch = 1,
        LongValKeyLE = 2,
        LongValKeyGE = 3,
        DoubleValKeyLE = 4,
        DoubleValKeyGE = 5,
        DamagePercentGE = 6,
        ObjectClass = 7,
        SpellCountGE = 8,
        SpellMatch = 9,
        MinDamageGE = 10,
        LongValKeyFlagExists = 11,
        LongValKeyE = 12,
        LongValKeyNE = 13,
        AnySimilarColor = 14,
        SimilarColorArmorType = 15,
        SlotSimilarColor = 16,
        SlotExactPalette = 17,

        //Character reqs, not based on the item
        CharacterSkillGE = 1000,
        CharacterMainPackEmptySlotsGE = 1001,
        CharacterLevelGE = 1002,
        CharacterLevelLE = 1003,
        CharacterBaseSkill = 1004,

        //Mag's requirement types
        BuffedMedianDamageGE = 2000, // Melee Weapon
        BuffedMissileDamageGE = 2001,
        BuffedLongValKeyGE = 2003,
        BuffedDoubleValKeyGE = 2005,
        CalcdBuffedTinkedDamageGE = 2006,
        TotalRatingsGE = 2007,
        CalcedBuffedTinkedTargetMeleeGE = 2008,

        DisabledRule = 9999,
    }
}
