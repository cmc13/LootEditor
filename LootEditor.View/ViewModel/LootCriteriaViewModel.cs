using GalaSoft.MvvmLight;
using LootEditor.Model;

namespace LootEditor.View.ViewModel
{
    public class LootCriteriaViewModel : ViewModelBase
    {
        private readonly LootCriteria criteria;

        public LootCriteriaViewModel(LootCriteria criteria)
        {
            this.criteria = criteria;
        }

        public override string ToString() => criteria.ToString();
    }
}