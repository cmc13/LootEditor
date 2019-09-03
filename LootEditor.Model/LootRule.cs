using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class LootRule
    {
        private List<LootCriteria> criteria = new List<LootCriteria>();

        public string CustomExpression { get; set; } = "";
        public int Priority { get; set; } = 0;
        public LootAction Action { get; set; } = LootAction.Keep;

        public int KeepUpToCount { get; set; } = 0;
        public string Name { get; set; }
        public IEnumerable<LootCriteria> Criteria => criteria.AsReadOnly();

        public static async Task<LootRule> ReadRuleAsync(int version, TextReader reader)
        {
            var rule = new LootRule()
            {
                Name = await reader.ReadLineForRealAsync().ConfigureAwait(false)
            };

            if (version == 1)
                rule.CustomExpression = await reader.ReadLineForRealAsync().ConfigureAwait(false);

            var bigLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            var lineItems = bigLine.Split(';');

            if (!int.TryParse(lineItems[0], out var priority))
            {
                throw new Exception();
            }

            rule.Priority = priority;

            if (!int.TryParse(lineItems[1], out var action))
            {
                throw new Exception();
            }

            rule.Action = (LootAction)action;

            if (action == (int)LootAction.KeepUpTo)
            {
                var keepUpToCountLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
                if (!int.TryParse(keepUpToCountLine, out var keepUpToCount))
                {
                    throw new Exception();
                }

                rule.KeepUpToCount = keepUpToCount;
            }

            for (int i = 0; i < lineItems.Length - 2; ++i)
            {
                if (!int.TryParse(lineItems[i + 2], out var rt))
                {
                    throw new Exception();
                }

                var ruleType = (LootCriteriaType)rt;
                var criteria = LootCriteria.CreateLootCriteria(ruleType);
                await criteria.ReadAsync(reader, version);

                rule.criteria.Add(criteria);
            }

            return rule;
        }

        public void RemoveCriteria(LootCriteria criteria) => this.criteria.Remove(criteria);

        public void AddCriteria(LootCriteria criteria) => this.criteria.Add(criteria);

        public async Task WriteAsync(TextWriter writer)
        {
            await writer.WriteLineForRealAsync(Name).ConfigureAwait(false);
            await writer.WriteLineForRealAsync(CustomExpression).ConfigureAwait(false);

            var bigLine = new List<string> { ((int)Action).ToString(), Priority.ToString() };
            if (Action == LootAction.KeepUpTo)
                bigLine.Add(KeepUpToCount.ToString());
            await writer.WriteLineForRealAsync(string.Join(";", bigLine.Concat(Criteria.Select(c => ((int)c.Type).ToString()))));
            foreach (var criteria in Criteria)
            {
                using (var subWriter = new StringWriter())
                {
                    await criteria.WriteAsync(subWriter).ConfigureAwait(false);
                    var str = subWriter.ToString();

                    await writer.WriteLineForRealAsync(str.Length.ToString()).ConfigureAwait(false);
                    await writer.WriteLineForRealAsync(str).ConfigureAwait(false);
                }
            }
        }
    }
}