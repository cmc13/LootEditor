using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models
{
    [Serializable]
    public class SlotExactPaletteLootCriteria : LootCriteria
    {
        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.SlotExactPalette;

        public int Slot { get; set; }
        public int Palette { get; set; }

        public override string Filter => $"{base.Filter}:{Slot}:{Palette:X}";

        public SlotExactPaletteLootCriteria()
        {
        }

        private SlotExactPaletteLootCriteria(SerializationInfo info, StreamingContext context)
        {
            Slot = info.GetInt32(nameof(Slot));
            Palette = info.GetInt32(nameof(Palette));
        }

        public override string ToString() => $"Slot {Slot} Palette 0x{Palette:X}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            Slot = await ReadValue<int>(reader).ConfigureAwait(false);
            Palette = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteInternalAsync(Stream stream)
        {
            await stream.WriteLineForRealAsync(Slot.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Palette.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Slot), Slot, typeof(int));
            info.AddValue(nameof(Palette), Palette, typeof(int));
        }

        public override bool IsMatch(string[] filter)
        {
            if (!base.IsMatch(filter))
                return false;

            if (filter.Length >= 3 && !string.IsNullOrEmpty(filter[2]))
            {
                if (!int.TryParse(filter[2], out var test) || test != Slot)
                    return false;
            }

            if (filter.Length >= 4 && !string.IsNullOrEmpty(filter[3]))
            {
                if (!int.TryParse(filter[3], System.Globalization.NumberStyles.HexNumber, null, out var test) || test != Palette)
                    return false;
            }

            return true;
        }
    }
}