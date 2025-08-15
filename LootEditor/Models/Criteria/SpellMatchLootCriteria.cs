using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models;

[Serializable]
public class SpellMatchLootCriteria : LootCriteria
{
    public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.SpellMatch;

    public string Match { get; set; }
    public string NoMatch { get; set; }
    public int SpellCount { get; set; }
    public override string Filter => $"{base.Filter}:{EscapeFilter(Match)}:{EscapeFilter(NoMatch)}:{SpellCount}";

    public SpellMatchLootCriteria()
    {
    }

    private SpellMatchLootCriteria(SerializationInfo info, StreamingContext context)
    {
        Match = info.GetString(nameof(Match));
        NoMatch = info.GetString(nameof(NoMatch));
        SpellCount = info.GetInt32(nameof(SpellCount));
    }

    public override string ToString() => $"{SpellCount} spells that match {Match} but not {NoMatch}";

    public override async Task ReadAsync(TextReader reader, int version)
    {
        await base.ReadAsync(reader, version).ConfigureAwait(false);
        Match = await ReadValue<string>(reader).ConfigureAwait(false);
        NoMatch = await ReadValue<string>(reader).ConfigureAwait(false);
        SpellCount = await ReadValue<int>(reader).ConfigureAwait(false);
    }

    public override async Task WriteInternalAsync(Stream stream)
    {
        await stream.WriteLineForRealAsync(Match ?? "").ConfigureAwait(false);
        await stream.WriteLineForRealAsync(NoMatch ?? "").ConfigureAwait(false);
        await stream.WriteLineForRealAsync(SpellCount.ToString()).ConfigureAwait(false);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Match), Match, typeof(string));
        info.AddValue(nameof(NoMatch), NoMatch, typeof(string));
        info.AddValue(nameof(SpellCount), SpellCount, typeof(int));
    }

    public override bool IsMatch(string[] filter)
    {
        if (!base.IsMatch(filter))
            return false;

        if (filter.Length >= 2 && !string.IsNullOrEmpty(filter[1]))
        {
            if (!Match.Contains(filter[1], filter[1].IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                return false;
        }

        if (filter.Length >= 3 && !string.IsNullOrEmpty(filter[2]))
        {
            if (!NoMatch.Contains(filter[2], filter[2].IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                return false;
        }

        if (filter.Length >= 4 && !string.IsNullOrEmpty(filter[3]))
        {
            if (!int.TryParse(filter[3], out var test) || test != SpellCount)
                return false;
        }

        return true;
    }
}