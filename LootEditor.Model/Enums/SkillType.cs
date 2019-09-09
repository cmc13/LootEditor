using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum SkillType
    {
        Axe = 1,
        Bow = 2,
        Crossbow = 3,
        Dagger = 4,
        Mace = 5,
        MeleeDefense = 6,
        MissileDefense = 7,
        Spear = 9,
        Staff = 10,
        Sword = 11,
        ThrownWeapons = 12,
        Unarmed = 13,
        ArcaneLore = 14,
        MagicDefense = 15,
        ManaConversion = 16,
        ItemTinkering = 18,
        AssessPerson = 19,
        Deception = 20,
        Healing = 21,
        Jump = 22,
        Lockpick = 23,
        Run = 24,
        AssessCreature = 27,
        WeaponTinkering = 28,
        ArmorTinkering = 29,
        MagicItemTinkering = 30,
        CreatureEnchantment = 31,
        ItemEnchantment = 32,
        LifeMagic = 33,
        WarMagic = 34,
        Leadership = 35,
        Loyalty = 36,
        Fletching = 37,
        Alchemy = 38,
        Cooking = 39,
        Salvaging = 40,
        TwoHandedCombat = 41,
        Gearcraft = 42,
        Void = 43,
        HeavyWeapons = 44,
        LightWeapons = 45,
        FinesseWeapons = 46,
        MissileWeapons = 47,
        Shield = 48,
        DualWield = 49,
        Recklessness = 50,
        SneakAttack = 51,
        DirtyFighting = 52,

        Summoning = 54
    }
}