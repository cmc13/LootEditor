using LootEditor.Models;
using LootEditor.Models.Enums;

namespace LootEditor.ViewModels
{
    public class LootCriteriaViewModel : DirtyViewModel
    {
        private LootCriteriaType? myType = null;
        public LootCriteria Criteria { get; }

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
                    OnPropertyChanged(nameof(Type));
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
                    OnPropertyChanged(nameof(RequirementLength));
                    IsDirty = true;
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