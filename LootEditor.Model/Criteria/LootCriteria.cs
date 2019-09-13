using LootEditor.Model.Enums;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public abstract class LootCriteria : ICloneable, ISerializable
    {
        public abstract LootCriteriaType Type { get; }
        public int RequirementLength { get; set; }

        public static LootCriteria CreateLootCriteria(LootCriteriaType type)
        {
            switch (type)
            {
                case LootCriteriaType.UnsupportedRequirement:
                default:
                    throw new Exception($"Unknown rule type: {type}");

                case LootCriteriaType.AnySimilarColor: return new ColorLootCriteria(type);
                case LootCriteriaType.BuffedDoubleValKeyGE: return new ValueKeyLootCriteria<DoubleValueKey, double>(type);
                case LootCriteriaType.BuffedLongValKeyGE: return new ValueKeyLootCriteria<LongValueKey, int>(type);
                case LootCriteriaType.BuffedMedianDamageGE: return new ValueLootCriteria<double>(type);
                case LootCriteriaType.BuffedMissileDamageGE: return new ValueLootCriteria<double>(type);
                case LootCriteriaType.CalcdBuffedTinkedDamageGE: return new ValueLootCriteria<double>(type);
                case LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE: return new CalculatedBuffedTinkedTargetMeleeGELootCriteria();
                case LootCriteriaType.CharacterBaseSkill: return new CharacterBaseSkillLootCriteria();
                case LootCriteriaType.CharacterLevelGE: return new ValueLootCriteria<int>(type);
                case LootCriteriaType.CharacterLevelLE: return new ValueLootCriteria<int>(type);
                case LootCriteriaType.CharacterMainPackEmptySlotsGE: return new ValueLootCriteria<int>(type);
                case LootCriteriaType.CharacterSkillGE: return new ValueKeyLootCriteria<SkillType, int>(type);
                case LootCriteriaType.DamagePercentGE: return new ValueLootCriteria<double>(type);
                case LootCriteriaType.DisabledRule: return new ValueLootCriteria<bool>(type);
                case LootCriteriaType.DoubleValKeyGE: return new ValueKeyLootCriteria<DoubleValueKey, double>(type);
                case LootCriteriaType.DoubleValKeyLE: return new ValueKeyLootCriteria<DoubleValueKey, double>(type);
                case LootCriteriaType.LongValKeyE: return new ValueKeyLootCriteria<LongValueKey, int>(type);
                case LootCriteriaType.LongValKeyFlagExists: return new ValueKeyLootCriteria<LongValueKey, int>(type);
                case LootCriteriaType.LongValKeyGE: return new ValueKeyLootCriteria<LongValueKey, int>(type);
                case LootCriteriaType.LongValKeyLE: return new ValueKeyLootCriteria<LongValueKey, int>(type);
                case LootCriteriaType.LongValKeyNE: return new ValueKeyLootCriteria<LongValueKey, int>(type);
                case LootCriteriaType.MinDamageGE: return new ValueLootCriteria<double>(type);
                case LootCriteriaType.ObjectClass: return new ValueLootCriteria<ObjectClass>(type);
                case LootCriteriaType.SimilarColorArmorType: return new SimilarArmorColorLootCriteria();
                case LootCriteriaType.SlotExactPalette: return new SlotExactPaletteLootCriteria();
                case LootCriteriaType.SlotSimilarColor: return new SlotSimilarColorLootCriteria();
                case LootCriteriaType.SpellCountGE: return new ValueLootCriteria<int>(type);
                case LootCriteriaType.SpellMatch: return new SpellMatchLootCriteria();
                case LootCriteriaType.SpellNameMatch: return new ValueLootCriteria<string>(type);
                case LootCriteriaType.StringValueMatch: return new ValueKeyLootCriteria<StringValueKey, string>(type);
                case LootCriteriaType.TotalRatingsGE: return new ValueLootCriteria<double>(type);
            }
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

        public async Task WriteAsync(Stream stream)
        {
            using (var internalStream = new MemoryStream())
            {
                await WriteInternalAsync(internalStream).ConfigureAwait(false);

                await stream.WriteLineForRealAsync(internalStream.Length.ToString()).ConfigureAwait(false);

                internalStream.Position = 0;
                await internalStream.CopyToAsync(stream).ConfigureAwait(false);
            }
        }

        public abstract Task WriteInternalAsync(Stream stream);

        protected async Task<TValue> ReadValue<TValue>(TextReader reader)
        {
            var line = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var value = Convert.ChangeType(line, typeof(TValue));
            return (TValue)value;
        }
    }
}
