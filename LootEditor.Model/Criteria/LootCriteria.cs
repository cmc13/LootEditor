using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public abstract class LootCriteria : ICloneable
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
                case LootCriteriaType.BuffedDoubleValKeyGE: return new ValueKeyLootCriteria<DoubleValueKey, double>(type, "{0} >= {1}");
                case LootCriteriaType.BuffedLongValKeyGE: return new ValueKeyLootCriteria<LongValueKey, int>(type, "Buffed {0} >= {1}");
                case LootCriteriaType.BuffedMedianDamageGE: return new ValueLootCriteria<double>(type, "Buffed Median Dmg >= {0}");
                case LootCriteriaType.BuffedMissileDamageGE: return new ValueLootCriteria<double>(type, "Buffed Missile Dmg >= {0}");
                case LootCriteriaType.CalcdBuffedTinkedDamageGE: return new ValueLootCriteria<double>(type, "Calc'd Buffed/Tinked Dmg >= {0}");
                case LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE: return new CalculatedBuffedTinkedTargetMeleeGELootCriteria();
                case LootCriteriaType.CharacterBaseSkill: return new CharacterBaseSkillLootCriteria();
                case LootCriteriaType.CharacterLevelGE: return new ValueLootCriteria<int>(type, "Character Level >= {0}");
                case LootCriteriaType.CharacterLevelLE: return new ValueLootCriteria<int>(type, "Character Level <= {0}");
                case LootCriteriaType.CharacterMainPackEmptySlotsGE: return new ValueLootCriteria<int>(type, "Empty Pack Slots >= {0}");
                case LootCriteriaType.CharacterSkillGE: return new ValueKeyLootCriteria<SkillType, int>(type, "{0} >= {1}");
                case LootCriteriaType.DamagePercentGE: return new ValueLootCriteria<double>(type, "Damage % >= {0}");
                case LootCriteriaType.DisabledRule: return new ValueLootCriteria<bool>(type, "This Rule is Disabled");
                case LootCriteriaType.DoubleValKeyGE: return new ValueKeyLootCriteria<DoubleValueKey, double>(type, "{0} >= {1}");
                case LootCriteriaType.DoubleValKeyLE: return new ValueKeyLootCriteria<DoubleValueKey, double>(type, "{0} <= ");
                case LootCriteriaType.LongValKeyE: return new ValueKeyLootCriteria<LongValueKey, int>(type, "{0} == {1}");
                case LootCriteriaType.LongValKeyFlagExists: return new ValueKeyLootCriteria<LongValueKey, int>(type, "{0} has flags {1} (0x{1:X})");
                case LootCriteriaType.LongValKeyGE: return new ValueKeyLootCriteria<LongValueKey, int>(type, "{0} >= {1}");
                case LootCriteriaType.LongValKeyLE: return new ValueKeyLootCriteria<LongValueKey, int>(type, "{0} <= {1}");
                case LootCriteriaType.LongValKeyNE: return new ValueKeyLootCriteria<LongValueKey, int>(type, "{0} != {1}");
                case LootCriteriaType.MinDamageGE: return new ValueLootCriteria<double>(type, "Min Damage >= {0}");
                case LootCriteriaType.ObjectClass: return new ValueLootCriteria<ObjectClass>(type, "Object Class == {0}");
                case LootCriteriaType.SimilarColorArmorType: return new SimilarArmorColorLootCriteria();
                case LootCriteriaType.SlotExactPalette: return new SlotExactPaletteLootCriteria();
                case LootCriteriaType.SlotSimilarColor: return new SlotSimilarColorLootCriteria();
                case LootCriteriaType.SpellCountGE: return new ValueLootCriteria<int>(type, "Spell Counts >= {0}");
                case LootCriteriaType.SpellMatch: return new SpellMatchLootCriteria();
                case LootCriteriaType.SpellNameMatch: return new ValueLootCriteria<string>(type, "Spell Name Matches: {0}");
                case LootCriteriaType.StringValueMatch: return new ValueKeyLootCriteria<StringValueKey, string>(type, "{0} == {1}");
                case LootCriteriaType.TotalRatingsGE: return new ValueLootCriteria<double>(type, "Total Ratings >= {0}");
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

        public virtual async Task ReadAsync(TextReader reader, int version)
        {
            if (version == 1)
            {
                var length = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                if (!int.TryParse(length, out var requirementLength))
                {
                    throw new Exception();
                }

                RequirementLength = requirementLength;
            }
        }

        public virtual async Task WriteAsync(TextWriter writer)
        {
            await writer.WriteLineForRealAsync(RequirementLength.ToString()).ConfigureAwait(false);
        }

        protected async Task<TValue> ReadValue<TValue>(TextReader reader)
        {
            var line = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var value = Convert.ChangeType(line, typeof(TValue));
            return (TValue)value;
        }
    }
}
