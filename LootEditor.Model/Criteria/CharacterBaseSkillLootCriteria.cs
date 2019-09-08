using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class CharacterBaseSkillLootCriteria : LootCriteria
    {
        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.CharacterBaseSkill;

        public Enums.SkillType SkillType { get; set; }
        public int MinSkillValue { get; set; } = 0;
        public int MaxSkillValue { get; set; } = 999;

        public override string ToString() => $"{MinSkillValue} <= {SkillType} <= {MaxSkillValue}";

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            SkillType = await ReadValue<Enums.SkillType>(reader).ConfigureAwait(false);
            MinSkillValue = await ReadValue<int>(reader).ConfigureAwait(false);
            MaxSkillValue = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(((int)SkillType).ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(MinSkillValue.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(MaxSkillValue.ToString()).ConfigureAwait(false);
        }
    }
}