using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LootEditor.View
{
    /// <summary>
    /// Usage: Bind EnumFlag and Two way binding on EnumValue instead of IsChecked
    /// Example: <myControl:CheckBoxForEnumWithFlagAttribute 
    ///                 EnumValue="{Binding SimulationNatureTypeToCreateStatsCacheAtEndOfSimulation, Mode=TwoWay}" 
    ///                 EnumFlag="{x:Static Core:SimulationNatureType.LoadFlow }">Load Flow results</myControl:CheckBoxForEnumWithFlagAttribute>
    /// </summary>
    public class CheckBoxForEnumWithFlagAttribute : CheckBox
    {
        // ************************************************************************
        public static DependencyProperty EnumValueProperty =
            DependencyProperty.Register("EnumValue", typeof(object), typeof(CheckBoxForEnumWithFlagAttribute), new PropertyMetadata(EnumValueChangedCallback));

        public static DependencyProperty EnumFlagProperty =
            DependencyProperty.Register("EnumFlag", typeof(object), typeof(CheckBoxForEnumWithFlagAttribute), new PropertyMetadata(EnumFlagChangedCallback));

        // ************************************************************************
        public CheckBoxForEnumWithFlagAttribute()
        {
            base.Checked += CheckBoxForEnumWithFlag_Checked;
            base.Unchecked += CheckBoxForEnumWithFlag_Unchecked;
        }

        // ************************************************************************
        private static void EnumValueChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var checkBoxForEnumWithFlag = dependencyObject as CheckBoxForEnumWithFlagAttribute;
            if (checkBoxForEnumWithFlag != null)
            {
                checkBoxForEnumWithFlag.RefreshCheckBoxState();
            }
        }

        // ************************************************************************
        private static void EnumFlagChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var checkBoxForEnumWithFlag = dependencyObject as CheckBoxForEnumWithFlagAttribute;
            if (checkBoxForEnumWithFlag != null)
            {
                checkBoxForEnumWithFlag.RefreshCheckBoxState();
            }
        }

        // ************************************************************************
        public object EnumValue
        {
            get { return GetValue(EnumValueProperty); }
            set { SetValue(EnumValueProperty, value); }
        }

        // ************************************************************************
        public object EnumFlag
        {
            get { return GetValue(EnumFlagProperty); }
            set { SetValue(EnumFlagProperty, value); }
        }

        // ************************************************************************
        private void RefreshCheckBoxState()
        {
            try
            {
                if (EnumValue != null)
                {
                    if (EnumValue is Enum)
                    {
                        Type underlyingType = Enum.GetUnderlyingType(EnumValue.GetType());
                        dynamic valueAsInt = Convert.ChangeType(EnumValue, underlyingType);
                        dynamic flagAsInt = Convert.ChangeType(EnumFlag, underlyingType);

                        base.IsChecked = ((valueAsInt & flagAsInt) > 0);
                    }
                }
            }
            catch { }
        }

        // ************************************************************************
        private void CheckBoxForEnumWithFlag_Checked(object sender, RoutedEventArgs e)
        {
            RefreshEnumValue();
        }

        // ************************************************************************
        void CheckBoxForEnumWithFlag_Unchecked(object sender, RoutedEventArgs e)
        {
            RefreshEnumValue();
        }

        // ************************************************************************
        private void RefreshEnumValue()
        {
            if (EnumValue != null)
            {
                if (EnumValue is Enum)
                {
                    Type underlyingType = Enum.GetUnderlyingType(EnumValue.GetType());
                    dynamic valueAsInt = Convert.ChangeType(EnumValue, underlyingType);
                    dynamic flagAsInt = Convert.ChangeType(EnumFlag, underlyingType);

                    dynamic newValueAsInt = valueAsInt;
                    if (base.IsChecked == true)
                    {
                        newValueAsInt = valueAsInt | flagAsInt;
                    }
                    else
                    {
                        newValueAsInt = valueAsInt & ~flagAsInt;
                    }

                    if (newValueAsInt != valueAsInt)
                    {
                        object o = Enum.ToObject(EnumValue.GetType(), newValueAsInt);

                        EnumValue = o;
                    }
                }
            }
        }

        // ************************************************************************
    }
}
