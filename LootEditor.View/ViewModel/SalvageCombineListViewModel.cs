using GalaSoft.MvvmLight;
using LootEditor.Model;
using LootEditor.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace LootEditor.View.ViewModel
{
    public class SalvageCombineListViewModel : ViewModelBase
    {
        public class SalvageObj : ObservableObject
        {
            private string combineRange;
            private int? combineValue;

            public string CombineRange
            {
                get => combineRange;
                set
                {
                    if (combineRange != value)
                    {
                        combineRange = value;
                        RaisePropertyChanged(nameof(CombineRange));
                    }
                }
            }

            public int? CombineValue
            {
                get => combineValue;
                set
                {
                    if (combineValue != value)
                    {
                        combineValue = value;
                        RaisePropertyChanged(nameof(CombineValue));
                        RaisePropertyChanged(nameof(HasCombineValue));
                    }
                }
            }

            public bool HasCombineValue
            {
                get => CombineValue.HasValue;
                set
                {
                    if (CombineValue.HasValue != value)
                    {
                        CombineValue = value ? (int?)0 : null;
                    }
                }
            }
        }


        private readonly SalvageCombineBlockType salvageCombineBlock;
        private bool isDirty = false;
        private KeyValuePair<Material, SalvageObj> selectedItem;

        public SalvageCombineListViewModel(LootFile lootFile)
        {
            salvageCombineBlock = lootFile.ExtraBlocks.SingleOrDefault(b => b is SalvageCombineBlockType) as SalvageCombineBlockType;

            if (salvageCombineBlock != null)
            {
                foreach (var item in salvageCombineBlock.Materials)
                {
                    if (!CombineRules.TryGetValue(item.Key, out var salvageObj))
                    {
                        salvageObj = new SalvageObj();
                        CombineRules.Add(item.Key, salvageObj);

                        salvageObj.PropertyChanged += Rule_PropertyChanged;
                    }

                    salvageObj.CombineRange = item.Value;
                }

                foreach (var item in salvageCombineBlock.MaterialValues)
                {
                    if (!CombineRules.TryGetValue(item.Key, out var salvageObj))
                    {
                        salvageObj = new SalvageObj();
                        CombineRules.Add(item.Key, salvageObj);

                        salvageObj.PropertyChanged += Rule_PropertyChanged;
                    }

                    salvageObj.CombineValue = item.Value;
                }
            }

            CombineRules.CollectionChanged += CombineRules_CollectionChanged;
        }

        public KeyValuePair<Material, SalvageObj> SelectedItem
        {
            get => selectedItem;
            set
            {
                if (!selectedItem.Equals(value))
                {
                    selectedItem = value;
                    RaisePropertyChanged(nameof(SelectedItem));
                }
            }
        }

        private void Rule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsDirty));
        }

        private void CombineRules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValuePair<Material, SalvageObj> item in e.NewItems)
                {
                    if (item.Value.CombineRange != null)
                        salvageCombineBlock.Materials.Add(item.Key, item.Value.CombineRange);
                    if (item.Value.HasCombineValue)
                        salvageCombineBlock.MaterialValues.Add(item.Key, item.Value.CombineValue.Value);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (KeyValuePair<Material, SalvageObj> item in e.OldItems)
                {
                    if (item.Value.CombineRange != null)
                        salvageCombineBlock.Materials.Add(item.Key, item.Value.CombineRange);
                    if (item.Value.HasCombineValue)
                        salvageCombineBlock.MaterialValues.Add(item.Key, item.Value.CombineValue.Value);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                for (var i = 0; i < e.NewItems.Count; ++i)
                {
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                salvageCombineBlock.Materials.Clear();
                salvageCombineBlock.MaterialValues.Clear();
            }

            IsDirty = true;
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

        public string DefaultCombineRange
        {
            get => salvageCombineBlock?.DefaultCombineString;
            set
            {
                if (salvageCombineBlock.DefaultCombineString != value)
                {
                    salvageCombineBlock.DefaultCombineString = value;
                    RaisePropertyChanged(nameof(DefaultCombineRange));
                    IsDirty = true;
                }
            }
        }

        public ObservableDictionary<Material, SalvageObj> CombineRules { get; } = new ObservableDictionary<Material, SalvageObj>();
    }
}
