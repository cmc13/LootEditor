using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LootEditor.Model
{
    public class LootRule : ICloneable
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

        public async Task WriteAsync(Stream stream)
        {
            await stream.WriteLineForRealAsync(Name).ConfigureAwait(false);
            await stream.WriteLineForRealAsync(CustomExpression).ConfigureAwait(false);

            var bigLine = new List<int> { Priority, (int)Action };
            await stream.WriteLineForRealAsync(string.Join(";", bigLine.Concat(Criteria.Select(c => (int)c.Type))));
            if (Action == LootAction.KeepUpTo)
                await stream.WriteLineForRealAsync(KeepUpToCount.ToString()).ConfigureAwait(false);
            foreach (var criteria in Criteria)
            {
                await criteria.WriteAsync(stream).ConfigureAwait(false);
            }
        }

        public object Clone()
        {
            var rule = new LootRule()
            {
                Name = this.Name,
                Action = this.Action,
                Priority = this.Priority,
                CustomExpression = this.CustomExpression,
                KeepUpToCount = this.KeepUpToCount
            };

            foreach (var criteria in this.Criteria)
                rule.AddCriteria(criteria.Clone() as LootCriteria);

            return rule;
        }

        public void MoveCriteria(int sourceIndex, int targetIndex)
        {
            var item = criteria[sourceIndex];
            criteria.RemoveAt(sourceIndex);
            criteria.Insert(targetIndex, item);
        }
    }
}