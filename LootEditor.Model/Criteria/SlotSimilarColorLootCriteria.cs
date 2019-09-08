using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SlotSimilarColorLootCriteria : ColorLootCriteria
    {
        public SlotSimilarColorLootCriteria() : base(LootCriteriaType.SlotSimilarColor)
        {
        }

        public ArmorSlot Slot { get; set; }

        public override string ToString() => $"{base.ToString()}; Slot:{Slot}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = (ArmorSlot)Enum.ToObject(typeof(ArmorSlot), await ReadValue<int>(reader).ConfigureAwait(false));
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(((int)Slot).ToString()).ConfigureAwait(false);
        }
    }
}