using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SpellMatchLootCriteria : LootCriteria
    {
        public override LootCriteriaType Type => LootCriteriaType.SpellMatch;

        public string Match { get; set; }
        public string NoMatch { get; set; }
        public int SpellCount { get; set; }

        public override string ToString() => $"{SpellCount} spells that match {Match} but not {NoMatch}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Match = await ReadValue<string>(reader).ConfigureAwait(false);
            NoMatch = await ReadValue<string>(reader).ConfigureAwait(false);
            SpellCount = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Match).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(NoMatch).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(SpellCount.ToString()).ConfigureAwait(false);
        }
    }
}