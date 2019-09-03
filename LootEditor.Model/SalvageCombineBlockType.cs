using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SalvageCombineBlockType : ExtraBlock
    {
        public string DefaultCombineString { get; set; }
        public int RuleCount { get; set; }

        public Dictionary<int, string> Materials { get; set; }
        public Dictionary<int, int> MaterialValues { get; set; }
        public int MaterialValueCount { get; set; }

        public override async Task ReadAsync(TextReader reader)
        {
            await base.ReadAsync(reader).ConfigureAwait(false);

            // Version - not used currently
            _ = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            DefaultCombineString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            var ruleCountString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (!int.TryParse(ruleCountString, out var ruleCount))
            {
                throw new Exception();
            }

            RuleCount = ruleCount;

            Materials = new Dictionary<int, string>();
            for (int i = 0; i < RuleCount; ++i)
            {
                var matString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                var combineString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

                Materials.Add(int.Parse(matString), combineString);
            }

            var materialValueCountString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (!int.TryParse(materialValueCountString, out var materialValueCount))
            {
                throw new Exception();
            }

            MaterialValueCount = materialValueCount;

            MaterialValues = new Dictionary<int, int>();
            for (int i = 0; i < MaterialValueCount; ++i)
            {
                var matString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                var valueString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

                MaterialValues.Add(int.Parse(matString), int.Parse(valueString));
            }
        }

        public override async Task WriteAsync(TextWriter writer)
        {
            var sb = new StringBuilder();
            using (var subWriter = new StringWriter(sb))
            {
                await subWriter.WriteLineForRealAsync("1").ConfigureAwait(false);
                await subWriter.WriteLineForRealAsync(DefaultCombineString).ConfigureAwait(false);
                await subWriter.WriteLineForRealAsync(RuleCount.ToString()).ConfigureAwait(false);
                foreach (var kvp in Materials)
                {
                    await subWriter.WriteLineForRealAsync(kvp.Key.ToString()).ConfigureAwait(false);
                    await subWriter.WriteLineForRealAsync(kvp.Value).ConfigureAwait(false);
                }
                await subWriter.WriteLineForRealAsync(MaterialValueCount.ToString()).ConfigureAwait(false);
                foreach (var kvp in MaterialValues)
                {
                    await subWriter.WriteLineForRealAsync(kvp.Key.ToString()).ConfigureAwait(false);
                    await subWriter.WriteLineForRealAsync(kvp.Value.ToString()).ConfigureAwait(false);
                }
            }

            Length = sb.Length;
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(sb.ToString()).ConfigureAwait(false);
        }
    }
}