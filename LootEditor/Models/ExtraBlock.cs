using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Models;

public abstract class ExtraBlock
{
    public string Name { get; protected set; }
    public long Length { get; set; }

    public static async Task<ExtraBlock> ReadBlockAsync(TextReader reader)
    {
        var blockType = await reader.ReadLineForRealAsync().ConfigureAwait(false);
        switch (blockType)
        {
            case "SalvageCombine":
                var block = new SalvageCombineBlockType() { Name = blockType };
                await block.ReadAsync(reader).ConfigureAwait(false);
                return block;

            default:
                throw new Exception($"Unknown block type: {blockType}");
        }
    }

    public virtual async Task WriteAsync(TextWriter stream)
    {
        await stream.WriteLineAsync(Name).ConfigureAwait(false);
        await stream.WriteLineAsync(Length.ToString()).ConfigureAwait(false);
    }

    public virtual async Task ReadAsync(TextReader reader)
    {
        var lengthLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
        if (!long.TryParse(lengthLine, out var length))
        {
            throw new Exception("Failed to parse length for extra block");
        }

        Length = length;
    }
}