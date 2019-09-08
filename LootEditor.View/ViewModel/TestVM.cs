using LootEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootEditor.View.ViewModel
{
    public class TestVM : LootCriteriaViewModel
    {
        public TestVM(ValueKeyLootCriteria<StringValueKey, string> criteria)
            : base(criteria)
        {

        }

        public string Value
        {
            get => ((ValueKeyLootCriteria<StringValueKey, string>)Criteria).Value;
            set
            {
                if (((ValueKeyLootCriteria<StringValueKey, string>)Criteria).Value == value)
                    return;
                ((ValueKeyLootCriteria<StringValueKey, string>)Criteria).Value = value;
                RaisePropertyChanged(nameof(Value));
                IsDirty = true;
            }
        }
    }
}
