using System.ComponentModel;

namespace LootEditor.Models.Enums;

[TypeConverter(typeof(EnumDescriptionConverter))]
public enum StringValueKey
{
    Name = 1,
    Title = 5,
    Inscription = 7,
    [Description("Inscribed By")] InscribedBy = 8,
    [Description("Fellowship Name")] FellowshipName = 10,
    [Description("Usage Instructions")] UsageInstructions = 14,
    [Description("Simple Description")] SimpleDescription = 15,
    [Description("Full Description")] FullDescription = 16,
    [Description("Monarch Name")] MonarchName = 21,
    [Description("Only Activated By")] OnlyActivatedBy = 25,
    Patron = 35,
    [Description("Portal Destination")] PortalDestination = 38,
    [Description("Last Tinkered By")] LastTinkeredBy = 39,
    [Description("Imbued By")] ImbuedBy = 40,
    [Description("Date Born")] DateBorn = 43,
    [Description("Secondary Name")] SecondaryName = 184549376,
}