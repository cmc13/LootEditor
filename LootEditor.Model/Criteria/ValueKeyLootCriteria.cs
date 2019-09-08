using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class ValueKeyLootCriteria<TKey, TValue> : LootCriteria where TKey : Enum
    {
        private readonly string template;

        public override Enums.LootCriteriaType Type { get; }

        public ValueKeyLootCriteria(Enums.LootCriteriaType type, string template)
        {
            Type = type;
            this.template = template;
        }

        public TValue Value { get; set; }
        public TKey Key { get; set; }

        public override string ToString() => string.Format(template, Key, Value);

        public override async Task ReadAsync(TextReader reader, int version)
        {
            await base.ReadAsync(reader, version).ConfigureAwait(false);

            var valueLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var value = Convert.ChangeType(valueLine, typeof(TValue));

            Value = (TValue)value;

            var keyLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var key = Enum.Parse(typeof(TKey), keyLine);

            Key = (TKey)key;
        }

        public override async Task WriteAsync(Stream stream)
        {
            await base.WriteAsync(stream).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Value.ToString()).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(Convert.ToInt32(Key).ToString()).ConfigureAwait(false);
        }
    }
}