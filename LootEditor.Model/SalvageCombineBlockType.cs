using LootEditor.Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SalvageCombineBlockType : ExtraBlock
    {
        public string DefaultCombineString { get; set; }
        public int RuleCount { get; set; }

        public Dictionary<Material, string> Materials { get; set; }
        public Dictionary<Material, int> MaterialValues { get; set; }
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

            Materials = new Dictionary<Material, string>();
            for (int i = 0; i < RuleCount; ++i)
            {
                var matString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                var combineString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

                Materials.Add((Material)Enum.ToObject(typeof(Material), int.Parse(matString)), combineString);
            }

            var materialValueCountString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (!int.TryParse(materialValueCountString, out var materialValueCount))
            {
                throw new Exception();
            }

            MaterialValueCount = materialValueCount;

            MaterialValues = new Dictionary<Material, int>();
            for (int i = 0; i < MaterialValueCount; ++i)
            {
                var matString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                var valueString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

                MaterialValues.Add((Material)Enum.ToObject(typeof(Material), int.Parse(matString)), int.Parse(valueString));
            }
        }

        public override async Task WriteAsync(Stream stream)
        {
            using (var subWriter = new MemoryStream())
            {
                await subWriter.WriteLineForRealAsync("1").ConfigureAwait(false);
                await subWriter.WriteLineForRealAsync(DefaultCombineString).ConfigureAwait(false);
                await subWriter.WriteLineForRealAsync(Materials.Count.ToString()).ConfigureAwait(false);
                foreach (var kvp in Materials)
                {
                    await subWriter.WriteLineForRealAsync(((int)kvp.Key).ToString()).ConfigureAwait(false);
                    await subWriter.WriteLineForRealAsync(kvp.Value).ConfigureAwait(false);
                }
                await subWriter.WriteLineForRealAsync(MaterialValues.Count.ToString()).ConfigureAwait(false);
                foreach (var kvp in MaterialValues)
                {
                    await subWriter.WriteLineForRealAsync(((int)kvp.Key).ToString()).ConfigureAwait(false);
                    await subWriter.WriteLineForRealAsync(kvp.Value.ToString()).ConfigureAwait(false);
                }

                subWriter.Position = 0;
                Length = subWriter.Length;
                await base.WriteAsync(stream).ConfigureAwait(false);
                await subWriter.CopyToAsync(stream).ConfigureAwait(false);
            }
        }
    }
}