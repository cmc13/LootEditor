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
            Slot = (Enums.ArmorSlot)info.GetValue(nameof(Slot), typeof(Enums.ArmorSlot));
        }

        public Enums.ArmorSlot Slot { get; set; }

        public override string ToString() => $"{base.ToString()}; Slot:{Slot}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = (Enums.ArmorSlot)Enum.ToObject(typeof(Enums.ArmorSlot), await ReadValue<int>(reader).ConfigureAwait(false));
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(((int)Slot).ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Slot), Slot, typeof(Enums.ArmorSlot));
            base.GetObjectData(info, context);
        }
    }
}