using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models
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

        public override string Filter => $"{base.Filter}:{Slot}";

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

        public override bool IsMatch(string[] filter)
        {
            if (!base.IsMatch(filter))
                return false;

            if (filter.Length >= 6 && !string.IsNullOrEmpty(filter[5]))
            {
                if (!int.TryParse(filter[5], out var test) || test != Slot)
                    return false;
            }

            return true;
        }
    }
}