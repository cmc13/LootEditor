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
        public DataTemplate DisabledRuleDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var vm = item as LootCriteriaViewModel;
            if (vm != null)
            {
                switch (vm.Type)
                {
                    case Model.LootCriteriaType.DisabledRule: return DisabledRuleDataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
