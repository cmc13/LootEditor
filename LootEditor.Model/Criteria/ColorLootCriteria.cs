using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class ColorLootCriteria : LootCriteria
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public double HDiff { get; set; }
        public decimal SVDiff { get; set; }

        public override LootCriteriaType Type { get; }

        public ColorLootCriteria(LootCriteriaType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            var c = ColorTranslator.FromHtml($"{R:X2}{G:X2}{B:X2}");
            return $"Any Color [{c.Name}]; {HDiff}; {SVDiff}";
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);

            var rLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var gLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var bLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var hDiffLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var svDiffLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            if (!int.TryParse(rLine, out var r)
                || !int.TryParse(gLine, out var g)
                || !int.TryParse(bLine, out var b)
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

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(R.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(G.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(B.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(HDiff.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(SVDiff.ToString()).ConfigureAwait(false);
        }
    }
}