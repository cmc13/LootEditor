using LootEditor.Models.Enums;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models
{
    [Serializable]
    public abstract class LootCriteria : ICloneable, ISerializable
    {
        public abstract LootCriteriaType Type { get; }
        public int RequirementLength { get; set; }

        public static LootCriteria CreateLootCriteria(LootCriteriaType type)
        {
            return type switch
            {
                LootCriteriaType.AnySimilarColor => new ColorLootCriteria(type),
                LootCriteriaType.BuffedDoubleValKeyGE => new ValueKeyLootCriteria<DoubleValueKey, double>(type),
                LootCriteriaType.BuffedLongValKeyGE => new ValueKeyLootCriteria<LongValueKey, int>(type),
                LootCriteriaType.BuffedMedianDamageGE => new ValueLootCriteria<double>(type),
                LootCriteriaType.BuffedMissileDamageGE => new ValueLootCriteria<double>(type),
                LootCriteriaType.CalcdBuffedTinkedDamageGE => new ValueLootCriteria<double>(type),
                LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE => new CalculatedBuffedTinkedTargetMeleeGELootCriteria(),
                LootCriteriaType.CharacterBaseSkill => new CharacterBaseSkillLootCriteria(),
                LootCriteriaType.CharacterLevelGE => new ValueLootCriteria<int>(type),
                LootCriteriaType.CharacterLevelLE => new ValueLootCriteria<int>(type),
                LootCriteriaType.CharacterMainPackEmptySlotsGE => new ValueLootCriteria<int>(type),
                LootCriteriaType.CharacterSkillGE => new ValueKeyLootCriteria<SkillType, int>(type),
                LootCriteriaType.DamagePercentGE => new ValueLootCriteria<double>(type),
                LootCriteriaType.DisabledRule => new ValueLootCriteria<bool>(type),
                LootCriteriaType.DoubleValKeyGE => new ValueKeyLootCriteria<DoubleValueKey, double>(type),
                LootCriteriaType.DoubleValKeyLE => new ValueKeyLootCriteria<DoubleValueKey, double>(type),
                LootCriteriaType.LongValKeyE => new ValueKeyLootCriteria<LongValueKey, int>(type),
                LootCriteriaType.LongValKeyFlagExists => new ValueKeyLootCriteria<LongValueKey, int>(type),
                LootCriteriaType.LongValKeyGE => new ValueKeyLootCriteria<LongValueKey, int>(type),
                LootCriteriaType.LongValKeyLE => new ValueKeyLootCriteria<LongValueKey, int>(type),
                LootCriteriaType.LongValKeyNE => new ValueKeyLootCriteria<LongValueKey, int>(type),
                LootCriteriaType.MinDamageGE => new ValueLootCriteria<double>(type),
                LootCriteriaType.ObjectClass => new ValueLootCriteria<ObjectClass>(type),
                LootCriteriaType.SimilarColorArmorType => new SimilarArmorColorLootCriteria(),
                LootCriteriaType.SlotExactPalette => new SlotExactPaletteLootCriteria(),
                LootCriteriaType.SlotSimilarColor => new SlotSimilarColorLootCriteria(),
                LootCriteriaType.SpellCountGE => new ValueLootCriteria<int>(type),
                LootCriteriaType.SpellMatch => new SpellMatchLootCriteria(),
                LootCriteriaType.SpellNameMatch => new ValueLootCriteria<string>(type),
                LootCriteriaType.StringValueMatch => new ValueKeyLootCriteria<StringValueKey, string>(type),
                LootCriteriaType.TotalRatingsGE => new ValueLootCriteria<double>(type),
                _ => throw new Exception($"Unknown rule type: {type}"),
            };
        }

        public object Clone()
        {
            var criteria = CreateLootCriteria(Type);

            foreach (var prop in criteria.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(criteria, prop.GetValue(this));
                }
            }

            return criteria;
        }

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        public virtual async Task ReadAsync(TextReader reader, int version)
        {
            if (version == 1)
            {
                var length = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                if (!int.TryParse(length, out var requirementLength))
                {
                    throw new Exception("Failed to parse requirement length for loot criteria");
                }

                RequirementLength = requirementLength;
            }
        }

        public async Task WriteAsync(TextWriter stream)
        {
            using var internalStream = new MemoryStream();
            await WriteInternalAsync(internalStream).ConfigureAwait(false);

            await stream.WriteLineAsync(internalStream.Length.ToString()).ConfigureAwait(false);

            internalStream.Seek(0, SeekOrigin.Begin);
            await stream.WriteAsync(System.Text.Encoding.UTF8.GetString(internalStream.ToArray()));
            //                await internalStream.CopyToAsync(stream).ConfigureAwait(false);
        }

        public abstract Task WriteInternalAsync(Stream stream);

        protected static async Task<TValue> ReadValue<TValue>(TextReader reader)
        {
            var line = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var value = Convert.ChangeType(line, typeof(TValue));
            return (TValue)value;
        }
    }
}
