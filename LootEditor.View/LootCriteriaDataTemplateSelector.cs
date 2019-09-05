using LootEditor.View.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LootEditor.View
{
    public class LootCriteriaDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ColorLootCriteriaTemplate { get; set; }
        public DataTemplate DisabledRuleTemplate { get; set; }
        public DataTemplate LongValueKeyTemplate { get; set; }
        public DataTemplate DoubleValueKeyTemplate { get; set; }
        public DataTemplate StringValueKeyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var criteria = item as LootCriteriaViewModel;
            if (criteria != null)
            {
                switch (criteria.Type)
                {
                    case Model.LootCriteriaType.AnySimilarColor: return ColorLootCriteriaTemplate;
                    case Model.LootCriteriaType.DisabledRule: return DisabledRuleTemplate;
                    case Model.LootCriteriaType.BuffedLongValKeyGE:
                    case Model.LootCriteriaType.LongValKeyFlagExists:
                    case Model.LootCriteriaType.LongValKeyGE:
                    case Model.LootCriteriaType.LongValKeyNE:
                    case Model.LootCriteriaType.LongValKeyLE:
                    case Model.LootCriteriaType.LongValKeyE: return LongValueKeyTemplate;
                    case Model.LootCriteriaType.BuffedDoubleValKeyGE:
                    case Model.LootCriteriaType.DoubleValKeyGE:
                    case Model.LootCriteriaType.DoubleValKeyLE: return DoubleValueKeyTemplate;
                    case Model.LootCriteriaType.StringValueMatch: return StringValueKeyTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
