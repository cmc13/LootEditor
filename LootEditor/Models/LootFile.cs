using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using static LootEditor.Models.Constants.Constants;

namespace LootEditor.Models;

public class LootFile
{
    private readonly List<LootRule> lootRules = [];

    public int Version { get; set; } = MAX_FILE_VERSION;

    public int RuleCount { get; set; }

    public IEnumerable<LootRule> Rules => lootRules.AsReadOnly();
    public IEnumerable<ExtraBlock> ExtraBlocks { get; set; } = [new SalvageCombineBlockType()];

    public async Task ReadFileAsync(StreamReader reader)
    {
        var firstLine = await reader.ReadLineAsync().ConfigureAwait(false);
        if (firstLine.Equals("UTL"))
        {
            var versionLine = await reader.ReadLineAsync().ConfigureAwait(false);
            if (!int.TryParse(versionLine, out var version))
            {
                if (Version > MAX_FILE_VERSION)
                {
                    throw new Exception($"Unknown file version detected. Max version is {MAX_FILE_VERSION}, file is version {version}.");
                }
            }

            Version = version;

            var countLine = await reader.ReadLineAsync().ConfigureAwait(false);
            if (!int.TryParse(countLine, out var ruleCount))
            {
                throw new Exception("Unable to read rule count from file.");
            }

            RuleCount = ruleCount;
        }
        else
        {
            Version = 0;

            if (!int.TryParse(firstLine, out var ruleCount))
            {
                throw new Exception("Unable to read rule count from file.");
            }

            RuleCount = ruleCount;
        }

        for (var i = 0; i < RuleCount; ++i)
        {
            if (reader.EndOfStream)
            {
                throw new Exception("Unexpected end of file found.");
            }

            try
            {
                var rule = await LootRule.ReadRuleAsync(Version, reader).ConfigureAwait(false);
                lootRules.Add(rule);
            }
            catch
            {
                throw;
            }
        }

        var blocks = new List<ExtraBlock>();
        while (!reader.EndOfStream)
        {
            var block = await ExtraBlock.ReadBlockAsync(reader).ConfigureAwait(false);

            blocks.Add(block);
        }

        ExtraBlocks = blocks.AsReadOnly();
    }

    public async Task WriteFileAsync(TextWriter writer)
    {
        await writer.WriteLineAsync("UTL").ConfigureAwait(false);
        await writer.WriteLineAsync(MAX_FILE_VERSION.ToString()).ConfigureAwait(false);
        await writer.WriteLineAsync(RuleCount.ToString()).ConfigureAwait(false);

        foreach (var rule in Rules)
            await rule.WriteAsync(writer);

        foreach (var block in ExtraBlocks)
            await block.WriteAsync(writer);
    }

    public void AddRule(LootRule newRule)
    {
        lootRules.Add(newRule);
        RuleCount++;
    }

    public void RemoveRule(LootRule rule)
    {
        lootRules.Remove(rule);
        RuleCount--;
    }

    public void MoveRule(int index, int newIndex)
    {
        var item = lootRules[index];
        lootRules.RemoveAt(index);
        lootRules.Insert(newIndex, item);
    }
}
