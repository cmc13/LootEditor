using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LootEditor.Model
{
    [Serializable]
    public class ColorLootCriteria : LootCriteria
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public Color Color
        {
            get => Color.FromArgb(255, R, G, B);
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }
        public double HDiff { get; set; }
        public decimal SVDiff { get; set; }

        public override Enums.LootCriteriaType Type { get; }

        public ColorLootCriteria(Enums.LootCriteriaType type)
        {
            Type = type;
        }

        protected ColorLootCriteria(SerializationInfo info, StreamingContext context)
        {
            Type = (Enums.LootCriteriaType)info.GetValue(nameof(Type), typeof(Enums.LootCriteriaType));
            var hexString = info.GetString(nameof(Color));
            Color = System.Windows.Media.Color.FromRgb(Convert.ToByte(hexString.Substring(1, 2), 16), Convert.ToByte(hexString.Substring(3, 2), 16), Convert.ToByte(hexString.Substring(5, 2), 16));
            HDiff = info.GetDouble(nameof(HDiff));
            SVDiff = info.GetDecimal(nameof(SVDiff));
        }

        public override string ToString()
        {
            var name = $"#{R:X2}{G:X2}{B:X2}";
            foreach (var color in typeof(Colors).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                if ((Color)color.GetValue(null) == Color)
                    name = color.Name;
            return $"Any Color [{name}]; {HDiff}; {SVDiff}";
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);

            var rLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var gLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var bLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var hDiffLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var svDiffLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            if (!byte.TryParse(rLine, out var r)
                || !byte.TryParse(gLine, out var g)
                || !byte.TryParse(bLine, out var b)
                || !double.TryParse(hDiffLine, out var hDiff)
                || !decimal.TryParse(svDiffLine, out var svDiff))
            {
                throw new Exception("Unable to parse Any Similar Color rule");
            }

            R = r;
            G = g;
            B = b;
            HDiff = hDiff;
            SVDiff = svDiff;
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(R.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(G.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(B.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(HDiff.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(SVDiff.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Type), Type, typeof(Enums.LootCriteriaType));
            info.AddValue(nameof(Color), $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}", typeof(string));
            info.AddValue(nameof(HDiff), HDiff, typeof(double));
            info.AddValue(nameof(SVDiff), SVDiff, typeof(decimal));
        }
    }
}