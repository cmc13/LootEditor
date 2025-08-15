using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.Models;

[Serializable]
public class ValueKeyLootCriteria<TKey, TValue> : ValueLootCriteria<TValue> where TKey : Enum
{
    public ValueKeyLootCriteria(Enums.LootCriteriaType type)
        : base(type)
    {
    }

    private ValueKeyLootCriteria(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Key = (TKey)info.GetValue(nameof(Key), typeof(TKey));
    }

    public TKey Key { get; set; }
    public override string Filter
    {
        get
        {
            var list = base.Filter.Split(':').ToList();
            list.Insert(2, Key.ToString());
            return string.Join(':', list);
        }
    }

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
                sb.Append(" ≥ ");
                break;

            case Enums.LootCriteriaType.DoubleValKeyLE:
            case Enums.LootCriteriaType.LongValKeyLE:
                sb.Append(" ≤ ");
                break;

            case Enums.LootCriteriaType.LongValKeyE:
                sb.Append(" = ");
                break;

            case Enums.LootCriteriaType.LongValKeyNE:
                sb.Append(" ≠ ");
                break;
        }



        if (Key is Enums.DoubleValueKey doubleKey)
        {
            switch (doubleKey)
            {
                case Enums.DoubleValueKey.AttackBonus:
                case Enums.DoubleValueKey.MeleeDefenseBonus:
                case Enums.DoubleValueKey.MissileDBonus:
                case Enums.DoubleValueKey.MagicDBonus:
                case Enums.DoubleValueKey.DamageBonus:
                    sb.AppendFormat("{0:N1}", (Convert.ToDouble(Value) - 1.0) * 100).Append('%');
                    break;
                case Enums.DoubleValueKey.ManaCBonus:
                    sb.AppendFormat("{0:N1}", Convert.ToDouble(Value) * 100).Append('%');
                    break;
                default:
                    sb.AppendFormat("{0:0.0##}", Value);
                    break;
            }
        }
        else
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

                case Enums.LongValueKey.DamageType:
                case Enums.LongValueKey.WandElemDmgType:
                    var dttd = TypeDescriptor.GetConverter(typeof(Enums.DamageType));
                    var dtValue = (Enums.DamageType)Enum.ToObject(typeof(Enums.DamageType), Convert.ToInt32(Value));
                    sb.Append(" (").Append(dttd.ConvertToInvariantString(dtValue)).Append(')');
                    break;
            }
        }

        return sb.ToString();
    }

    public override async Task ReadAsync(TextReader reader, int version)
    {
        await base.ReadAsync(reader, version).ConfigureAwait(false);

        var keyLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
        var key = Enum.Parse(typeof(TKey), keyLine);

        Key = (TKey)key;

        // breeches are funny
        if (Key is Enums.LongValueKey l && l == Enums.LongValueKey.Coverage && Convert.ToInt32(Value) == 19)
            Value = (TValue)Convert.ChangeType(18, typeof(TValue));
    }

    public override async Task WriteInternalAsync(Stream stream)
    {
        // Handle weirdness of breeches
        if (Key is Enums.LongValueKey l && l == Enums.LongValueKey.Coverage && Convert.ToInt32(Value) == 18)
            await stream.WriteLineForRealAsync("19").ConfigureAwait(false);
        else
            await base.WriteInternalAsync(stream).ConfigureAwait(false);
        await stream.WriteLineForRealAsync(Convert.ToInt32(Key).ToString()).ConfigureAwait(false);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Key), Key, typeof(TKey));
    }

    public override bool IsMatch(string[] filter)
    {
        if (!BaseMatch(filter))
            return false;

        if (filter.Length >= 2 && !string.IsNullOrEmpty(filter[1]))
        {
            if (Key is Enum)
            {
                if (!Enum.TryParse(typeof(TKey), filter[1], out var test) || !test.Equals(Key))
                    return false;
            }
            else
            {
                try
                {
                    var test = Convert.ChangeType(filter[1], typeof(TKey));
                    if (!test.Equals(Key))
                        return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        if (filter.Length >= 3 && !string.IsNullOrEmpty(filter[2]))
        {
            switch (Value)
            {
                case string str:
                    return str.Contains(filter[2]);

                case Enum:
                    return Enum.TryParse(typeof(TValue), filter[2], out var test) && test.Equals(Value);

                default:
                    try
                    {
                        var testValue = Convert.ChangeType(filter[2], typeof(TValue));
                        return testValue.Equals(Value);
                    }
                    catch
                    {
                        return false;
                    }
            }
        }

        return true;
    }
}