using System;
using GalaSoft.MvvmLight;
using LootEditor.Model;

namespace LootEditor.View.ViewModel
{
    public class LootCriteriaViewModel : ViewModelBase
    {
        private bool isDirty;

        public LootCriteriaViewModel(LootCriteria criteria)
        {
            Criteria = criteria;
        }

        public LootCriteriaType Type
        {
            get => Criteria.Type;
            set
            {
                if (Criteria.Type != value)
                {
                    Criteria = LootCriteria.CreateLootCriteria(value);
                    IsDirty = true;
                }
            }
        }

        public bool IsDirty
        {
            get => isDirty;
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public LootCriteria Criteria { get; private set; }

        public override string ToString() => Criteria.ToString();

        public void Clean()
        {
            IsDirty = false;
        }
    }
}