﻿using System.ComponentModel;

namespace LootEditor.Models.Enums;

[TypeConverter(typeof(EnumDescriptionConverter))]
public enum LootAction
{
    [Description("Don't Loot")] NoLoot = 0,
    Keep = 1,
    Salvage = 2,
    Sell = 3,
    Read = 4,
    User1 = 5,
    User2 = 6,
    User3 = 7,
    User4 = 8,
    User5 = 9,
    [Description("Keep Up To")]KeepUpTo = 10,
}
