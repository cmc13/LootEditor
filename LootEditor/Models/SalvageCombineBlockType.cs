using LootEditor.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Models
{
    public class SalvageCombineBlockType : ExtraBlock
    {
        public string DefaultCombineString { get; set; }
        public int RuleCount { get; set; }

        public Dictionary<Material, string> Materials { get; } = new Dictionary<Material, string>();
        public Dictionary<Material, int> MaterialValues { get; } = new Dictionary<Material, int>();
        public int MaterialValueCount { get; set; }

        public SalvageCombineBlockType()
        {
            Name = "SalvageCombine";
        }

        public override async Task ReadAsync(TextReader reader)
        {
            await base.ReadAsync(reader).ConfigureAwait(false);

            // Version - not used currently
            _ = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            DefaultCombineString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            var ruleCountString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (!int.TryParse(ruleCountString, out var ruleCount))
            {
                throw new Exception($"Failed to parse material combine rule count for salvage block");
            }

            RuleCount = ruleCount;

            for (int i = 0; i < RuleCount; ++i)
            {
                var matString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                var combineString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

                Materials.Add((Material)Enum.ToObject(typeof(Material), int.Parse(matString)), combineString);
            }

            var materialValueCountString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (materialValueCountString != null)
            {
                if (!int.TryParse(materialValueCountString, out var materialValueCount))
                {
                    throw new Exception($"Failed to parse material value count for salvage block");
                }

                MaterialValueCount = materialValueCount;

                for (int i = 0; i < MaterialValueCount; ++i)
                {
                    var matString = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                    var valueString = await reader.ReadLineForRealAsync().ConfigureAwait(false);

                    MaterialValues.Add((Material)Enum.ToObject(typeof(Material), int.Parse(matString)), int.Parse(valueString));
                }
            }
        }

        public override async Task WriteAsync(TextWriter stream)
        {
            using (var subWriter = new MemoryStream())
            {
                await subWriter.WriteLineForRealAsync("1").ConfigureAwait(false);
                await subWriter.WriteLineForRealAsync(DefaultCombineString ?? "").ConfigureAwait(false);
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

                subWriter.Seek(0, SeekOrigin.Begin);
                Length = subWriter.Length;
                await base.WriteAsync(stream).ConfigureAwait(false);
                //await subWriter.CopyToAsync(stream).ConfigureAwait(false);
                await stream.WriteAsync(System.Text.Encoding.UTF8.GetString(subWriter.ToArray())).ConfigureAwait(false);
            }
        }
    }
}