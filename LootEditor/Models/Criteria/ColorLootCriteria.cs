using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LootEditor.Models
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

        public override string Filter => $"{base.Filter}:#{Color.A:X}{Color.R:X}{Color.G:X}{Color.B:X}:{HDiff}:{SVDiff}";

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

        public override async Task WriteInternalAsync(Stream stream)
        {
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

        public override bool IsMatch(string[] filter)
        {
            if (!base.IsMatch(filter))
                return false;

            if (filter.Length >= 3 && !string.IsNullOrEmpty(filter[2]))
            {
                //color match
                if (filter[2].StartsWith('#'))
                {
                    // try hex
                    if (filter[2].Length == 9)
                    {
                        if (!int.TryParse(filter[2].Substring(1), System.Globalization.NumberStyles.HexNumber, null, out var argb))
                            return false;

                        if (argb != ((Color.A << 24) | (Color.R << 16) | (Color.G << 8) | Color.B))
                            return false;
                    }
                    else if (filter[2].Length == 7)
                    {
                        if (!int.TryParse(filter[2].Substring(1), System.Globalization.NumberStyles.HexNumber, null, out var rgb))
                            return false;

                        if (rgb != ((Color.R << 16) | (Color.G << 8) | Color.B))
                            return false;
                    }
                    else
                        return false;
                }
                else if (Regex.IsMatch(filter[2], @"^\d+(,\d+){2,3}$"))
                {
                    var rgb = filter[2].Split(',');
                    if (rgb.Length == 4)
                    {
                        if (!byte.TryParse(rgb[0], out var a) || a != Color.A)
                            return false;
                        if (!byte.TryParse(rgb[1], out var r) || r != Color.R)
                            return false;
                        if (!byte.TryParse(rgb[2], out var g) || g != Color.G)
                            return false;
                        if (!byte.TryParse(rgb[3], out var b) || b != Color.B)
                            return false;
                    }
                    else if (rgb.Length == 3)
                    {
                        if (!byte.TryParse(rgb[0], out var r) || r != Color.R)
                            return false;
                        if (!byte.TryParse(rgb[1], out var g) || g != Color.G)
                            return false;
                        if (!byte.TryParse(rgb[2], out var b) || b != Color.B)
                            return false;
                    }
                    else // Can't really get here
                        return false;
                }
                else
                {
                    // try color name
                    var name = Color.GetColorName();
                    if (!name.Equals(filter[2], filter[2].IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                        return false;
                }
            }

            if (filter.Length >= 4 && !string.IsNullOrEmpty(filter[3]))
            {
                if (!double.TryParse(filter[3], out var test) || test != HDiff)
                    return false;
            }

            if (filter.Length >= 5 && !string.IsNullOrEmpty(filter[4]))
            {
                if (!decimal.TryParse(filter[4], out var test) || test != SVDiff)
                    return false;
            }

            return true;
        }
    }
}