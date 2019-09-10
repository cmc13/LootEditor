using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    [Serializable]
    public class ValueLootCriteria<T> : LootCriteria
    {
        public ValueLootCriteria(Enums.LootCriteriaType type)
        {
            Type = type;
        }

        public override Enums.LootCriteriaType Type { get; }
        public T Value { get; set; }

        private ValueLootCriteria(SerializationInfo info, StreamingContext context)
        {
            Type = (Enums.LootCriteriaType)info.GetValue(nameof(Type), typeof(Enums.LootCriteriaType));
            Value = (T)info.GetValue(nameof(Value), typeof(T));
        }

        public override string ToString()
        {
            switch (Type)
            {
                case Enums.LootCriteriaType.SpellNameMatch:
                    return $"Spell Name Matches: {Value}";

                case Enums.LootCriteriaType.ObjectClass:
                    var octd = TypeDescriptor.GetConverter(typeof(Enums.ObjectClass));
                    return $"Object Class: {octd.ConvertToInvariantString(Value)}";

                case Enums.LootCriteriaType.DisabledRule:
                    return "Rule is " + (Convert.ToBoolean(Value) ? "Disabled" : "Enabled");
            }

            var td = TypeDescriptor.GetConverter(typeof(Enums.LootCriteriaType));
            return $"{td.ConvertToInvariantString(Type)} {Value}";
        }

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);
            var value = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (typeof(T).IsEnum)
            {
                Value = (T)Enum.Parse(typeof(T), value);
            }
            else
            {
                Value = (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            if (typeof(T) == typeof(bool))
            {
                await stream.WriteLineForRealAsync(Value.ToString().ToLower()).ConfigureAwait(false);
            }
            else if (typeof(T).IsEnum)
            {
                await stream.WriteLineForRealAsync(Convert.ToInt32(Value).ToString()).ConfigureAwait(false);
            }
            else
            {
                await stream.WriteLineForRealAsync(Value.ToString()).ConfigureAwait(false);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Type), Type, typeof(Enums.LootCriteriaType));
            info.AddValue(nameof(Value), Value, typeof(T));
        }
    }
}