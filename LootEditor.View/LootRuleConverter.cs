using LootEditor.Model.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace LootEditor.Model
{
    public class LootRuleConverter : JsonConverter<LootRule>
    {
        public override LootRule ReadJson(JsonReader reader, Type objectType, LootRule existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var rule = jo.ToObject<LootRule>();
            foreach (var crit in jo["Criteria"].AsJEnumerable())
            {
                var type = crit["Type"].ToObject<LootCriteriaType>();
                var criteriaObj = LootCriteria.CreateLootCriteria(type);
                foreach (var j in crit)
                {
                    continue;
                }
                rule.AddCriteria(criteriaObj);
            }
            return rule;
        }

        public override void WriteJson(JsonWriter writer, LootRule value, JsonSerializer serializer)
            => serializer.Serialize(writer, value);
    }
}