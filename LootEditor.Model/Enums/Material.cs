using System.ComponentModel;

namespace LootEditor.Model.Enums
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    public enum Material
    {
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Agate")] Agate = 10,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Alabaster")] Alabaster = 66,
        [SalvageGroup(SalvageGroup.Gearcrafting)]
        [Description("Amber")] Amber = 11,
        [Description("Amethyst")] Amethyst = 12,
        [Description("Aquamarine")] Aquamarine = 13,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Armoredillo Hide")] ArmoredilloHide = 53,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Azurite")] Azurite = 14,
        [Description("Black Garnet")] BlackGarnet = 15,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Black Opal")] BlackOpal = 16,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Bloodstone")] Bloodstone = 17,
        [SalvageGroup(SalvageGroup.BrassGraniteIronMahogany)]
        [Description("Brass")] Brass = 57,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Bronze")] Bronze = 58,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Carnelian")] Carnelian = 18,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Ceramic")] Ceramic = 1,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Citrine")] Citrine = 19,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Copper")] Copper = 59,
        [SalvageGroup(SalvageGroup.Gearcrafting)]
        [Description("Diamond")] Diamond = 20,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Ebony")] Ebony = 73,
        [Description("Emerald")] Emerald = 21,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Fire Opal")] FireOpal = 22,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Gold")] Gold = 60,
        [SalvageGroup(SalvageGroup.BrassGraniteIronMahogany)]
        [Description("Granite")] Granite = 67,
        [Description("Green Garnet")] GreenGarnet = 23,
        [Description("Green Jade")] GreenJade = 24,
        [SalvageGroup(SalvageGroup.Gearcrafting)]
        [Description("Gromnie Hide")] GromnieHide = 54,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Hematite")] Hematite = 25,
        [Description("Imperial Topaz")] ImperialTopaz = 26,
        [SalvageGroup(SalvageGroup.BrassGraniteIronMahogany)]
        [Description("Iron")] Iron = 61,
        [SalvageGroup(SalvageGroup.BasicTinkering)]
        [Description("Ivory")] Ivory = 51,
        [Description("Jet")] Jet = 27,
        [Description("Lapis Lazuli")] LapisLazuli = 28,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Lavender Jade")] LavenderJade = 29,
        [SalvageGroup(SalvageGroup.BasicTinkering)]
        [Description("Leather")] Leather = 52,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Linen")] Linen = 4,
        [SalvageGroup(SalvageGroup.BrassGraniteIronMahogany)]
        [Description("Mahogany")] Mahogany = 74,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Malachite")] Malachite = 30,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Marble")] Marble = 68,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Moonstone")] Moonstone = 31,
        [Description("Oak")] Oak = 75,
        [Description("Obsidian")] Obsidian = 69,
        [Description("Onyx")] Onyx = 32,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Opal")] Opal = 33,
        [SalvageGroup(SalvageGroup.ArmorImbues)]
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Peridot")] Peridot = 34,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Pine")] Pine = 76,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Porcelain")] Porcelain = 2,
        [SalvageGroup(SalvageGroup.Gearcrafting)]
        [Description("Pyreal")] Pyreal = 62,
        [Description("Red Garnet")] RedGarnet = 35,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Red Jade")] RedJade = 36,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Reed Shark Hide")] ReedSharkHide = 55,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Rose Quartz")] RoseQuartz = 37,
        [SalvageGroup(SalvageGroup.Gearcrafting)]
        [Description("Ruby")] Ruby = 38,
        [Description("Sandstone")] Sandstone = 70,
        [SalvageGroup(SalvageGroup.Gearcrafting)]
        [Description("Sapphire")] Sapphire = 39,
        [Description("Satin")] Satin = 5,
        [Description("Serpentine")] Serpentine = 71,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Silk")] Silk = 6,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Silver")] Silver = 63,
        [Description("Smoky Quartz")] SmokyQuartz = 40,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Steel")] Steel = 64,
        [SalvageGroup(SalvageGroup.MagicItemTinkering)]
        [Description("Sunstone")] Sunstone = 41,
        [SalvageGroup(SalvageGroup.ItemTinkering)]
        [Description("Teak")] Teak = 77,
        [Description("Tiger Eye")] TigerEye = 42,
        [Description("Tourmaline")] Tourmaline = 43,
        [Description("Turquoise")] Turquoise = 44,
        [Description("Velvet")] Velvet = 7,
        [Description("White Jade")] WhiteJade = 45,
        [Description("White Quartz")] WhiteQuartz = 46,
        [Description("White Sapphire")] WhiteSapphire = 47,
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Wool")] Wool = 8,
        [Description("Yellow Garnet")] YellowGarnet = 48,
        [SalvageGroup(SalvageGroup.ArmorImbues)]
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Yellow Topaz")] YellowTopaz = 49,
        [SalvageGroup(SalvageGroup.ArmorImbues)]
        [SalvageGroup(SalvageGroup.ArmorTinkering)]
        [Description("Zircon")] Zircon = 50

    }
}
