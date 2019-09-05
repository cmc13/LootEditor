using GalaSoft.MvvmLight;
using LootEditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace LootEditor.View.ViewModel
{

    public class DynamicCriteria : DynamicObject, INotifyPropertyChanged
    {
        private readonly LootCriteria criteria;
        public Dictionary<string, object> props = new Dictionary<string, object>();

        public DynamicCriteria(LootCriteria criteria)
        {
            foreach (var prop in criteria.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                props.Add(prop.Name, prop.GetValue(criteria));
            }

            this.criteria = criteria;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return props.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (props.ContainsKey(binder.Name))
            {
                var prop = criteria.GetType().GetProperty(binder.Name, BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(criteria, value.GetType() != prop.PropertyType ? Convert.ChangeType(value, prop.PropertyType) : value);
                props[binder.Name] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(binder.Name));
                return true;
            }

            return false;
        }

    }
    public class LootCriteriaViewModel : ViewModelBase
    {
        private bool isDirty;

        public LootCriteriaViewModel(LootCriteria criteria)
        {
            Criteria = criteria;
            DynamicCriteria = new DynamicCriteria(Criteria);
            ((INotifyPropertyChanged)DynamicCriteria).PropertyChanged += LootCriteriaViewModel_PropertyChanged; ;
        }

        public LootCriteriaType Type
        {
            get => Criteria.Type;
            set
            {
                if (Criteria.Type != value)
                {
                    Criteria = LootCriteria.CreateLootCriteria(value);
                    DynamicCriteria = new DynamicCriteria(Criteria);
                    ((INotifyPropertyChanged)DynamicCriteria).PropertyChanged += LootCriteriaViewModel_PropertyChanged;
                    IsDirty = true;
                }
            }
        }

        private void LootCriteriaViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Criteria));
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

        public LootCriteria Criteria { get; private set; }
        public dynamic DynamicCriteria { get; private set; }

        public override string ToString() => Criteria.ToString();

        public void Clean()
        {
            IsDirty = false;
        }
    }
}