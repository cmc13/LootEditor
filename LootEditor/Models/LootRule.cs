﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LootEditor.Models;

[Serializable]
public class LootRule : ICloneable, ISerializable
{
    private readonly List<LootCriteria> criteria = [];

    public string CustomExpression { get; set; } = "";
    public int Priority { get; set; } = 0;
    public Enums.LootAction Action { get; set; } = Enums.LootAction.Keep;

    public int KeepUpToCount { get; set; } = 0;
    public string Name { get; set; }
    public IEnumerable<LootCriteria> Criteria => criteria.AsReadOnly();

    public LootRule()
    {
    }

    private LootRule(SerializationInfo info, StreamingContext context)
    {
        Name = info.GetString(nameof(Name));
        Action = (Enums.LootAction)info.GetValue(nameof(Action), typeof(Enums.LootAction));
        Priority = info.GetInt32(nameof(Priority));
        CustomExpression = info.GetString(nameof(CustomExpression));
        KeepUpToCount = info.GetInt32(nameof(KeepUpToCount));

        var count = info.GetInt32($"{nameof(Criteria)}Count");
        for (var i = 0; i < count; ++i)
        {
            var crit = (LootCriteria)info.GetValue($"{nameof(Criteria)}[{i}]", typeof(LootCriteria));
            AddCriteria(crit);
        }
    }

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
            throw new Exception($"Failed to parse priority for loot rule ({rule.Name})");
        }

        rule.Priority = priority;

        if (!int.TryParse(lineItems[1], out var action))
        {
            throw new Exception($"Failed to parse action for loot rule ({rule.Name})");
        }

        rule.Action = (Enums.LootAction)action;

        if (action == (int)Enums.LootAction.KeepUpTo)
        {
            var keepUpToCountLine = await reader.ReadLineForRealAsync().ConfigureAwait(false);
            if (!int.TryParse(keepUpToCountLine, out var keepUpToCount))
            {
                throw new Exception($"Failed to parse keep count for loot rule ({rule.Name})");
            }

            rule.KeepUpToCount = keepUpToCount;
        }

        for (int i = 2; i < lineItems.Length; ++i)
        {
            if (!int.TryParse(lineItems[i], out var rt))
            {
                throw new Exception($"Failed to parse criteria type for loot rule ({rule.Name})");
            }

            var ruleType = (Enums.LootCriteriaType)rt;
            var criteria = LootCriteria.CreateLootCriteria(ruleType);
            await criteria.ReadAsync(reader, version);

            rule.criteria.Add(criteria);
        }

        return rule;
    }

    public void RemoveCriteria(LootCriteria criteria) => this.criteria.Remove(criteria);

    public void AddCriteria(LootCriteria criteria) => this.criteria.Add(criteria);

    public void AddCriteria(LootCriteria criteria, int idx) => this.criteria.Insert(idx, criteria);

    public async Task WriteAsync(TextWriter writer)
    {
        await writer.WriteLineAsync(Name).ConfigureAwait(false);
        await writer.WriteLineAsync(CustomExpression).ConfigureAwait(false);

        var bigLine = new List<int> { Priority, (int)Action };
        await writer.WriteLineAsync(string.Join(";", bigLine.Concat(Criteria.Select(c => (int)c.Type))));
        if (Action == Enums.LootAction.KeepUpTo)
            await writer.WriteLineAsync(KeepUpToCount.ToString()).ConfigureAwait(false);
        foreach (var criteria in Criteria)
        {
            await criteria.WriteAsync(writer).ConfigureAwait(false);
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

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Name), Name, typeof(string));
        info.AddValue(nameof(Action), Action, typeof(Enums.LootAction));
        info.AddValue(nameof(Priority), Priority, typeof(int));
        info.AddValue(nameof(CustomExpression), CustomExpression, typeof(string));
        info.AddValue(nameof(KeepUpToCount), KeepUpToCount, typeof(int));

        info.AddValue($"{nameof(Criteria)}Count", criteria.Count, typeof(int));
        for (var i = 0; i < criteria.Count; ++i)
            info.AddValue($"{nameof(Criteria)}[{i}]", criteria[i], typeof(LootCriteria));
    }
}