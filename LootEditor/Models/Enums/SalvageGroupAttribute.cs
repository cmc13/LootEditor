using System;

namespace LootEditor.Models.Enums;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class SalvageGroupAttribute : Attribute
{
    public SalvageGroupAttribute(SalvageGroup salvageGroup)
    {
        SalvageGroup = salvageGroup;
    }

    public SalvageGroup SalvageGroup { get; }
}
