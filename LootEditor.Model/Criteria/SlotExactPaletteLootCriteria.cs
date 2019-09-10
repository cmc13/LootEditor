using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public class SlotExactPaletteLootCriteria : LootCriteria
    {
        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.SlotExactPalette;

        public Enums.ArmorSlot Slot { get; set; }
        public int Palette { get; set; }

        public SlotExactPaletteLootCriteria()
        {
        }

        private SlotExactPaletteLootCriteria(SerializationInfo info, StreamingContext context)
        {
            Slot = (Enums.ArmorSlot)info.GetValue(nameof(Slot), typeof(Enums.ArmorSlot));
            Palette = info.GetInt32(nameof(Palette));
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = (Enums.ArmorSlot)Enum.ToObject(typeof(Enums.ArmorSlot), await ReadValue<int>(reader).ConfigureAwait(false));
            Palette = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(((int)Slot).ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Palette.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Slot), Slot, typeof(Enums.ArmorSlot));
            info.AddValue(nameof(Palette), Palette, typeof(int));
        }
    }
}