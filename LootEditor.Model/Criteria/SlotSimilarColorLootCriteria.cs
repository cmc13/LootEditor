using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public class SlotSimilarColorLootCriteria : ColorLootCriteria, ISerializable
    {
        public SlotSimilarColorLootCriteria() : base(Enums.LootCriteriaType.SlotSimilarColor)
        {
        }

        private SlotSimilarColorLootCriteria(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Slot = info.GetInt32(nameof(Slot));
        }

        public int Slot { get; set; }

        public override string ToString() => $"{base.ToString()}; Slot:{Slot}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteInternalAsync(Stream stream)
        {
            await base.WriteInternalAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Slot.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Slot), Slot, typeof(int));
            base.GetObjectData(info, context);
        }
    }
}