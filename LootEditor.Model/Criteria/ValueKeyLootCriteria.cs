using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class ValueKeyLootCriteria<TKey, TValue> : LootCriteria where TKey : Enum
    {
        private readonly string template;

        public override LootCriteriaType Type { get; }

        public ValueKeyLootCriteria(LootCriteriaType type, string template)
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

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Value.ToString()).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Convert.ToInt32(Key).ToString()).ConfigureAwait(false);
        }
    }
}