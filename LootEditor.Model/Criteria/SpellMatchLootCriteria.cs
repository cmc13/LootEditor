using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SpellMatchLootCriteria : LootCriteria
    {
        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.SpellMatch;

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

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Match).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(NoMatch).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(SpellCount.ToString()).ConfigureAwait(false);
        }
    }
}