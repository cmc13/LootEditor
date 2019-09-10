using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public class ValueKeyLootCriteria<TKey, TValue> : LootCriteria where TKey : Enum
    {
        public override Enums.LootCriteriaType Type { get; }

        public ValueKeyLootCriteria(Enums.LootCriteriaType type)
        {
            Type = type;
        }

        private ValueKeyLootCriteria(SerializationInfo info, StreamingContext context)
        {
            Type = (Enums.LootCriteriaType)info.GetValue(nameof(Type), typeof(Enums.LootCriteriaType));
            Key = (TKey)info.GetValue(nameof(Key), typeof(TKey));
            Value = (TValue)info.GetValue(nameof(Value), typeof(TValue));
        }

        public TValue Value { get; set; }
        public TKey Key { get; set; }

        public override string ToString()
        {
            var keyTd = TypeDescriptor.GetConverter(typeof(TKey));

            var sb = new StringBuilder();
            if (Type == Enums.LootCriteriaType.BuffedDoubleValKeyGE || Type == Enums.LootCriteriaType.BuffedLongValKeyGE)
                sb.Append("Buffed ");

            sb.Append(keyTd.ConvertToInvariantString(Key));

            switch (Type)
            {
                case Enums.LootCriteriaType.StringValueMatch:
                    sb.Append(" Matches ");
                    break;

                case Enums.LootCriteriaType.BuffedDoubleValKeyGE:
                case Enums.LootCriteriaType.BuffedLongValKeyGE:
                case Enums.LootCriteriaType.CharacterSkillGE:
                case Enums.LootCriteriaType.DoubleValKeyGE:
                case Enums.LootCriteriaType.LongValKeyGE:
                    sb.Append(" >= ");
                    break;

                case Enums.LootCriteriaType.DoubleValKeyLE:
                case Enums.LootCriteriaType.LongValKeyLE:
                    sb.Append(" <= ");
                    break;

                case Enums.LootCriteriaType.LongValKeyE:
                    sb.Append(" == ");
                    break;

                case Enums.LootCriteriaType.LongValKeyNE:
                    sb.Append(" != ");
                    break;
            }

            sb.Append(Value);

            if (Type == Enums.LootCriteriaType.LongValKeyFlagExists)
            {
                sb.Append(" (0x").Append(Convert.ToInt32(Value).ToString("X")).Append(')');
            }
            else if (Key is Enums.LongValueKey longKey)
            {
                switch (longKey)
                {
                    case Enums.LongValueKey.Material:
                        var mtd = TypeDescriptor.GetConverter(typeof(Enums.Material));
                        var matValue = (Enums.Material)Enum.ToObject(typeof(Enums.Material), Convert.ToInt32(Value));
                        sb.Append(" (").Append(mtd.ConvertToInvariantString(matValue)).Append(')');
                        break;

                    case Enums.LongValueKey.EquippableSlots:
                        var std = TypeDescriptor.GetConverter(typeof(Enums.EquippableSlot));
                        var sValue = (Enums.EquippableSlot)Enum.ToObject(typeof(Enums.EquippableSlot), Convert.ToInt32(Value));
                        var slots = Enum.GetValues(typeof(Enums.EquippableSlot)).Cast<Enums.EquippableSlot>()
                            .Where(testValue => (sValue & testValue) != 0)
                            .Select(slot => std.ConvertToInvariantString(slot));
                        sb.Append(" (").Append(string.Join(", ", slots)).Append(')');
                        break;

                    case Enums.LongValueKey.Coverage:
                        var ctd = TypeDescriptor.GetConverter(typeof(Enums.Coverage));
                        var cValue = (Enums.Coverage)Enum.ToObject(typeof(Enums.Coverage), Convert.ToInt32(Value));
                        var coverageSlots = Enum.GetValues(typeof(Enums.Coverage)).Cast<Enums.Coverage>()
                            .Where(testValue => (cValue & testValue) != 0)
                            .Select(slot => ctd.ConvertToInvariantString(slot));
                        sb.Append(" (").Append(string.Join(", ", coverageSlots)).Append(')');
                        break;

                    case Enums.LongValueKey.WeaponMasteryCategory:
                        var wmctd = TypeDescriptor.GetConverter(typeof(Enums.WeaponMasteryCategory));
                        var wmcValue = (Enums.WeaponMasteryCategory)Enum.ToObject(typeof(Enums.WeaponMasteryCategory), Convert.ToInt32(Value));
                        sb.Append(" (").Append(wmctd.ConvertToInvariantString(wmcValue)).Append(')');
                        break;

                    case Enums.LongValueKey.EquipSkill:
                    case Enums.LongValueKey.WieldReqAttribute:
                        var sttd = TypeDescriptor.GetConverter(typeof(Enums.SkillType));
                        var stValue = (Enums.SkillType)Enum.ToObject(typeof(Enums.SkillType), Convert.ToInt32(Value));
                        sb.Append(" (").Append(sttd.ConvertToInvariantString(stValue)).Append(')');
                        break;

                    case Enums.LongValueKey.ArmorSetID:
                        var astd = TypeDescriptor.GetConverter(typeof(Enums.ArmorSet));
                        var asValue = (Enums.ArmorSet)Enum.ToObject(typeof(Enums.ArmorSet), Convert.ToInt32(Value));
                        sb.Append(" (").Append(astd.ConvertToInvariantString(asValue)).Append(')');
                        break;
                }
            }

            return sb.ToString();
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);

            var valueLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var value = Convert.ChangeType(valueLine, typeof(TValue));

            Value = (TValue)value;

            var keyLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var key = Enum.Parse(typeof(TKey), keyLine);

            Key = (TKey)key;

            // breeches are funny
            if (Key is Enums.LongValueKey l && l == Enums.LongValueKey.Coverage && Convert.ToInt32(Value) == 19)
                Value = (TValue)Convert.ChangeType(18, typeof(TValue));
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            // Handle weirdness of breeches
            if (Key is Enums.LongValueKey l && l == Enums.LongValueKey.Coverage && Convert.ToInt32(Value) == 18)
                await stream.WriteLineForRealAsync("19").ConfigureAwait(false);
            else
                await stream.WriteLineForRealAsync(Value.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Convert.ToInt32(Key).ToString()).ConfigureAwait(false);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Type), Type, typeof(Enums.LootCriteriaType));
            info.AddValue(nameof(Key), Key, typeof(TKey));
            info.AddValue(nameof(Value), Value, typeof(TValue));
        }
    }
}