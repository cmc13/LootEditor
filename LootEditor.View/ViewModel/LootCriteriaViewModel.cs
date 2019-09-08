using GalaSoft.MvvmLight;
using LootEditor.Model;

namespace LootEditor.View.ViewModel
{
    public class LootCriteriaViewModel : ViewModelBase
    {
        private LootCriteriaType? myType = null;
        public LootCriteria Criteria { get; }
        private bool isDirty;

        public LootCriteriaViewModel(LootCriteria criteria)
        {
            Criteria = criteria;
        }

        public LootCriteriaType Type
        {
            get => myType ?? Criteria.Type;
            set
            {
                if (Criteria.Type != value)
                {
                    myType = value;
                    RaisePropertyChanged(nameof(Type));
                    IsDirty = true;
                }
            }
        }

        public int RequirementLength
        {
            get => Criteria.RequirementLength;
            set
            {
                if (Criteria.RequirementLength != value)
                {
                    Criteria.RequirementLength = value;
                    RaisePropertyChanged(nameof(RequirementLength));
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

        public string DisplayValue => Criteria.ToString();

        public void Clean()
        {
            IsDirty = false;
        }
    }
}