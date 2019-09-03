﻿using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SlotSimilarColorLootCriteria : ColorLootCriteria
    {
        public SlotSimilarColorLootCriteria() : base(LootCriteriaType.SlotSimilarColor)
        {
        }

        public int Slot { get; set; }

        public override string ToString() => $"{base.ToString()}; Slot#{Slot}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Slot.ToString()).ConfigureAwait(false);
        }
    }
}