using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public abstract class ExtraBlock
    {
        public string Name { get; set; }
        public int Length { get; set; }

        public static async Task<ExtraBlock> ReadBlockAsync(TextReader reader)
        {
            var blockType = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            switch (blockType)
            {
                case "SalvageCombine":
                    var block = new SalvageCombineBlockType();
                    await block.ReadAsync(reader).ConfigureAwait(false);
                    return block;

                default:
                    throw new Exception($"Unknown block type: {blockType}");
            }
        }

        public virtual async Task WriteAsync(TextWriter writer)
        {
            await writer.WriteLineForRealAsync(Name).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Length.ToString()).ConfigureAwait(false);
        }

        public virtual async Task ReadAsync(TextReader reader)
        {
            var lengthLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (!int.TryParse(lengthLine, out var length))
            {
                throw new Exception();
            }

            Length = length;
        }
    }
}