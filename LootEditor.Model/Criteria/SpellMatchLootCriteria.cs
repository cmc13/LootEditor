using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public class SpellMatchLootCriteria : LootCriteria
    {
        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.SpellMatch;

        public string Match { get; set; }
        public string NoMatch { get; set; }
        public int SpellCount { get; set; }

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

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Match).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(NoMatch).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(SpellCount.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Match), Match, typeof(string));
            info.AddValue(nameof(NoMatch), NoMatch, typeof(string));
            info.AddValue(nameof(SpellCount), SpellCount, typeof(int));
        }
    }
}