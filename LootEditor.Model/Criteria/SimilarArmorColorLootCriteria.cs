using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class SimilarArmorColorLootCriteria : ColorLootCriteria
    {
        public SimilarArmorColorLootCriteria() : base(LootCriteriaType.SimilarColorArmorType)
        {
        }

        public string ArmorGroup { get; set; }

        public override string ToString() => $"{base.ToString()}; {ArmorGroup}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            ArmorGroup = await ReadValue<string>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(ArmorGroup).ConfigureAwait(false);
        }
    }
}