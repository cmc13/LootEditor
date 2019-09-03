using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class ValueLootCriteria<T> : LootCriteria
    {
        private readonly string template;

        public ValueLootCriteria(LootCriteriaType type, string template)
        {
            Type = type;
            this.template = template;
        }

        public override LootCriteriaType Type { get; }
        public T Value { get; set; }

        public override string ToString() => string.Format(template, Value);

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

        public override async Task WriteAsync(TextWriter writer)
        {
            await base.WriteAsync(writer).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(Value.ToString()).ConfigureAwait(false);
        }
    }
}