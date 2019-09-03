using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SlotExactPaletteLootCriteria : LootCriteria
    {
        public override LootCriteriaType Type => LootCriteriaType.SlotExactPalette;

        public int Slot { get; set; }
        public int Palette { get; set; }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = await ReadValue<int>(reader).ConfigureAwait(false);
            Palette = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Slot.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Palette.ToString()).ConfigureAwait(false);
        }
    }
}