using System;
using System.IO;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class ValueLootCriteria<T> : LootCriteria
    {
        private readonly string template;

        public ValueLootCriteria(Enums.LootCriteriaType type, string template)
        {
            Type = type;
            this.template = template;
        }

        public override Enums.LootCriteriaType Type { get; }
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
    }
}