using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models
{
    [Serializable]
    public class CharacterBaseSkillLootCriteria : LootCriteria
    {
        public override Enums.LootCriteriaType Type => Enums.LootCriteriaType.CharacterBaseSkill;

        public Enums.SkillType SkillType { get; set; }
        public int MinSkillValue { get; set; } = 0;
        public int MaxSkillValue { get; set; } = 999;

        public CharacterBaseSkillLootCriteria() { }

        private CharacterBaseSkillLootCriteria(SerializationInfo info, StreamingContext context)
        {
            SkillType = (Enums.SkillType)info.GetValue(nameof(SkillType), typeof(Enums.SkillType));
            MinSkillValue = info.GetInt32(nameof(MinSkillValue));
            MaxSkillValue = info.GetInt32(nameof(MaxSkillValue));
        }

        public override string ToString()
        {
            var td = TypeDescriptor.GetConverter(typeof(Enums.SkillType));
            return $"{MinSkillValue} <= {td.ConvertToInvariantString(SkillType)} <= {MaxSkillValue}";
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            SkillType = (Enums.SkillType)Enum.ToObject(typeof(Enums.SkillType), await ReadValue<int>(reader).ConfigureAwait(false));
            MinSkillValue = await ReadValue<int>(reader).ConfigureAwait(false);
            MaxSkillValue = await ReadValue<int>(reader).ConfigureAwait(false);
        }

        public override async Task WriteInternalAsync(Stream stream)
        {
            await stream.WriteLineForRealAsync(((int)SkillType).ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(MinSkillValue.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(MaxSkillValue.ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(SkillType), SkillType, typeof(Enums.SkillType));
            info.AddValue(nameof(MinSkillValue), MinSkillValue, typeof(int));
            info.AddValue(nameof(MaxSkillValue), MaxSkillValue, typeof(int));
        }
    }
}