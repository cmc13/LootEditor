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
        public override string Filter => $"{base.Filter}:{SkillType}:{MinSkillValue}:{MaxSkillValue}";

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
            return $"{MinSkillValue} ≤ {td.ConvertToInvariantString(SkillType)} ≤ {MaxSkillValue}";
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

        public override bool IsMatch(string[] filter)
        {
            if (!base.IsMatch(filter))
                return false;

            if (filter.Length >= 3 && !string.IsNullOrEmpty(filter[2]))
            {
                if (!Enum.TryParse<Enums.SkillType>(filter[2], out var test) || test != SkillType)
                    return false;
            }

            if (filter.Length >= 4 && !string.IsNullOrEmpty(filter[3]))
            {
                if (!int.TryParse(filter[3], out var test2) || test2 != MinSkillValue)
                    return false;
            }

            if (filter.Length >= 5 && !string.IsNullOrEmpty(filter[4]))
            {
                if (!int.TryParse(filter[4], out var test2) || test2 != MaxSkillValue)
                    return false;
            }

            return true;
        }
    }
}