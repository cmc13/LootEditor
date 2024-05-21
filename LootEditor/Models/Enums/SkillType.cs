using System.ComponentModel;

namespace LootEditor.Models.Enums;

[TypeConverter(typeof(EnumDescriptionConverter))]
public enum SkillType
{
    Axe = 1,
    Bow = 2,
    Crossbow = 3,
    Dagger = 4,
    Mace = 5,
    [Description("Melee Defense")] MeleeDefense = 6,
    [Description("Missile Defense")] MissileDefense = 7,
    Spear = 9,
    Staff = 10,
    Sword = 11,
    [Description("Thrown Weapons")] ThrownWeapons = 12,
    Unarmed = 13,
    [Description("Arcane Lore")] ArcaneLore = 14,
    [Description("Magic Defense")] MagicDefense = 15,
    [Description("Mana Conversion")] ManaConversion = 16,
    [Description("Item Tinkering")] ItemTinkering = 18,
    [Description("Assess Person")] AssessPerson = 19,
    Deception = 20,
    Healing = 21,
    Jump = 22,
    Lockpick = 23,
    Run = 24,
    [Description("Assess Creature")] AssessCreature = 27,
    [Description("Weapon Tinkering")] WeaponTinkering = 28,
    [Description("Armor Tinkering")] ArmorTinkering = 29,
    [Description("Magic Item Tinkering")] MagicItemTinkering = 30,
    [Description("Creature Enchantment")] CreatureEnchantment = 31,
    [Description("Item Enchantment")] ItemEnchantment = 32,
    [Description("Life Magic")] LifeMagic = 33,
    [Description("War Magic")] WarMagic = 34,
    Leadership = 35,
    Loyalty = 36,
    Fletching = 37,
    Alchemy = 38,
    Cooking = 39,
    Salvaging = 40,
    [Description("Two Handed Combat")] TwoHandedCombat = 41,
    Gearcraft = 42,
    Void = 43,
    [Description("Heavy Weapons")] HeavyWeapons = 44,
    [Description("Light Weapons")] LightWeapons = 45,
    [Description("Finesse Weapons")] FinesseWeapons = 46,
    [Description("Missile Weapons")] MissileWeapons = 47,
    Shield = 48,
    [Description("Dual Wield")] DualWield = 49,
    Recklessness = 50,
    [Description("Sneak Attack")] SneakAttack = 51,
    [Description("Dirty Fighting")] DirtyFighting = 52,


    Summoning = 54
}