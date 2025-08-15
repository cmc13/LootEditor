using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models;

[Serializable]
public class SimilarArmorColorLootCriteria : ColorLootCriteria, ISerializable
{
    public SimilarArmorColorLootCriteria() : base(Enums.LootCriteriaType.SimilarColorArmorType)
    {
    }

    private SimilarArmorColorLootCriteria(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ArmorGroup = info.GetString(nameof(ArmorGroup));
    }

    public string ArmorGroup { get; set; }

    public override string Filter => $"{base.Filter}:{EscapeFilter(ArmorGroup)}";

    public override string ToString() => $"{base.ToString()}; {ArmorGroup}";

    public override async Task ReadAsync(TextReader reader, int version)
    {
        await base.ReadAsync(reader, version).ConfigureAwait(false);
        ArmorGroup = await ReadValue<string>(reader).ConfigureAwait(false);
    }

    public override async Task WriteInternalAsync(Stream stream)
    {
        await base.WriteInternalAsync(stream).ConfigureAwait(false);
        await stream.WriteLineForRealAsync(ArmorGroup).ConfigureAwait(false);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ArmorGroup), ArmorGroup, typeof(string));
    }

    public override bool IsMatch(string[] filter)
    {
        if (!base.IsMatch(filter))
            return false;

        if (filter.Length >= 5 && !string.IsNullOrEmpty(filter[4]))
        {
            if (!ArmorGroup.Equals(filter[4], filter[4].IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                return false;
        }

        return true;
    }
}